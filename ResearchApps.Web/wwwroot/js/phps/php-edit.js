// Penerimaan Hasil Produksi Edit page - Alpine.js component
function phpEdit(initialLines, config) {
    return {
        // Header state
        header: {
            recId: config.recId,
            phpId: config.phpId,
            phpDate: config.phpDate,
            descr: config.descr,
            refId: config.refId,
            notes: config.notes
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
                phpLineId: '',
                phpId: '',
                phpRecId: 0,
                itemId: '',
                itemName: '',
                whId: '',
                whName: '',
                qty: 1,
                unitId: '',
                unitName: '',
                price: 0,
                prodId: '',
                prodDisplayText: '',
                notes: ''
            },
            errors: {},
            isSaving: false
        },
        
        // Production selector modal state
        prodModal: {
            show: false,
            isLoading: false,
            items: [],
            search: '',
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
        },
        
        // Delete modal state
        showDeleteConfirm: false,
        deleteLineIndex: null,
        isDeleting: false,
        
        // TomSelect instances
        warehouseSelect: null,
        
        init() {
            // Nothing to initialize on page load
            // TomSelect will be initialized when modal opens
        },
        
        showNotification(message, isError = false) {
            if (window.showNotificationModal) {
                window.showNotificationModal(message, isError);
            } else {
                alert(message);
            }
        },
        
        async saveHeader() {
            this.isHeaderSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            try {
                const payload = {
                    RecId: this.header.recId,
                    PhpId: this.header.phpId,
                    Notes: this.header.notes
                };
                
                const response = await fetch(`/api/Phps/${this.header.recId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(payload)
                });
                
                const data = await response.json();
                
                if (response.ok) {
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
                phpLineId: '',
                phpId: '',
                phpRecId: 0,
                itemId: '',
                itemName: '',
                whId: '',
                whName: '',
                qty: 1,
                unitId: '',
                unitName: '',
                price: 0,
                prodId: '',
                prodDisplayText: '',
                notes: ''
            };
            this.lineModal.show = true;
            
            // Initialize TomSelect after modal is shown
            this.$nextTick(async () => {
                await this.initWarehouseSelect();
            });
        },
        
        editLine(index) {
            const line = this.lines[index];
            this.lineModal.mode = 'edit';
            this.lineModal.errors = {};
            
            this.lineModal.data = {
                phpLineId: line.phpLineId,
                phpId: line.phpId,
                phpRecId: line.phpRecId,
                itemId: line.itemId,
                itemName: line.itemName,
                whId: line.whId,
                whName: line.whName,
                qty: line.qty,
                unitId: line.unitId,
                unitName: line.unitName,
                price: line.price,
                prodId: line.prodId || '',
                prodDisplayText: line.prodId ? `${line.prodId}` : '',
                notes: line.notes || ''
            };
            
            this.lineModal.show = true;
            
            // Initialize TomSelect after modal is shown
            this.$nextTick(async () => {
                await this.initWarehouseSelect();
                
                // Set initial warehouse value
                if (this.warehouseSelect && line.whId) {
                    this.warehouseSelect.setValue(String(line.whId));
                }
            });
        },
        
        async initWarehouseSelect() {
            // Destroy existing instance
            if (this.warehouseSelect) {
                this.warehouseSelect.destroy();
                this.warehouseSelect = null;
            }
            
            // Wait for next tick to ensure DOM is ready
            await this.$nextTick();
            
            const whEl = this.$refs.warehouseSelect;
            if (!whEl) {
                console.error('Warehouse select element not found');
                return;
            }
            
            // Get all warehouses and filter out WhId = 1
            try {
                const response = await fetch('/api/Warehouses/cbo', {
                    headers: { 'X-TomSelect': 'true' }
                });
                const allWarehouses = await response.json();
                const filteredWarehouses = allWarehouses.filter(item => item.value !== '1' && item.value !== 1);
                
                // Clear existing options
                whEl.innerHTML = '<option value="">Select Warehouse</option>';
                
                // Add filtered options
                filteredWarehouses.forEach(wh => {
                    const option = document.createElement('option');
                    option.value = wh.value;
                    option.textContent = wh.text;
                    whEl.appendChild(option);
                });
                
                // Initialize TomSelect
                if (typeof TomSelect !== 'undefined') {
                    this.warehouseSelect = new TomSelect(whEl, {
                        placeholder: 'Select Warehouse',
                        onChange: (value) => {
                            this.lineModal.data.whId = value;
                            // Get selected option text
                            const option = this.warehouseSelect.getOption(value);
                            if (option) {
                                this.lineModal.data.whName = option.textContent;
                            }
                        }
                    });
                }
            } catch (error) {
                console.error('Error loading warehouses:', error);
            }
        },
        
        closeLineModal() {
            if (!this.lineModal.isSaving) {
                this.lineModal.show = false;
                
                // Cleanup TomSelect instance
                if (this.warehouseSelect) {
                    this.warehouseSelect.destroy();
                    this.warehouseSelect = null;
                }
            }
        },
        
        /**
         * Open Production selector modal
         */
        openProdSelector() {
            this.prodModal.show = true;
            this.prodModal.search = '';
            this.prodModal.pageNumber = 1;
            this.searchProductions();
        },
        
        /**
         * Search productions
         */
        async searchProductions() {
            this.prodModal.isLoading = true;
            
            try {
                let url = `/api/Prods?PageNumber=${this.prodModal.pageNumber}&PageSize=${this.prodModal.pageSize}`;
                
                if (this.prodModal.search) {
                    url += `&Filters[ProdId]=${encodeURIComponent(this.prodModal.search)}`;
                }
                
                const response = await fetch(url);
                
                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
                }
                
                const result = await response.json();
                
                if (result && result.data && result.data.items) {
                    this.prodModal.items = result.data.items;
                    this.prodModal.totalCount = result.data.totalCount;
                    this.prodModal.totalPages = result.data.totalPages;
                } else {
                    this.prodModal.items = [];
                    this.prodModal.totalCount = 0;
                    this.prodModal.totalPages = 0;
                }
            } catch (error) {
                console.error('Error searching productions:', error);
                this.showNotification('Failed to load productions: ' + error.message, true);
                this.prodModal.items = [];
            } finally {
                this.prodModal.isLoading = false;
            }
        },
        
        /**
         * Select a production and fetch its details
         */
        async selectProduction(prod) {
            this.lineModal.data.prodId = prod.prodId;
            this.lineModal.data.prodDisplayText = `${prod.prodId} - ${prod.descr || ''}`;
            this.prodModal.show = false;
            
            // Fetch full production details to get ItemId
            try {
                const response = await fetch(`/api/Prods/by-prodid/${encodeURIComponent(prod.prodId)}`);
                if (response.ok) {
                    const result = await response.json();
                    if (result && result.data) {
                        this.lineModal.data.itemId = result.data.itemId;
                        this.lineModal.data.itemName = result.data.itemName;
                        this.lineModal.data.unitId = result.data.unitId;
                        this.lineModal.data.unitName = result.data.unitName;
                    }
                } else {
                    console.error('Failed to fetch production details');
                }
            } catch (error) {
                console.error('Error fetching production details:', error);
            }
        },
        
        confirmDeleteLine(index) {
            this.deleteLineIndex = index;
            this.showDeleteConfirm = true;
        },
        
        validateLine() {
            this.lineModal.errors = {};
            
            if (!this.lineModal.data.prodId) {
                this.lineModal.errors.prodId = 'Please select a production order first';
            }
            
            if (!this.lineModal.data.itemId) {
                this.lineModal.errors.itemId = 'Item not loaded from production order';
            }
            
            if (!this.lineModal.data.whId) {
                this.lineModal.errors.whId = 'Please select a warehouse';
            }
            
            // Check if WhId is 1 (TL warehouse - not allowed)
            if (this.lineModal.data.whId && (this.lineModal.data.whId === '1' || this.lineModal.data.whId === 1)) {
                this.lineModal.errors.whId = 'Warehouse TL (ID=1) is not allowed';
            }
            
            if (!this.lineModal.data.qty || this.lineModal.data.qty <= 0) {
                this.lineModal.errors.qty = 'Quantity must be greater than 0';
            }
            
            if (!this.lineModal.data.prodId) {
                this.lineModal.errors.prodId = 'Please select a production order';
            }
            
            return Object.keys(this.lineModal.errors).length === 0;
        },
        
        async saveLine() {
            if (!this.validateLine()) return;
            
            this.lineModal.isSaving = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            const payload = {
                PhpRecId: this.header.recId,
                ItemId: parseInt(this.lineModal.data.itemId),
                WhId: parseInt(this.lineModal.data.whId),
                Qty: parseFloat(this.lineModal.data.qty),
                Price: parseFloat(this.lineModal.data.price) || 0,
                ProdId: this.lineModal.data.prodId,
                Notes: this.lineModal.data.notes
            };
            
            let url = '/api/PhpLines';
            let method = 'POST';
            
            if (this.lineModal.mode === 'edit' && this.lineModal.data.phpRecId) {
                url = `/api/PhpLines/${this.lineModal.data.phpRecId}`;
                method = 'PUT';
                payload.PhpLineId = this.lineModal.data.phpLineId;
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
            if (this.deleteLineIndex === null) return;
            
            const line = this.lines[this.deleteLineIndex];
            if (!line || !line.phpRecId) {
                this.showDeleteConfirm = false;
                return;
            }
            
            this.isDeleting = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            try {
                const response = await fetch(`/api/PhpLines/${line.phpRecId}`, {
                    method: 'DELETE',
                    headers: {
                        'RequestVerificationToken': token
                    }
                });
                
                if (response.ok) {
                    // Remove from local array
                    this.lines.splice(this.deleteLineIndex, 1);
                    this.showDeleteConfirm = false;
                    this.deleteLineIndex = null;
                    this.showNotification('Line deleted successfully');
                } else {
                    const result = await response.json();
                    this.showNotification(result.message || 'Error deleting line', true);
                }
            } catch (error) {
                console.error('Delete line error:', error);
                this.showNotification('Error deleting line', true);
            } finally {
                this.isDeleting = false;
            }
        }
    };
}
