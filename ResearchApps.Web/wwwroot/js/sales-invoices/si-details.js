/**
 * Sales Invoice Details Page Alpine.js Component
 */
function siDetailsPage() {
    return {
        showDeleteModal: false,
        isSubmitting: false,
        
        /**
         * Initialize the component
         */
        init() {
            // Any initialization logic
        },
        
        /**
         * Show delete confirmation modal
         */
        confirmDelete() {
            this.showDeleteModal = true;
        },
        
        /**
         * Execute delete
         */
        executeDelete() {
            this.isSubmitting = true;
            document.getElementById('delete-form').submit();
        }
    };
}

window.siDetailsPage = siDetailsPage;