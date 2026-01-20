/**
 * Supplier create page component
 * Handles TomSelect initialization for TOP dropdown
 * @returns {Object} Alpine.js component
 */
function supplierCreate() {
    return {
        /** @type {TomSelect|null} TomSelect instance for TOP dropdown */
        topSelect: null,

        /**
         * Initialize component and TomSelect
         * @returns {void}
         */
        init() {
            this.initTopSelect();
        },

        /**
         * Initialize TomSelect for TOP (Terms of Payment) dropdown
         * Loads TOP options from API endpoint
         * @returns {void}
         */
        initTopSelect() {
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
                        } catch (error) {
                            console.error('[Supplier Create] Error loading TOP data:', error);
                            showError('Failed to load TOP options. Please refresh the page.');
                            callback();
                        }
                    },
                    placeholder: '-- Select TOP --',
                    allowEmptyOption: true,
                    create: false
                });

                console.log('[Supplier Create] TomSelect initialized successfully');
            } catch (error) {
                console.error('[Supplier Create] Error initializing TomSelect:', error);
                showError('Failed to initialize TOP dropdown. Please refresh the page.');
            }
        }
    };
}
