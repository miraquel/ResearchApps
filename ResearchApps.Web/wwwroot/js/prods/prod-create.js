/**
 * Production Order Create Page Component
 * Handles form validation, TomSelect/Flatpickr initialization, and submission
 */
function prodCreateForm() {
    return {
        form: {
            prodDate: '',
            customerId: '',
            itemId: '',
            planQty: '',
            notes: ''
        },
        errors: {},
        isSubmitting: false,
        showConfirmModal: false,
        requireConfirmation: false, // Toggle to control confirmation dialog
        customerSelect: null,
        itemSelect: null,
        datePicker: null,
        
        init() {
            this.initializeComponents();
        },
        
        initializeComponents() {
            // Initialize Flatpickr
            this.datePicker = initFlatpickr('#ProdDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: 'today'
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
        
        validateForm() {
            this.errors = {};
            
            const dateInput = document.getElementById('ProdDate');
            if (!dateInput.value) {
                this.errors.ProdDate = 'Production Date is required';
            }
            
            if (!this.customerSelect || !this.customerSelect.getValue()) {
                this.errors.CustomerId = 'Customer is required';
            }
            
            if (!this.itemSelect || !this.itemSelect.getValue()) {
                this.errors.ItemId = 'Item is required';
            }
            
            const planQty = parseFloat(document.getElementById('PlanQty').value);
            if (!planQty || planQty <= 0) {
                this.errors.PlanQty = 'Planned Quantity must be greater than 0';
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
            const form = document.getElementById('prod-form');
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
window.prodCreateForm = prodCreateForm;
