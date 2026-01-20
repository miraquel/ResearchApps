/**
 * View-Specific JavaScript Helpers
 * Reusable functions for common patterns in Razor views
 */

import { DOMUtils } from '../core/dom-utils.js';

/**
 * Form Helper - Common form operations
 */
export class FormHelper {
    /**
     * Initialize a Select2 dropdown with AJAX
     * @param {string} selector - Element selector
     * @param {Object} options - Configuration options
     * @returns {Object} Select2 instance
     */
    static initSelect2Ajax(selector, options = {}) {
        const defaults = {
            theme: 'bootstrap5',
            placeholder: options.placeholder || 'Select an option',
            ajax: {
                url: options.url,
                type: 'GET',
                dataType: 'json',
                delay: 500,
                headers: {
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                data: function (params) {
                    return {
                        term: params.term ? `%${params.term}%` : '',
                        ...(options.extraData || {})
                    };
                },
                processResults: function (data) {
                    const items = data.data || data;
                    return {
                        results: items.map(item => ({
                            id: item[options.idField || 'id'],
                            text: item[options.textField || 'name']
                        }))
                    };
                },
                error: function(xhr) {
                    console.error('Select2 AJAX error:', xhr);
                }
            },
            ...options.select2Options
        };
        
        return $(selector).select2(defaults);
    }
    
    /**
     * Initialize a simple Select2 dropdown (no AJAX)
     * @param {string} selector - Element selector
     * @param {Object} options - Configuration options
     * @returns {Object} Select2 instance
     */
    static initSelect2(selector, options = {}) {
        const defaults = {
            theme: 'bootstrap5',
            placeholder: options.placeholder || 'Select an option',
            ...options
        };
        
        return $(selector).select2(defaults);
    }
    
    /**
     * Set selected value for Select2 with AJAX
     * @param {string} selector - Element selector
     * @param {number|string} id - Item ID
     * @param {string} text - Item text
     */
    static setSelect2Value(selector, id, text) {
        if (id) {
            const option = new Option(text, id, true, true);
            $(selector).append(option).trigger('change');
        }
    }
    
    /**
     * Initialize Flatpickr date picker
     * @param {string} selector - Element selector
     * @param {Object} options - Configuration options
     * @returns {Object} Flatpickr instance
     */
    static initDatePicker(selector, options = {}) {
        const defaults = {
            dateFormat: 'd M Y',
            defaultDate: options.defaultDate || new Date(),
            ...options
        };
        
        return flatpickr(selector, defaults);
    }
    
    /**
     * Initialize form validation
     * @param {string} formSelector - Form selector
     * @returns {Object} Validation instance
     */
    static initValidation(formSelector) {
        return $(formSelector).validate();
    }
    
    /**
     * Setup auto-calculation between fields
     * @param {Array<string>} inputSelectors - Input field selectors
     * @param {string} outputSelector - Output field selector
     * @param {Function} calculationFn - Calculation function
     */
    static setupCalculation(inputSelectors, outputSelector, calculationFn) {
        const calculate = () => {
            const values = inputSelectors.map(sel => parseFloat($(sel).val()) || 0);
            const result = calculationFn(...values);
            $(outputSelector).val(result);
        };
        
        inputSelectors.forEach(selector => {
            $(selector).on('input change', calculate);
        });
        
        // Initial calculation
        calculate();
    }
    
    /**
     * Get anti-forgery token from page
     * @returns {string} Token value
     */
    static getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').val();
    }
    
    /**
     * Show confirmation modal before form submission
     * @param {string} btnSelector - Button selector
     * @param {string} modalSelector - Modal selector
     * @param {string} confirmBtnSelector - Confirm button selector
     * @param {string} formSelector - Form selector
     * @param {Object} options - Configuration options
     */
    static setupConfirmation(btnSelector, modalSelector, confirmBtnSelector, formSelector, options = {}) {
        const validateFirst = options.validateFirst !== false;
        const form = $(formSelector);
        
        $(btnSelector).on('click', function() {
            if (validateFirst) {
                form.validate();
                if (!form.valid()) {
                    return;
                }
            }
            
            const modal = new bootstrap.Modal(document.getElementById(modalSelector.replace('#', '')));
            modal.show();
        });
        
        $(confirmBtnSelector).on('click', function() {
            if (options.onConfirm) {
                options.onConfirm();
            } else {
                form.submit();
            }
        });
    }
}

/**
 * DataTable Helper - Common DataTable operations
 */
export class DataTableHelper {
    /**
     * Initialize a DataTable with server-side processing
     * @param {string} selector - Table selector
     * @param {Object} options - Configuration options
     * @returns {Object} DataTable instance
     */
    static init(selector, options = {}) {
        const defaults = {
            searchDelay: 1000,
            order: options.order || [[0, 'desc']],
            processing: true,
            paging: true,
            serverSide: true,
            searching: true,
            ordering: true,
            autoWidth: false,
            responsive: true,
            scrollX: true,
            ajax: this.buildAjaxConfig(options),
            createdRow: DataTableHelper.initializeBootstrapDropdowns,
            initComplete: function() {
                $('.dt-button').removeClass('dt-button');
                if (options.initComplete) {
                    options.initComplete.call(this);
                } else if (options.enableFooterSearch !== false) {
                    DataTableHelper.setupFooterSearch(this);
                }
            }
        };
        
        const config = { ...defaults, ...options.tableOptions };
        
        return $(selector).DataTable(config);
    }
    
    /**
     * Build AJAX configuration for DataTable
     * @param {Object} options - Configuration options
     * @returns {Object} AJAX configuration
     */
    static buildAjaxConfig(options) {
        const token = FormHelper.getAntiForgeryToken();
        
        return {
            url: options.apiUrl,
            data: function(d) {
                const query = {
                    pageNumber: (d.start / d.length) + 1,
                    pageSize: d.length,
                };
                
                if (d.search && d.search.value) {
                    query.Search = d.search.value;
                }
                
                if (d.order && d.order.length) {
                    query.SortBy = d.columns[d.order[0].column].data;
                    query.IsSortAscending = d.order[0].dir === 'asc';
                }
                
                d.columns.forEach((column) => {
                    if (column.search && column.search.value) {
                        const fieldName = column.data.charAt(0).toUpperCase() + column.data.slice(1);
                        query[`Filters.${fieldName}`] = column.search.value;
                    }
                });
                
                if (options.extraParams) {
                    Object.assign(query, options.extraParams);
                }
                
                return query;
            },
            beforeSend: function(xhr) {
                xhr.setRequestHeader("RequestVerificationToken", token);
            },
            dataSrc: function(json) {
                if (json.data && Array.isArray(json.data.items)) {
                    json.recordsTotal = json.data.totalCount;
                    json.recordsFiltered = json.data.totalCount;
                    return json.data.items;
                }
                return json;
            },
            error: function(xhr) {
                const message = xhr.responseJSON?.errorMessage || "An error occurred while fetching data";
                alert(message);
                console.error('DataTable AJAX error:', xhr);
            }
        };
    }
    
    /**
     * Setup footer search for DataTable
     * @param {Object} dtInstance - DataTable instance
     */
    static setupFooterSearch(dtInstance) {
        const api = dtInstance.api();
        
        api.columns().every(function() {
            const column = this;
            const title = column.footer()?.textContent;
            const header = column.header();
            
            // Style header
            header.style.backgroundColor = '#4F81BD';
            header.style.color = 'white';
            
            // Skip Actions column (usually last)
            if (column.index() === (api.columns().count() - 1)) {
                if (column.footer()) {
                    column.footer().replaceChildren();
                }
                return;
            }
            
            if (!column.footer()) return;
            
            // Create search input
            const input = document.createElement('input');
            input.placeholder = title;
            input.className = "form-control";
            input.style.width = '100%';
            column.footer().replaceChildren(input);
            
            // Debounced search
            input.addEventListener('keyup', DOMUtils.debounce(() => {
                if (column.search() !== input.value) {
                    column.search(input.value).draw();
                }
            }, 1000));
        });
    }
    
    /**
     * Add action buttons to DataTable
     * @param {string} layoutPosition - Button position ('topStart', 'topEnd', etc.)
     * @param {Array} buttons - Button configurations
     * @returns {Object} Layout configuration
     */
    static addButtons(layoutPosition, buttons) {
        return {
            [layoutPosition]: {
                buttons: buttons.map(btn => ({
                    text: btn.icon ? `<i class="${btn.icon} align-bottom me-1"></i> ${btn.text}` : btn.text,
                    className: btn.className || 'btn btn-primary btn-sm',
                    action: btn.action
                }))
            }
        };
    }
    
    /**
     * Format currency in DataTable column
     * @param {*} data - Column data
     * @param {string} type - Render type
     * @returns {*} Formatted data
     */
    static formatCurrency(data, type) {
        if (type === 'sort' || type === 'filter') {
            return data ? parseFloat(data) : 0;
        }
        return data ? numberFormat.format(data) : '';
    }
    
    /**
     * Format date in DataTable column
     * @param {*} data - Column data
     * @param {string} type - Render type
     * @param {string} format - Date format
     * @returns {*} Formatted data
     */
    static formatDate(data, type, format = 'DD MMM YYYY') {
        if (type === 'sort' || type === 'filter') {
            return data || '';
        }
        return data ? moment(data).format(format) : '';
    }
    
    /**
     * Initialize Bootstrap dropdowns with proper overflow handling
     * Prevents dropdown clipping in scrollable containers like DataTables
     * @param {HTMLElement} row - Table row element
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
}

/**
 * Modal Helper - Common modal operations
 */
export class ModalHelper {
    /**
     * Create a confirmation modal HTML
     * @param {string} id - Modal ID
     * @param {Object} options - Configuration options
     * @returns {string} Modal HTML
     */
    static createConfirmationModal(id, options = {}) {
        const title = options.title || 'Are you sure?';
        const message = options.message || 'Are you sure you want to proceed?';
        const confirmText = options.confirmText || 'Yes, Proceed';
        const cancelText = options.cancelText || 'Close';
        const icon = options.icon || 'https://cdn.lordicon.com/exymduqj.json';
        
        return `
            <div id="${id}" class="modal fade zoomIn" tabindex="-1" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div class="mt-2 text-center">
                                <lord-icon
                                    src="${icon}"
                                    trigger="loop"
                                    delay="2000"
                                    stroke="light"
                                    state="hover-line"
                                    style="width:100px;height:100px">
                                </lord-icon>
                                <div class="mt-4 pt-2 fs-15 mx-4 mx-sm-5">
                                    <h4>${title}</h4>
                                    <p class="text-muted mx-4 mb-0">${message}</p>
                                </div>
                            </div>
                            <div class="d-flex gap-2 justify-content-center mt-4 mb-2">
                                <button type="button" class="btn w-sm btn-light" data-bs-dismiss="modal">${cancelText}</button>
                                <button type="button" class="btn w-sm btn-primary" id="${id}-confirm">${confirmText}</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }
    
    /**
     * Show a modal
     * @param {string} selector - Modal selector
     * @returns {Object} Bootstrap modal instance
     */
    static show(selector) {
        const modalEl = document.querySelector(selector);
        if (!modalEl) {
            console.error(`Modal ${selector} not found`);
            return null;
        }
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
        return modal;
    }
    
    /**
     * Hide a modal
     * @param {string} selector - Modal selector
     */
    static hide(selector) {
        const modalEl = document.querySelector(selector);
        if (modalEl) {
            const modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) modal.hide();
        }
    }
}

// Export for non-module usage
if (typeof window !== 'undefined') {
    window.FormHelper = FormHelper;
    window.DataTableHelper = DataTableHelper;
    window.ModalHelper = ModalHelper;
}

export default { FormHelper, DataTableHelper, ModalHelper };
