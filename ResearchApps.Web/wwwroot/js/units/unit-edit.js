/**
 * Unit Edit Page Component
 * Handles form initialization for unit editing with status dropdown
 * @param {number} initialStatusId - Pre-selected Status ID for edit mode
 * @returns {Object} Alpine.js component
 */
function unitEdit(initialStatusId) {
    return {
        /**
         * Initialize component and populate status dropdown
         * @returns {void}
         */
        init() {
            this.initStatusSelect(initialStatusId);
            console.log('[Unit Edit] Component initialized');
        },

        /**
         * Initialize status dropdown with Active/Inactive options and pre-select value
         * @param {number} selectedStatusId - Status ID to pre-select
         * @returns {void}
         */
        initStatusSelect(selectedStatusId) {
            const select = document.getElementById('statusSelect');
            if (!select) return;

            // Add status options
            const options = [
                { value: '1', text: 'Active' },
                { value: '0', text: 'Inactive' }
            ];

            options.forEach(opt => {
                const option = document.createElement('option');
                option.value = opt.value;
                option.textContent = opt.text;
                select.appendChild(option);
            });

            // Set pre-selected value
            if (selectedStatusId !== null && selectedStatusId !== undefined) {
                select.value = selectedStatusId.toString();
                console.log('[Unit Edit] Pre-selected Status ID:', selectedStatusId);
            }
        }
    };
}

// Make available globally
window.unitEdit = unitEdit;
