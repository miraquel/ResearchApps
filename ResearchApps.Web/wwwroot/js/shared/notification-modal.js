/**
 * Reusable Notification Modal
 * Displays success or error messages in a styled modal
 * 
 * Usage:
 *   showNotificationModal('Operation completed successfully');
 *   showNotificationModal('An error occurred', true);
 *   showNotificationModal('Data saved', 'success');
 *   showNotificationModal('Failed to save', 'error');
 * 
 * @param {string} message - The message to display
 * @param {boolean|string} isErrorOrType - true/false or 'success'/'error'/'warning'/'info'
 */
window.showNotificationModal = function(message, isErrorOrType = false) {
    const modal = document.getElementById('notificationModal');
    const icon = document.getElementById('notificationIcon');
    const iconElement = document.getElementById('notificationIconElement');
    const title = document.getElementById('notificationTitle');
    const messageElement = document.getElementById('notificationMessage');
    const button = document.getElementById('notificationButton');
    
    if (!modal) {
        console.error('Notification modal not found. Make sure _NotificationModal.cshtml is included in your layout.');
        alert(message); // Fallback to alert
        return;
    }
    
    // Determine type
    let type = 'success';
    if (typeof isErrorOrType === 'boolean') {
        type = isErrorOrType ? 'error' : 'success';
    } else if (typeof isErrorOrType === 'string') {
        type = isErrorOrType.toLowerCase();
    }
    
    // Configuration for different types
    const config = {
        success: {
            iconBg: 'bg-success bg-opacity-10',
            iconClass: 'ri-checkbox-circle-line text-success',
            titleClass: 'text-success',
            titleText: 'Success!',
            buttonClass: 'btn-success'
        },
        error: {
            iconBg: 'bg-danger bg-opacity-10',
            iconClass: 'ri-close-circle-line text-danger',
            titleClass: 'text-danger',
            titleText: 'Error!',
            buttonClass: 'btn-danger'
        },
        warning: {
            iconBg: 'bg-warning bg-opacity-10',
            iconClass: 'ri-error-warning-line text-warning',
            titleClass: 'text-warning',
            titleText: 'Warning!',
            buttonClass: 'btn-warning'
        },
        info: {
            iconBg: 'bg-info bg-opacity-10',
            iconClass: 'ri-information-line text-info',
            titleClass: 'text-info',
            titleText: 'Information',
            buttonClass: 'btn-info'
        }
    };
    
    const selectedConfig = config[type] || config.success;
    
    // Reset classes
    icon.className = 'rounded-circle d-inline-flex align-items-center justify-content-center mb-3';
    iconElement.className = '';
    title.className = 'fw-bold mb-0';
    button.className = 'btn px-5 rounded-pill shadow-sm';
    
    // Apply new classes
    icon.classList.add(...selectedConfig.iconBg.split(' '));
    iconElement.classList.add(...selectedConfig.iconClass.split(' '));
    title.classList.add(...selectedConfig.titleClass.split(' '));
    button.classList.add(...selectedConfig.buttonClass.split(' '));
    
    // Set content
    title.textContent = selectedConfig.titleText;
    messageElement.textContent = message;
    
    // Show modal
    const bsModal = new bootstrap.Modal(modal, {
        backdrop: true,
        keyboard: true
    });
    bsModal.show();
};
