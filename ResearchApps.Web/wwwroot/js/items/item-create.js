/**
 * Item create page component
 * Handles TomSelect initialization for ItemType, ItemDept, Unit, and Warehouse dropdowns
 * @returns {Object} Alpine.js component
 */
function itemCreate() {
    return {
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
window.itemCreate = itemCreate;
