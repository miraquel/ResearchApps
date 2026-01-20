/**
 * DatePicker Component
 * Reusable Flatpickr initializer using data-attributes
 * 
 * Usage in HTML:
 * <input data-datepicker
 *        data-format="d M Y"
 *        data-default-date="today"
 *        data-min-date="2024-01-01"
 *        data-max-date="2025-12-31">
 */

export class DatePickerComponent {
    static defaults = {
        dateFormat: 'd M Y',
        allowInput: true
    };

    /**
     * Initialize Flatpickr on an element
     * @param {HTMLElement|string} element - Input element or selector
     * @param {Object} options - Configuration options
     * @returns {Object} Flatpickr instance
     */
    static init(element, options = {}) {
        const el = typeof element === 'string' ? document.querySelector(element) : element;
        if (!el) return null;

        const config = this.buildConfig(el, options);
        return flatpickr(el, config);
    }

    /**
     * Build Flatpickr configuration from element data-attributes and options
     */
    static buildConfig(element, options) {
        const dataset = element.dataset;

        const config = {
            dateFormat: dataset.format || options.dateFormat || this.defaults.dateFormat,
            allowInput: dataset.allowInput !== 'false' && (options.allowInput ?? this.defaults.allowInput)
        };

        // Default date
        if (dataset.defaultDate === 'today' || options.defaultDate === 'today') {
            config.defaultDate = new Date();
        } else if (dataset.defaultDate || options.defaultDate) {
            config.defaultDate = dataset.defaultDate || options.defaultDate;
        }

        // Min/Max dates
        if (dataset.minDate || options.minDate) {
            config.minDate = dataset.minDate || options.minDate;
        }
        if (dataset.maxDate || options.maxDate) {
            config.maxDate = dataset.maxDate || options.maxDate;
        }

        // Time picker
        if (dataset.enableTime === 'true' || options.enableTime) {
            config.enableTime = true;
            config.dateFormat = dataset.format || options.dateFormat || 'd M Y H:i';
        }

        // Callback handlers
        if (options.onChange) config.onChange = options.onChange;
        if (options.onClose) config.onClose = options.onClose;

        return config;
    }

    /**
     * Set date value
     */
    static setDate(element, date) {
        const instance = element._flatpickr;
        if (instance) {
            instance.setDate(date, true);
        }
    }

    /**
     * Clear date
     */
    static clear(element) {
        const instance = element._flatpickr;
        if (instance) {
            instance.clear();
        }
    }

    /**
     * Auto-initialize all date pickers with data-datepicker attribute
     */
    static autoInit() {
        document.querySelectorAll('[data-datepicker]').forEach(el => {
            this.init(el);
        });
    }
}

// Export for non-module usage
if (typeof window !== 'undefined') {
    window.DatePickerComponent = DatePickerComponent;
}

export default DatePickerComponent;
