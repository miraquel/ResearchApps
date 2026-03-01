/**
 * Workflow Details/Edit Page Component
 * Handles approval step CRUD operations inline
 * @param {number} wfFormId - The WfForm ID to manage steps for
 * @returns {Object} Alpine.js component
 */
function workflowDetails(wfFormId) {
    return {
        wfFormId: wfFormId,
        showStepForm: false,
        isSaving: false,
        editingStepId: null,
        stepForm: {
            WfFormId: wfFormId,
            Index: 1,
            UserId: ''
        },

        /** @type {TomSelect|null} */
        userSelect: null,

        init() {
            // Steps are already rendered server-side in the partial
        },

        /**
         * Initialize TomSelect for user dropdown
         */
        initUserSelect() {
            if (this.userSelect) {
                this.userSelect.destroy();
                this.userSelect = null;
            }

            const self = this;
            const el = document.getElementById('stepUserIdSelect');
            if (!el) return;

            this.userSelect = new TomSelect(el, {
                valueField: 'value',
                labelField: 'text',
                searchField: ['text', 'value'],
                placeholder: 'Search user...',
                allowEmptyOption: false,
                create: false,
                load: async function (query, callback) {
                    try {
                        const params = new URLSearchParams();
                        if (query) params.set('term', query);
                        const response = await fetch(`/api/Users/cbo?${params.toString()}`, {
                            headers: { 'X-TomSelect': 'true' }
                        });
                        if (!response.ok) throw new Error(`HTTP ${response.status}`);
                        const data = await response.json();
                        callback(data);
                    } catch (error) {
                        console.error('[Workflow Details] Error loading users:', error);
                        callback();
                    }
                },
                onChange: function (value) {
                    self.stepForm.UserId = value;
                },
                onInitialize: function () {
                    this.load('');
                }
            });
        },

        /**
         * Show the add step form
         */
        showAddStep() {
            this.editingStepId = null;
            this.stepForm = {
                WfFormId: this.wfFormId,
                Index: 1,
                UserId: ''
            };
            this.showStepForm = true;

            this.$nextTick(() => {
                this.initUserSelect();
            });
        },

        /**
         * Edit an existing step
         * @param {number} wfId - Step ID
         * @param {number} wfFormId - Form ID
         * @param {number} index - Approval level
         * @param {string} userId - Approver user ID
         */
        editStep(wfId, wfFormId, index, userId) {
            this.editingStepId = wfId;
            this.stepForm = {
                WfId: wfId,
                WfFormId: wfFormId,
                Index: index,
                UserId: userId
            };
            this.showStepForm = true;

            this.$nextTick(() => {
                this.initUserSelect();
                // Pre-select the current user after options load
                if (this.userSelect && userId) {
                    const trySet = () => {
                        if (this.userSelect.options[userId]) {
                            this.userSelect.setValue(userId, true);
                        } else {
                            setTimeout(trySet, 150);
                        }
                    };
                    setTimeout(trySet, 200);
                }
            });
        },

        /**
         * Cancel step form
         */
        cancelStep() {
            this.showStepForm = false;
            this.editingStepId = null;
            this.stepForm = {
                WfFormId: this.wfFormId,
                Index: 1,
                UserId: ''
            };
            if (this.userSelect) {
                this.userSelect.destroy();
                this.userSelect = null;
            }
        },

        /**
         * Save step (insert or update)
         */
        async saveStep() {
            if (!this.stepForm.UserId || !this.stepForm.Index) {
                if (typeof showError === 'function') {
                    showError('Please fill in all required fields.');
                }
                return;
            }

            this.isSaving = true;
            try {
                const isEdit = !!this.editingStepId;
                const url = '/api/Workflows/steps';
                const method = isEdit ? 'PUT' : 'POST';

                const payload = {
                    WfFormId: this.wfFormId,
                    Index: this.stepForm.Index,
                    UserId: this.stepForm.UserId
                };

                if (isEdit) {
                    payload.WfId = this.editingStepId;
                }

                const response = await fetch(url, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                    },
                    body: JSON.stringify(payload)
                });

                const data = await response.json();

                if (response.ok) {
                    this.cancelStep();
                    await this.refreshSteps();
                    if (typeof showSuccess === 'function') {
                        showSuccess(isEdit ? 'Step updated successfully.' : 'Step added successfully.');
                    }
                } else {
                    const errorMsg = data.message || 'Failed to save step.';
                    if (typeof showError === 'function') {
                        showError(errorMsg);
                    }
                }
            } catch (error) {
                console.error('[Workflow Details] Save step error:', error);
                if (typeof showError === 'function') {
                    showError('An error occurred while saving the step.');
                }
            } finally {
                this.isSaving = false;
            }
        },

        /**
         * Delete an approval step
         * @param {number} wfId - Step ID to delete
         */
        async deleteStep(wfId) {
            if (!confirm('Are you sure you want to delete this approval step?')) return;

            try {
                const response = await fetch(`/api/Workflows/steps/${wfId}`, {
                    method: 'DELETE',
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                    }
                });

                const data = await response.json();

                if (response.ok) {
                    await this.refreshSteps();
                    if (typeof showSuccess === 'function') {
                        showSuccess('Step deleted successfully.');
                    }
                } else {
                    const errorMsg = data.message || 'Failed to delete step.';
                    if (typeof showError === 'function') {
                        showError(errorMsg);
                    }
                }
            } catch (error) {
                console.error('[Workflow Details] Delete step error:', error);
                if (typeof showError === 'function') {
                    showError('An error occurred while deleting the step.');
                }
            }
        },

        /**
         * Refresh steps list from server
         */
        async refreshSteps() {
            try {
                const response = await fetch(`/Admin/Workflows/Steps?wfFormId=${this.wfFormId}`);
                if (response.ok) {
                    const html = await response.text();
                    const container = document.getElementById('workflow-steps-container');
                    if (container) {
                        container.innerHTML = html;
                    }
                }
            } catch (error) {
                console.error('[Workflow Details] Failed to refresh steps:', error);
            }
        }
    };
}

window.workflowDetails = workflowDetails;
