/**
 * Item edit page component
 * Handles TomSelect initialization with preselected values for ItemType, ItemDept, Unit, Warehouse, and Status dropdowns
 * @param {Object} config - Configuration object with initial values
 * @returns {Object} Alpine.js component
 */
function itemEdit(config = {}) {
    return {
        /** @type {Object} Configuration with initial values */
        config: config,

        /** @type {TomSelect|null} TomSelect instance for ItemType dropdown */
        itemTypeSelect: null,
        
        /** @type {TomSelect|null} TomSelect instance for ItemDept dropdown */
        itemDeptSelect: null,
        
        /** @type {TomSelect|null} TomSelect instance for ItemGroup01 dropdown */
        itemGroup01Select: null,
        
        /** @type {TomSelect|null} TomSelect instance for ItemGroup02 dropdown */
        itemGroup02Select: null,
        
        /** @type {TomSelect|null} TomSelect instance for Unit dropdown */
        unitSelect: null,

        /** @type {TomSelect|null} TomSelect instance for Warehouse dropdown */
        warehouseSelect: null,

        /** @type {TomSelect|null} TomSelect instance for Status dropdown */
        statusSelect: null,

        /** @type {string|null} Image preview data URL */
        imagePreview: null,

        /** @type {string} Image file name */
        imageFileName: '',

        /**
         * Initialize component and TomSelect instances
         * @returns {void}
         */
        init() {
            this.initializeComponents();
            this.preselectValues();
            this.initStatusSelect();
        },

        /**
         * Initialize all TomSelect dropdowns using the global helper function
         * @returns {void}
         */
        initializeComponents() {
            // Initialize ItemType TomSelect
            this.itemTypeSelect = initTomSelect('#itemTypeSelect', {
                placeholder: 'Select Item Type',
                maxOptions: 50
            });
            
            // Initialize ItemDept TomSelect
            this.itemDeptSelect = initTomSelect('#itemDeptSelect', {
                placeholder: 'Select Department',
                maxOptions: 50
            });
            
            // Initialize ItemGroup01 TomSelect
            this.itemGroup01Select = initTomSelect('#itemGroup01Select', {
                placeholder: 'Select Item Group 01',
                maxOptions: 50
            });
            
            // Initialize ItemGroup02 TomSelect
            this.itemGroup02Select = initTomSelect('#itemGroup02Select', {
                placeholder: 'Select Item Group 02',
                maxOptions: 50
            });
            
            // Initialize Unit TomSelect
            this.unitSelect = initTomSelect('#unitSelect', {
                placeholder: 'Select Unit',
                maxOptions: 50
            });

            // Initialize Warehouse TomSelect
            this.warehouseSelect = initTomSelect('#warehouseSelect', {
                placeholder: 'Select Warehouse',
                maxOptions: 50
            });
        },

        /**
         * Pre-select values in TomSelect dropdowns after initialization
         * @returns {void}
         */
        preselectValues() {
            // Pre-populate ItemType
            if (this.itemTypeSelect && this.config.itemTypeId) {
                this.itemTypeSelect.addOption({
                    value: this.config.itemTypeId.toString(),
                    text: this.config.itemTypeName || 'Loading...'
                });
                this.itemTypeSelect.setValue(this.config.itemTypeId.toString());
            }

            // Pre-populate ItemDept
            if (this.itemDeptSelect && this.config.itemDeptId) {
                this.itemDeptSelect.addOption({
                    value: this.config.itemDeptId.toString(),
                    text: this.config.itemDeptName || 'Loading...'
                });
                this.itemDeptSelect.setValue(this.config.itemDeptId.toString());
            }

            // Pre-populate ItemGroup01
            if (this.itemGroup01Select && this.config.itemGroup01Id) {
                this.itemGroup01Select.addOption({
                    value: this.config.itemGroup01Id.toString(),
                    text: this.config.itemGroup01Name || 'Loading...'
                });
                this.itemGroup01Select.setValue(this.config.itemGroup01Id.toString());
            }

            // Pre-populate ItemGroup02
            if (this.itemGroup02Select && this.config.itemGroup02Id) {
                this.itemGroup02Select.addOption({
                    value: this.config.itemGroup02Id.toString(),
                    text: this.config.itemGroup02Name || 'Loading...'
                });
                this.itemGroup02Select.setValue(this.config.itemGroup02Id.toString());
            }

            // Pre-populate Unit
            if (this.unitSelect && this.config.unitId) {
                this.unitSelect.addOption({
                    value: this.config.unitId.toString(),
                    text: this.config.unitName || 'Loading...'
                });
                this.unitSelect.setValue(this.config.unitId.toString());
            }

            // Pre-populate Warehouse
            if (this.warehouseSelect && this.config.whId) {
                this.warehouseSelect.addOption({
                    value: this.config.whId.toString(),
                    text: this.config.whName || 'Loading...'
                });
                this.warehouseSelect.setValue(this.config.whId.toString());
            }
        },

        /**
         * Initialize TomSelect for Status dropdown, loading options from API
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
                                headers: { 'X-TomSelect': 'true' }
                            });
                            if (!response.ok) throw new Error(`HTTP ${response.status}`);
                            const data = await response.json();
                            callback(data);
                            if (self.config.statusId !== null && self.statusSelect) {
                                setTimeout(() => self.statusSelect.setValue(self.config.statusId.toString()), 100);
                            }
                        } catch (error) {
                            console.error('[Item Edit] Error loading Status data:', error);
                            callback();
                        }
                    },
                    placeholder: '-- Select Status --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function() { this.load(''); }
                });
            } catch (error) {
                console.error('[Item Edit] Error initializing Status TomSelect:', error);
            }
        },

        /**
         * Handle image file upload
         * @param {Event} event - File input change event
         * @returns {void}
         */
        handleImageUpload(event) {
            const file = event.target.files[0];
            if (file) {
                // Validate file type
                if (!file.type.startsWith('image/')) {
                    alert('Please select an image file');
                    event.target.value = '';
                    return;
                }

                // Validate file size (max 5MB)
                if (file.size > 5 * 1024 * 1024) {
                    alert('Image size must be less than 5MB');
                    event.target.value = '';
                    return;
                }

                // Create preview
                const reader = new FileReader();
                reader.onload = (e) => {
                    this.imagePreview = e.target.result;
                };
                reader.readAsDataURL(file);

                // Store filename
                this.imageFileName = file.name;
            }
        },

        /**
         * Clear image selection
         * @returns {void}
         */
        clearImage() {
            this.imagePreview = null;
            this.imageFileName = '';
            document.getElementById('imageUpload').value = '';
        },
    };
}

// Make available globally
window.itemEdit = itemEdit;

