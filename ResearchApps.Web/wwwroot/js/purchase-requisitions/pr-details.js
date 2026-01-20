/**
 * Purchase Requisition Details - Alpine.js Component
 */
function prDetails(prId, recId) {
    return {
        prId: prId,
        recId: recId,
        lines: [],
        lineDetailModal: {
            show: false,
            line: null
        },
        showWorkflow: false,

        /**
         * Initialize component
         */
        init() {
            // Load lines data from window if available
            if (window.prLinesData) {
                this.lines = window.prLinesData;
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
        },
        
        /**
         * Format date for display
         */
        formatDate(dateString) {
            if (!dateString) return '-';
            const date = new Date(dateString);
            return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
        }
    };
}

// Make available globally
window.prDetails = prDetails;
