/**
 * Penyesuaian Stock Index Page Component
 * Handles listing, filtering, sorting, and pagination of penyesuaian stock
 */
function psIndex() {
    return {
        isLoading: false,
        sortBy: 'PsId',
        sortAsc: false,
        dateRangePicker: null,
        filters: {
            PsId: '',
            PsDateFrom: '',
            PsDateTo: '',
            Descr: '',
            AmountOperator: '=',
            Amount: '',
            PsStatusId: ''
        },
        
        init() {
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'ps-list-container') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'ps-list-container') {
                    this.isLoading = false;
                    // Initialize flatpickr after content is swapped in
                    this.initializeDatePicker();
                }
            });
            
            // Handle errors - response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'ps-list-container') {
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
                if (e.detail.target && e.detail.target.id === 'ps-list-container') {
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
            const dateInput = document.querySelector('#ps-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                allowInput: false,
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        // Range selected - fetch results
                        this.filters.PsDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.PsDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 1) {
                        // First date selected - just store it, don't fetch yet
                        this.filters.PsDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.PsDateTo = '';
                    } else {
                        // Cleared - reset and fetch
                        this.filters.PsDateFrom = '';
                        this.filters.PsDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            // Restore previously selected dates if they exist
            if (this.filters.PsDateFrom && this.filters.PsDateTo) {
                this.dateRangePicker.setDate([this.filters.PsDateFrom, this.filters.PsDateTo], false);
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
                PsId: '',
                PsDateFrom: '',
                PsDateTo: '',
                Descr: '',
                AmountOperator: '=',
                Amount: '',
                PsStatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = 'PsId';
            this.sortAsc = false;
            this.fetchList();
        },
        
        /**
         * Format number for display with thousand separators
         */
        formatNumber(value) {
            if (!value || value === '') return '';
            const num = typeof value === 'string' ? parseFloat(value.replace(/,/g, '')) : value;
            if (isNaN(num)) return '';
            return num.toLocaleString('en-US', {
                minimumFractionDigits: 0,
                maximumFractionDigits: 2
            });
        },
        
        /**
         * Parse number from formatted string (remove thousand separators)
         */
        parseNumber(value) {
            if (!value || value === '') return '';
            return value.toString().replace(/,/g, '');
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('ps-list-container');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            // Add column filters
            Object.entries(this.filters).forEach(([key, value]) => {
                // Skip AmountOperator and Amount individually, handle them together below
                if (key === 'AmountOperator' || key === 'Amount') {
                    return;
                }
                if (value && value.trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            // Only add Amount filters if both operator and amount are provided
            if (this.filters.Amount && this.filters.Amount.trim() !== '') {
                params.set('filters[AmountOperator]', this.filters.AmountOperator);
                params.set('filters[Amount]', this.filters.Amount);
            }
            
            // Add sort parameters
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/Pss/List?${params.toString()}`;
            
            // Use htmx to fetch
            htmx.ajax('GET', url, {
                target: '#ps-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        }
    };
}

// Make available globally
window.psIndex = psIndex;
