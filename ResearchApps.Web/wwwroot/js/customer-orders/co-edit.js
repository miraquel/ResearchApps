/**
 * Customer Order Edit Page Component
 * Handles header editing, line item management, and workflow actions
 * 
 * @param {Array} initialLines - Initial line items from server
 * @param {Object} config - Configuration object
 * @param {number} config.recId - Record ID
 * @param {string} config.coId - Customer Order ID
 * @param {string} config.coDate - Order date (YYYY-MM-DD)
 * @param {number} config.customerId - Customer ID
 * @param {string} config.customerName - Customer name
 * @param {number} config.coTypeId - Order type ID
 * @param {string} config.coTypeName - Order type name
 * @param {string} config.notes - Order notes
 */
function coEdit(initialLines, config = {}) {
    return {
        // Configuration
        config: {
            recId: 0,
            coId: '',
            coDate: '',
            customerId: 0,
            customerName: '',
            coTypeId: 0,
            coTypeName: '',
            notes: '',
            ...config
        },
        
        // Header state
        header: {
            recId: config.recId || 0,
            coId: config.coId || '',
            notes: config.notes || ''
        },
        isHeaderSaving: false,
        showSaveModal: false,
        requireConfirmation: false, // Toggle to control confirmation dialog
        
        // Lines state
        lines: initialLines || [],
        
        // Line modal state
        lineModal: {
            show: false,
            mode: 'add', // 'add' or 'edit'
            data: {
                recId: 0,
                coLineId: 0,
                itemId: '',
                itemName: '',
                wantedDeliveryDate: '',
                qty: 1,
                price: 0,
                amount: 0,
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
        
        // Line detail modal state
        lineDetailModal: {
            show: false,
            line: null
        },
        
        // TomSelect instances
        customerSelect: null,
        coTypeSelect: null,
        lineItemSelect: null,
        
        // Flatpickr instances
        coDatePicker: null,
        lineWantedDatePicker: null,
        
        init() {
            this.header.recId = this.config.recId;
            this.header.coId = this.config.coId;
            this.header.notes = this.config.notes;
            this.initHeaderComponents();
        },
        
        initHeaderComponents() {
            // Initialize date picker
            this.coDatePicker = initFlatpickr('#Header_CoDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: this.config.coDate
            });
            
            // Initialize Customer TomSelect
            this.customerSelect = initTomSelect('#Header_CustomerId', {
                url: '/api/Customers/cbo',
                placeholder: 'Select Customer'
            });
            
            // Pre-select current customer
            if (this.customerSelect && this.config.customerId) {
                this.customerSelect.addOption({
                    value: String(this.config.customerId),
                    text: this.config.customerName
                });
                this.customerSelect.setValue(String(this.config.customerId));
            }
            
            // Initialize Order Type TomSelect
            this.coTypeSelect = initTomSelect('#Header_CoTypeId', {
                url: '/api/CustomerOrders/types/cbo',
                placeholder: 'Select Order Type'
            });
            
            // Pre-select current type
            if (this.coTypeSelect && this.config.coTypeId) {
                this.coTypeSelect.addOption({
                    value: String(this.config.coTypeId),
                    text: this.config.coTypeName
                });
                this.coTypeSelect.setValue(String(this.config.coTypeId));
            }
        },
        
        initLineItemSelect() {
            // Destroy existing instance if present
            if (this.lineItemSelect) {
                this.lineItemSelect.destroy();
            }
            
            // Initialize Item TomSelect for line modal
            this.lineItemSelect = initTomSelect('#lineItemId', {
                url: '/api/Items/cbo',
                placeholder: 'Select Item',
                maxOptions: 100,
                onChange: (value) => {
                    // Get selected item details
                    const option = this.lineItemSelect.options[value];
                    if (option) {
                        this.lineModal.data.itemId = value;
                        this.lineModal.data.itemName = option.text;
                    }
                }
            });
            
            // Initialize wanted date picker for line modal
            if (this.lineWantedDatePicker) {
                this.lineWantedDatePicker.destroy();
            }
            
            this.lineWantedDatePicker = initFlatpickr('#lineWantedDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
            });
        },
        
        // ============================================
        // Header Methods
        // ============================================
        
        saveHeader() {
            if (this.requireConfirmation) {
                this.showSaveModal = true;
            } else {
                this.confirmSaveHeader();
            }
        },
        
        async confirmSaveHeader() {
            this.showSaveModal = false;
            this.isHeaderSaving = true;
            
            const form = document.getElementById('coForm');
            const formData = new FormData(form);
            
            try {
                const response = await fetch('/CustomerOrders/Edit', {
                    method: 'POST',
                    body: formData
                });
                
                if (response.redirected) {
                    window.location.href = response.url;
                } else if (response.ok) {
                    window.location.href = `/CustomerOrders/Details/${this.header.recId}`;
                } else {
                    const errorMessage = await this.parseErrorResponse(response);
                    console.error('Save error:', errorMessage);
                    alert('Error saving order:\n\n' + errorMessage);
                }
            } catch (error) {
                console.error('Save error:', error);
                alert('Error saving order:\n\n' + error.message);
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
                
                // Check for span.text-danger
                const dangerSpans = doc.querySelectorAll('span.text-danger');
                if (dangerSpans.length > 0) {
                    errors = errors.concat(
                        Array.from(dangerSpans)
                            .map(el => el.textContent.trim())
                            .filter(text => text.length > 0)
                    );
                }
                
                if (errors.length > 0) {
                    errors = [...new Set(errors)];
                    return `Validation errors:\n\n${errors.map(e => `• ${e}`).join('\n')}`;
                }
                
                console.error('Full HTML response:', text);
                return `Server returned ${response.status} error.\n\nNo specific validation errors found.\nCheck browser console for full response.`;
            }
            
            const text = await response.text();
            return text || `Server returned ${response.status}: ${response.statusText}`;
        },
        
        // ============================================
        // Line Detail Modal
        // ============================================
        
        showLineDetailModal(line) {
            this.lineDetailModal.line = line;
            this.lineDetailModal.show = true;
        },
        
        // ============================================
        // Line Modal Methods
        // ============================================
        
        showLineModal(mode, line = null) {
            this.lineModal.mode = mode;
            this.lineModal.errors = {};
            
            if (mode === 'add') {
                this.lineModal.data = {
                    recId: 0,
                    coLineId: 0,
                    itemId: '',
                    itemName: '',
                    wantedDeliveryDate: new Date().toISOString().split('T')[0],
                    qty: 1,
                    price: 0,
                    amount: 0,
                    notes: ''
                };
            } else if (line) {
                this.lineModal.data = {
                    recId: line.recId,
                    coLineId: line.coLineId,
                    itemId: line.itemId.toString(),
                    itemName: line.itemName,
                    wantedDeliveryDate: line.wantedDeliveryDate || '',
                    qty: line.qty,
                    price: line.price,
                    amount: line.amount,
                    notes: line.notes || ''
                };
            }
            
            this.lineModal.show = true;
            
            // Initialize components after modal is shown
            this.$nextTick(() => {
                this.initLineItemSelect();
                
                // Pre-select item if editing
                if (mode === 'edit' && line && this.lineItemSelect) {
                    this.lineItemSelect.addOption({
                        value: line.itemId.toString(),
                        text: line.itemName
                    });
                    this.lineItemSelect.setValue(line.itemId.toString());
                }
                
                // Set date
                if (this.lineWantedDatePicker && this.lineModal.data.wantedDeliveryDate) {
                    this.lineWantedDatePicker.setDate(this.lineModal.data.wantedDeliveryDate);
                }
            });
        },
        
        closeLineModal() {
            if (!this.lineModal.isSaving) {
                this.lineModal.show = false;
            }
        },
        
        calculateLineAmount() {
            this.lineModal.data.amount = (this.lineModal.data.qty || 0) * (this.lineModal.data.price || 0);
        },
        
        validateLine() {
            this.lineModal.errors = {};
            
            if (!this.lineModal.data.itemId) {
                this.lineModal.errors.itemId = 'Please select an item';
            }
            
            if (!this.lineModal.data.qty || this.lineModal.data.qty <= 0) {
                this.lineModal.errors.qty = 'Quantity must be greater than 0';
            }
            
            if (this.lineModal.data.price < 0) {
                this.lineModal.errors.price = 'Price cannot be negative';
            }
            
            return Object.keys(this.lineModal.errors).length === 0;
        },
        
        async saveLine() {
            if (!this.validateLine()) return;
            
            this.lineModal.isSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            // Get date from picker
            const dateInput = document.getElementById('lineWantedDate');
            const wantedDate = dateInput?.value || this.lineModal.data.wantedDeliveryDate;
            
            const lineData = {
                recId: this.lineModal.data.recId || 0,
                coLineId: this.lineModal.data.coLineId || 0,
                coRecId: this.header.recId,
                itemId: parseInt(this.lineModal.data.itemId),
                wantedDeliveryDate: wantedDate || null,
                qty: this.lineModal.data.qty,
                price: this.lineModal.data.price,
                notes: this.lineModal.data.notes
            };
            
            const url = '/api/CustomerOrderLines';
            const method = this.lineModal.mode === 'add' ? 'POST' : 'PUT';
            
            try {
                const response = await fetch(url, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(lineData)
                });
                
                const result = await response.json();
                
                if (response.ok) {
                    // Refresh page to get updated lines
                    window.location.reload();
                } else {
                    alert(result.message || 'Error saving line');
                }
            } catch (error) {
                console.error('Save line error:', error);
                alert('Error saving line');
            } finally {
                this.lineModal.isSaving = false;
            }
        },
        
        // ============================================
        // Delete Modal Methods
        // ============================================
        
        confirmDeleteLine(line) {
            this.deleteModal.line = line;
            this.deleteModal.show = true;
        },
        
        async deleteLine() {
            if (!this.deleteModal.line) return;
            
            this.deleteModal.isDeleting = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            try {
                const response = await fetch(`/api/CustomerOrderLines/${this.deleteModal.line.recId}`, {
                    method: 'DELETE',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });
                
                if (response.ok) {
                    // Remove from local array
                    this.lines = this.lines.filter(l => l.recId !== this.deleteModal.line.recId);
                    this.deleteModal.show = false;
                    this.deleteModal.line = null;
                } else {
                    const result = await response.json();
                    alert(result.message || 'Error deleting line');
                }
            } catch (error) {
                console.error('Delete line error:', error);
                alert('Error deleting line');
            } finally {
                this.deleteModal.isDeleting = false;
            }
        },
        
        // ============================================
        // Utility Methods
        // ============================================
        
        calculateTotal() {
            return this.lines.reduce((sum, line) => sum + (line.amount || 0), 0);
        },
    };
}

// Make available globally
window.coEdit = coEdit;
