/**
 * Item Type Edit Page Component
 * Handles TomSelect initialization for Status dropdown with preselected value
 * @param {number|null} initialStatusId - Initial Status ID to preselect
 * @returns {Object} Alpine.js component
 */
function itemTypeEdit(initialStatusId = null) {
    return {
        /** @type {TomSelect|null} TomSelect instance for Status dropdown */
        statusSelect: null,

        /** @type {number|null} Initial Status ID */
        initialStatusId: initialStatusId,

        /**
         * Initialize component and TomSelect
         * @returns {void}
         */
        init() {
            this.initStatusSelect();
        },

        /**
         * Initialize TomSelect for Status dropdown
         * Loads Status options from API endpoint and preselects the initial value
         * @returns {void}
         */
        initStatusSelect() {
            const self = this;

            try {
                this.statusSelect = new TomSelect('#StatusId', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const response = await fetch('/api/Status/cbo', {
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
                            if (self.initialStatusId !== null && self.statusSelect) {
                                setTimeout(() => {
                                    self.statusSelect.setValue(self.initialStatusId.toString());
                                }, 100);
                            }
                        } catch (error) {
                            console.error('[Item Type Edit] Error loading Status data:', error);
                            if (typeof showError === 'function') {
                                showError('Failed to load Status options. Please refresh the page.');
                            }
                            callback();
                        }
                    },
                    placeholder: '-- Select Status --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function() {
                        // Trigger load on initialize
                        this.load('');
                    }
                });

                console.log('[Item Type Edit] TomSelect initialized successfully');
            } catch (error) {
                console.error('[Item Type Edit] Error initializing TomSelect:', error);
                if (typeof showError === 'function') {
                    showError('Failed to initialize Status dropdown. Please refresh the page.');
                }
            }
        }
    };
}

/**
 * Item Type Create Page Component
 * Handles TomSelect initialization for Status dropdown
 * @returns {Object} Alpine.js component
 */
function itemTypeCreate() {
    return {
        /** @type {TomSelect|null} TomSelect instance for Status dropdown */
        statusSelect: null,

        /**
         * Initialize component and TomSelect
         * @returns {void}
         */
        init() {
            this.initStatusSelect();
        },

        /**
         * Initialize TomSelect for Status dropdown
         * Loads Status options from API endpoint
         * @returns {void}
         */
        initStatusSelect() {
            try {
                this.statusSelect = new TomSelect('#StatusId', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const response = await fetch('/api/Status/cbo', {
                                headers: {
                                    'X-TomSelect': 'true'
                                }
                            });

                            if (!response.ok) {
                                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                            }

                            const data = await response.json();
                            callback(data);

                            // Set default value to Active (1)
                            if (this && this.setValue) {
                                setTimeout(() => {
                                    this.setValue('1');
                                }, 100);
                            }
                        } catch (error) {
                            console.error('[Item Type Create] Error loading Status data:', error);
                            if (typeof showError === 'function') {
                                showError('Failed to load Status options. Please refresh the page.');
                            }
                            callback();
                        }
                    },
                    placeholder: '-- Select Status --',
                    allowEmptyOption: false,
                    create: false
                });

                console.log('[Item Type Create] TomSelect initialized successfully');
            } catch (error) {
                console.error('[Item Type Create] Error initializing TomSelect:', error);
                if (typeof showError === 'function') {
                    showError('Failed to initialize Status dropdown. Please refresh the page.');
                }
            }
        }
    };
}

// Make available globally
window.itemTypeEdit = itemTypeEdit;
window.itemTypeCreate = itemTypeCreate;
