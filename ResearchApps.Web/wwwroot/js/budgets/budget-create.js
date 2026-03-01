/**
 * Budget Create Page Component
 * Handles Flatpickr date pickers and number formatting for Amount
 * @returns {Object} Alpine.js component
 */
function budgetCreate() {
    return {
        /** @type {Object|null} Flatpickr instance for Start Date */
        startDatePicker: null,
        /** @type {Object|null} Flatpickr instance for End Date */
        endDatePicker: null,
        /** @type {string} Formatted display value for Amount */
        amountDisplay: '',
        /** @type {string} Raw numeric value for Amount (submitted to server) */
        amountValue: '',

        /**
         * Initialize component, date pickers, and number formatting
         * @returns {void}
         */
        init() {
            this.initDatePickers();
        },

        /**
         * Initialize Flatpickr date pickers for StartDate and EndDate
         * EndDate is constrained to be >= StartDate
         * @returns {void}
         */
        initDatePickers() {
            const self = this;

            // Start Date picker
            this.startDatePicker = flatpickr(this.$refs.startDate, {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                onChange: function (selectedDates) {
                    if (selectedDates.length > 0 && self.endDatePicker) {
                        // Set min date on end date picker
                        self.endDatePicker.set('minDate', selectedDates[0]);
                    }
                }
            });

            // End Date picker
            this.endDatePicker = flatpickr(this.$refs.endDate, {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y'
            });

            console.log('[Budget Create] Date pickers initialized');
        },

        /**
         * Format the Amount input with thousand separators
         * Updates both display and hidden field values
         * @returns {void}
         */
        formatAmount() {
            // Remove non-numeric characters except decimal point
            let raw = this.amountDisplay.replace(/[^0-9]/g, '');
            
            if (raw === '') {
                this.amountDisplay = '';
                this.amountValue = '';
                return;
            }

            // Parse as number
            const num = parseInt(raw, 10);
            
            // Set raw value for form submission
            this.amountValue = num.toString();
            
            // Format with thousand separators for display
            this.amountDisplay = num.toLocaleString('id-ID');
        }
    };
}

// Make available globally
window.budgetCreate = budgetCreate;
