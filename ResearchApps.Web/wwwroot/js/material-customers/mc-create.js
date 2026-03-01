/**
 * Material Customer Create Page Component
 * Handles form validation, TomSelect/Flatpickr initialization, and submission
 */
function mcCreateForm() {
    return {
        form: {
            mcDate: '',
            customerId: '',
            sjNo: '',
            refNo: '',
            notes: ''
        },
        errors: {},
        isSubmitting: false,
        showConfirmModal: false,
        requireConfirmation: false, // Toggle to control confirmation dialog
        customerSelect: null,
        datePicker: null,
        
        // Config can be passed during initialization
        config: {
            preselectedCustomerId: null
        },
        
        init(config = {}) {
            this.config = { ...this.config, ...config };
            this.initializeComponents();
        },
        
        initializeComponents() {
            // Initialize Flatpickr
            this.datePicker = initFlatpickr('#Header_McDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: 'today'
            });
            
            // Initialize Customer TomSelect
            this.customerSelect = initTomSelect('#Header_CustomerId', {
                url: '/api/Customers/cbo',
                placeholder: 'Select Customer',
                maxOptions: 50
            });
            
            // Pre-select customer if provided
            if (this.config.preselectedCustomerId) {
                this.preselectCustomer(this.config.preselectedCustomerId);
            }
        },
        
        async preselectCustomer(customerId) {
            try {
                const response = await fetch(`/api/Customers/${customerId}`);
                const data = await response.json();
                if (data.data) {
                    this.customerSelect.addOption({
                        value: data.data.customerId.toString(),
                        text: data.data.customerName
                    });
                    this.customerSelect.setValue(data.data.customerId.toString());
                }
            } catch (error) {
                console.error('Error pre-selecting customer:', error);
            }
        },
        
        validateForm() {
            this.errors = {};
            
            const dateInput = document.getElementById('Header_McDate');
            if (!dateInput.value) {
                this.errors.McDate = 'MC Date is required';
            }
            
            if (!this.customerSelect || !this.customerSelect.getValue()) {
                this.errors.CustomerId = 'Customer is required';
            }
            
            return Object.keys(this.errors).length === 0;
        },
        
        confirmCreate() {
            if (this.validateForm()) {
                if (this.requireConfirmation) {
                    this.showConfirmModal = true;
                } else {
                    this.proceedSubmit();
                }
            }
        },
        
        proceedSubmit() {
            this.showConfirmModal = false;
            this.isSubmitting = true;
            
            // Create a hidden submit button and click it to bypass htmx
            const form = document.getElementById('mc-form');
            const submitBtn = document.createElement('button');
            submitBtn.type = 'submit';
            submitBtn.style.display = 'none';
            submitBtn.setAttribute('data-htmx-disable', 'true'); // Disable htmx for this button
            form.appendChild(submitBtn);
            submitBtn.click();
        }
    };
}

// Make available globally
window.mcCreateForm = mcCreateForm;
