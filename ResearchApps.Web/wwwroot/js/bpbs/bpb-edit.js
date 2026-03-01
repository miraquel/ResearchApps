/**
 * BPB (Bon Pengambilan Barang) Edit Page Component
 * Handles editing of BPB header and line items with stock validation
 */
function bpbEdit(initialLines, config) {
    return {
        // Header state
        header: {
            recId: config.recId,
            bpbId: config.bpbId,
            bpbDate: config.bpbDate,
            refId: config.refId,
            refName: config.refName || '',
            descr: config.descr || '',
            notes: config.notes || '',
            bpbStatusId: config.bpbStatusId || 0
        },
        
        // Lines state
        lines: initialLines || [],
        
        // UI state
        isHeaderSaving: false,
        
        // Line modal state
        lineModal: {
            show: false,
            mode: 'add', // 'add' or 'edit'
            data: {
                bpbRecId: 0,
                bpbLineId: '',
                itemId: '',
                itemName: '',
                whId: '',
                whName: '',
                qty: 1,
                notes: ''
            },
            errors: {},
            isSaving: false,
            stockCheck: null
        },
        
        // Delete modal state
        deleteModal: {
            show: false,
            line: null,
            isDeleting: false
        },
        
        // TomSelect instances
        itemSelect: null,
        warehouseSelect: null,
        datePicker: null,
        
        init() {
            this.initDatePicker();
        },
        
        showNotification(message, isError = false) {
            if (window.showNotificationModal) {
                window.showNotificationModal(message, isError);
            } else {
                alert(message);
            }
        },
        
        initDatePicker() {
            const dateEl = this.$refs.bpbDatePicker;
            if (dateEl && typeof flatpickr !== 'undefined') {
                // Check if already initialized
                if (dateEl._flatpickr) {
                    this.datePicker = dateEl._flatpickr;
                    this.datePicker.setDate(this.header.bpbDate, false);
                } else {
                    // Remove data-flatpickr to prevent double initialization
                    dateEl.removeAttribute('data-flatpickr');
                    
                    this.datePicker = flatpickr(dateEl, {
                        dateFormat: 'Y-m-d',
                        defaultDate: this.header.bpbDate,
                        allowInput: true,
                        onChange: (selectedDates, dateStr) => {
                            this.header.bpbDate = dateStr;
                        }
                    });
                }
            }
        },
        
        async saveHeader() {
            const form = document.getElementById('bpbForm');
            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }
            
            this.isHeaderSaving = true;
            try {
                const formData = new FormData(form);
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                
                const response = await fetch(`/api/Bpbs/${this.header.recId}`, {
                    method: 'PUT',
                    headers: {
                        'RequestVerificationToken': token
                    },
                    body: formData
                });
                
                const data = await response.json();
                
                if (response.ok && data) {
                    this.showNotification('Header saved successfully');
                } else {
                    this.showNotification(data.message || 'Failed to save header', true);
                }
            } catch (error) {
                console.error('Save header error:', error);
                this.showNotification('An error occurred while saving', true);
            } finally {
                this.isHeaderSaving = false;
            }
        },
        
        showAddLineModal() {
            this.lineModal.mode = 'add';
            this.lineModal.errors = {};
            this.lineModal.stockCheck = null;
            this.lineModal.data = {
                bpbRecId: 0,
                bpbLineId: '',
                itemId: '',
                itemName: '',
                whId: '',
                whName: '',
                qty: 1,
                notes: ''
            };
            this.lineModal.show = true;
            
            // Initialize TomSelect after modal opens
            this.$nextTick(() => {
                this.initLineItemSelect();
                this.initLineWarehouseSelect();
            });
        },
        
        editLine(index) {
            const line = this.lines[index];
            this.lineModal.mode = 'edit';
            this.lineModal.errors = {};
            this.lineModal.stockCheck = null;
            
            this.lineModal.data = {
                bpbRecId: line.bpbRecId,
                bpbLineId: line.bpbLineId,
                itemId: line.itemId,
                itemName: line.itemName,
                whId: line.whId,
                whName: line.whName,
                qty: line.qty,
                notes: line.notes || ''
            };
            
            this.lineModal.show = true;
            
            // Initialize TomSelect after modal opens
            this.$nextTick(() => {
                this.initLineItemSelect(line.itemId, line.itemName);
                this.initLineWarehouseSelect(line.whId, line.whName);
            });
        },
        
        initLineItemSelect(initialValue = null, initialText = null) {
            // Destroy existing instance
            if (this.itemSelect) {
                this.itemSelect.destroy();
                this.itemSelect = null;
            }
            
            const el = document.getElementById('lineItemId');
            if (!el) return;
            
            this.itemSelect = initTomSelect('#lineItemId', {
                url: '/api/Items/cbo',
                placeholder: 'Select Item',
                maxOptions: 50,
                onChange: (value) => {
                    this.lineModal.data.itemId = value;
                    // Get the item name from the option
                    if (this.itemSelect) {
                        const option = this.itemSelect.options[value];
                        if (option) {
                            this.lineModal.data.itemName = option.text;
                        }
                    }
                    // Trigger stock check when item changes
                    this.checkStock();
                }
            });
            
            // Set initial value if editing
            if (initialValue && initialText) {
                this.itemSelect.addOption({ value: initialValue, text: initialText });
                this.itemSelect.setValue(String(initialValue));
            }
        },
        
        initLineWarehouseSelect(initialValue = null, initialText = null) {
            // Destroy existing instance
            if (this.warehouseSelect) {
                this.warehouseSelect.destroy();
                this.warehouseSelect = null;
            }
            
            const el = document.getElementById('lineWhId');
            if (!el) return;
            
            this.warehouseSelect = initTomSelect('#lineWhId', {
                url: '/api/Warehouses/cbo',
                placeholder: 'Select Warehouse',
                maxOptions: 50,
                onChange: (value) => {
                    this.lineModal.data.whId = value;
                    // Get the warehouse name from the option
                    if (this.warehouseSelect) {
                        const option = this.warehouseSelect.options[value];
                        if (option) {
                            this.lineModal.data.whName = option.text;
                        }
                    }
                    // Trigger stock check when warehouse changes
                    this.checkStock();
                }
            });
            
            // Set initial value if editing
            if (initialValue && initialText) {
                this.warehouseSelect.addOption({ value: initialValue, text: initialText });
                this.warehouseSelect.setValue(String(initialValue));
            }
        },
        
        async checkStock() {
            const { itemId, whId, qty } = this.lineModal.data;
            
            // Clear previous check
            this.lineModal.stockCheck = null;
            
            if (!itemId || !whId || !qty || qty <= 0) {
                return;
            }
            
            try {
                const response = await fetch(`/api/Bpbs/stock-check?itemId=${itemId}&whId=${whId}&qty=${qty}`);
                
                if (response.ok) {
                    const result = await response.json();
                    if (result.data) {
                        this.lineModal.stockCheck = result.data;
                    }
                }
            } catch (error) {
                console.error('Stock check error:', error);
            }
        },
        
        closeLineModal() {
            if (!this.lineModal.isSaving) {
                this.lineModal.show = false;
                
                // Cleanup TomSelect instances
                if (this.itemSelect) {
                    this.itemSelect.destroy();
                    this.itemSelect = null;
                }
                if (this.warehouseSelect) {
                    this.warehouseSelect.destroy();
                    this.warehouseSelect = null;
                }
            }
        },
        
        confirmDeleteLine(index) {
            this.deleteModal.line = this.lines[index];
            this.deleteModal.show = true;
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
            
            // Check stock availability (warning only, not blocking)
            if (this.lineModal.stockCheck && !this.lineModal.stockCheck.isAvailable) {
                // Show warning but don't block - user can still proceed
                console.warn('Stock warning:', this.lineModal.stockCheck.message);
            }
            
            return Object.keys(this.lineModal.errors).length === 0;
        },
        
        async saveLine() {
            if (!this.validateLine()) return;
            
            this.lineModal.isSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            const payload = {
                BpbRecId: this.header.recId,
                ItemId: this.lineModal.data.itemId,
                WhId: this.lineModal.data.whId,
                Qty: parseFloat(this.lineModal.data.qty),
                ProdId: this.header.refId || '',
                Notes: this.lineModal.data.notes || ''
            };
            
            let url = '/api/Bpbs/line';
            let method = 'POST';
            
            if (this.lineModal.mode === 'edit' && this.lineModal.data.bpbRecId) {
                url = `/api/Bpbs/line/${this.lineModal.data.bpbRecId}`;
                method = 'PUT';
                payload.bpbRecId = this.lineModal.data.bpbRecId;
                payload.BpbLineId = this.lineModal.data.bpbLineId;
            }
            
            try {
                const response = await fetch(url, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(payload)
                });
                
                const result = await response.json();
                
                if (response.ok) {
                    // Refresh page to get updated lines and totals
                    window.location.reload();
                } else {
                    this.showNotification(result.message || 'Error saving line', true);
                }
            } catch (error) {
                console.error('Save line error:', error);
                this.showNotification('Error saving line', true);
            } finally {
                this.lineModal.isSaving = false;
            }
        },
        
        async deleteLine() {
            if (!this.deleteModal.line) return;
            
            this.deleteModal.isDeleting = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            try {
                const response = await fetch(`/api/Bpbs/line/${this.deleteModal.line.bpbLineId}`, {
                    method: 'DELETE',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });
                
                if (response.ok) {
                    // Remove from local array
                    this.lines = this.lines.filter(l => l.bpbLineId !== this.deleteModal.line.bpbRecId);
                    this.deleteModal.show = false;
                    this.deleteModal.line = null;
                    this.showNotification('Line deleted successfully');
                } else {
                    const result = await response.json();
                    this.showNotification(result.message || 'Error deleting line', true);
                }
            } catch (error) {
                console.error('Delete line error:', error);
                this.showNotification('Error deleting line', true);
            } finally {
                this.deleteModal.isDeleting = false;
            }
        },
        
        async reloadLines() {
            try {
                const response = await fetch(`/api/Bpbs/${this.header.recId}/lines`);
                const data = await response.json();
                
                if (data.data) {
                    this.lines = data.data.map(l => ({
                        bpbLineId: l.bpbLineId,
                        bpbRecId: l.bpbRecId,
                        itemId: l.itemId,
                        itemName: l.itemName,
                        whId: l.whId,
                        whName: l.whName,
                        qty: l.qty,
                        unitName: l.unitName,
                        notes: l.notes || ''
                    }));
                }
            } catch (error) {
                console.error('Error reloading lines:', error);
            }
        },
    };
}
