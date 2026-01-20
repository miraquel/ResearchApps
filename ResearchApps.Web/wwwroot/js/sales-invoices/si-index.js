/**
 * Sales Invoice Index Page Component
 * Handles listing, filtering, sorting, and pagination of sales invoices
 */
function siIndex() {
    return {
        isLoading: false,
        sortBy: 'SiId',
        sortAsc: false,
        dateRangePicker: null,
        filters: {
            SiId: '',
            SiDateFrom: '',
            SiDateTo: '',
            CustomerName: '',
            PoNo: '',
            TaxNo: '',
            AmountOperator: '=',
            Amount: '',
            SiStatusId: ''
        },
        
        init() {
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'si-list-container') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'si-list-container') {
                    this.isLoading = false;
                    this.initializeDatePicker();
                }
            });
            
            // Handle errors
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'si-list-container') {
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
                if (e.detail.target && e.detail.target.id === 'si-list-container') {
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
            if (this.dateRangePicker) {
                this.dateRangePicker.destroy();
            }
            
            const dateInput = document.querySelector('#si-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                allowInput: false,
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        this.filters.SiDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.SiDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 1) {
                        this.filters.SiDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.SiDateTo = '';
                    } else {
                        this.filters.SiDateFrom = '';
                        this.filters.SiDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            if (this.filters.SiDateFrom && this.filters.SiDateTo) {
                this.dateRangePicker.setDate([this.filters.SiDateFrom, this.filters.SiDateTo], false);
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
                SiId: '',
                SiDateFrom: '',
                SiDateTo: '',
                CustomerName: '',
                PoNo: '',
                TaxNo: '',
                AmountOperator: '=',
                Amount: '',
                SiStatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = 'SiId';
            this.sortAsc = false;
            this.fetchList();
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('si-list-container');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            Object.entries(this.filters).forEach(([key, value]) => {
                if (key === 'AmountOperator' || key === 'Amount') {
                    return;
                }
                if (value && value.trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            if (this.filters.Amount && this.filters.Amount.trim() !== '') {
                params.set('filters[AmountOperator]', this.filters.AmountOperator);
                params.set('filters[Amount]', this.filters.Amount);
            }
            
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/SalesInvoices/List?${params.toString()}`;
            
            htmx.ajax('GET', url, {
                target: '#si-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        },
        
        formatNumber(value) {
            if (!value || value === '' || isNaN(value)) return '';
            return new Intl.NumberFormat('id-ID').format(parseFloat(value));
        },
        
        parseNumber(value) {
            if (!value) return '';
            return value.replace(/[^\d]/g, '');
        }
    };
}
