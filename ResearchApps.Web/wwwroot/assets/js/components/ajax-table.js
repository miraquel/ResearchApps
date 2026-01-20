/**
 * AJAX Table Component
 * Dynamic table that loads data via AJAX without DataTables
 * 
 * Usage in HTML:
 * <table data-ajax-table
 *        data-url="/api/Items/outstanding"
 *        data-columns="id,name,qty"
 *        data-empty-message="No items found">
 *     <tbody></tbody>
 * </table>
 */

export class AjaxTableComponent {
    /**
     * Initialize AJAX table on an element
     * @param {HTMLElement|string} element - Table element or selector
     * @param {Object} options - Configuration options
     */
    static init(element, options = {}) {
        const el = typeof element === 'string' ? document.querySelector(element) : element;
        if (!el) return null;

        const config = this.buildConfig(el, options);
        
        return {
            element: el,
            config,
            load: (params = {}) => this.load(el, config, params),
            clear: () => this.clear(el),
            setRowRenderer: (fn) => config.rowRenderer = fn
        };
    }

    /**
     * Build configuration from element data-attributes
     */
    static buildConfig(element, options) {
        const dataset = element.dataset;

        return {
            url: dataset.url || options.url,
            columns: (dataset.columns || options.columns || '').split(',').map(s => s.trim()).filter(Boolean),
            emptyMessage: dataset.emptyMessage || options.emptyMessage || 'No data available',
            rowRenderer: options.rowRenderer,
            onLoad: options.onLoad,
            onError: options.onError
        };
    }

    /**
     * Load data from URL
     */
    static async load(element, config, params = {}) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const tbody = element.querySelector('tbody') || element;

        // Build URL with query params
        let url = config.url;
        if (Object.keys(params).length) {
            const queryString = new URLSearchParams(params).toString();
            url += (url.includes('?') ? '&' : '?') + queryString;
        }

        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'RequestVerificationToken': token,
                    'Accept': 'application/json'
                }
            });

            if (!response.ok) throw new Error(`HTTP ${response.status}`);

            const json = await response.json();
            const data = json.data || json;

            this.render(tbody, data, config);

            if (config.onLoad) config.onLoad(data);

            return data;
        } catch (error) {
            console.error('AJAX Table error:', error);
            this.renderEmpty(tbody, config.emptyMessage);
            if (config.onError) config.onError(error);
            return null;
        }
    }

    /**
     * Render table rows
     */
    static render(tbody, data, config) {
        tbody.innerHTML = '';

        if (!data || !data.length) {
            this.renderEmpty(tbody, config.emptyMessage);
            return;
        }

        data.forEach((item, index) => {
            const row = document.createElement('tr');
            
            if (config.rowRenderer) {
                row.innerHTML = config.rowRenderer(item, index);
            } else if (config.columns.length) {
                config.columns.forEach(col => {
                    const cell = document.createElement('td');
                    cell.textContent = this.getNestedValue(item, col) ?? '';
                    row.appendChild(cell);
                });
            } else {
                // Auto-render all properties
                Object.values(item).forEach(value => {
                    const cell = document.createElement('td');
                    cell.textContent = value ?? '';
                    row.appendChild(cell);
                });
            }

            tbody.appendChild(row);
        });
    }

    /**
     * Render empty message
     */
    static renderEmpty(tbody, message) {
        const colCount = tbody.closest('table')?.querySelector('thead tr')?.children.length || 1;
        tbody.innerHTML = `<tr><td colspan="${colCount}" class="text-center text-muted">${message}</td></tr>`;
    }

    /**
     * Clear table
     */
    static clear(element) {
        const tbody = element.querySelector('tbody') || element;
        tbody.innerHTML = '';
    }

    /**
     * Get nested object value by dot notation
     */
    static getNestedValue(obj, path) {
        return path.split('.').reduce((o, k) => o?.[k], obj);
    }

    /**
     * Auto-initialize all AJAX tables with data-ajax-table attribute
     */
    static autoInit() {
        document.querySelectorAll('[data-ajax-table]').forEach(el => {
            this.init(el);
        });
    }
}

// Export for non-module usage
if (typeof window !== 'undefined') {
    window.AjaxTableComponent = AjaxTableComponent;
}

export default AjaxTableComponent;
