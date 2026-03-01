/**
 * Sales Price Edit Page Component
 * Handles TomSelect and Flatpickr initialization for Item, Customer, and date fields
 * @param {number|null} initialItemId - Initial Item ID to preselect
 * @param {string|null} initialItemName - Initial Item Name for display
 * @param {number|null} initialCustomerId - Initial Customer ID to preselect
 * @param {string|null} initialCustomerName - Initial Customer Name for display
 * @returns {Object} Alpine.js component
 */
function salesPriceEdit(initialItemId = null, initialItemName = null, initialCustomerId = null, initialCustomerName = null, initialSalesPriceValue = 0) {
    return {
        /** @type {TomSelect|null} */
        itemSelect: null,
        /** @type {TomSelect|null} */
        customerSelect: null,
        /** @type {Object|null} */
        startDatePicker: null,
        /** @type {Object|null} */
        endDatePicker: null,
        salesPriceValue: initialSalesPriceValue,
        initialItemId: initialItemId,
        initialItemName: initialItemName,
        initialCustomerId: initialCustomerId,
        initialCustomerName: initialCustomerName,

        init() {
            this.initItemSelect();
            this.initCustomerSelect();
            this.initStartDatePicker();
            this.initEndDatePicker();
        },

        /**
         * Initialize Flatpickr for Start Date
         */
        initStartDatePicker() {
            this.startDatePicker = initFlatpickr('#StartDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y'
            });
        },

        /**
         * Initialize Flatpickr for End Date
         */
        initEndDatePicker() {
            this.endDatePicker = initFlatpickr('#EndDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y'
            });
        },

        /**
         * Initialize TomSelect for Item dropdown
         */
        initItemSelect() {
            const self = this;
            try {
                this.itemSelect = new TomSelect('#ItemId', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const params = new URLSearchParams();
                            if (query) params.set('term', query);
                            const response = await fetch(`/api/Items/cbo?${params.toString()}`, {
                                headers: { 'X-TomSelect': 'true' }
                            });
                            if (!response.ok) throw new Error(`HTTP ${response.status}`);
                            const data = await response.json();
                            callback(data);

                            // Preselect the initial value
                            if (self.initialItemId !== null && self.itemSelect) {
                                setTimeout(() => {
                                    // Add the option if not loaded
                                    if (!self.itemSelect.options[self.initialItemId]) {
                                        self.itemSelect.addOption({
                                            value: self.initialItemId.toString(),
                                            text: self.initialItemName || ''
                                        });
                                    }
                                    self.itemSelect.setValue(self.initialItemId.toString());
                                }, 100);
                            }
                        } catch (error) {
                            console.error('[Sales Price Edit] Error loading Items:', error);
                            callback();
                        }
                    },
                    placeholder: '-- Select Item --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function () {
                        this.load('');
                    }
                });
            } catch (error) {
                console.error('[Sales Price Edit] Error initializing Item TomSelect:', error);
            }
        },

        /**
         * Initialize TomSelect for Customer dropdown
         */
        initCustomerSelect() {
            const self = this;
            try {
                this.customerSelect = new TomSelect('#CustomerId', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const params = new URLSearchParams();
                            if (query) params.set('term', query);
                            const response = await fetch(`/api/Customers/cbo?${params.toString()}`, {
                                headers: { 'X-TomSelect': 'true' }
                            });
                            if (!response.ok) throw new Error(`HTTP ${response.status}`);
                            const data = await response.json();
                            callback(data);

                            // Preselect the initial value
                            if (self.initialCustomerId !== null && self.customerSelect) {
                                setTimeout(() => {
                                    if (!self.customerSelect.options[self.initialCustomerId]) {
                                        self.customerSelect.addOption({
                                            value: self.initialCustomerId.toString(),
                                            text: self.initialCustomerName || ''
                                        });
                                    }
                                    self.customerSelect.setValue(self.initialCustomerId.toString());
                                }, 100);
                            }
                        } catch (error) {
                            console.error('[Sales Price Edit] Error loading Customers:', error);
                            callback();
                        }
                    },
                    placeholder: '-- Select Customer --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function () {
                        this.load('');
                    }
                });
            } catch (error) {
                console.error('[Sales Price Edit] Error initializing Customer TomSelect:', error);
            }
        }
    };
}

/**
 * Sales Price Create Page Component
 * Handles TomSelect and Flatpickr initialization for Item, Customer, and date fields
 * @returns {Object} Alpine.js component
 */
function salesPriceCreate() {
    return {
        /** @type {TomSelect|null} */
        itemSelect: null,
        /** @type {TomSelect|null} */
        customerSelect: null,
        /** @type {Object|null} */
        startDatePicker: null,
        /** @type {Object|null} */
        endDatePicker: null,
        salesPriceValue: 0,

        init() {
            this.initItemSelect();
            this.initCustomerSelect();
            this.initStartDatePicker();
            this.initEndDatePicker();
        },

        /**
         * Initialize Flatpickr for Start Date (defaults to today)
         */
        initStartDatePicker() {
            this.startDatePicker = initFlatpickr('#StartDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: 'today'
            });
        },

        /**
         * Initialize Flatpickr for End Date
         */
        initEndDatePicker() {
            this.endDatePicker = initFlatpickr('#EndDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y'
            });
        },

        /**
         * Initialize TomSelect for Item dropdown
         */
        initItemSelect() {
            try {
                this.itemSelect = new TomSelect('#ItemId', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const params = new URLSearchParams();
                            if (query) params.set('term', query);
                            const response = await fetch(`/api/Items/cbo?${params.toString()}`, {
                                headers: { 'X-TomSelect': 'true' }
                            });
                            if (!response.ok) throw new Error(`HTTP ${response.status}`);
                            const data = await response.json();
                            callback(data);
                        } catch (error) {
                            console.error('[Sales Price Create] Error loading Items:', error);
                            callback();
                        }
                    },
                    placeholder: '-- Select Item --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function () {
                        this.load('');
                    }
                });
            } catch (error) {
                console.error('[Sales Price Create] Error initializing Item TomSelect:', error);
            }
        },

        /**
         * Initialize TomSelect for Customer dropdown
         */
        initCustomerSelect() {
            try {
                this.customerSelect = new TomSelect('#CustomerId', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const params = new URLSearchParams();
                            if (query) params.set('term', query);
                            const response = await fetch(`/api/Customers/cbo?${params.toString()}`, {
                                headers: { 'X-TomSelect': 'true' }
                            });
                            if (!response.ok) throw new Error(`HTTP ${response.status}`);
                            const data = await response.json();
                            callback(data);
                        } catch (error) {
                            console.error('[Sales Price Create] Error loading Customers:', error);
                            callback();
                        }
                    },
                    placeholder: '-- Select Customer --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function () {
                        this.load('');
                    }
                });
            } catch (error) {
                console.error('[Sales Price Create] Error initializing Customer TomSelect:', error);
            }
        }
    };
}

// Make available globally
window.salesPriceEdit = salesPriceEdit;
window.salesPriceCreate = salesPriceCreate;
