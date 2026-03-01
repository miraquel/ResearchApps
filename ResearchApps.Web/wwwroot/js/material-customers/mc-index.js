/**
 * Material Customer Index Page Component
 * Handles list filtering, sorting, pagination, and htmx integration
 */
function mcIndex() {
    return {
        // State
        isLoading: false,
        
        // Filters
        filters: {
            McId: '',
            McDateFrom: '',
            McDateTo: '',
            CustomerName: '',
            SjNo: '',
            RefNo: '',
            McStatusId: ''
        },
        
        // Sorting
        sortBy: 'McId',
        sortAsc: false,
        
        // Date range picker
        dateRangePicker: null,
        
        init() {
            // Listen for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.target.id === 'mc-list-container' || e.target.closest('#mc-list-container')) {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.target.id === 'mc-list-container' || e.target.closest('#mc-list-container')) {
                    this.isLoading = false;

                    // Initialize date range picker
                    this.initializeDatePicker();
                }
            });
            
            document.body.addEventListener('htmx:responseError', (e) => {
                this.isLoading = false;
                console.error('HTMX error:', e.detail);
            });
            
            document.body.addEventListener('htmx:sendError', (e) => {
                this.isLoading = false;
                console.error('HTMX send error:', e.detail);
            });
        },
        
        initializeDatePicker() {
            if (this.dateRangePicker) {
                this.dateRangePicker.destroy();
            }

            // Initialize flatpickr for date range
            const dateInput = document.querySelector('#mc-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,  // Enable wrap mode for data-input and data-clear
                mode: 'range',
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                allowInput: false,
                onChange: (selectedDates, dateStr, instance) => {
                    if (selectedDates.length === 2) {
                        this.filters.McDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.McDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 0) {
                        this.filters.McDateFrom = '';
                        this.filters.McDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            // Restore previously selected dates if they exist
            if (this.filters.McDateFrom && this.filters.McDateTo) {
                this.dateRangePicker.setDate([this.filters.McDateFrom, this.filters.McDateTo], false);
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
                McId: '',
                McDateFrom: '',
                McDateTo: '',
                CustomerName: '',
                SjNo: '',
                RefNo: '',
                McStatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = 'McId';
            this.sortAsc = false;
            this.fetchList();
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('mc-list-container');
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
            
            const url = `/MaterialCustomers/List?${params.toString()}`;
            
            // Use htmx to fetch
            htmx.ajax('GET', url, {
                target: '#mc-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        },
        
        getSortIcon(column) {
            if (this.sortBy !== column) {
                return 'bi bi-arrow-down-up text-muted';
            }
            return this.sortAsc ? 'bi bi-sort-up' : 'bi bi-sort-down';
        }
    };
}

// Make available globally
window.mcIndex = mcIndex;
