/**
 * Customer Order Details page Alpine.js component
 */
function coDetails(coId, recId) {
    return {
        workflow: null,
        isLoading: false,
        lineDetailModal: {
            show: false,
            line: null
        },

        /**
         * Initialize the component
         */
        init() {
            // Initialize workflow component with config object
            this.workflow = createWorkflowComponent({
                recId: recId,
                refId: coId,
                baseUrl: '/CustomerOrders',
                redirectUrl: `/CustomerOrders/Details/${recId}`,
                actions: {
                    submit: { enabled: false }, // Not available in Details view
                    approve: { enabled: true },
                    reject: { enabled: true },
                    recall: { enabled: true },
                    close: { enabled: true }
                }
            });

            // Watch workflow processing state
            this.$watch('workflow.modal.isProcessing', value => {
                this.isLoading = value;
            });
        },

        /**
         * Show line detail modal with the provided line data
         * @param {string|object} lineJson - Line data as JSON string or object
         */
        showLineDetailModal(lineJson) {
            try {
                const line = typeof lineJson === 'string' ? JSON.parse(lineJson) : lineJson;
                this.lineDetailModal.line = line;
                this.lineDetailModal.show = true;
            } catch (error) {
                console.error('Failed to parse line data:', error);
            }
        }
    };
}
