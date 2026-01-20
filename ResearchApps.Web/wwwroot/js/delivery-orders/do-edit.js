/**
 * Delivery Order Edit Page Alpine.js Component
 * Handles header editing, line management, CO change warning
 */
function doEditForm(linesData, config) {
    return {
        // Header data from config
        header: {
            recId: config.recId || 0,
            doId: config.doId || '',
            doDate: config.doDate || '',
            customerId: config.customerId || null,
            customerName: config.customerName || '',
            coId: config.coId || '',
            coRecId: config.coRecId || '',
            refId: config.refId || '',
            descr: config.descr || '',
            dn: config.dn || '',
            notes: config.notes || '',
            doStatusId: config.doStatusId || 0,
            doStatusName: config.doStatusName || ''
        },
        
        // Original values for change detection
        originalCustomerId: config.customerId || null,
        originalCoRecId: config.coRecId || '',
        
        // Lines data
        lines: linesData || [],
        
        // State
        errors: {},
        lineErrors: {},
        isHeaderSaving: false,
        isLineSaving: false,
        
        // Modals
        showSaveModal: false,
        showCoChangeWarningModal: false,
        showAddLineModal: false,
        showEditLineModal: false,
        showDeleteLineModal: false,
        
        // Line editing
        pendingCoRecId: '',
        editingLine: {
            recId: null,
            doLineId: null,
            itemName: '',
            qty: 0,
            maxQty: 0,
            notes: ''
        },
        deletingLine: {
            recId: null,
            doLineId: null,
            itemName: '',
            qty: 0,
            notes: ''
        },
        newLine: {
            coLineId: null,
            itemId: null,
            itemName: '',
            whId: null,
            qty: 0,
            maxQty: 0,
            notes: ''
        },
        
        // Outstanding CO lines for add modal
        outstandingLines: [],
        isLoadingOutstanding: false,
        isFetchingOutstanding: false,
        
        // Warehouses for add modal
        warehouses: [],
        
        // TomSelect instances
        customerSelect: null,
        coSelect: null,
        
        // Flatpickr instance
        datePicker: null,
        
        // Initialization flag
        isInitializing: true,
        
        /**
         * Check if CO has changed (blocks line operations until saved)
         */
        get isCoChanged() {
            const current = this.header.coRecId ? parseInt(this.header.coRecId) : null;
            const original = this.originalCoRecId ? parseInt(this.originalCoRecId) : null;
            return current !== original;
        },
        
        /**
         * Initialize the component
         */
        init() {
            this.initializeDatePicker();
            this.initializeCustomerSelect();
            this.initializeCoSelect();
            
            // Allow change detection after initialization
            this.$nextTick(() => {
                this.isInitializing = false;
            });
        },
        
        /**
         * Initialize Flatpickr date picker
         */
        initializeDatePicker() {
            this.datePicker = initFlatpickr('#Header_DoDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: this.header.doDate
            });
        },
        
        /**
         * Initialize Customer TomSelect
         */
        initializeCustomerSelect() {
            const self = this;
            this.customerSelect = initTomSelect('#Header_CustomerId', {
                url: '/api/Customers/cbo',
                valueField: 'value',
                labelField: 'text',
                searchField: 'text',
                preload: 'focus',
                onChange: function(value) {
                    self.onCustomerChange(value);
                }
            });
            
            // Pre-select current customer
            if (this.header.customerId) {
                this.customerSelect.addOption({
                    value: this.header.customerId,
                    text: this.header.customerName
                });
                this.customerSelect.setValue(this.header.customerId);
            }
        },
        
        /**
         * Initialize CO TomSelect
         */
        initializeCoSelect() {
            const self = this;
            this.coSelect = initTomSelect('#Header_CoRecId', {
                url: `/api/CustomerOrders/Cbo?customerId=${this.header.customerId || 0}`,
                valueField: 'value',
                labelField: 'text',
                searchField: 'text',
                preload: 'focus',
                onChange: function(value) {
                    self.onCoChange(value);
                }
            });
            
            // Pre-select current CO
            if (this.header.coRecId) {
                this.coSelect.addOption({
                    value: this.header.coRecId,
                    text: this.header.coId
                });
                this.coSelect.setValue(this.header.coRecId);
            }
        },
        
        /**
         * Handle customer selection change
         */
        onCustomerChange(value) {
            // Skip validation during initialization
            if (this.isInitializing) {
                this.header.customerId = value;
                return;
            }
            
            if (value !== this.header.customerId) {
                // Customer changed - warn about CO being cleared
                if (confirm('Changing customer will clear the Customer Order selection. Continue?')) {
                    this.header.customerId = value;
                    this.header.coRecId = '';
                    
                    // Clear and reinitialize CO select
                    if (this.coSelect) {
                        this.coSelect.clear();
                        this.coSelect.destroy();
                    }
                    
                    this.initializeCoSelect();
                } else {
                    // Revert customer selection
                    this.customerSelect.setValue(this.header.customerId);
                }
            }
        },
        
        /**
         * Handle CO selection change
         */
        onCoChange(value) {
            // Convert to int for comparison (TomSelect passes string)
            const newCoRecId = value ? parseInt(value) : null;
            const origCoRecId = this.originalCoRecId ? parseInt(this.originalCoRecId) : null;
            
            if (!newCoRecId || newCoRecId === origCoRecId) {
                this.header.coRecId = newCoRecId;
                return;
            }
            
            // CO changed - check if there are existing lines
            if (origCoRecId && this.lines.length > 0) {
                this.pendingCoRecId = newCoRecId;
                this.showCoChangeWarningModal = true;
                
                // Revert the selection WITHOUT triggering onChange (silent mode)
                this.coSelect.setValue(this.header.coRecId, true);
            } else {
                this.header.coRecId = newCoRecId;
            }
        },
        
        /**
         * Confirm CO change (after warning)
         */
        confirmCoChange() {
            this.header.coRecId = this.pendingCoRecId;
            // Set value silently to avoid triggering onChange loop
            this.coSelect.setValue(this.pendingCoRecId, true);
            this.showCoChangeWarningModal = false;
            this.pendingCoRecId = '';
            // Note: isCoChanged will now be true, blocking line operations until save
        },
        
        /**
         * Cancel CO change
         */
        cancelCoChange() {
            this.showCoChangeWarningModal = false;
            this.pendingCoRecId = '';
            // Set value silently to avoid triggering onChange
            this.coSelect.setValue(this.header.coRecId, true);
        },
        
        /**
         * Show save confirmation modal
         */
        confirmSave() {
            if (this.validateHeader()) {
                this.showSaveModal = true;
            }
        },
        
        /**
         * Validate header before save
         */
        validateHeader() {
            this.errors = {};
            
            const doDate = document.getElementById('Header_DoDate')?.value;
            if (!doDate) {
                this.errors.DoDate = 'Please select a delivery date';
            }
            
            if (!this.header.customerId) {
                this.errors.CustomerId = 'Please select a customer';
            }
            
            return Object.keys(this.errors).length === 0;
        },
        
        /**
         * Submit header form
         */
        proceedSave() {
            this.showSaveModal = false;
            this.isHeaderSaving = true;
            document.getElementById('doForm').submit();
        },
        
        /**
         * Open add line modal
         */
        async openAddLineModal() {
            this.resetNewLine();
            this.showAddLineModal = true;
            
            // Initialize warehouse TomSelect
            this.$nextTick(() => {
                this.initializeWarehouseSelect();
            });
            
            // Load outstanding lines
            await this.loadOutstandingLines();
        },
        
        /**
         * Reset new line data
         */
        resetNewLine() {
            this.newLine = {
                coLineId: null,
                itemId: null,
                itemName: '',
                whId: null,
                qty: 0,
                maxQty: 0,
                notes: ''
            };
        },
        
        /**
         * Initialize warehouse TomSelect for add modal
         */
        initializeWarehouseSelect() {
            const selectElement = document.getElementById('NewLine_WhId');
            if (selectElement && !selectElement.tomselect) {
                const self = this;
                initTomSelect('#NewLine_WhId', {
                    url: '/api/Warehouses/Cbo',
                    valueField: 'value',
                    labelField: 'text',
                    searchField: 'text',
                    preload: 'focus',
                    onChange: function(value) {
                        self.newLine.whId = value ? parseInt(value) : null;
                    }
                });
            }
        },
        
        /**
         * Initialize warehouse TomSelect for edit modal
         */
        initializeEditWarehouseSelect() {
            const selectElement = document.getElementById('EditLine_WhId');
            if (selectElement) {
                // Destroy existing instance if any
                if (selectElement.tomselect) {
                    selectElement.tomselect.destroy();
                }
                
                const self = this;
                const whSelect = initTomSelect('#EditLine_WhId', {
                    url: '/api/Warehouses/Cbo',
                    valueField: 'value',
                    labelField: 'text',
                    searchField: 'text',
                    preload: 'focus',
                    onChange: function(value) {
                        self.editingLine.whId = value ? parseInt(value) : null;
                    }
                });
                
                // Pre-select current warehouse
                if (this.editingLine.whId && this.editingLine.whName) {
                    whSelect.addOption({
                        value: this.editingLine.whId,
                        text: this.editingLine.whName
                    });
                    whSelect.setValue(this.editingLine.whId);
                }
            }
        },
        
        /**
         * Load outstanding CO lines for add line modal
         */
        async loadOutstandingLines() {
            if (!this.header.customerId) {
                this.outstandingLines = [];
                return;
            }
            
            this.isLoadingOutstanding = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                let url;
                // If DO has a CO reference with RecId, use efficient Co_OsById endpoint
                if (this.header.coRecId && this.header.coRecId > 0) {
                    url = `/api/CustomerOrders/${this.header.coRecId}/outstanding`;
                } else {
                    // popup error if no coRecId
                    showError('Cannot load outstanding lines because the Delivery Order is not linked to a valid Customer Order.', 'Error Loading Outstanding Lines');
                    this.outstandingLines = [];
                    return;
                }
                
                const response = await fetch(url, {
                    headers: { 'RequestVerificationToken': token }
                });
                const result = await response.json();
                const data = result.data || result;
                
                if (data && data.length > 0) {
                    // Get set of CO line IDs that already exist in this DO
                    const existingCoLineIds = new Set(this.lines.map(line => line.coLineId));
                    
                    // Map and filter outstanding lines - exclude lines already in this DO
                    this.outstandingLines = data
                        .filter(item => !existingCoLineIds.has(item.coLineId)) // Exclude existing lines
                        .map(item => ({
                            coLineId: item.coLineId,
                            coId: item.coId,
                            itemId: item.itemId,
                            itemName: item.itemName,
                            whId: item.whId,
                            qtyCo: item.qtyCo || 0,
                            qtyDo: item.qtyDo || 0,
                            qtyOs: item.qtyOs || item.qtyOutstanding || 0,
                            unitName: item.unitName
                        }))
                        .filter(os => os.qtyOs > 0); // Only show lines with available qty
                    
                } else {
                    this.outstandingLines = [];
                }
            } catch (error) {
                console.error('Error loading outstanding lines:', error);
                this.outstandingLines = [];
            } finally {
                this.isLoadingOutstanding = false;
            }
        },
        
        /**
         * Select an outstanding line in add modal
         */
        selectOutstandingLine(line) {
            this.newLine.coLineId = line.coLineId;
            this.newLine.itemId = line.itemId;
            this.newLine.itemName = line.itemName;
            this.newLine.whId = line.whId;
            this.newLine.maxQty = line.qtyOs;
            this.newLine.qty = line.qtyOs; // Default to full outstanding
        },
        
        /**
         * Add a new line
         */
        async addLine() {
            if (!this.newLine.coLineId || !this.newLine.whId || !this.newLine.qty || this.newLine.qty <= 0) {
                showWarning('Please select a CO line, warehouse, and enter a valid quantity.', 'Validation Error');
                return;
            }
            
            if (this.newLine.qty > this.newLine.maxQty) {
                showWarning(`Quantity cannot exceed outstanding amount (${this.newLine.maxQty})`, 'Quantity Exceeded');
                return;
            }
            
            this.isLineSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                const response = await fetch('/api/DeliveryOrderLines', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify({
                        doRecId: this.header.recId,
                        itemId: this.newLine.itemId,
                        whId: this.newLine.whId,
                        qty: this.newLine.qty,
                        notes: this.newLine.notes || '',
                        coLineId: this.newLine.coLineId,
                        coId: this.header.coId || ''
                    })
                });
                
                const result = await response.json();
                
                if (response.ok) {
                    this.showAddLineModal = false;
                    location.reload(); // Reload to get updated lines
                } else {
                    // Extract error message from API response
                    const errorMessage = result.detail || result.message || result.title || 'Error adding line';
                    showError(errorMessage, 'Failed to Add Line');
                }
            } catch (error) {
                console.error('Error adding line:', error);
                showError('Error adding line. Please try again.', 'Error');
            } finally {
                this.isLineSaving = false;
            }
        },
        
        /**
         * Open edit line modal
         */
        async openEditLineModal(line) {
            // Clear any previous errors
            this.lineErrors = {};
            
            // Set basic line data first
            this.editingLine = {
                doLineId: line.doLineId,
                coId: line.coId || '',
                coLineId: line.coLineId,
                itemName: line.itemName,
                whId: line.whId,
                whName: line.whName || '',
                originalQty: line.qty,
                qty: line.qty,
                qtyOutstanding: 0,
                notes: line.notes || ''
            };
            
            this.showEditLineModal = true;
            
            // Initialize warehouse select for edit modal
            this.$nextTick(() => {
                this.initializeEditWarehouseSelect();
            });
            
            // Fetch outstanding for this CO line
            await this.fetchOutstandingForLine(line.coLineId, line.qty);
        },
        
        /**
         * Fetch outstanding quantity for a CO line
         */
        async fetchOutstandingForLine(coLineId, currentQty) {
            this.isFetchingOutstanding = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                // Get outstanding for this specific CO line
                const response = await fetch(`/api/CustomerOrders/outstanding/${coLineId}`, {
                    headers: { 'RequestVerificationToken': token }
                });
                
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}`);
                }
                
                const result = await response.json();
                const coLine = result.data || result;
                
                if (coLine && coLine.qtyOs !== undefined) {
                    // Outstanding is the CO line outstanding + current line qty (since we're editing)
                    this.editingLine.qtyOutstanding = coLine.qtyOs + currentQty;
                } else {
                    // If not found, just use current qty as max
                    this.editingLine.qtyOutstanding = currentQty;
                }
            } catch (error) {
                console.error('Error fetching outstanding:', error);
                // Fallback to current qty
                this.editingLine.qtyOutstanding = currentQty;
            } finally {
                this.isFetchingOutstanding = false;
            }
        },
        
        /**
         * Update a line
         */
        async updateLine() {
            // Clear previous errors
            this.lineErrors = {};
            
            if (!this.editingLine.qty || this.editingLine.qty <= 0) {
                this.lineErrors.qty = 'Please enter a valid quantity.';
                return;
            }
            
            if (this.editingLine.qty > this.editingLine.qtyOutstanding) {
                this.lineErrors.qty = `Quantity cannot exceed outstanding amount (${this.formatNumber(this.editingLine.qtyOutstanding)})`;
                return;
            }
            
            this.isLineSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                const response = await fetch('/api/DeliveryOrderLines', {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify({
                        doLineId: this.editingLine.doLineId,
                        whId: this.editingLine.whId,
                        qty: this.editingLine.qty,
                        notes: this.editingLine.notes
                    })
                });
                
                const result = await response.json();
                
                if (response.ok) {
                    this.showEditLineModal = false;
                    location.reload(); // Reload to get updated lines
                } else {
                    // Extract error message from API response
                    const errorMessage = result.detail || result.message || result.title || 'Error updating line';
                    showError(errorMessage, 'Failed to Update Line');
                }
            } catch (error) {
                console.error('Error updating line:', error);
                showError('Error updating line. Please try again.', 'Error');
            } finally {
                this.isLineSaving = false;
            }
        },
        
        /**
         * Open delete line confirmation
         */
        openDeleteLineModal(line) {
            console.log('openDeleteLineModal called with line:', line);
            this.deletingLine = line;
            this.showDeleteLineModal = true;
            console.log('showDeleteLineModal set to:', this.showDeleteLineModal);
        },
        
        /**
         * Delete a line
         */
        async deleteLine() {
            console.log('deleteLine called, deletingLine:', this.deletingLine);
            console.log('deletingLine.doLineId:', this.deletingLine.doLineId);
            
            if (!this.deletingLine.doLineId) {
                console.log('No recId found, returning early');
                return;
            }
            
            this.isLineSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                console.log('Sending DELETE request to:', `/api/DeliveryOrderLines/${this.deletingLine.doLineId}`);
                const response = await fetch(`/api/DeliveryOrderLines/${this.deletingLine.doLineId}`, {
                    method: 'DELETE',
                    headers: { 'RequestVerificationToken': token }
                });
                
                console.log('Response status:', response.status);
                
                if (response.ok) {
                    this.showDeleteLineModal = false;
                    location.reload(); // Reload to get updated lines
                } else {
                    const result = await response.json();
                    // Extract error message from API response
                    const errorMessage = result.detail || result.message || result.title || 'Error deleting line';
                    showError(errorMessage, 'Failed to Delete Line');
                }
            } catch (error) {
                console.error('Error deleting line:', error);
                showError('Error deleting line. Please try again.', 'Error');
            } finally {
                this.isLineSaving = false;
            }
        },
        
        /**
         * Format number with thousand separators
         */
        formatNumber(value) {
            if (value === null || value === undefined || value === '') return '';
            const num = parseFloat(String(value).replace(/,/g, ''));
            if (isNaN(num)) return value;
            return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
    };
}

// Make available globally
window.doEditForm = doEditForm;
