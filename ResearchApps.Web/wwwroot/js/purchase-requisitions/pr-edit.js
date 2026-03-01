/**
 * Purchase Requisition Edit Page Component
 * Handles header editing, line item management, and workflow actions
 * 
 * @param {Object} config - Configuration object
 * @param {number} config.recId - Record ID
 * @param {string} config.prId - Purchase Requisition ID
 * @param {string} config.prDate - PR date (YYYY-MM-DD)
 * @param {string} config.prName - PR name/title
 * @param {number} config.budgetId - Budget ID
 * @param {string} config.budgetName - Budget name
 * @param {string} config.notes - PR notes
 * @param {number} config.prStatusId - PR status ID
 */
function prEdit(config = {}) {
    return {
        // Configuration
        config: {
            recId: 0,
            prId: '',
            prDate: '',
            prName: '',
            budgetId: 0,
            budgetName: '',
            notes: '',
            prStatusId: 0,
            ...config
        },
        
        // Header state
        header: {
            prDate: config.prDate || '',
            prName: config.prName || '',
            budgetId: config.budgetId || 0,
            notes: config.notes || ''
        },
        headerErrors: {},
        isHeaderSaving: false,
        
        // Lines state
        lines: [],
        
        // Line modal state
        lineModal: {
            show: false,
            mode: 'add', // 'add' or 'edit'
            data: {
                prLineId: 0,
                itemId: '',
                itemName: '',
                requestDate: '',
                qty: 1,
                unitId: 0,
                unitName: '',
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
        
        // TomSelect instances
        budgetSelect: null,
        
        // Flatpickr instances
        prDatePicker: null,
        lineRequestDatePicker: null,
        
        // Item Selector state (shared component)
        itemSelector: createItemSelector({
            useTypeFilter: false,
            useDeptFilter: false,
            priceField: 'purchasePrice'
        }),
        
        init() {
            this.initHeaderComponents();
        },
        
        initHeaderComponents() {
            // Initialize PR date picker
            this.prDatePicker = initFlatpickr('#Header_PrDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: this.config.prDate
            });
            
            // Initialize Budget TomSelect
            this.budgetSelect = initTomSelect('#Header_BudgetId', {
                url: '/api/Budgets/cbo',
                placeholder: 'Select Budget'
            });
            
            // Pre-select current budget
            if (this.budgetSelect && this.config.budgetId) {
                this.budgetSelect.addOption({
                    value: String(this.config.budgetId),
                    text: this.config.budgetName
                });
                this.budgetSelect.setValue(String(this.config.budgetId));
            }
        },
        
        initLineItemSelect() {
            // No TomSelect initialization needed for items - using item selector modal
            
            // Initialize request date picker for line modal
            if (this.lineRequestDatePicker) {
                this.lineRequestDatePicker.destroy();
            }
            
            this.lineRequestDatePicker = initFlatpickr('#lineRequestDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
            });
        },
        
        openItemSelector() {
            this.itemSelector.open(null, null, this.$nextTick.bind(this));
        },
        
        resetItemFilters() {
            this.itemSelector.resetFilters();
        },
        
        searchItems() {
            this.itemSelector.search();
        },
        
        selectItem(item) {
            this.itemSelector.select(item, this.lineModal.data, this.lineModal.data.qty);
        },
        
        handleItemSearchEnter() {
            this.itemSelector.handleSearchEnter();
        },
        
        goToItemPage(page) {
            this.itemSelector.goToPage(page);
        },
        
        getVisiblePages() {
            return this.itemSelector.getVisiblePages();
        },
        
        // ============================================
        // Header Methods
        // ============================================
        
        async saveHeader() {
            this.isHeaderSaving = true;
            this.headerErrors = {};
            
            const form = document.getElementById('prForm');
            const formData = new FormData(form);
            
            try {
                const response = await fetch('/Prs/Edit', {
                    method: 'POST',
                    body: formData
                });
                
                if (response.redirected) {
                    window.location.href = response.url;
                } else if (response.ok) {
                    const result = await response.json();
                    if (result.ok) {
                        window.location.href = `/Prs/Details/${this.config.recId}`;
                    } else {
                        showNotificationModal('Error saving PR: ' + result.message, true);
                    }
                } else {
                    const errorMessage = await this.parseErrorResponse(response);
                    console.error('Save error:', errorMessage);
                    showNotificationModal('Error saving PR: ' + errorMessage, true);
                }
            } catch (error) {
                console.error('Save error:', error);
                showNotificationModal('Error saving PR: ' + error.message, true);
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
                
                console.error('Full HTML response:', text);
                return `Server returned ${response.status} error.\n\nNo specific validation errors found.\nCheck browser console for full response.`;
            }
            
            const text = await response.text();
            return text || `Server returned ${response.status}: ${response.statusText}`;
        },
        
        // ============================================
        // Line Loading
        // ============================================
        
        async loadLines() {
            try {
                const response = await fetch(`/api/PrLines/${this.config.prId}`);
                const result = await response.json();
                
                if (response.ok) {
                    // API returns array in result.data
                    this.lines = Array.isArray(result.data) ? result.data : [];
                } else {
                    console.error('Error loading lines:', result.message);
                    this.lines = [];
                }
            } catch (error) {
                console.error('Error loading lines:', error);
                this.lines = [];
            }
        },
        
        // ============================================
        // Line Modal Methods
        // ============================================
        
        showLineModal(mode, line = null) {
            this.lineModal.mode = mode;
            this.lineModal.errors = {};
            
            if (mode === 'add') {
                this.lineModal.data = {
                    prLineId: 0,
                    itemId: '',
                    itemName: '',
                    requestDate: new Date().toISOString().split('T')[0],
                    qty: 1,
                    unitId: 0,
                    unitName: '',
                    price: 0,
                    amount: 0,
                    notes: '',
                };
            } else if (line) {
                this.lineModal.data = {
                    prLineId: line.prLineId,
                    itemId: line.itemId.toString(),
                    itemName: line.itemName,
                    requestDate: line.requestDate || '',
                    qty: line.qty,
                    unitId: line.unitId || 0,
                    unitName: line.unitName || '',
                    price: line.price,
                    amount: line.amount,
                    notes: line.notes || '',
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
                if (this.lineRequestDatePicker && this.lineModal.data.requestDate) {
                    this.lineRequestDatePicker.setDate(this.lineModal.data.requestDate);
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
            const dateInput = document.getElementById('lineRequestDate');
            const requestDate = dateInput?.value || this.lineModal.data.requestDate;
            
            const lineData = {
                prLineId: this.lineModal.data.prLineId || 0,
                prRecId: this.config.recId,
                itemId: parseInt(this.lineModal.data.itemId),
                requestDate: requestDate || null,
                qty: parseFloat(this.lineModal.data.qty) || 0,
                price: parseFloat(this.lineModal.data.price) || 0,
                notes: this.lineModal.data.notes || ''
            };
            
            const url = '/api/PrLines';
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
                    // Reload lines from server
                    await this.loadLines();
                    this.lineModal.show = false;
                } else {
                    showNotificationModal(result.message || 'Error saving line', true);
                }
            } catch (error) {
                console.error('Save line error:', error);
                showNotificationModal('Error saving line', true);
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
                const response = await fetch(`/api/PrLines/${this.deleteModal.line.prLineId}`, {
                    method: 'DELETE',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });
                
                const result = await response.json();
                
                if (response.ok) {
                    // Remove from local array
                    this.lines = this.lines.filter(l => l.prLineId !== this.deleteModal.line.prLineId);
                    this.deleteModal.show = false;
                    this.deleteModal.line = null;
                } else {
                    showNotificationModal(result.message || 'Error deleting line', true);
                }
            } catch (error) {
                console.error('Delete line error:', error);
                showNotificationModal('Error deleting line', true);
            } finally {
                this.deleteModal.isDeleting = false;
            }
        },
        
        // ============================================
        // Utility Methods
        // ============================================
        
        calculateTotal() {
            if (!Array.isArray(this.lines)) return 0;
            return this.lines.reduce((sum, line) => sum + (line.amount || 0), 0);
        },
    };
}

// Make available globally
window.prEdit = prEdit;
