/**
 * Supplier edit page component
 * Handles TomSelect initialization for TOP and Status dropdowns with pre-selected values
 * @param {number} initialTopId - Pre-selected TOP ID for edit mode
 * @param {number} initialStatusId - Pre-selected Status ID for edit mode
 * @returns {Object} Alpine.js component
 */
function supplierEdit(initialTopId, initialStatusId) {
    return {
        /** @type {TomSelect|null} TomSelect instance for TOP dropdown */
        topSelect: null,

        /** @type {TomSelect|null} TomSelect instance for Status dropdown */
        statusSelect: null,

        /**
         * Initialize component and TomSelect with selected values
         * @returns {void}
         */
        init() {
            this.initTopSelect(initialTopId);
            this.initStatusSelect(initialStatusId);
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
         * Loads TOP options from API and sets pre-selected value
         * @param {number} selectedTopId - TOP ID to pre-select
         * @returns {void}
         */
        /**
         * Initialize TomSelect for Status dropdown
         * Loads Status options from API and sets pre-selected value
         * @param {number} selectedStatusId - Status ID to pre-select
         * @returns {void}
         */
        initStatusSelect(selectedStatusId) {
            const self = this;
            try {
                this.statusSelect = new TomSelect('#StatusId', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const response = await fetch('/api/Status/cbo', {
                                headers: { 'X-TomSelect': 'true' }
                            });

                            if (!response.ok) {
                                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                            }

                            const data = await response.json();
                            callback(data);

                            if (selectedStatusId !== null && self.statusSelect) {
                                setTimeout(() => self.statusSelect.setValue(selectedStatusId.toString()), 100);
                            }
                        } catch (error) {
                            console.error('[Supplier Edit] Error loading Status data:', error);
                            callback();
                        }
                    },
                    placeholder: '-- Select Status --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function() { this.load(''); }
                });

                console.log('[Supplier Edit] Status TomSelect initialized successfully');
            } catch (error) {
                console.error('[Supplier Edit] Error initializing Status TomSelect:', error);
                showError('Failed to initialize Status dropdown. Please refresh the page.');
            }
        },

        initTopSelect(selectedTopId) {
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
                            
                            if (selectedTopId) {
                                setTimeout(() => this.topSelect.setValue(selectedTopId.toString()), 100);
                            }
                        } catch (error) {
                            console.error('[Supplier Edit] Error loading TOP data:', error);
                            callback();
                        }
                    },
                    placeholder: '-- Select TOP --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function() { this.load(''); }
                });

                console.log('[Supplier Edit] TomSelect initialized successfully');
            } catch (error) {
                console.error('[Supplier Edit] Error initializing TomSelect:', error);
                showError('Failed to initialize TOP dropdown. Please refresh the page.');
            }
        }
    };
}
