/**
 * Confirm Modal Component
 * Reusable confirmation modal using data-attributes
 * 
 * Usage in HTML:
 * <button data-confirm
 *         data-confirm-modal="deleteModal"
 *         data-confirm-title="Are you sure?"
 *         data-confirm-message="This action cannot be undone."
 *         data-confirm-form="#delete-form"
 *         data-confirm-validate="true">
 *     Delete
 * </button>
 * 
 * Or attach to existing modal:
 * <button data-confirm-trigger="#myModal" data-confirm-form="#myForm">Submit</button>
 */

export class ConfirmModalComponent {
    static modalTemplate = `
        <div id="{id}" class="modal fade zoomIn" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="mt-2 text-center">
                            <lord-icon
                                src="{icon}"
                                trigger="loop"
                                delay="2000"
                                {iconColors}
                                style="width:100px;height:100px">
                            </lord-icon>
                            <div class="mt-4 pt-2 fs-15 mx-4 mx-sm-5">
                                <h4>{title}</h4>
                                <p class="text-muted mx-4 mb-0">{message}</p>
                            </div>
                        </div>
                        <div class="d-flex gap-2 justify-content-center mt-4 mb-2">
                            <button type="button" class="btn w-sm btn-light" data-bs-dismiss="modal">{cancelText}</button>
                            <button type="button" class="btn w-sm {confirmClass}" data-confirm-action>{confirmText}</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;

    static icons = {
        confirm: 'https://cdn.lordicon.com/exymduqj.json',
        delete: 'https://cdn.lordicon.com/gsqxdxog.json',
        warning: 'https://cdn.lordicon.com/tdrtiskw.json',
        success: 'https://cdn.lordicon.com/lupuorrc.json'
    };

    /**
     * Initialize confirmation behavior on a button
     * @param {HTMLElement|string} element - Button element or selector
     * @param {Object} options - Configuration options
     */
    static init(element, options = {}) {
        const el = typeof element === 'string' ? document.querySelector(element) : element;
        if (!el) return null;

        const config = this.buildConfig(el, options);
        
        // Create or find modal
        let modal = config.modal;
        if (!modal && config.modalId) {
            modal = document.getElementById(config.modalId);
            if (!modal) {
                modal = this.createModal(config);
                document.body.appendChild(modal);
            }
        }

        // Attach click handler
        el.addEventListener('click', (e) => {
            e.preventDefault();
            this.handleClick(el, modal, config);
        });

        return { element: el, modal, config };
    }

    /**
     * Build configuration from element data-attributes and options
     */
    static buildConfig(element, options) {
        const dataset = element.dataset;
        const isDelete = dataset.confirmType === 'delete' || options.type === 'delete';

        return {
            modalId: dataset.confirmModal || options.modalId || `confirm-modal-${Date.now()}`,
            modal: options.modal,
            title: dataset.confirmTitle || options.title || 'Are you sure?',
            message: dataset.confirmMessage || options.message || 'Are you sure you want to proceed?',
            icon: dataset.confirmIcon || options.icon || (isDelete ? this.icons.delete : this.icons.confirm),
            iconColors: isDelete ? 'colors="primary:#f7b84b,secondary:#f06548"' : 'stroke="light" state="hover-line"',
            confirmText: dataset.confirmText || options.confirmText || (isDelete ? 'Yes, Delete It!' : 'Yes, Proceed'),
            cancelText: dataset.cancelText || options.cancelText || 'Close',
            confirmClass: dataset.confirmClass || options.confirmClass || (isDelete ? 'btn-danger' : 'btn-primary'),
            formSelector: dataset.confirmForm || options.formSelector,
            validate: dataset.confirmValidate === 'true' || options.validate === true,
            onConfirm: options.onConfirm
        };
    }

    /**
     * Create modal element from template
     */
    static createModal(config) {
        const html = this.modalTemplate
            .replace('{id}', config.modalId)
            .replace('{icon}', config.icon)
            .replace('{iconColors}', config.iconColors)
            .replace('{title}', config.title)
            .replace('{message}', config.message)
            .replace('{cancelText}', config.cancelText)
            .replace('{confirmText}', config.confirmText)
            .replace('{confirmClass}', config.confirmClass);

        const template = document.createElement('template');
        template.innerHTML = html.trim();
        return template.content.firstChild;
    }

    /**
     * Handle button click
     */
    static handleClick(trigger, modalEl, config) {
        // Validate form if required
        if (config.validate && config.formSelector) {
            const form = document.querySelector(config.formSelector);
            if (form && typeof $.fn.validate === 'function') {
                const $form = $(form);
                $form.validate();
                if (!$form.valid()) {
                    return;
                }
            }
        }

        // Show modal
        const modal = new bootstrap.Modal(modalEl);
        modal.show();

        // Attach confirm action handler
        const confirmBtn = modalEl.querySelector('[data-confirm-action]');
        const newConfirmBtn = confirmBtn.cloneNode(true);
        confirmBtn.parentNode.replaceChild(newConfirmBtn, confirmBtn);

        newConfirmBtn.addEventListener('click', () => {
            modal.hide();
            if (config.onConfirm) {
                config.onConfirm(trigger);
            } else if (config.formSelector) {
                const form = document.querySelector(config.formSelector);
                if (form) form.submit();
            }
        });
    }

    /**
     * Show confirmation modal directly
     */
    static show(options) {
        const config = {
            modalId: options.modalId || `confirm-modal-${Date.now()}`,
            title: options.title || 'Are you sure?',
            message: options.message || 'Are you sure you want to proceed?',
            icon: options.icon || this.icons.confirm,
            iconColors: options.iconColors || 'stroke="light" state="hover-line"',
            confirmText: options.confirmText || 'Yes, Proceed',
            cancelText: options.cancelText || 'Close',
            confirmClass: options.confirmClass || 'btn-primary',
            onConfirm: options.onConfirm
        };

        let modalEl = document.getElementById(config.modalId);
        if (!modalEl) {
            modalEl = this.createModal(config);
            document.body.appendChild(modalEl);
        }

        const modal = new bootstrap.Modal(modalEl);
        modal.show();

        const confirmBtn = modalEl.querySelector('[data-confirm-action]');
        confirmBtn.addEventListener('click', () => {
            modal.hide();
            if (config.onConfirm) config.onConfirm();
        }, { once: true });

        // Clean up modal after hidden
        modalEl.addEventListener('hidden.bs.modal', () => {
            if (!options.modalId) modalEl.remove();
        }, { once: true });

        return modal;
    }

    /**
     * Auto-initialize all confirm buttons with data-confirm attribute
     */
    static autoInit() {
        document.querySelectorAll('[data-confirm]').forEach(el => {
            this.init(el);
        });
    }
}

// Export for non-module usage
if (typeof window !== 'undefined') {
    window.ConfirmModalComponent = ConfirmModalComponent;
}

export default ConfirmModalComponent;
