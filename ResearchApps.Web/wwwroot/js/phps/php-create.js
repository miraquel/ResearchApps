// Penerimaan Hasil Produksi Create page - Alpine.js component
function phpCreate() {
    return {
        // Header state
        header: {
            phpDate: '',
            descr: '',
            refId: '',
            notes: ''
        },
        
        // UI state
        errors: {},
        isSubmitting: false,
        showConfirmModal: false,
        
        // Flatpickr instance
        datePicker: null,
        
        init() {
            this.initDatePicker();
        },
        
        initDatePicker() {
            const dateEl = this.$refs.phpDatePicker;
            if (dateEl && typeof flatpickr !== 'undefined') {
                // Check if already initialized
                if (dateEl._flatpickr) {
                    this.datePicker = dateEl._flatpickr;
                } else {
                    // Remove data-flatpickr to prevent double initialization
                    dateEl.removeAttribute('data-flatpickr');
                    
                    this.datePicker = flatpickr(dateEl, {
                        dateFormat: 'Y-m-d',
                        defaultDate: new Date(),
                        allowInput: true,
                        onChange: (selectedDates, dateStr) => {
                            this.header.phpDate = dateStr;
                        }
                    });
                }
            }
        },
        
        showNotification(message, isError = false) {
            if (window.showNotificationModal) {
                window.showNotificationModal(message, isError);
            } else {
                alert(message);
            }
        },
        
        validateForm() {
            this.errors = {};
            
            const phpDate = document.getElementById('Header_PhpDate').value;
            if (!phpDate) {
                this.errors.PhpDate = 'PHP Date is required';
            }
            
            return Object.keys(this.errors).length === 0;
        },
        
        confirmSubmit() {
            if (!this.validateForm()) {
                return;
            }
            this.showConfirmModal = true;
        },
        
        async submitForm() {
            this.showConfirmModal = false;
            this.isSubmitting = true;
            
            try {
                const form = document.getElementById('php-form');
                form.submit();
            } catch (error) {
                console.error('Submit error:', error);
                this.showNotification('An error occurred while saving', true);
                this.isSubmitting = false;
            }
        }
    };
}
