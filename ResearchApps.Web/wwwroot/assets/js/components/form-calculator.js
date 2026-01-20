/**
 * Form Calculator Component
 * Auto-calculate field values based on other inputs
 * 
 * Usage in HTML:
 * <input data-calculate
 *        data-calculate-inputs="#qty,#price"
 *        data-calculate-formula="multiply"
 *        data-calculate-decimals="2">
 * 
 * Or with custom formula:
 * <input data-calculate
 *        data-calculate-inputs="#subtotal,#discount,#tax"
 *        data-calculate-formula="custom"
 *        data-calculate-expression="(a - b) * (1 + c/100)">
 */

export class FormCalculatorComponent {
    static formulas = {
        add: (...values) => values.reduce((a, b) => a + b, 0),
        subtract: (...values) => values.reduce((a, b) => a - b),
        multiply: (...values) => values.reduce((a, b) => a * b, 1),
        divide: (...values) => values.length >= 2 ? values[0] / values[1] : 0,
        sum: (...values) => values.reduce((a, b) => a + b, 0),
        average: (...values) => values.length ? values.reduce((a, b) => a + b, 0) / values.length : 0,
        percentage: (...values) => values.length >= 2 ? (values[0] * values[1]) / 100 : 0
    };

    /**
     * Initialize calculator on an output field
     * @param {HTMLElement|string} element - Output element or selector
     * @param {Object} options - Configuration options
     */
    static init(element, options = {}) {
        const el = typeof element === 'string' ? document.querySelector(element) : element;
        if (!el) return null;

        const config = this.buildConfig(el, options);
        const inputs = config.inputSelectors.map(sel => document.querySelector(sel)).filter(Boolean);

        const calculate = () => {
            const values = inputs.map(input => parseFloat(input.value) || 0);
            let result;

            if (config.formula === 'custom' && config.expression) {
                result = this.evaluateExpression(config.expression, values);
            } else if (this.formulas[config.formula]) {
                result = this.formulas[config.formula](...values);
            } else if (typeof config.formula === 'function') {
                result = config.formula(...values);
            } else {
                result = values.reduce((a, b) => a * b, 1);
            }

            // Apply decimals
            if (config.decimals !== null) {
                result = parseFloat(result.toFixed(config.decimals));
            }

            // Round if needed
            if (config.round) {
                result = Math.round(result);
            }

            el.value = result;
            
            // Trigger change event
            el.dispatchEvent(new Event('change', { bubbles: true }));
        };

        // Attach event listeners
        inputs.forEach(input => {
            input.addEventListener('input', calculate);
            input.addEventListener('change', calculate);
        });

        // Initial calculation
        calculate();

        return { element: el, inputs, calculate };
    }

    /**
     * Build configuration from element data-attributes
     */
    static buildConfig(element, options) {
        const dataset = element.dataset;

        return {
            inputSelectors: (dataset.calculateInputs || options.inputs || '').split(',').map(s => s.trim()).filter(Boolean),
            formula: dataset.calculateFormula || options.formula || 'multiply',
            expression: dataset.calculateExpression || options.expression,
            decimals: dataset.calculateDecimals !== undefined 
                ? parseInt(dataset.calculateDecimals) 
                : (options.decimals ?? 2),
            round: dataset.calculateRound === 'true' || options.round === true
        };
    }

    /**
     * Evaluate custom expression
     * Variables are named a, b, c, d, etc.
     */
    static evaluateExpression(expression, values) {
        const vars = 'abcdefghijklmnopqrstuvwxyz';
        let expr = expression;
        
        values.forEach((val, i) => {
            if (i < vars.length) {
                expr = expr.replace(new RegExp(vars[i], 'g'), val.toString());
            }
        });

        try {
            // Use Function constructor for safer evaluation
            return new Function(`return ${expr}`)();
        } catch (e) {
            console.error('Error evaluating expression:', e);
            return 0;
        }
    }

    /**
     * Auto-initialize all calculators with data-calculate attribute
     */
    static autoInit() {
        document.querySelectorAll('[data-calculate]').forEach(el => {
            this.init(el);
        });
    }
}

// Export for non-module usage
if (typeof window !== 'undefined') {
    window.FormCalculatorComponent = FormCalculatorComponent;
}

export default FormCalculatorComponent;
