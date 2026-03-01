/**
 * Report Inventory Transactions Index Page Component
 * Handles filtering and generating inventory transaction reports with HTMX
 */
function repInventTransIndex() {
    return {
        isLoading: false,
        isExporting: false,
        itemId: null,
        startDate: '',
        endDate: '',
        resultCount: 0,
        itemSelect: null,
        startDatePicker: null,
        endDatePicker: null,

        /**
         * Initialize component with TomSelect and Flatpickr
         */
        init() {
            // Initialize TomSelect for Item dropdown
            this.itemSelect = initTomSelect('#ItemId', {
                url: '/api/Items/cbo',
                placeholder: 'Search and select item...',
                maxOptions: 50
            });

            // Bind item selection
            if (this.itemSelect) {
                this.itemSelect.on('change', (value) => {
                    this.itemId = value ? parseInt(value) : null;
                });
            }

            // Initialize Flatpickr for date fields
            this.startDatePicker = flatpickr(this.$refs.startDateInput, {
                dateFormat: 'd M Y',
                allowInput: false,
                onChange: (selectedDates) => {
                    this.startDate = selectedDates.length > 0 
                        ? selectedDates[0].toISOString().split('T')[0] 
                        : '';
                }
            });

            this.endDatePicker = flatpickr(this.$refs.endDateInput, {
                dateFormat: 'd M Y',
                allowInput: false,
                onChange: (selectedDates) => {
                    this.endDate = selectedDates.length > 0 
                        ? selectedDates[0].toISOString().split('T')[0] 
                        : '';
                }
            });

            // Listen for HTMX events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'result-container') {
                    this.isLoading = true;
                }
            });

            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'result-container') {
                    this.isLoading = false;
                    const countEl = document.getElementById('result-count');
                    this.resultCount = countEl ? parseInt(countEl.value) : 0;
                }
            });

            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'result-container') {
                    this.isLoading = false;
                    e.detail.target.innerHTML = `
                        <div class="text-center py-5">
                            <div class="mb-3">
                                <i class="ri-error-warning-line text-danger" style="font-size: 3rem;"></i>
                            </div>
                            <h5 class="text-danger">Failed to Load Data</h5>
                            <p class="text-muted">Server returned ${e.detail.xhr.status} ${e.detail.xhr.statusText}</p>
                        </div>
                    `;
                }
            });

            document.body.addEventListener('htmx:sendError', (e) => {
                if (e.detail.target && e.detail.target.id === 'result-container') {
                    this.isLoading = false;
                    e.detail.target.innerHTML = `
                        <div class="text-center py-5">
                            <div class="mb-3">
                                <i class="ri-wifi-off-line text-warning" style="font-size: 3rem;"></i>
                            </div>
                            <h5 class="text-warning">Network Error</h5>
                            <p class="text-muted">Unable to connect to the server.</p>
                        </div>
                    `;
                }
            });
        },

        /**
         * Generate the report with current filters
         */
        generate() {
            if (!this.itemId) return;

            const params = new URLSearchParams();
            params.set('itemId', this.itemId);
            if (this.startDate) params.set('startDate', this.startDate);
            if (this.endDate) params.set('endDate', this.endDate);

            const url = `/RepInventTrans/Results?${params.toString()}`;

            htmx.ajax('GET', url, {
                target: '#result-container',
                swap: 'innerHTML'
            });
        },

        /**
         * Clear all filters
         */
        clearFilters() {
            this.itemId = null;
            this.startDate = '';
            this.endDate = '';
            this.resultCount = 0;

            if (this.itemSelect) this.itemSelect.clear();
            if (this.startDatePicker) this.startDatePicker.clear();
            if (this.endDatePicker) this.endDatePicker.clear();

            document.getElementById('result-container').innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="ri-file-search-line" style="font-size: 3rem;"></i>
                    <p class="mb-0 mt-2">Select an item and click Generate to view results</p>
                </div>
            `;
        },

        /**
         * Export results to Excel
         */
        async exportToExcel() {
            if (!this.itemId || this.resultCount === 0) return;
            this.isExporting = true;

            try {
                const table = document.querySelector('#result-container table');
                if (!table) throw new Error('No table data to export');

                const { utils, writeFile } = XLSX || window.XLSX;
                if (!utils) {
                    // Fallback: HTML table export
                    this.exportTableToCSV();
                    return;
                }

                const wb = utils.book_new();
                const ws = utils.table_to_sheet(table);
                utils.book_append_sheet(wb, ws, 'InventTrans');
                writeFile(wb, `InventTrans_Report_${new Date().toISOString().slice(0, 10)}.xlsx`);

                if (window.showSuccess) {
                    window.showSuccess('Excel file exported successfully');
                }
            } catch (error) {
                console.error('[RepInventTrans] Export error:', error);
                // Fallback to CSV
                this.exportTableToCSV();
            } finally {
                this.isExporting = false;
            }
        },

        /**
         * Fallback CSV export
         */
        exportTableToCSV() {
            const table = document.querySelector('#result-container table');
            if (!table) return;

            let csv = [];
            const rows = table.querySelectorAll('tr');

            rows.forEach(row => {
                const cols = row.querySelectorAll('td, th');
                const rowData = [];
                cols.forEach(col => rowData.push('"' + col.innerText.replace(/"/g, '""') + '"'));
                csv.push(rowData.join(','));
            });

            const csvContent = csv.join('\n');
            const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `InventTrans_Report_${new Date().toISOString().slice(0, 10)}.csv`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);

            if (window.showSuccess) {
                window.showSuccess('CSV file exported successfully');
            }
        }
    };
}

window.repInventTransIndex = repInventTransIndex;
