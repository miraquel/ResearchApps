/**
 * Report Custom Index Page Component
 * Handles Tools and Tools Analysis reports with tabs
 */
function repCustomIndex() {
    return {
        activeTab: 'tools',
        isExporting: false,

        // Tools tab state
        isLoadingTools: false,
        toolsYear: new Date().getFullYear(),
        toolsMonth: new Date().getMonth() + 1,
        toolsResultCount: 0,

        // Tools Analysis tab state
        isLoadingAnalysis: false,
        analysisYear: new Date().getFullYear(),
        analysisMonth: new Date().getMonth() + 1,
        analysisResultCount: 0,

        /**
         * Initialize component
         */
        init() {
            // HTMX event listeners for Tools
            this.setupHtmxListeners('tools-result-container', 'tools-result-count',
                (count) => { this.toolsResultCount = count; },
                (loading) => { this.isLoadingTools = loading; }
            );

            // HTMX event listeners for Tools Analysis
            this.setupHtmxListeners('analysis-result-container', 'analysis-result-count',
                (count) => { this.analysisResultCount = count; },
                (loading) => { this.isLoadingAnalysis = loading; }
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
         * Generate Tools report
         */
        generateTools() {
            if (!this.toolsYear || !this.toolsMonth) return;

            const params = new URLSearchParams();
            params.set('year', this.toolsYear);
            params.set('month', this.toolsMonth);

            htmx.ajax('GET', `/RepCustom/ToolsResults?${params.toString()}`, {
                target: '#tools-result-container',
                swap: 'innerHTML'
            });
        },

        /**
         * Generate Tools Analysis report
         */
        generateAnalysis() {
            if (!this.analysisYear || !this.analysisMonth) return;

            const params = new URLSearchParams();
            params.set('year', this.analysisYear);
            params.set('month', this.analysisMonth);

            htmx.ajax('GET', `/RepCustom/ToolsAnalysisResults?${params.toString()}`, {
                target: '#analysis-result-container',
                swap: 'innerHTML'
            });
        },

        /**
         * Clear Tools filters
         */
        clearToolsFilters() {
            this.toolsYear = new Date().getFullYear();
            this.toolsMonth = new Date().getMonth() + 1;
            this.toolsResultCount = 0;

            document.getElementById('tools-result-container').innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="ri-file-search-line" style="font-size: 3rem;"></i>
                    <p class="mb-0 mt-2">Select year and month, then click Generate to view results</p>
                </div>
            `;
        },

        /**
         * Clear Tools Analysis filters
         */
        clearAnalysisFilters() {
            this.analysisYear = new Date().getFullYear();
            this.analysisMonth = new Date().getMonth() + 1;
            this.analysisResultCount = 0;

            document.getElementById('analysis-result-container').innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="ri-file-search-line" style="font-size: 3rem;"></i>
                    <p class="mb-0 mt-2">Select year and month, then click Generate to view results</p>
                </div>
            `;
        },

        /**
         * Export to Excel (CSV fallback)
         * @param {string} type - 'tools' or 'analysis'
         */
        async exportToExcel(type) {
            const containerId = type === 'tools' ? 'tools-result-container' : 'analysis-result-container';
            const fileName = type === 'tools' ? 'Tools_Report' : 'ToolsAnalysis_Report';

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
                console.error('[RepCustom] Export error:', error);
                if (window.showError) window.showError('Failed to export: ' + error.message);
            } finally {
                this.isExporting = false;
            }
        }
    };
}

window.repCustomIndex = repCustomIndex;
