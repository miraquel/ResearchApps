/**
 * Penyesuaian Stock Create Page Alpine.js Component
 * Handles form validation and submission
 */
function psCreateForm() {
    return {
        // State
        errors: {},
        isSubmitting: false,
        
        // Flatpickr instance
        datePicker: null,
        
        /**
         * Initialize the component
         */
        init() {
            this.initializeDatePicker();
            
            // Set up form submit handler
            const form = document.getElementById('ps-form');
            if (form) {
                form.addEventListener('submit', (e) => {
                    if (!this.validateForm()) {
                        e.preventDefault();
                    } else {
                        this.isSubmitting = true;
                    }
                });
            }
        },
        
        /**
         * Initialize Flatpickr date picker
         */
        initializeDatePicker() {
            this.datePicker = initFlatpickr('#Header_PsDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: 'today'
            });
        },
        
        /**
         * Validate form
         */
        validateForm() {
            this.errors = {};
            
            const psDate = document.getElementById('Header_PsDate')?.value;
            if (!psDate) {
                this.errors.PsDate = 'PS Date is required';
            }
            
            const descr = document.getElementById('Header_Descr')?.value;
            if (!descr || descr.trim() === '') {
                this.errors.Descr = 'Description is required';
            }
            
            return Object.keys(this.errors).length === 0;
        }
    };
}
