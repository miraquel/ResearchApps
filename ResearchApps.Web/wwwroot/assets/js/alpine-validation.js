/**
 * Alpine.js ASP.NET Validation Component
 * Replaces jQuery Unobtrusive Validation by reading data-val-* attributes
 * and providing real-time client-side validation via Alpine.js.
 *
 * Supported validators:
 *   data-val-required        → required field
 *   data-val-length-max      → max length
 *   data-val-length-min      → min length
 *   data-val-regex-pattern   → regex match
 *   data-val-range-min/max   → numeric range
 *   data-val-email           → email format (also data-val-regex with email)
 *   data-val-equalto-other   → compare fields (e.g. confirm password)
 *
 * Usage (simple form):
 *   <form x-data="aspValidation()" @submit.prevent="handleSubmit($el)">
 *       <input name="Name" data-val="true" data-val-required="Name is required" />
 *       <span x-text="getError('Name')" class="text-danger"></span>
 *       <button type="submit">Save</button>
 *   </form>
 *
 * Usage (with confirm modal — integrate into existing Alpine data):
 *   In your Alpine component, spread ...aspValidationMixin() and call
 *   this.validateForm(formEl) before submission.
 */
document.addEventListener('alpine:init', () => {

    // =========================================================================
    // Core validation engine (shared between component and mixin)
    // =========================================================================
    const ValidationEngine = {

        /**
         * Discover all validatable fields inside a form and build rule map.
         * @param {HTMLFormElement} form
         * @returns {Map<string, Array<{type:string, message:string, params:Object}>>}
         */
        buildRules(form) {
            const rulesMap = new Map();
            const fields = form.querySelectorAll('[data-val="true"]');

            fields.forEach(field => {
                const name = field.getAttribute('name');
                if (!name) return;

                const rules = [];
                const ds = field.dataset;

                // Required
                if (ds.valRequired) {
                    rules.push({ type: 'required', message: ds.valRequired, params: {} });
                }

                // String length
                if (ds.valLength) {
                    rules.push({
                        type: 'length',
                        message: ds.valLength,
                        params: {
                            min: ds.valLengthMin ? parseInt(ds.valLengthMin, 10) : null,
                            max: ds.valLengthMax ? parseInt(ds.valLengthMax, 10) : null
                        }
                    });
                }

                // Regex
                if (ds.valRegex) {
                    rules.push({
                        type: 'regex',
                        message: ds.valRegex,
                        params: { pattern: ds.valRegexPattern }
                    });
                }

                // Range (numeric)
                if (ds.valRange) {
                    rules.push({
                        type: 'range',
                        message: ds.valRange,
                        params: {
                            min: ds.valRangeMin != null ? parseFloat(ds.valRangeMin) : null,
                            max: ds.valRangeMax != null ? parseFloat(ds.valRangeMax) : null
                        }
                    });
                }

                // Email
                if (ds.valEmail) {
                    rules.push({ type: 'email', message: ds.valEmail, params: {} });
                }

                // Compare / Equal-to
                if (ds.valEqualto) {
                    // data-val-equalto-other="*.Password" — ASP uses *. prefix
                    let otherField = ds.valEqualtoOther || '';
                    otherField = otherField.replace(/^\*\./, '');
                    rules.push({
                        type: 'equalto',
                        message: ds.valEqualto,
                        params: { other: otherField }
                    });
                }

                if (rules.length > 0) {
                    rulesMap.set(name, rules);
                }
            });

            return rulesMap;
        },

        /**
         * Validate a single field value against its rules.
         * @param {string} value
         * @param {Array} rules
         * @param {HTMLFormElement} form - for cross-field lookups
         * @returns {string|null} error message or null
         */
        validateField(value, rules, form) {
            for (const rule of rules) {
                const trimmed = (value ?? '').toString().trim();

                switch (rule.type) {
                    case 'required':
                        if (trimmed === '') return rule.message;
                        break;

                    case 'length': {
                        const len = trimmed.length;
                        if (rule.params.min != null && len < rule.params.min) return rule.message;
                        if (rule.params.max != null && len > rule.params.max) return rule.message;
                        break;
                    }

                    case 'regex':
                        if (trimmed !== '' && rule.params.pattern) {
                            const re = new RegExp(rule.params.pattern);
                            if (!re.test(trimmed)) return rule.message;
                        }
                        break;

                    case 'range': {
                        if (trimmed === '') break; // skip range check if empty (required handles that)
                        const num = parseFloat(trimmed);
                        if (isNaN(num)) return rule.message;
                        if (rule.params.min != null && num < rule.params.min) return rule.message;
                        if (rule.params.max != null && num > rule.params.max) return rule.message;
                        break;
                    }

                    case 'email':
                        if (trimmed !== '' && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmed)) {
                            return rule.message;
                        }
                        break;

                    case 'equalto': {
                        const otherInput = form.querySelector(`[name="${rule.params.other}"], [name$=".${rule.params.other}"]`);
                        const otherValue = otherInput ? otherInput.value : '';
                        if (value !== otherValue) return rule.message;
                        break;
                    }
                }
            }
            return null;
        },

        /**
         * Validate all fields in a form.
         * @param {HTMLFormElement} form
         * @returns {{isValid: boolean, errors: Object}}
         */
        validateAll(form) {
            const rulesMap = this.buildRules(form);
            const errors = {};
            let isValid = true;

            rulesMap.forEach((rules, name) => {
                const field = form.querySelector(`[name="${name}"]`);
                const value = field ? field.value : '';
                const error = this.validateField(value, rules, form);
                if (error) {
                    errors[name] = error;
                    isValid = false;
                }
            });

            return { isValid, errors };
        },

        /**
         * Apply Bootstrap 5 validation CSS classes to form fields.
         * @param {HTMLFormElement} form
         * @param {Object} errors - { fieldName: errorMessage }
         */
        applyValidationClasses(form, errors) {
            // Clear previous validation state
            form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
            form.querySelectorAll('.alpine-validation-error').forEach(el => el.remove());

            Object.entries(errors).forEach(([name, message]) => {
                const field = form.querySelector(`[name="${name}"]`);
                if (!field) return;

                field.classList.add('is-invalid');

                // Insert error span if there's no Alpine x-text binding for this field
                const existingSpan = field.parentElement.querySelector(`[data-valmsg-for="${name}"]`);
                const aspSpan = field.parentElement.querySelector(`span[class*="text-danger"]`);

                if (existingSpan) {
                    existingSpan.textContent = message;
                } else if (aspSpan && aspSpan.textContent === '') {
                    aspSpan.textContent = message;
                    aspSpan.classList.add('alpine-validation-error');
                } else {
                    const span = document.createElement('span');
                    span.className = 'text-danger alpine-validation-error';
                    span.textContent = message;
                    field.parentElement.appendChild(span);
                }
            });
        },

        /**
         * Clear all validation state from a form.
         * @param {HTMLFormElement} form
         */
        clearValidation(form) {
            form.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
            form.querySelectorAll('.alpine-validation-error').forEach(el => {
                el.textContent = '';
                el.remove();
            });
            // Also clear asp-validation-for spans that we populated
            form.querySelectorAll('span[class*="text-danger"]').forEach(el => {
                if (el.classList.contains('alpine-validation-error') || el.hasAttribute('data-valmsg-for')) {
                    el.textContent = '';
                }
            });
        }
    };

    // =========================================================================
    // Alpine data component — for standalone forms
    // =========================================================================
    Alpine.data('aspValidation', () => ({
        errors: {},
        isSubmitting: false,

        /**
         * Validate the form and return true if valid.
         * @param {HTMLFormElement} form
         * @returns {boolean}
         */
        validateForm(form) {
            const result = ValidationEngine.validateAll(form);
            this.errors = result.errors;
            ValidationEngine.applyValidationClasses(form, this.errors);
            return result.isValid;
        },

        /**
         * Clear all errors.
         * @param {HTMLFormElement} form
         */
        clearErrors(form) {
            this.errors = {};
            if (form) ValidationEngine.clearValidation(form);
        },

        /**
         * Get error for a specific field.
         * @param {string} name
         * @returns {string}
         */
        getError(name) {
            return this.errors[name] || '';
        },

        /**
         * Check if a field has an error.
         * @param {string} name
         * @returns {boolean}
         */
        hasError(name) {
            return !!this.errors[name];
        },

        /**
         * Handle form submit: validate, then submit if valid.
         * @param {HTMLFormElement} form
         */
        handleSubmit(form) {
            if (!this.validateForm(form)) return;
            this.isSubmitting = true;
            form.submit();
        }
    }));

    // =========================================================================
    // Mixin — for use inside existing Alpine data components
    // =========================================================================
    window.aspValidationMixin = function () {
        return {
            _valErrors: {},

            validateForm(form) {
                const result = ValidationEngine.validateAll(form);
                this._valErrors = result.errors;
                ValidationEngine.applyValidationClasses(form, this._valErrors);
                return result.isValid;
            },

            clearValidationErrors(form) {
                this._valErrors = {};
                if (form) ValidationEngine.clearValidation(form);
            },

            getValidationError(name) {
                return this._valErrors[name] || '';
            },

            hasValidationError(name) {
                return !!this._valErrors[name];
            }
        };
    };

    // =========================================================================
    // Real-time "on blur" validation (optional enhancement)
    // Attach to a form: <form @focusout="validateOnBlur($event)">
    // =========================================================================
    window.validateFieldOnBlur = function (event, errorsObj) {
        const field = event.target;
        if (!field || field.tagName === 'BUTTON') return;

        const form = field.closest('form');
        if (!form || field.getAttribute('data-val') !== 'true') return;

        const name = field.getAttribute('name');
        if (!name) return;

        const rulesMap = ValidationEngine.buildRules(form);
        const rules = rulesMap.get(name);
        if (!rules) return;

        const error = ValidationEngine.validateField(field.value, rules, form);

        if (error) {
            errorsObj[name] = error;
            field.classList.add('is-invalid');
        } else {
            delete errorsObj[name];
            field.classList.remove('is-invalid');
        }

        // Update the visible error span
        const span = field.parentElement.querySelector('span[class*="text-danger"]');
        if (span) span.textContent = error || '';
    };

    // Expose engine for advanced usage
    window.AlpineValidationEngine = ValidationEngine;
});
