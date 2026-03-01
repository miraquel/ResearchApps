/**
 * Material Customer Edit Page Component
 * Handles header editing and line item management
 * 
 * @param {Array} initialLines - Initial line items from server
 * @param {Object} config - Configuration object
 * @param {number} config.recId - Record ID
 * @param {string} config.mcId - Material Customer ID
 * @param {string} config.mcDate - MC date (YYYY-MM-DD)
 * @param {number} config.customerId - Customer ID
 * @param {string} config.customerName - Customer name
 * @param {string} config.sjNo - SJ No
 * @param {string} config.refNo - Reference No
 * @param {string} config.notes - Notes
 */
function mcEdit(initialLines, config = {}) {
    return {
        // Configuration
        config: {
            recId: 0,
            mcId: '',
            mcDate: '',
            customerId: 0,
            customerName: '',
            sjNo: '',
            refNo: '',
            notes: '',
            ...config
        },
        
        // Header state
        header: {
            recId: config.recId || 0,
            mcId: config.mcId || '',
            notes: config.notes || ''
        },
        isHeaderSaving: false,
        
        // Lines state
        lines: initialLines || [],
        
        // Line modal state
        lineModal: {
            show: false,
            data: {
                mcLineId: 0,
                itemId: '',
                itemName: '',
                whId: '',
                whName: '',
                qty: 1,
                notes: ''
            },
            errors: {},
            isSaving: false
        },
        
        // Delete modal state
        deleteModal: {
            show: false,
            line: null,
            isDeleting: false
        },
        
        // TomSelect instances
        lineItemSelect: null,
        lineWhSelect: null,
        
        init() {
            this.header.recId = this.config.recId;
            this.header.mcId = this.config.mcId;
            this.header.notes = this.config.notes;
        },
        
        // ============================================
        // Header Methods
        // ============================================
        
        async saveHeader() {
            this.isHeaderSaving = true;
            
            const form = document.getElementById('mcForm');
            const formData = new FormData(form);
            
            // Add CSRF token
            const token = document.querySelector('input[name="__RequestVerificationToken"]');
            if (token) {
                formData.set('__RequestVerificationToken', token.value);
            }
            
            try {
                const response = await fetch('/MaterialCustomers/Edit', {
                    method: 'POST',
                    body: formData
                });
                
                if (response.redirected) {
                    window.location.href = response.url;
                } else if (response.ok) {
                    window.location.href = `/MaterialCustomers/Details/${this.header.recId}`;
                } else {
                    const errorMessage = await this.parseErrorResponse(response);
                    console.error('Save error:', errorMessage);
                    showNotificationModal('Error saving: ' + errorMessage, true);
                }
            } catch (error) {
                console.error('Save error:', error);
                showNotificationModal('Error saving: ' + error.message, true);
            } finally {
                this.isHeaderSaving = false;
            }
        },
        
        async parseErrorResponse(response) {
            const contentType = response.headers.get('content-type');
            
            if (contentType && contentType.includes('application/json')) {
                const result = await response.json();
                return result.message || result.title || JSON.stringify(result);
            }
            
            if (contentType && contentType.includes('text/html')) {
                const text = await response.text();
                const parser = new DOMParser();
                const doc = parser.parseFromString(text, 'text/html');
                
                let errors = [];
                
                // Check for validation summary
                const validationSummary = doc.querySelector('.validation-summary-errors ul');
                if (validationSummary) {
                    errors = Array.from(validationSummary.querySelectorAll('li'))
                        .map(li => li.textContent.trim());
                }
                
                // Check for field-level validation errors
                const fieldErrors = doc.querySelectorAll('.field-validation-error, .invalid-feedback');
                if (fieldErrors.length > 0) {
                    errors = errors.concat(
                        Array.from(fieldErrors)
                            .map(el => el.textContent.trim())
                            .filter(text => text.length > 0)
                    );
                }
                
                if (errors.length > 0) {
                    errors = [...new Set(errors)];
                    return `Validation errors:\n\n${errors.map(e => `• ${e}`).join('\n')}`;
                }
                
                return `Server returned ${response.status} error.`;
            }
            
            const text = await response.text();
            return text || `Server returned ${response.status}: ${response.statusText}`;
        },
        
        // ============================================
        // Line Modal Methods
        // ============================================
        
        showLineModal(mode, line = null) {
            this.lineModal.errors = {};
            
            this.lineModal.data = {
                mcLineId: 0,
                itemId: '',
                itemName: '',
                whId: '',
                whName: '',
                qty: 1,
                notes: ''
            };
            
            this.lineModal.show = true;
            
            // Initialize TomSelect components after modal is shown
            this.$nextTick(() => {
                this.initLineItemSelect();
            });
        },
        
        initLineItemSelect() {
            // Clean up existing instances
            if (this.lineItemSelect) {
                this.lineItemSelect.destroy();
            }
            if (this.lineWhSelect) {
                this.lineWhSelect.destroy();
            }
            
            // Initialize Item TomSelect
            this.lineItemSelect = initTomSelect('#lineItemId', {
                url: '/api/Items/cbo',
                placeholder: 'Select Item',
                maxOptions: 50,
                onChange: (value) => {
                    if (value) {
                        const option = this.lineItemSelect.options[value];
                        if (option) {
                            this.lineModal.data.itemId = value;
                            this.lineModal.data.itemName = option.text;
                        }
                    }
                }
            });
            
            // Initialize Warehouse TomSelect
            this.lineWhSelect = initTomSelect('#lineWhId', {
                url: '/api/Warehouses/cbo',
                placeholder: 'Select Warehouse',
                maxOptions: 50,
                onChange: (value) => {
                    if (value) {
                        const option = this.lineWhSelect.options[value];
                        if (option) {
                            this.lineModal.data.whId = value;
                            this.lineModal.data.whName = option.text;
                        }
                    }
                }
            });
        },
        
        closeLineModal() {
            if (!this.lineModal.isSaving) {
                this.lineModal.show = false;
            }
        },
        
        validateLine() {
            this.lineModal.errors = {};
            
            if (!this.lineModal.data.itemId) {
                this.lineModal.errors.itemId = 'Please select an item';
            }
            
            if (!this.lineModal.data.whId) {
                this.lineModal.errors.whId = 'Please select a warehouse';
            }
            
            if (!this.lineModal.data.qty || this.lineModal.data.qty <= 0) {
                this.lineModal.errors.qty = 'Quantity must be greater than 0';
            }
            
            return Object.keys(this.lineModal.errors).length === 0;
        },
        
        async saveLine() {
            if (!this.validateLine()) return;
            
            this.lineModal.isSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]');
            
            const lineData = {
                mcLineId: 0,
                recId: this.header.recId,
                itemId: parseInt(this.lineModal.data.itemId),
                whId: parseInt(this.lineModal.data.whId),
                qty: this.lineModal.data.qty,
                notes: this.lineModal.data.notes
            };
            
            try {
                const response = await fetch('/api/MaterialCustomerLines', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token?.value || ''
                    },
                    body: JSON.stringify(lineData)
                });
                
                const result = await response.json();
                
                if (response.ok) {
                    window.location.reload();
                } else {
                    const errorMsg = result.message || 'Failed to add line';
                    showNotificationModal(errorMsg, true);
                }
            } catch (error) {
                console.error('Save line error:', error);
                showNotificationModal('Error adding line: ' + error.message, true);
            } finally {
                this.lineModal.isSaving = false;
            }
        },
        
        // ============================================
        // Delete Line Methods
        // ============================================
        
        deleteLine(line) {
            this.deleteModal.line = line;
            this.deleteModal.show = true;
        },
        
        async confirmDeleteLine() {
            if (!this.deleteModal.line) return;
            
            this.deleteModal.isDeleting = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]');
            
            try {
                const response = await fetch(`/api/MaterialCustomerLines/${this.deleteModal.line.mcLineId}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token?.value || ''
                    }
                });
                
                const result = await response.json();
                
                if (response.ok) {
                    window.location.reload();
                } else {
                    const errorMsg = result.message || 'Failed to delete line';
                    showNotificationModal(errorMsg, true);
                }
            } catch (error) {
                console.error('Delete line error:', error);
                showNotificationModal('Error deleting line: ' + error.message, true);
            } finally {
                this.deleteModal.isDeleting = false;
            }
        },
        
        // ============================================
        // Utility Methods
        // ============================================
        
        formatNumber(value) {
            if (value === null || value === undefined) return '0';
            return new Intl.NumberFormat('id-ID').format(value);
        },
        
        formatCurrency(value) {
            if (value === null || value === undefined) return 'Rp 0';
            return new Intl.NumberFormat('id-ID', {
                style: 'currency',
                currency: 'IDR',
                minimumFractionDigits: 0
            }).format(value);
        }
    };
}

// Make available globally
window.mcEdit = mcEdit;
