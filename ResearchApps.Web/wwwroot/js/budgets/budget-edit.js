/**
 * Budget Edit Page Component
 * Handles Flatpickr date pickers and number formatting for Amount with pre-populated values
 * @param {number} initialAmount - Pre-populated amount value
 * @param {string} initialStartDate - Pre-populated start date (YYYY-MM-DD)
 * @param {string} initialEndDate - Pre-populated end date (YYYY-MM-DD)
 * @returns {Object} Alpine.js component
 */
function budgetEdit(initialAmount, initialStartDate, initialEndDate) {
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
         * Initialize component with existing values
         * @returns {void}
         */
        init() {
            // Initialize amount display from existing value
            if (initialAmount) {
                const num = Math.round(initialAmount);
                this.amountValue = num.toString();
                this.amountDisplay = num.toLocaleString('id-ID');
            }

            this.initDatePickers();
        },

        /**
         * Initialize Flatpickr date pickers with pre-selected dates
         * @returns {void}
         */
        initDatePickers() {
            const self = this;

            // Start Date picker with pre-selected value
            this.startDatePicker = flatpickr(this.$refs.startDate, {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: initialStartDate || null,
                onChange: function (selectedDates) {
                    if (selectedDates.length > 0 && self.endDatePicker) {
                        self.endDatePicker.set('minDate', selectedDates[0]);
                    }
                }
            });

            // End Date picker with pre-selected value
            this.endDatePicker = flatpickr(this.$refs.endDate, {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: initialEndDate || null,
                minDate: initialStartDate || null
            });

            console.log('[Budget Edit] Date pickers initialized');
        },

        /**
         * Format the Amount input with thousand separators
         * Updates both display and hidden field values
         * @returns {void}
         */
        formatAmount() {
            // Remove non-numeric characters
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
window.budgetEdit = budgetEdit;
