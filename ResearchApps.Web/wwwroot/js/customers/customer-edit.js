/**
 * Customer edit page component
 * Handles TomSelect initialization for TOP dropdown with preselected value
 * @param {number|null} initialTopId - Initial TOP ID to preselect
 * @returns {Object} Alpine.js component
 */
function customerEdit(initialTopId = null) {
    return {
        /** @type {TomSelect|null} TomSelect instance for TOP dropdown */
        topSelect: null,
        
        /** @type {number|null} Initial TOP ID */
        initialTopId: initialTopId,

        /**
         * Initialize component and TomSelect
         * @returns {void}
         */
        init() {
            this.initTopSelect();
        },

        /**
         * Initialize TomSelect for TOP (Terms of Payment) dropdown
         * Loads TOP options from API endpoint and preselects the initial value
         * @returns {void}
         */
        initTopSelect() {
            const self = this;
            
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
                            
                            // After loading, set the initial value if provided
                            if (self.initialTopId && self.topSelect) {
                                setTimeout(() => {
                                    self.topSelect.setValue(self.initialTopId.toString());
                                }, 100);
                            }
                        } catch (error) {
                            console.error('[Customer Edit] Error loading TOP data:', error);
                            if (typeof showError === 'function') {
                                showError('Failed to load TOP options. Please refresh the page.');
                            }
                            callback();
                        }
                    },
                    placeholder: '-- Select TOP --',
                    allowEmptyOption: true,
                    create: false,
                    onInitialize: function() {
                        // Trigger load on initialize
                        this.load('');
                    }
                });

                console.log('[Customer Edit] TomSelect initialized successfully');
            } catch (error) {
                console.error('[Customer Edit] Error initializing TomSelect:', error);
                if (typeof showError === 'function') {
                    showError('Failed to initialize TOP dropdown. Please refresh the page.');
                }
            }
        }
    };
}

// Make available globally
window.customerEdit = customerEdit;
