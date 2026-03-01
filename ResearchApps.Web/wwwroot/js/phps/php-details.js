// Penerimaan Hasil Produksi Details page - Alpine.js component
function phpDetails() {
    return {
        // UI state
        showDeleteConfirm: false,
        
        init() {
            // No special initialization needed
        },
        
        showNotification(message, isError = false) {
            if (window.showNotificationModal) {
                window.showNotificationModal(message, isError);
            } else {
                alert(message);
            }
        }
    };
}
