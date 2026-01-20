/**
 * Components Index
 * Re-exports all component modules for easy importing
 * 
 * Usage:
 * import { DataTableComponent, Select2Component } from '/assets/js/components/index.js';
 */

export { DataTableComponent, ColumnRenderers } from './data-table.js';
export { Select2Component } from './select2.js';
export { DatePickerComponent } from './datepicker.js';
export { ConfirmModalComponent } from './confirm-modal.js';
export { FormCalculatorComponent } from './form-calculator.js';
export { AjaxTableComponent } from './ajax-table.js';
export { ComponentsManager } from './ui-components.js';

// Default export for convenience
export default {
    DataTableComponent: (await import('./data-table.js')).DataTableComponent,
    Select2Component: (await import('./select2.js')).Select2Component,
    DatePickerComponent: (await import('./datepicker.js')).DatePickerComponent,
    ConfirmModalComponent: (await import('./confirm-modal.js')).ConfirmModalComponent,
    FormCalculatorComponent: (await import('./form-calculator.js')).FormCalculatorComponent,
    AjaxTableComponent: (await import('./ajax-table.js')).AjaxTableComponent
};
