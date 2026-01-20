/**
 * Delivery Order Details page Alpine.js component
 */
function doDetails(doId, recId) {
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
            // Component initialized
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
         * Format number with thousand separators
         * @param {number} value - Number to format
         * @returns {string} Formatted number string
         */
        formatNumber(value) {
            if (value === null || value === undefined || value === '') return '';
            const num = parseFloat(String(value).replace(/,/g, ''));
            if (isNaN(num)) return value;
            return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
    };
}

// Make available globally
window.doDetails = doDetails;
