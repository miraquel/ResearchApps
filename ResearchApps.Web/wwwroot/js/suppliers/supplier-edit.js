/**
 * Supplier edit page component
 * Handles TomSelect initialization for TOP dropdown with pre-selected value
 * @param {number} initialTopId - Pre-selected TOP ID for edit mode
 * @returns {Object} Alpine.js component
 */
function supplierEdit(initialTopId) {
    return {
        /** @type {TomSelect|null} TomSelect instance for TOP dropdown */
        topSelect: null,

        /**
         * Initialize component and TomSelect with selected value
         * @returns {void}
         */
        init() {
            this.initTopSelect(initialTopId);
        },

        /**
         * Initialize TomSelect for TOP (Terms of Payment) dropdown
         * Loads TOP options from API and sets pre-selected value
         * @param {number} selectedTopId - TOP ID to pre-select
         * @returns {void}
         */
        initTopSelect(selectedTopId) {
            try {
                this.topSelect = new TomSelect('#topSelect', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const response = await fetch('/api/Tops/cbo', {
                                headers: {
                                    'X-TomSelect': 'true'
                                }
                            });
                            
                            if (!response.ok) {
                                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                            }
                            
                            const data = await response.json();
                            callback(data);
                            
                            // Set selected value after options are loaded
                            if (selectedTopId) {
                                this.topSelect.setValue(selectedTopId.toString(), true);
                                console.log('[Supplier Edit] Pre-selected TOP ID:', selectedTopId);
                            }
                        } catch (error) {
                            console.error('[Supplier Edit] Error loading TOP data:', error);
                            showError('Failed to load TOP options. Please refresh the page.');
                            callback();
                        }
                    },
                    placeholder: '-- Select TOP --',
                    allowEmptyOption: true,
                    create: false
                });

                console.log('[Supplier Edit] TomSelect initialized successfully');
            } catch (error) {
                console.error('[Supplier Edit] Error initializing TomSelect:', error);
                showError('Failed to initialize TOP dropdown. Please refresh the page.');
            }
        }
    };
}
