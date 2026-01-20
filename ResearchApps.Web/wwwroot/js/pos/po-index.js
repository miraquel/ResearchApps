/**
 * Purchase Order Index Page - Alpine.js Component
 */
function poIndex() {
    return {
        // State
        currentPage: 1,
        sortBy: 'PoId',
        sortAsc: false,
        filters: {
            PoId: '',
            PoDateFrom: '',
            PoDateTo: '',
            SupplierName: '',
            Pic: '',
            RefNo: '',
            SubTotal: '',
            SubTotalOperator: '=',
            Ppn: '',
            PpnOperator: '=',
            Total: '',
            TotalOperator: '=',
            PoStatusId: ''
        },
        isLoading: false,
        isExporting: false,
        dateRangePicker: null,
        
        // Report Modal State
        showReportModal: false,
        reportType: 'summary',
        reportStartDate: '',
        reportEndDate: '',
        isGeneratingReport: false,

        /**
         * Initialize component
         */
        init() {
            // Listen for HTMX events
            document.body.addEventListener('htmx:beforeRequest', () => {
                this.isLoading = true;
            });

            document.body.addEventListener('htmx:afterRequest', () => {
                this.isLoading = false;
            });

            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'po-list-container') {
                    this.isLoading = false;
                    // Initialize flatpickr after content is swapped in
                    this.initializeDatePicker();
                }
            });

            // Listen for successful workflow actions that should refresh the list
            window.addEventListener('po-updated', () => {
                this.fetchList();
            });
        },

        /**
         * Initialize Flatpickr date range picker
         */
        initializeDatePicker() {
            // Destroy existing instance if any
            if (this.dateRangePicker) {
                this.dateRangePicker.destroy();
            }

            // Initialize flatpickr for date range
            const dateInput = document.querySelector('#po-date-range');
            if (!dateInput) return;

            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                allowInput: false,
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        // Range selected - fetch results
                        this.filters.PoDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.PoDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 1) {
                        // First date selected - just store it, don't fetch yet
                        this.filters.PoDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.PoDateTo = '';
                    } else {
                        // Cleared - reset and fetch
                        this.filters.PoDateFrom = '';
                        this.filters.PoDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            // Restore previously selected dates if they exist
            if (this.filters.PoDateFrom && this.filters.PoDateTo) {
                this.dateRangePicker.setDate([this.filters.PoDateFrom, this.filters.PoDateTo], false);
            }
        },

        /**
         * Sort by column
         */
        sort(column) {
            if (this.sortBy === column) {
                this.sortAsc = !this.sortAsc;
            } else {
                this.sortBy = column;
                this.sortAsc = true;
            }
            this.currentPage = 1;
            this.fetchList();
        },

        /**
         * Go to specific page
         */
        goToPage(page) {
            if (page < 1) return;
            this.currentPage = page;
            this.fetchList();
        },

        /**
         * Apply filters and reset to page 1
         */
        applyFilters() {
            this.currentPage = 1;
            this.fetchList();
        },

        /**
         * Clear all filters
         */
        clearFilters() {
            this.filters = {
                PoId: '',
                PoDateFrom: '',
                PoDateTo: '',
                SupplierName: '',
                Pic: '',
                RefNo: '',
                SubTotal: '',
                SubTotalOperator: '=',
                Ppn: '',
                PpnOperator: '=',
                Total: '',
                TotalOperator: '=',
                PoStatusId: ''
            };
            
            // Clear date picker
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            
            this.currentPage = 1;
            this.sortBy = 'PoDate';
            this.sortAsc = false;
            this.fetchList();
        },

        /**
         * Format date to ISO string (YYYY-MM-DD)
         */
        formatDateISO(date) {
            if (!date) return '';
            const d = new Date(date);
            const year = d.getFullYear();
            const month = String(d.getMonth() + 1).padStart(2, '0');
            const day = String(d.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        },

        /**
         * Format number with thousand separators
         */
        formatNumber(value) {
            if (!value) return '';
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },

        /**
         * Parse formatted number back to plain number
         */
        parseNumber(value) {
            if (!value) return '';
            return value.toString().replace(/,/g, '');
        },

        /**
         * Trigger HTMX fetch for the list
         */
        fetchList() {
            const container = document.getElementById('po-list-container');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', this.currentPage);
            
            // Add column filters
            Object.entries(this.filters).forEach(([key, value]) => {
                // Skip operators individually, handle them with their corresponding values
                if (key === 'TotalOperator' || key === 'SubTotalOperator' || key === 'PpnOperator') {
                    return;
                }
                // Skip numeric values, handle them below with operators
                if (key === 'Total' || key === 'SubTotal' || key === 'Ppn') {
                    return;
                }
                if (value && value.trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            // Only add Total filters if amount is provided
            if (this.filters.Total && this.filters.Total.trim() !== '') {
                params.set('filters[TotalOperator]', this.filters.TotalOperator);
                params.set('filters[Total]', this.filters.Total);
            }
            
            // Only add SubTotal filters if amount is provided
            if (this.filters.SubTotal && this.filters.SubTotal.trim() !== '') {
                params.set('filters[SubTotalOperator]', this.filters.SubTotalOperator);
                params.set('filters[SubTotal]', this.filters.SubTotal);
            }
            
            // Only add Ppn filters if amount is provided
            if (this.filters.Ppn && this.filters.Ppn.trim() !== '') {
                params.set('filters[PpnOperator]', this.filters.PpnOperator);
                params.set('filters[Ppn]', this.filters.Ppn);
            }
            
            // Add sort parameters
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/Pos/List?${params.toString()}`;
            
            // Use htmx to fetch
            htmx.ajax('GET', url, {
                target: '#po-list-container',
                swap: 'innerHTML'
            });
        },

        /**
         * Export to Excel
         */
        async exportToExcel() {
            if (this.isExporting) return;
            
            try {
                this.isExporting = true;
                
                // Build query parameters
                const params = new URLSearchParams();
                params.append('sortBy', this.sortBy);
                params.append('sortAsc', this.sortAsc);

                // Add non-empty filters
                Object.entries(this.filters).forEach(([key, value]) => {
                    if (value && value.trim() !== '') {
                        params.append(`filters[${key}]`, value);
                    }
                });

                // Call export endpoint (to be implemented)
                const url = `/Pos/Export?${params.toString()}`;
                window.location.href = url;
                
                // Reset state after a delay
                setTimeout(() => {
                    this.isExporting = false;
                }, 2000);
            } catch (error) {
                console.error('Export failed:', error);
                alert('Failed to export data. Please try again.');
                this.isExporting = false;
            }
        },

        /**
         * Generate Report
         */
        async generateReport() {
            if (this.isGeneratingReport || !this.reportStartDate || !this.reportEndDate) return;
            
            try {
                this.isGeneratingReport = true;
                
                // Build query parameters
                const params = new URLSearchParams();
                params.append('reportType', this.reportType);
                params.append('startDate', this.reportStartDate);
                params.append('endDate', this.reportEndDate);

                // Add current filters
                Object.entries(this.filters).forEach(([key, value]) => {
                    if (value && value.trim() !== '') {
                        params.append(`filters[${key}]`, value);
                    }
                });

                // Call report endpoint (to be implemented)
                const url = `/Pos/GenerateReport?${params.toString()}`;
                window.open(url, '_blank');
                
                // Close modal and reset state
                this.showReportModal = false;
                this.isGeneratingReport = false;
            } catch (error) {
                console.error('Report generation failed:', error);
                alert('Failed to generate report. Please try again.');
                this.isGeneratingReport = false;
            }
        }
    };
}
