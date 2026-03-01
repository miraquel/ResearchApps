/**
 * Unit Create Page Component
 * Handles form initialization for unit creation
 * @returns {Object} Alpine.js component
 */
function unitCreate() {
    return {
        /**
         * Initialize component
         * @returns {void}
         */
        init() {
            console.log('[Unit Create] Component initialized');
        }
    };
}

// Make available globally
window.unitCreate = unitCreate;
