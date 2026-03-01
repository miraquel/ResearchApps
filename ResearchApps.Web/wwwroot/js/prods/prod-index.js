/**
 * Production Order Index Page Component
 * Handles listing, filtering, sorting, and pagination of production orders
 */
function prodIndex() {
    return {
        isLoading: false,
        sortBy: 'ProdId',
        sortAsc: false,
        dateRangePicker: null,
        filters: {
            ProdId: '',
            ProdDateFrom: '',
            ProdDateTo: '',
            CustomerName: '',
            ItemName: '',
            PlanQtyOperator: '=',
            PlanQty: '',
            ResultQtyOperator: '=',
            ResultQty: '',
            ProdStatusId: ''
        },
        
        init() {
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'prod-list-container') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'prod-list-container') {
                    this.isLoading = false;
                    // Initialize flatpickr after content is swapped in
                    this.initializeDatePicker();
                }
            });
            
            // Handle errors - response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'prod-list-container') {
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
            
            // Handle network errors (connection issues, timeouts)
            document.body.addEventListener('htmx:sendError', (e) => {
                if (e.detail.target && e.detail.target.id === 'prod-list-container') {
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
        },
        
        initializeDatePicker() {
            // Destroy existing instance if any
            if (this.dateRangePicker) {
                this.dateRangePicker.destroy();
            }
            
            // Initialize flatpickr for date range
            const dateInput = document.querySelector('#prod-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                allowInput: false,
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        // Range selected - fetch results
                        this.filters.ProdDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.ProdDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 1) {
                        // First date selected - just store it, don't fetch yet
                        this.filters.ProdDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.ProdDateTo = '';
                    } else {
                        // Cleared - reset and fetch
                        this.filters.ProdDateFrom = '';
                        this.filters.ProdDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            // Restore previously selected dates if they exist
            if (this.filters.ProdDateFrom && this.filters.ProdDateTo) {
                this.dateRangePicker.setDate([this.filters.ProdDateFrom, this.filters.ProdDateTo], false);
            }
        },
        
        formatDateISO(date) {
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        },
        
        sort(column) {
            if (this.sortBy === column) {
                this.sortAsc = !this.sortAsc;
            } else {
                this.sortBy = column;
                this.sortAsc = true;
            }
            this.fetchList();
        },
        
        clearFilters() {
            this.filters = {
                ProdId: '',
                ProdDateFrom: '',
                ProdDateTo: '',
                CustomerName: '',
                ItemName: '',
                PlanQtyOperator: '=',
                PlanQty: '',
                ResultQtyOperator: '=',
                ResultQty: '',
                ProdStatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = 'ProdId';
            this.sortAsc = false;
            this.fetchList();
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('prod-list-container');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            // Add column filters
            Object.entries(this.filters).forEach(([key, value]) => {
                // Skip operators individually, handle them together with their values
                if (key === 'PlanQtyOperator' || key === 'ResultQtyOperator') {
                    return;
                }
                if (value && value.toString().trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            // Only add operator-based filters if the value is provided
            if (this.filters.PlanQty && this.filters.PlanQty.toString().trim() !== '') {
                params.set('filters[PlanQtyOperator]', this.filters.PlanQtyOperator);
            }
            if (this.filters.ResultQty && this.filters.ResultQty.toString().trim() !== '') {
                params.set('filters[ResultQtyOperator]', this.filters.ResultQtyOperator);
            }
            
            // Add sort parameters
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/Prods/List?${params.toString()}`;
            
            // Use htmx to fetch
            htmx.ajax('GET', url, {
                target: '#prod-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        }
    };
}

// Make available globally
window.prodIndex = prodIndex;
