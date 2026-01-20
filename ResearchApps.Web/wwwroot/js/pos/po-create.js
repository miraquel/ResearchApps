/**
 * Purchase Order Create Form - Alpine.js Component
 */
function poCreateForm() {
    return {
        // State
        isSubmitting: false,
        showConfirmModal: false,

        // TomSelect instances
        supplierSelect: null,

        // Flatpickr instance
        poDatePicker: null,

        /**
         * Initialize component
         */
        init() {
            this.initDatePicker();
            this.initSupplierSelect();
        },

        /**
         * Initialize date picker
         */
        initDatePicker() {
            if (this.$refs.poDatePicker) {
                this.poDatePicker = initFlatpickr('#Header_PoDate', {
                    dateFormat: 'Y-m-d',
                    altInput: true,
                    altFormat: 'd M Y',
                    defaultDate: 'today',
                    allowInput: true
                });
            }
        },

        /**
         * Initialize supplier dropdown using global helper
         */
        initSupplierSelect() {
            if (this.$refs.supplierSelect) {
                this.supplierSelect = initTomSelect('#Header_SupplierId', {
                    url: '/api/Suppliers/cbo',
                    placeholder: 'Select Supplier',
                    maxOptions: 50
                });
            }
        },

        /**
         * Show confirmation modal
         */
        confirmCreate() {
            const form = document.getElementById('po-create-form');
            
            // Check HTML5 validation
            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }

            this.showConfirmModal = true;
        },

        /**
         * Proceed with form submission
         */
        proceedSubmit() {
            this.showConfirmModal = false;
            this.isSubmitting = true;

            const form = document.getElementById('po-create-form');
            form.submit();
        }
    };
}
