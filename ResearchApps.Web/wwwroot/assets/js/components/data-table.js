/**
 * DataTable Component
 * Reusable DataTable initializer using data-attributes
 * 
 * Usage in HTML:
 * <table data-datatable
 *        data-api-url="/api/Items/"
 *        data-order="0,desc"
 *        data-create-url="/Items/Create"
 *        data-create-text="Add Item"
 *        data-columns='[...]'>
 * 
 * Or configure via JavaScript:
 * DataTableComponent.init(element, options);
 */

import { DOMUtils } from '../core/dom-utils.js';

export class DataTableComponent {
    static defaults = {
        searchDelay: 1000,
        processing: true,
        paging: true,
        serverSide: true,
        searching: true,
        ordering: true,
        autoWidth: false,
        responsive: true,
        scrollX: true
    };

    /**
     * Initialize a DataTable on an element
     * @param {HTMLElement|string} element - Table element or selector
     * @param {Object} options - Configuration options (optional, can use data-attributes)
     * @returns {Object} DataTable instance
     */
    static init(element, options = {}) {
        const el = typeof element === 'string' ? document.querySelector(element) : element;
        if (!el) {
            console.warn('DataTableComponent: Element not found', element);
            return null;
        }

        // Ensure columns array is valid
        if (options.columns && !Array.isArray(options.columns)) {
            console.error('DataTableComponent: columns must be an array');
            return null;
        }

        // Clear any existing tbody content to prevent DataTables from processing it
        const tbody = el.querySelector('tbody');
        if (tbody && tbody.children.length > 0) {
            console.warn('DataTableComponent: Clearing existing tbody content');
            tbody.innerHTML = '';
        }

        // Ensure tbody exists
        if (!tbody) {
            el.appendChild(document.createElement('tbody'));
        }

        const config = this.buildConfig(el, options);
        
        try {
            return $(el).DataTable(config);
        } catch (error) {
            console.error('DataTableComponent: Failed to initialize DataTable', error);
            console.error('Element:', el);
            console.error('Config:', JSON.stringify(config, null, 2));
            throw error;
        }
    }

    /**
     * Build DataTable configuration from element data-attributes and options
     */
    static buildConfig(element, options) {
        const dataset = element.dataset;
        
        // Parse order from data-order="0,desc"
        let order = [[0, 'desc']];
        if (dataset.order) {
            const [col, dir] = dataset.order.split(',');
            order = [[parseInt(col), dir]];
        }
        if (options.order) order = options.order;

        // Parse columns from data-columns (JSON string)
        let columns = options.columns;
        if (dataset.columns) {
            try {
                columns = JSON.parse(dataset.columns);
            } catch (e) {
                console.error('Invalid data-columns JSON:', e);
            }
        }

        // Parse column definitions
        let columnDefs = options.columnDefs;
        if (dataset.columnDefs) {
            try {
                columnDefs = JSON.parse(dataset.columnDefs);
            } catch (e) {
                console.error('Invalid data-column-defs JSON:', e);
            }
        }

        // Build layout with create button if specified
        let layout = options.layout;
        if (dataset.createUrl) {
            layout = {
                topEnd: {
                    buttons: [{
                        text: `<i class="ri-add-line align-bottom me-1"></i> ${dataset.createText || 'Create'}`,
                        className: 'btn btn-primary btn-sm',
                        action: () => window.location.href = dataset.createUrl
                    }]
                }
            };
        }

        const config = {
            ...this.defaults,
            order,
            ajax: this.buildAjaxConfig(dataset.apiUrl || options.apiUrl, options.extraParams),
            createdRow: this.initializeBootstrapDropdowns,
            initComplete: function() {
                $('.dt-button').removeClass('dt-button');
                if (dataset.footerSearch !== 'false') {
                    DataTableComponent.setupFooterSearch(this);
                }
            }
        };

        if (columns) config.columns = columns;
        if (columnDefs) config.columnDefs = columnDefs;
        if (layout) config.layout = layout;

        return config;
    }

    /**
     * Build AJAX configuration for DataTable
     */
    static buildAjaxConfig(apiUrl, extraParams = {}) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        
        return {
            url: apiUrl,
            data: function(d) {
                const query = {
                    pageNumber: (d.start / d.length) + 1,
                    pageSize: d.length,
                };
                
                if (d.search?.value) {
                    query.Search = d.search.value;
                }
                
                if (d.order?.length) {
                    query.SortBy = d.columns[d.order[0].column].data;
                    query.IsSortAscending = d.order[0].dir === 'asc';
                }
                
                d.columns.forEach((column) => {
                    if (column.search?.value) {
                        const fieldName = column.data.charAt(0).toUpperCase() + column.data.slice(1);
                        query[`Filters.${fieldName}`] = column.search.value;
                    }
                });
                
                Object.assign(query, extraParams);
                return query;
            },
            beforeSend: function(xhr) {
                if (token) {
                    xhr.setRequestHeader("RequestVerificationToken", token);
                }
            },
            dataSrc: function(json) {
                // ServiceResponse<PagedListVm<T>> format: { data: { items: [...], totalCount: N } }
                if (json.data && Array.isArray(json.data.items)) {
                    json.recordsTotal = json.data.totalCount || json.data.totalFilteredCount || 0;
                    json.recordsFiltered = json.data.totalFilteredCount || json.data.totalCount || 0;
                    return json.data.items;
                }
                // Direct PagedListVm<T> format: { items: [...], totalCount: N }
                if (json.items && Array.isArray(json.items)) {
                    json.recordsTotal = json.totalCount || json.totalFilteredCount || 0;
                    json.recordsFiltered = json.totalFilteredCount || json.totalCount || 0;
                    return json.items;
                }
                // Direct array format
                if (Array.isArray(json)) {
                    return json;
                }
                // Fallback: return empty array to prevent DataTables errors
                console.warn('DataTable: Unexpected response format', json);
                json.recordsTotal = 0;
                json.recordsFiltered = 0;
                return [];
            },
            error: function(xhr) {
                const message = xhr.responseJSON?.errorMessage || "An error occurred while fetching data";
                console.error('DataTable AJAX error:', xhr);
            }
        };
    }

    /**
     * Setup footer search inputs
     */
    static setupFooterSearch(dtInstance) {
        const api = dtInstance.api();
        
        api.columns().every(function() {
            const column = this;
            const title = column.footer()?.textContent;
            const header = column.header();
            
            header.style.backgroundColor = '#4F81BD';
            header.style.color = 'white';
            
            // Skip last column (Actions)
            if (column.index() === (api.columns().count() - 1)) {
                if (column.footer()) {
                    column.footer().replaceChildren();
                }
                return;
            }
            
            if (!column.footer()) return;
            
            const input = document.createElement('input');
            input.placeholder = title;
            input.className = "form-control";
            input.style.width = '100%';
            column.footer().replaceChildren(input);
            
            input.addEventListener('keyup', DOMUtils.debounce(() => {
                if (column.search() !== input.value) {
                    column.search(input.value).draw();
                }
            }, 1000));
        });
    }

    /**
     * Initialize Bootstrap dropdowns in table rows
     */
    static initializeBootstrapDropdowns(row) {
        const dropdown = row.querySelector('[data-bs-toggle="dropdown"]');
        if (dropdown) {
            new bootstrap.Dropdown(dropdown, {
                popperConfig: {
                    strategy: 'fixed',
                    modifiers: [{
                        name: 'preventOverflow',
                        options: { boundary: document.body }
                    }]
                }
            });
        }
    }

    /**
     * Auto-initialize all DataTables with data-datatable attribute
     */
    static autoInit() {
        document.querySelectorAll('[data-datatable]').forEach(el => {
            this.init(el);
        });
    }
}

// Column renderer helpers
export const ColumnRenderers = {
    /**
     * Format currency
     */
    currency: (locale = 'en-US', currency = 'USD') => (data, type) => {
        if (type === 'sort' || type === 'filter') return data ?? 0;
        return data != null 
            ? new Intl.NumberFormat(locale, { style: 'currency', currency }).format(data) 
            : '';
    },

    /**
     * Format date
     */
    date: (format = { year: 'numeric', month: 'short', day: 'numeric' }) => (data, type) => {
        if (type === 'sort' || type === 'filter') return data || '';
        if (!data) return '';
        return new Date(data).toLocaleDateString('en-US', format);
    },

    /**
     * Boolean badge
     */
    booleanBadge: (trueText = 'Active', falseText = 'Inactive') => (data, type) => {
        if (type === 'display') {
            return data
                ? `<span class="badge bg-success">${trueText}</span>`
                : `<span class="badge bg-danger">${falseText}</span>`;
        }
        return data;
    },

    /**
     * Action dropdown
     */
    actions: (config) => (data, type, row) => {
        const idField = config.idField || 'recId';
        const id = row[idField];
        const baseUrl = config.baseUrl;
        
        let items = '';
        
        if (config.view !== false) {
            items += `<li><a href="${baseUrl}/Details/${id}" class="dropdown-item"><i class="ri-eye-fill align-bottom me-2 text-muted"></i> View</a></li>`;
        }
        if (config.edit !== false) {
            items += `<li><a href="${baseUrl}/Edit/${id}" class="dropdown-item"><i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit</a></li>`;
        }
        if (config.delete !== false) {
            items += `<li><a href="${baseUrl}/Delete/${id}" class="dropdown-item"><i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete</a></li>`;
        }
        
        if (config.extraItems) {
            items += '<li><hr class="dropdown-divider"></li>';
            config.extraItems.forEach(item => {
                const url = item.url.replace('{id}', id);
                items += `<li><a href="${url}" class="dropdown-item"><i class="${item.icon} align-bottom me-2 text-muted"></i> ${item.text}</a></li>`;
            });
        }
        
        return `
            <div class="dropdown d-inline-block">
                <button class="btn btn-soft-secondary btn-sm dropdown" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="ri-more-fill align-middle"></i>
                </button>
                <ul class="dropdown-menu dropdown-menu-end">${items}</ul>
            </div>`;
    }
};

// Export for non-module usage
if (typeof window !== 'undefined') {
    window.DataTableComponent = DataTableComponent;
    window.ColumnRenderers = ColumnRenderers;
}

export default DataTableComponent;
