/**
 * App Initializer
 * Auto-initializes all components based on data-attributes
 * 
 * Include this script on pages that need auto-initialization:
 * <script type="module" src="/assets/js/app-init.js"></script>
 * 
 * Or import in your module:
 * import { AppInit } from '/assets/js/app-init.js';
 * AppInit.init();
 */

import { DataTableComponent, ColumnRenderers } from './components/data-table.js';
// Select2Component is loaded globally via jQuery plugin pattern
import { DatePickerComponent } from './components/datepicker.js';
import { ConfirmModalComponent } from './components/confirm-modal.js';
import { FormCalculatorComponent } from './components/form-calculator.js';
import { AjaxTableComponent } from './components/ajax-table.js';

export class AppInit {
    static components = {
        datatable: DataTableComponent,
        select2: window.Select2Component,
        datepicker: DatePickerComponent,
        confirm: ConfirmModalComponent,
        calculator: FormCalculatorComponent,
        ajaxTable: AjaxTableComponent
    };

    static initialized = false;

    /**
     * Initialize all components
     */
    static init() {
        if (this.initialized) return;
        
        // Wait for DOM if not ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.initAll());
        } else {
            this.initAll();
        }
        
        this.initialized = true;
    }

    /**
     * Initialize all registered components
     */
    static initAll() {
        Object.values(this.components).forEach(component => {
            if (typeof component.autoInit === 'function') {
                component.autoInit();
            }
        });
    }

    /**
     * Initialize a specific component type
     */
    static initComponent(type) {
        const component = this.components[type];
        if (component && typeof component.autoInit === 'function') {
            component.autoInit();
        }
    }

    /**
     * Re-initialize components in a container (useful after AJAX loads)
     */
    static refresh(container = document) {
        // DataTables
        container.querySelectorAll('[data-datatable]:not(.dataTable)').forEach(el => {
            DataTableComponent.init(el);
        });

        // Select2 - use classic jQuery pattern
        $(container).find('[data-select2]').each(function () {
            if (!$(this).hasClass('select2-hidden-accessible')) {
                new Select2Component(this);
            }
        });

        // DatePicker
        container.querySelectorAll('[data-datepicker]:not(.flatpickr-input)').forEach(el => {
            DatePickerComponent.init(el);
        });

        // Confirm modals
        container.querySelectorAll('[data-confirm]:not([data-confirm-initialized])').forEach(el => {
            ConfirmModalComponent.init(el);
            el.dataset.confirmInitialized = 'true';
        });

        // Calculators
        container.querySelectorAll('[data-calculate]:not([data-calculate-initialized])').forEach(el => {
            FormCalculatorComponent.init(el);
            el.dataset.calculateInitialized = 'true';
        });
    }

    /**
     * Register a custom component
     */
    static registerComponent(name, component) {
        this.components[name] = component;
    }
}

// Export components for direct use
export {
    DataTableComponent,
    ColumnRenderers,
    DatePickerComponent,
    ConfirmModalComponent,
    FormCalculatorComponent,
    AjaxTableComponent
};

// Note: Auto-initialization removed to prevent double-init
// Views should call AppInit.refresh() in their @section scripts

// Export for non-module usage
if (typeof window !== 'undefined') {
    window.AppInit = AppInit;
    window.DataTableComponent = DataTableComponent;
    window.ColumnRenderers = ColumnRenderers;
    // Select2Component already on window from jQuery plugin
    window.DatePickerComponent = DatePickerComponent;
    window.ConfirmModalComponent = ConfirmModalComponent;
    window.FormCalculatorComponent = FormCalculatorComponent;
    window.AjaxTableComponent = AjaxTableComponent;
}

export default AppInit;