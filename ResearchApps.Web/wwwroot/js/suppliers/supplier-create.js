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
            this.initEmailTrim();
        },

        /**
         * Normalise email field before form submission
         * Trims whitespace around each semicolon-separated address
         * @returns {void}
         */
        initEmailTrim() {
            const form = this.$el.querySelector('form');
            if (!form) return;
            form.addEventListener('submit', () => {
                const emailInput = form.querySelector('input[name="Email"]');
                if (emailInput) {
                    emailInput.value = emailInput.value
                        .split(';')
                        .map(e => e.trim())
                        .filter(e => e.length > 0)
                        .join(';');
                }
            });
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
                    preload: true,
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
                            callback();
                        }
                    },
                    placeholder: '-- Select TOP --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function() { this.load(''); }
                });

                console.log('[Supplier Create] TomSelect initialized successfully');
            } catch (error) {
                console.error('[Supplier Create] Error initializing TomSelect:', error);
                showError('Failed to initialize TOP dropdown. Please refresh the page.');
            }
        }
    };
}
