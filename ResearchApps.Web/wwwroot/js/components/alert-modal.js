/**
 * Simple Alert Modal Component using Alpine.js + Bootstrap 5
 * Replacement for alert() and SweetAlert2
 */
window.showAlert = function(options) {
    const defaults = {
        title: 'Alert',
        message: '',
        type: 'info', // 'success', 'error', 'warning', 'info'
        confirmText: 'OK',
        onConfirm: null
    };
    
    const config = { ...defaults, ...options };
    
    // Icon mapping
    const icons = {
        success: 'ri-checkbox-circle-line text-success',
        error: 'ri-error-warning-line text-danger',
        warning: 'ri-alert-line text-warning',
        info: 'ri-information-line text-info'
    };
    
    // Create modal HTML
    const modalId = 'alertModal_' + Date.now();
    const modalHtml = `
        <div class="modal fade" id="${modalId}" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header border-0 pb-0">
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body text-center pt-0">
                        <div class="mb-3">
                            <i class="${icons[config.type]} fs-1"></i>
                        </div>
                        <h5 class="mb-2">${config.title}</h5>
                        <p class="text-muted mb-0">${config.message}</p>
                    </div>
                    <div class="modal-footer border-0 pt-0 justify-content-center">
                        <button type="button" class="btn btn-primary" data-bs-dismiss="modal">
                            ${config.confirmText}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Append modal to body
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    
    // Get modal element and initialize
    const modalElement = document.getElementById(modalId);
    const modal = new bootstrap.Modal(modalElement);
    
    // Show modal
    modal.show();
    
    // Clean up when hidden
    modalElement.addEventListener('hidden.bs.modal', function() {
        if (config.onConfirm) {
            config.onConfirm();
        }
        modalElement.remove();
    });
};

// Shorthand methods
window.showSuccess = (message, title = 'Success') => 
    showAlert({ type: 'success', title, message });

window.showError = (message, title = 'Error') => 
    showAlert({ type: 'error', title, message });

window.showWarning = (message, title = 'Warning') => 
    showAlert({ type: 'warning', title, message });

window.showInfo = (message, title = 'Information') => 
    showAlert({ type: 'info', title, message });
