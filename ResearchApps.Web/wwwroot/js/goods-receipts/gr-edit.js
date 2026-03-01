// Goods Receipt Edit page - Alpine.js component
function grEdit(initialLines, config) {
    return {
        // Header state
        header: {
            recId: config.recId,
            grId: config.grId,
            grDate: config.grDate,
            supplierId: config.supplierId,
            supplierName: config.supplierName,
            refNo: config.refNo,
            notes: config.notes
        },
        
        // Lines state
        lines: initialLines || [],
        
        // UI state
        isHeaderSaving: false,
        
        // Line modal state (matching CO pattern)
        lineModal: {
            show: false,
            mode: 'add', // 'add' or 'edit'
            data: {
                recId: 0,
                grLineId: '',
                poLineId: '',
                poId: '',
                poDisplayText: '',
                itemId: '',
                itemName: '',
                qty: 1,
                maxQty: 0,
                unitId: '',
                unitName: '',
                price: 0,
                whId: 1,
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
        supplierSelect: null,
        datePicker: null,
        
        // PO Line Selector modal (integrated like prLineSelector in po-edit.js)
        poLineSelector: {
            show: false,
            isLoading: false,
            items: [],
            filters: {
                poId: '',
                itemName: ''
            },
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0,
            supplierId: null
        },
        
        init() {
            this.initSupplierSelect();
            this.initDatePicker();
        },
        
        showNotification(message, isError = false) {
            if (window.showNotificationModal) {
                window.showNotificationModal(message, isError);
            } else {
                alert(message);
            }
        },
        
        initSupplierSelect() {
            if (this.supplierSelect) {
                this.supplierSelect.destroy();
            }
            
            this.supplierSelect = initTomSelect('#Header_SupplierId', {
                url: '/api/Suppliers/cbo',
                placeholder: 'Select a supplier',
                maxOptions: 100,
                onChange: (value) => {
                    this.header.supplierId = value;
                }
            });
            
            // preset initial value
            if (this.header.supplierId) {
                this.supplierSelect.addOption({
                    value: this.header.supplierId,
                    text: this.header.supplierName
                });
                this.supplierSelect.setValue(String(this.header.supplierId));
            }
        },
        
        initDatePicker() {
            const dateEl = this.$refs.grDatePicker;
            if (dateEl && typeof flatpickr !== 'undefined') {
                // Check if already initialized
                if (dateEl._flatpickr) {
                    this.datePicker = dateEl._flatpickr;
                    this.datePicker.setDate(this.header.grDate, false);
                } else {
                    // Remove data-flatpickr to prevent double initialization
                    dateEl.removeAttribute('data-flatpickr');
                    
                    this.datePicker = flatpickr(dateEl, {
                        dateFormat: 'Y-m-d',
                        defaultDate: this.header.grDate,
                        allowInput: true,
                        onChange: (selectedDates, dateStr) => {
                            this.header.grDate = dateStr;
                        }
                    });
                }
            }
        },
        
        /**
         * Open PO Line selector modal
         */
        openPoLineSelector() {
            if (!this.header.supplierId) {
                this.showNotification('Please select a supplier first', true);
                return;
            }
            
            this.poLineSelector.supplierId = this.header.supplierId;
            this.poLineSelector.show = true;
            this.searchPoLines();
        },
        
        /**
         * Close PO Line selector modal
         */
        closePoLineSelector() {
            this.poLineSelector.show = false;
            this.poLineSelector.items = [];
            this.poLineSelector.filters = {
                poId: '',
                itemName: ''
            };
            this.poLineSelector.pageNumber = 1;
        },
        
        /**
         * Search PO lines with filters
         */
        async searchPoLines() {
            this.poLineSelector.isLoading = true;
            
            try {
                const response = await fetch(`/api/GoodsReceipts/outstanding/${this.poLineSelector.supplierId}`);
                
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                }
                
                const result = await response.json();
                
                if (result && result.data) {
                    // No mapping needed - properties already match Po_OsSelect SP output
                    const mappedData = result.data;
                    
                    // Filter out PO lines already added to this GR
                    const existingPoLineIds = this.lines.map(line => line.poLineId);
                    let filteredItems = mappedData.filter(po => !existingPoLineIds.includes(po.poLineId));
                    
                    // Filter client-side based on search criteria
                    
                    if (this.poLineSelector.filters.poId) {
                        const searchPo = this.poLineSelector.filters.poId.toLowerCase();
                        filteredItems = filteredItems.filter(po => 
                            po.poId && po.poId.toLowerCase().includes(searchPo)
                        );
                    }
                    
                    if (this.poLineSelector.filters.itemName) {
                        const searchItem = this.poLineSelector.filters.itemName.toLowerCase();
                        filteredItems = filteredItems.filter(po => 
                            po.itemName && po.itemName.toLowerCase().includes(searchItem)
                        );
                    }
                    
                    // Client-side pagination
                    this.poLineSelector.totalCount = filteredItems.length;
                    this.poLineSelector.totalPages = Math.ceil(filteredItems.length / this.poLineSelector.pageSize);
                    
                    const startIndex = (this.poLineSelector.pageNumber - 1) * this.poLineSelector.pageSize;
                    const endIndex = startIndex + this.poLineSelector.pageSize;
                    this.poLineSelector.items = filteredItems.slice(startIndex, endIndex);
                } else {
                    this.poLineSelector.items = [];
                    this.poLineSelector.totalCount = 0;
                    this.poLineSelector.totalPages = 0;
                }
            } catch (error) {
                console.error('Error searching PO lines:', error);
                this.showNotification('Failed to load PO lines: ' + error.message, true);
                this.poLineSelector.items = [];
            } finally {
                this.poLineSelector.isLoading = false;
            }
        },
        
        /**
         * Change page in PO line selector
         */
        changePoLinePage(page) {
            if (page >= 1 && page <= this.poLineSelector.totalPages) {
                this.poLineSelector.pageNumber = page;
                this.searchPoLines();
            }
        },
        
        /**
         * Select a PO Line from the selector
         */
        selectPoLine(po) {
            // Populate line form with PO line data
            this.lineModal.data.poLineId = po.poLineId;
            this.lineModal.data.poId = po.poId;
            this.lineModal.data.poDisplayText = `${po.poId} - ${po.itemName} (Outstanding: ${po.qtyOs})`;
            this.lineModal.data.itemId = po.itemId;
            this.lineModal.data.itemName = po.itemName;
            this.lineModal.data.unitId = po.unitId;
            this.lineModal.data.unitName = po.unitName;
            this.lineModal.data.price = po.price || 0;
            this.lineModal.data.qty = po.qtyOs; // Default to outstanding qty
            this.lineModal.data.maxQty = po.qtyOs; // Store max allowed qty
            
            this.closePoLineSelector();
        },
        
        async saveHeader() {
            const form = document.getElementById('grForm');
            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }
            
            this.isHeaderSaving = true;
            try {
                const formData = new FormData(form);
                formData.set('IsPpn', document.getElementById('Header_IsPpn').checked);
                
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                
                const response = await fetch(`/api/GoodsReceipts/${this.header.recId}`, {
                    method: 'PUT',
                    headers: {
                        'RequestVerificationToken': token
                    },
                    body: formData
                });
                
                const data = await response.json();
                
                if (data) {
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
            this.lineModal.data = {
                recId: 0,
                grLineId: '',
                poLineId: '',
                poId: '',
                poDisplayText: '',
                itemId: '',
                itemName: '',
                qty: 1,
                maxQty: 0,
                unitId: '',
                unitName: '',
                price: 0,
                whId: 1,
                notes: ''
            };
            this.lineModal.show = true;
        },
        
        editLine(index) {
            const line = this.lines[index];
            this.lineModal.mode = 'edit';
            this.lineModal.errors = {};
            
            this.lineModal.data = {
                recId: line.recId,
                grLineId: line.grLineId,
                poLineId: line.poLineId,
                itemId: line.itemId,
                itemName: line.itemName,
                qty: line.qty,
                maxQty: 0,
                unitId: line.unitId,
                unitName: line.unitName,
                price: line.price,
                whId: line.whId || 1,
                notes: line.notes || ''
            };
            
            // Fetch outstanding data for validation
            this.fetchOutstandingForEdit(line.poLineId);
            
            this.lineModal.show = true;
        },
        
        /**
         * Fetch outstanding data for a PO line when editing
         */
        async fetchOutstandingForEdit(poLineId) {
            try {
                const response = await fetch(`/api/GoodsReceipts/outstanding-by-poline/${poLineId}`);
                
                if (!response.ok) {
                    console.error('Failed to fetch outstanding data');
                    return;
                }
                
                const result = await response.json();
                
                if (result && result.data && result.data.length > 0) {
                    const osData = result.data[0];
                    
                    // Update modal with current outstanding quantity
                    // Add back the current line's qty since it's already been received
                    this.lineModal.data.maxQty = osData.qtyOs + this.lineModal.data.qty;
                    this.lineModal.data.poDisplayText = `${osData.poId} - ${osData.itemName} (Outstanding: ${osData.qtyOs})`;
                } else {
                    console.warn('No outstanding data found for PO line');
                }
            } catch (error) {
                console.error('Error fetching outstanding data:', error);
            }
        },
        
        closeLineModal() {
            if (!this.lineModal.isSaving) {
                this.lineModal.show = false;
            }
        },
        
        confirmDeleteLine(index) {
            this.deleteModal.line = this.lines[index];
            this.deleteModal.show = true;
        },
        
        validateLine() {
            this.lineModal.errors = {};
            
            if (!this.lineModal.data.poLineId) {
                this.lineModal.errors.poLineId = 'Please select a PO line';
            }
            
            if (!this.lineModal.data.qty || this.lineModal.data.qty <= 0) {
                this.lineModal.errors.qty = 'Quantity must be greater than 0';
            }
            
            if (this.lineModal.data.maxQty > 0 && this.lineModal.data.qty > this.lineModal.data.maxQty) {
                this.lineModal.errors.qty = `Quantity cannot exceed outstanding quantity of ${formatNumber(this.lineModal.data.maxQty)}`;
            }
            
            return Object.keys(this.lineModal.errors).length === 0;
        },
        
        async saveLine() {
            if (!this.validateLine()) return;
            
            this.lineModal.isSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            const payload = {
                GrRecId: this.header.recId,
                PoLineId: this.lineModal.data.poLineId,
                Qty: parseFloat(this.lineModal.data.qty),
                WhId: this.lineModal.data.whId || 1,
                Notes: this.lineModal.data.notes
            };
            
            let url = '/api/GoodsReceiptLines';
            let method = 'POST';
            
            if (this.lineModal.mode === 'edit' && this.lineModal.data.recId) {
                url = `/api/GoodsReceiptLines/${this.lineModal.data.recId}`;
                method = 'PUT';
                payload.RecId = this.lineModal.data.recId;
                payload.GrLineId = this.lineModal.data.grLineId;
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
                    // Refresh page to get updated lines
                    window.location.reload();
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
        
        async deleteLine() {
            if (!this.deleteModal.line) return;
            
            this.deleteModal.isDeleting = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            try {
                const response = await fetch(`/api/GoodsReceiptLines/${this.deleteModal.line.recId}`, {
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
                    showNotificationModal(result.message || 'Error deleting line', true);
                }
            } catch (error) {
                console.error('Delete line error:', error);
                showNotificationModal('Error deleting line', true);
            } finally {
                this.deleteModal.isDeleting = false;
            }
        },
        
        async reloadLines() {
            try {
                const response = await fetch(`/api/GoodsReceiptLines/by-gr/${this.header.recId}`);
                const data = await response.json();
                
                if (data.data) {
                    this.lines = data.data.map(l => ({
                        recId: l.recId,
                        grLineId: l.grLineId,
                        grRecId: l.grRecId,
                        poLineId: l.poLineId,
                        itemId: l.itemId,
                        itemName: l.itemName,
                        description: l.description || '',
                        qtyPo: l.qtyPo,
                        qtyReceived: l.qtyReceived,
                        unitId: l.unitId,
                        unitName: l.unitName,
                        price: l.price,
                        amount: l.amount,
                        notes: l.notes || '',
                        createdDate: l.createdDate,
                        createdBy: l.createdBy,
                        modifiedDate: l.modifiedDate,
                        modifiedBy: l.modifiedBy
                    }));
                }
            } catch (error) {
                console.error('Error reloading lines:', error);
            }
        }
    };
}
