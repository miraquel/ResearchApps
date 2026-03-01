// Goods Receipt Create page - Alpine.js component
function grCreate() {
    return {
        showConfirmModal: false,
        isSubmitting: false,
        errors: {},
        header: {
            supplierId: '',
            supplierName: '',
            grDate: '',
            refNo: '',
            notes: ''
        },
        supplierSelect: null,
        datePicker: null,
        
        init() {
            this.initSupplierSelect();
            this.initDatePicker();
        },
        
        initSupplierSelect() {
            if (this.$refs.supplierSelect) {
                this.supplierSelect = initTomSelect('#Header_SupplierId', {
                    url: '/api/Suppliers/cbo',
                    placeholder: 'Select Supplier',
                    maxOptions: 50
                });
            }
        },
        
        initDatePicker() {
            if (this.$refs.grDatePicker) {
                this.datePicker = initFlatpickr('#Header_GrDate', {
                    dateFormat: 'Y-m-d',
                    altInput: true,
                    altFormat: 'd M Y',
                    defaultDate: 'today',
                    allowInput: true
                });
            }
        },
        
        validateForm() {
            this.errors = {};
            
            const dateInput = document.getElementById('Header_GrDate');
            if (!dateInput.value) {
                this.errors.GrDate = 'GR Date is required';
            }
            
            if (!this.supplierSelect || !this.supplierSelect.getValue()) {
                this.errors.SupplierId = 'Supplier is required';
            }
            
            return Object.keys(this.errors).length === 0;
        },
        
        confirmSubmit() {
            if (!this.validateForm()) {
                return;
            }
            this.showConfirmModal = true;
        },
        
        submitForm() {
            this.showConfirmModal = false;
            this.isSubmitting = true;
            
            // Submit the form normally
            const form = document.getElementById('gr-form');
            form.submit();
        }
    };
}
