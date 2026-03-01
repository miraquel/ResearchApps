/**
 * Report Stock Index Page Component
 * Handles Stock Card Monthly report
 */
function repStockIndex() {
    return {
        activeTab: 'stockcard',
        isExporting: false,

        // Stock Card Monthly tab state
        isLoadingSc: false,
        scItemId: null,
        scYear: new Date().getFullYear(),
        scMonth: new Date().getMonth() + 1,
        scResultCount: 0,
        scItemSelect: null,

        /**
         * Initialize component
         */
        init() {
            // Init TomSelect for Stock Card item
            this.scItemSelect = initTomSelect('#ScItemId', {
                url: '/api/Items/cbo',
                placeholder: 'Search and select item...',
                maxOptions: 50
            });
            if (this.scItemSelect) {
                this.scItemSelect.on('change', (value) => {
                    this.scItemId = value ? parseInt(value) : null;
                });
            }

            // HTMX event listeners for Stock Card
            this.setupHtmxListeners('sc-result-container', 'sc-result-count',
                (count) => { this.scResultCount = count; },
                (loading) => { this.isLoadingSc = loading; }
            );
        },

        /**
         * Setup HTMX listeners for a container
         */
        setupHtmxListeners(containerId, countId, setCount, setLoading) {
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === containerId) {
                    setLoading(true);
                }
            });

            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === containerId) {
                    setLoading(false);
                    const countEl = document.getElementById(countId);
                    setCount(countEl ? parseInt(countEl.value) : 0);
                }
            });

            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === containerId) {
                    setLoading(false);
                    e.detail.target.innerHTML = `
                        <div class="text-center py-5">
                            <i class="ri-error-warning-line text-danger" style="font-size: 3rem;"></i>
                            <h5 class="text-danger mt-3">Failed to Load Data</h5>
                            <p class="text-muted">Server returned ${e.detail.xhr.status} ${e.detail.xhr.statusText}</p>
                        </div>
                    `;
                }
            });

            document.body.addEventListener('htmx:sendError', (e) => {
                if (e.detail.target && e.detail.target.id === containerId) {
                    setLoading(false);
                    e.detail.target.innerHTML = `
                        <div class="text-center py-5">
                            <i class="ri-wifi-off-line text-warning" style="font-size: 3rem;"></i>
                            <h5 class="text-warning mt-3">Network Error</h5>
                            <p class="text-muted">Unable to connect to the server.</p>
                        </div>
                    `;
                }
            });
        },

        /**
         * Generate Stock Card Monthly report
         */
        generateStockCard() {
            if (!this.scItemId || !this.scYear || !this.scMonth) return;

            const params = new URLSearchParams();
            params.set('itemId', this.scItemId);
            params.set('year', this.scYear);
            params.set('month', this.scMonth);

            htmx.ajax('GET', `/RepStock/StockCardResults?${params.toString()}`, {
                target: '#sc-result-container',
                swap: 'innerHTML'
            });
        },

        /**
         * Clear Stock Card filters
         */
        clearScFilters() {
            this.scItemId = null;
            this.scYear = new Date().getFullYear();
            this.scMonth = new Date().getMonth() + 1;
            this.scResultCount = 0;

            if (this.scItemSelect) this.scItemSelect.clear();

            document.getElementById('sc-result-container').innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="ri-file-search-line" style="font-size: 3rem;"></i>
                    <p class="mb-0 mt-2">Select an item, year, month and click Generate to view results</p>
                </div>
            `;
        },

        /**
         * Export to Excel (CSV fallback)
         * @param {string} type - 'sc' for StockCard
         */
        async exportToExcel(type) {
            const containerId = 'sc-result-container';
            const fileName = 'StockCard_Report';

            this.isExporting = true;
            try {
                const table = document.querySelector(`#${containerId} table`);
                if (!table) throw new Error('No table data to export');

                // Try XLSX library first
                if (typeof XLSX !== 'undefined') {
                    const wb = XLSX.utils.book_new();
                    const ws = XLSX.utils.table_to_sheet(table);
                    XLSX.utils.book_append_sheet(wb, ws, 'Report');
                    XLSX.writeFile(wb, `${fileName}_${new Date().toISOString().slice(0, 10)}.xlsx`);
                } else {
                    // Fallback to CSV
                    let csv = [];
                    table.querySelectorAll('tr').forEach(row => {
                        const rowData = [];
                        row.querySelectorAll('td, th').forEach(col => 
                            rowData.push('"' + col.innerText.replace(/"/g, '""') + '"'));
                        csv.push(rowData.join(','));
                    });

                    const blob = new Blob([csv.join('\n')], { type: 'text/csv;charset=utf-8;' });
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = `${fileName}_${new Date().toISOString().slice(0, 10)}.csv`;
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    document.body.removeChild(a);
                }

                if (window.showSuccess) window.showSuccess('File exported successfully');
            } catch (error) {
                console.error('[RepStock] Export error:', error);
                if (window.showError) window.showError('Failed to export: ' + error.message);
            } finally {
                this.isExporting = false;
            }
        }
    };
}

window.repStockIndex = repStockIndex;
