/**
 * Sales Invoice Edit Page Alpine.js Component
 */
function siEditForm() {
    return {
        // State
        errors: {},
        isSubmitting: false,
        showConfirmModal: false,
        showDeleteModal: false,
        
        // TomSelect instances
        customerSelect: null,
        
        // Flatpickr instance
        datePicker: null,
        
        /**
         * Initialize the component
         */
        init() {
            this.initializeDatePicker();
            this.initializeCustomerSelect();
        },
        
        /**
         * Initialize Flatpickr date picker
         */
        initializeDatePicker() {
            this.datePicker = initFlatpickr('#Header_SiDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y'
            });
        },
        
        /**
         * Initialize Customer TomSelect (readonly for edit)
         */
        initializeCustomerSelect() {
            const customerEl = document.getElementById('Header_CustomerId');
            if (customerEl) {
                this.customerSelect = initTomSelect('#Header_CustomerId', {
                    url: '/api/Customers/cbo',
                    placeholder: 'Select Customer',
                    maxOptions: 50
                });
                
                // Disable customer selection on edit
                if (this.customerSelect) {
                    this.customerSelect.disable();
                }
            }
        },
        
        /**
         * Validate form
         */
        validateForm() {
            this.errors = {};
            
            const siDate = document.getElementById('Header_SiDate')?.value;
            if (!siDate) {
                this.errors.siDate = 'Invoice date is required';
            }
            
            return Object.keys(this.errors).length === 0;
        },
        
        /**
         * Handle form submission
         */
        submitForm() {
            if (!this.validateForm()) {
                return;
            }
            
            this.showConfirmModal = true;
        },
        
        /**
         * Confirm and submit
         */
        confirmSubmit() {
            this.isSubmitting = true;
            this.showConfirmModal = false;
            document.getElementById('si-form').submit();
        },
        
        /**
         * Show delete confirmation modal
         */
        confirmDelete() {
            this.showDeleteModal = true;
        },
        
        /**
         * Execute delete
         */
        executeDelete() {
            this.isSubmitting = true;
            document.getElementById('delete-form').submit();
        }
    };
}

window.siEditForm = siEditForm;
