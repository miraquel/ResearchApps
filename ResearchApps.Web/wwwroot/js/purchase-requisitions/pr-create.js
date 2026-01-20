/**
 * Purchase Requisition Create Page Component
 * Handles form validation, TomSelect/Flatpickr initialization, and submission
 */
function prCreateForm() {
    return {
        form: {
            prDate: '',
            budgetId: '',
            prName: '',
            notes: ''
        },
        errors: {},
        isSubmitting: false,
        showConfirmModal: false,
        requireConfirmation: false, // Toggle to control confirmation dialog
        budgetSelect: null,
        datePicker: null,
        
        init() {
            this.initializeComponents();
        },
        
        initializeComponents() {
            // Initialize Flatpickr
            this.datePicker = initFlatpickr('#PrDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: 'today'
            });
            
            // Initialize Budget TomSelect
            this.budgetSelect = initTomSelect('#BudgetId', {
                url: '/api/Budgets/cbo',
                placeholder: 'Select Budget',
                maxOptions: 50
            });
        },
        
        validateForm() {
            this.errors = {};
            
            const dateInput = document.getElementById('PrDate');
            if (!dateInput.value) {
                this.errors.PrDate = 'PR Date is required';
            }
            
            const prNameInput = document.getElementById('PrName');
            if (!prNameInput.value || prNameInput.value.trim() === '') {
                this.errors.PrName = 'PR Name is required';
            }
            
            if (!this.budgetSelect || !this.budgetSelect.getValue()) {
                this.errors.BudgetId = 'Budget is required';
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
            const form = document.getElementById('pr-form');
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
window.prCreateForm = prCreateForm;
