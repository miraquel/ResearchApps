/**
 * Sales Price Index Page Component
 * Handles listing, filtering, sorting, and pagination of sales prices with HTMX
 */
function salesPriceIndex() {
    return {
        isLoading: false,
        sortBy: 'RecId',
        sortAsc: false,
        filters: {
            RecId: '',
            ItemName: '',
            CustomerName: '',
            StartDate: '',
            EndDate: '',
            SalesPrice: '',
            StatusId: ''
        },

        /**
         * Initialize component and setup HTMX event listeners
         * @returns {void}
         */
        init() {
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'sales-price-list-container') {
                    this.isLoading = true;
                }
            });

            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'sales-price-list-container') {
                    this.isLoading = false;
                }
            });

            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'sales-price-list-container') {
                    this.isLoading = false;
                    const status = e.detail.xhr.status;
                    const statusText = e.detail.xhr.statusText || 'Error';
                    e.detail.target.innerHTML = `
                        <div class="text-center py-5">
                            <div class="mb-3">
                                <i class="ri-error-warning-line text-danger" style="font-size: 3rem;"></i>
                            </div>
                            <h5 class="text-danger">Failed to Load Data</h5>
                            <p class="text-muted">Server returned ${status} ${statusText}</p>
                            <button type="button" class="btn btn-primary" onclick="location.reload()">
                                <i class="ri-refresh-line me-1"></i> Reload Page
                            </button>
                        </div>
                    `;
                }
            });

            document.body.addEventListener('htmx:sendError', (e) => {
                if (e.detail.target && e.detail.target.id === 'sales-price-list-container') {
                    this.isLoading = false;
                    e.detail.target.innerHTML = `
                        <div class="text-center py-5">
                            <div class="mb-3">
                                <i class="ri-wifi-off-line text-warning" style="font-size: 3rem;"></i>
                            </div>
                            <h5 class="text-warning">Network Error</h5>
                            <p class="text-muted">Unable to connect to the server. Please check your connection.</p>
                            <button type="button" class="btn btn-primary" onclick="location.reload()">
                                <i class="ri-refresh-line me-1"></i> Retry
                            </button>
                        </div>
                    `;
                }
            });

            // Initial data load
            this.fetchList();
        },

        /**
         * Sort table by column
         * @param {string} column - Column name to sort by
         * @returns {void}
         */
        sort(column) {
            if (this.sortBy === column) {
                this.sortAsc = !this.sortAsc;
            } else {
                this.sortBy = column;
                this.sortAsc = true;
            }
            this.fetchList();
        },

        /**
         * Clear all filters and reset sort to default
         * @returns {void}
         */
        clearFilters() {
            this.filters = {
                RecId: '',
                ItemName: '',
                CustomerName: '',
                StartDate: '',
                EndDate: '',
                SalesPrice: '',
                StatusId: ''
            };
            this.sortBy = 'RecId';
            this.sortAsc = false;
            this.fetchList();
        },

        /**
         * Fetch sales price list with current filters, sorting, and pagination
         * @param {number} page - Page number to fetch (default: 1)
         * @returns {void}
         */
        fetchList(page = 1) {
            const container = document.getElementById('sales-price-list-container');
            if (!container) return;

            const params = new URLSearchParams();
            params.set('page', page);

            // Add column filters
            Object.entries(this.filters).forEach(([key, value]) => {
                if (value && value.toString().trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });

            // Add sort parameters
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }

            const url = `/SalesPrices/List?${params.toString()}`;

            htmx.ajax('GET', url, {
                target: '#sales-price-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        }
    };
}

// Make available globally
window.salesPriceIndex = salesPriceIndex;
