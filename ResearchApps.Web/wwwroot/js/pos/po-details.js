/**
 * Purchase Order Details - Alpine.js Component
 */
function poDetails(poId, recId) {
    return {
        poId: poId,
        recId: recId,
        lines: [],
        lineDetailModal: {
            show: false,
            line: null
        },

        /**
         * Initialize component
         */
        init() {
            // Load lines data from window
            if (window.poLinesData) {
                this.lines = window.poLinesData;
            }
        },

        /**
         * Show line detail modal
         */
        showLineDetail(index) {
            if (this.lines[index]) {
                this.lineDetailModal.line = this.lines[index];
                this.lineDetailModal.show = true;
            }
        },

        /**
         * Close line detail modal
         */
        closeLineDetail() {
            this.lineDetailModal.show = false;
        },

        /**
         * Format number for display
         */
        formatNumber(value) {
            const num = parseFloat(value) || 0;
            return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
    };
}
