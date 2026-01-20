/**
 * Workflow Component - Reusable Alpine.js Component for Workflow Actions
 * Can be used across different entities (PO, PR, CO, etc.)
 */
function createWorkflowComponent(config) {
    const { recId, refId, baseUrl = '/Pos', customActionUrls = {} } = config;

    const actionUrls = {
        'submit': `${baseUrl}/Submit`,
        'approve': `${baseUrl}/Approve`,
        'reject': `${baseUrl}/Reject`,
        'recall': `${baseUrl}/Recall`,
        'close': `${baseUrl}/Close`,
        ...customActionUrls
    };

    return {
        modal: {
            show: false,
            action: null,
            notes: '',
            isProcessing: false
        },

        /**
         * Show workflow modal
         */
        showModal(action) {
            this.modal.show = true;
            this.modal.action = action;
            this.modal.notes = '';
        },

        /**
         * Close workflow modal
         */
        closeModal() {
            if (!this.modal.isProcessing) {
                this.modal.show = false;
                this.modal.action = null;
                this.modal.notes = '';
            }
        },

        /**
         * Execute workflow action
         */
        async execute() {
            // Validate reject action requires notes
            if (this.modal.action === 'reject' && (!this.modal.notes || this.modal.notes.trim() === '')) {
                alert('Please enter a reason for rejection.');
                return;
            }

            this.modal.isProcessing = true;

            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

                const body = new URLSearchParams();
                body.append('RecId', recId);
                body.append('RefId', refId);
                body.append('Notes', this.modal.notes);
                body.append('__RequestVerificationToken', token);

                const response = await fetch(actionUrls[this.modal.action], {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: body.toString()
                });

                if (response.ok || response.redirected) {
                    // Redirect to the response URL or details page
                    window.location.href = response.redirected 
                        ? response.url 
                        : `${baseUrl}/Details/${recId}`;
                } else {
                    alert('An error occurred. Please try again.');
                    this.modal.isProcessing = false;
                }
            } catch (error) {
                console.error('Workflow action failed:', error);
                alert('An error occurred. Please try again.');
                this.modal.isProcessing = false;
            }
        }
    };
}
