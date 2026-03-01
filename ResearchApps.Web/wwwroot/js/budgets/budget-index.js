/**
 * Budget Index Page Component
 * Handles listing, filtering, sorting, and pagination of budgets with HTMX
 */
function budgetIndex() {
    return {
        isLoading: false,
        sortBy: 'BudgetId',
        sortAsc: false,
        startDateFilterPicker: null,
        endDateFilterPicker: null,
        filters: {
            BudgetId: '',
            Year: '',
            BudgetName: '',
            StartDateFrom: '',
            StartDateTo: '',
            EndDateFrom: '',
            EndDateTo: '',
            Amount: '',
            AmountOperator: '=',
            RemAmount: '',
            RemAmountOperator: '=',
            StatusId: ''
        },

        /**
         * Initialize component and setup HTMX event listeners
         * @returns {void}
         */
        init() {
            this.initDateFilters();

            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'budget-list-container') {
                    this.isLoading = true;
                }
            });

            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'budget-list-container') {
                    this.isLoading = false;
                    // Re-initialize date filter pickers after HTMX swap
                    this.$nextTick(() => this.initDateFilters());
                }
            });

            // Handle errors - response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'budget-list-container') {
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
                if (e.detail.target && e.detail.target.id === 'budget-list-container') {
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

        /**
         * Initialize Flatpickr date range filters
         * @returns {void}
         */
        initDateFilters() {
            const self = this;

            // Start date range filter
            const startDateEl = this.$refs.startDateFilter;
            if (startDateEl && !startDateEl._flatpickr) {
                this.startDateFilterPicker = flatpickr(startDateEl, {
                    mode: 'range',
                    dateFormat: 'Y-m-d',
                    altInput: true,
                    altFormat: 'd M Y',
                    wrap: true,
                    onChange: function (selectedDates) {
                        if (selectedDates.length === 2) {
                            self.filters.StartDateFrom = selectedDates[0].toISOString().split('T')[0];
                            self.filters.StartDateTo = selectedDates[1].toISOString().split('T')[0];
                            self.fetchList();
                        } else if (selectedDates.length === 0) {
                            self.filters.StartDateFrom = '';
                            self.filters.StartDateTo = '';
                            self.fetchList();
                        }
                    }
                });
            }

            // End date range filter
            const endDateEl = this.$refs.endDateFilter;
            if (endDateEl && !endDateEl._flatpickr) {
                this.endDateFilterPicker = flatpickr(endDateEl, {
                    mode: 'range',
                    dateFormat: 'Y-m-d',
                    altInput: true,
                    altFormat: 'd M Y',
                    wrap: true,
                    onChange: function (selectedDates) {
                        if (selectedDates.length === 2) {
                            self.filters.EndDateFrom = selectedDates[0].toISOString().split('T')[0];
                            self.filters.EndDateTo = selectedDates[1].toISOString().split('T')[0];
                            self.fetchList();
                        } else if (selectedDates.length === 0) {
                            self.filters.EndDateFrom = '';
                            self.filters.EndDateTo = '';
                            self.fetchList();
                        }
                    }
                });
            }
        },

        /**
         * Sort table by column
         * Toggles sort direction if same column, otherwise sets new column ascending
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
                BudgetId: '',
                Year: '',
                BudgetName: '',
                StartDateFrom: '',
                StartDateTo: '',
                EndDateFrom: '',
                EndDateTo: '',
                Amount: '',
                AmountOperator: '=',
                RemAmount: '',
                RemAmountOperator: '=',
                StatusId: ''
            };
            this.sortBy = 'BudgetId';
            this.sortAsc = false;

            // Clear Flatpickr instances
            if (this.startDateFilterPicker) this.startDateFilterPicker.clear();
            if (this.endDateFilterPicker) this.endDateFilterPicker.clear();

            this.fetchList();
        },

        /**
         * Fetch budget list with current filters, sorting, and pagination
         * Uses HTMX to load partial view into container
         * @param {number} page - Page number to fetch (default: 1)
         * @returns {void}
         */
        fetchList(page = 1) {
            const container = document.getElementById('budget-list-container');
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

            const url = `/Budgets/List?${params.toString()}`;

            // Use htmx to fetch
            htmx.ajax('GET', url, {
                target: '#budget-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        }
    };
}

// Make available globally
window.budgetIndex = budgetIndex;
