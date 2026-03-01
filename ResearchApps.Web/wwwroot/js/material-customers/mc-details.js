/**
 * Material Customer Details page Alpine.js component
 */
function mcDetails(mcId, recId) {
    return {
        isLoading: false,
        lineDetailModal: {
            show: false,
            line: null
        },

        /**
         * Initialize the component
         */
        init() {
            // No workflow component needed for Material Customer
            // Simple document viewing only
        },

        /**
         * Show line detail modal with the provided line data
         * @param {string|object} lineJson - Line data as JSON string or object
         */
        showLineDetailModal(lineJson) {
            try {
                const line = typeof lineJson === 'string' ? JSON.parse(lineJson) : lineJson;
                this.lineDetailModal.line = line;
                this.lineDetailModal.show = true;
            } catch (error) {
                console.error('Failed to parse line data:', error);
            }
        },
        
        /**
         * Format number with Indonesian locale
         */
        formatNumber(value) {
            if (value === null || value === undefined) return '0';
            return new Intl.NumberFormat('id-ID').format(value);
        },
        
        /**
         * Format currency with Indonesian locale
         */
        formatCurrency(value) {
            if (value === null || value === undefined) return 'Rp 0';
            return new Intl.NumberFormat('id-ID', {
                style: 'currency',
                currency: 'IDR',
                minimumFractionDigits: 0
            }).format(value);
        }
    };
}

// Make available globally
window.mcDetails = mcDetails;
