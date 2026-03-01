/**
 * Production Order Edit Page Component
 * Handles form validation, TomSelect/Flatpickr initialization, and submission
 */
function prodEditForm(config) {
    return {
        config: config, // Store config for preselection
        form: {
            recId: config.recId,
            prodId: config.prodId,
            prodDate: config.prodDate,
            customerId: config.customerId,
            itemId: config.itemId,
            planQty: config.planQty,
            notes: config.notes
        },
        errors: {},
        isSubmitting: false,
        customerSelect: null,
        itemSelect: null,
        datePicker: null,
        
        init() {
            this.initializeComponents();
            this.preselectValues();
        },
        
        initializeComponents() {
            // Initialize Flatpickr
            this.datePicker = initFlatpickr('#ProdDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: this.form.prodDate
            });
            
            // Initialize Customer TomSelect
            this.customerSelect = initTomSelect('#CustomerId', {
                url: '/api/Customers/cbo',
                placeholder: 'Select Customer',
                maxOptions: 50
            });
            
            // Initialize Item TomSelect
            this.itemSelect = initTomSelect('#ItemId', {
                url: '/api/Items/cbo',
                placeholder: 'Select Item',
                maxOptions: 50
            });
        },
        
        preselectValues() {
            // Pre-populate Customer
            if (this.customerSelect && this.config.customerId) {
                this.customerSelect.addOption({
                    value: this.config.customerId.toString(),
                    text: this.config.customerName || 'Loading...'
                });
                this.customerSelect.setValue(this.config.customerId.toString());
            }
            
            // Pre-populate Item
            if (this.itemSelect && this.config.itemId) {
                this.itemSelect.addOption({
                    value: this.config.itemId.toString(),
                    text: this.config.itemName || 'Loading...'
                });
                this.itemSelect.setValue(this.config.itemId.toString());
            }
        }
    };
}

// Make available globally
window.prodEditForm = prodEditForm;
