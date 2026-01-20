/**
 * Purchase Order Edit Page Component
 * Handles header editing, line item management, and workflow actions
 * 
 * @param {Array} initialLines - Initial line items from server
 * @param {Object} config - Configuration object
 * @param {number} config.recId - Record ID
 * @param {string} config.poId - Purchase Order ID
 * @param {string} config.poDate - Order date (YYYY-MM-DD)
 * @param {number} config.supplierId - Supplier ID
 * @param {string} config.supplierName - Supplier name
 */
function poEdit(initialLines = [], config = {}) {
    return {
        // Configuration
        config: {
            recId: 0,
            poId: '',
            poDate: '',
            supplierId: 0,
            supplierName: '',
            ...config
        },
        
        // Header state
        header: {
            recId: config.recId || 0,
            poId: config.poId || ''
        },
        isHeaderSaving: false,

        // Lines state
        lines: initialLines || [],

        // Line modal state
        lineModal: {
            show: false,
            mode: 'add',
            data: {
                poLineId: '',
                recId: 0,
                prLineId: 0,
                prLineName: '',
                itemId: 0,
                itemName: '',
                qty: 1,
                price: 0,
                amount: 0,
                unitId: 0,
                unitName: '',
                outstandingQty: 0,
                deliveryDate: null,
                notes: ''
            },
            errors: {},
            isSaving: false
        },

        // PR Line Selector modal
        prLineSelector: {
            show: false,
            isLoading: false,
            items: [],
            filters: {
                prId: '',
                itemName: '',
                dateFrom: ''
            },
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0,
            totalPages: 0
        },

        // Delete modal
        deleteModal: {
            show: false,
            line: null,
            isDeleting: false
        },

        // TomSelect instances
        supplierSelect: null,

        // Flatpickr instances
        poDatePicker: null,
        lineDeliveryDatePicker: null,

        /**
         * Initialize component
         */
        init() {
            this.header.recId = this.config.recId;
            this.header.poId = this.config.poId;
            this.initHeaderComponents();
        },

        /**
         * Initialize header form components
         */
        initHeaderComponents() {
            // Initialize date picker
            this.poDatePicker = initFlatpickr('#Header_PoDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: this.config.poDate
            });
            
            // Initialize Supplier TomSelect
            this.supplierSelect = initTomSelect('#Header_SupplierId', {
                url: '/api/Suppliers/cbo',
                placeholder: 'Select Supplier'
            });
            
            // Pre-select current supplier
            if (this.supplierSelect && this.config.supplierId) {
                this.supplierSelect.addOption({
                    value: String(this.config.supplierId),
                    text: this.config.supplierName
                });
                this.supplierSelect.setValue(String(this.config.supplierId));
            }
        },

        /**
         * Show line modal for add or edit
         */
        showLineModal(mode, line = null) {
            this.lineModal.mode = mode;
            this.lineModal.errors = {};
            this.lineModal.show = true;

            if (mode === 'edit' && line) {
                this.lineModal.data = { ...line };
            } else {
                this.lineModal.data = {
                    poLineId: '',
                    recId: 0,
                    prLineId: 0,
                    prLineName: '',
                    itemId: 0,
                    itemName: '',
                    qty: 1,
                    price: 0,
                    amount: 0,
                    unitId: 0,
                    unitName: '',
                    outstandingQty: 0,
                    deliveryDate: null,
                    notes: ''
                };
            }

            this.$nextTick(() => this.initLineDeliveryDatePicker());
        },

        /**
         * Close line modal
         */
        closeLineModal() {
            if (!this.lineModal.isSaving) {
                this.lineModal.show = false;
                if (this.lineDeliveryDatePicker) {
                    this.lineDeliveryDatePicker.destroy();
                    this.lineDeliveryDatePicker = null;
                }
            }
        },

        /**
         * Initialize delivery date picker
         */
        initLineDeliveryDatePicker() {
            if (this.lineDeliveryDatePicker) {
                this.lineDeliveryDatePicker.destroy();
            }
            this.lineDeliveryDatePicker = initFlatpickr('#lineDeliveryDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: this.lineModal.data.deliveryDate || null
            });
        },

        /**
         * Open PR Line selector modal
         */
        openPrLineSelector() {
            this.prLineSelector.show = true;
            this.prLineSelector.pageNumber = 1;
            this.searchPrLines();
        },

        /**
         * Close PR Line selector modal
         */
        closePrLineSelector() {
            this.prLineSelector.show = false;
        },

        /**
         * Search PR Lines with filters and pagination
         */
        async searchPrLines() {
            this.prLineSelector.isLoading = true;
            
            try {
                const params = new URLSearchParams({
                    poRecId: this.config.recId,
                    pageNumber: this.prLineSelector.pageNumber,
                    pageSize: this.prLineSelector.pageSize,
                    prId: this.prLineSelector.filters.prId || '',
                    itemName: this.prLineSelector.filters.itemName || '',
                    dateFrom: this.prLineSelector.filters.dateFrom || ''
                });

                const response = await fetch(`/api/PrLines/ForPo?${params}`);
                
                if (response.ok) {
                    const result = await response.json();
                    this.prLineSelector.items = result.data || [];
                    this.prLineSelector.totalCount = result.totalCount || this.prLineSelector.items.length;
                    this.prLineSelector.totalPages = Math.ceil(this.prLineSelector.totalCount / this.prLineSelector.pageSize);
                } else {
                    console.error('Failed to load PR lines');
                    this.prLineSelector.items = [];
                }
            } catch (error) {
                console.error('Error loading PR lines:', error);
                this.prLineSelector.items = [];
            } finally {
                this.prLineSelector.isLoading = false;
            }
        },

        /**
         * Change PR Line page
         */
        changePrLinePage(page) {
            if (page >= 1 && page <= this.prLineSelector.totalPages) {
                this.prLineSelector.pageNumber = page;
                this.searchPrLines();
            }
        },

        /**
         * Select a PR Line from the selector
         */
        selectPrLine(prLine) {
            // Populate line modal with PR line data
            this.lineModal.data.prLineId = prLine.prLineId;
            this.lineModal.data.prLineName = `${prLine.prId} - ${prLine.itemName}`;
            this.lineModal.data.itemId = prLine.itemId;
            this.lineModal.data.itemName = prLine.itemName;
            this.lineModal.data.unitId = prLine.unitId;
            this.lineModal.data.unitName = prLine.unitName;
            this.lineModal.data.outstandingQty = prLine.outstandingQty || prLine.qty;
            this.lineModal.data.qty = prLine.outstandingQty || prLine.qty;
            this.lineModal.data.price = prLine.price || 0;
            this.lineModal.data.deliveryDate = prLine.requestDate;

            // Update delivery date picker
            if (this.lineDeliveryDatePicker && prLine.requestDate) {
                this.lineDeliveryDatePicker.setDate(prLine.requestDate);
            }

            this.calculateLineAmount();
            this.closePrLineSelector();
        },

        /**
         * Calculate line amount
         */
        calculateLineAmount() {
            const qty = parseFloat(this.lineModal.data.qty) || 0;
            const price = parseFloat(this.lineModal.data.price) || 0;
            this.lineModal.data.amount = qty * price;
        },

        /**
         * Save line item
         */
        async saveLine() {
            if (!this.validateLine()) return;

            this.lineModal.isSaving = true;

            try {
                const url = this.lineModal.mode === 'add'
                    ? '/api/PoLines'
                    : `/api/PoLines/${this.lineModal.data.poLineId}`;
                const method = this.lineModal.mode === 'add' ? 'POST' : 'PUT';

                const payload = {
                    RecId: this.config.recId,
                    PrLineId: this.lineModal.data.prLineId,
                    ItemId: this.lineModal.data.itemId,
                    DeliveryDate: this.lineModal.data.deliveryDate || null,
                    Qty: parseFloat(this.lineModal.data.qty),
                    Price: parseFloat(this.lineModal.data.price),
                    UnitId: this.lineModal.data.unitId,
                    Notes: this.lineModal.data.notes || ''
                };

                const response = await fetch(url, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    // Success - reload page to get updated data
                    window.location.reload();
                } else {
                    const errorMessage = await this.parseErrorResponse(response);
                    console.error('Save line error:', errorMessage);
                    alert('Error saving line item:\n\n' + errorMessage);
                }
            } catch (error) {
                console.error('Save line failed:', error);
                alert('An error occurred while saving the line item:\n\n' + error.message);
            } finally {
                this.lineModal.isSaving = false;
            }
        },

        /**
         * Parse error response from server (HTML or JSON)
         */
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

        /**
         * Validate line data
         */
        validateLine() {
            this.lineModal.errors = {};
            
            if (!this.lineModal.data.prLineId) {
                this.lineModal.errors.prLineId = 'Please select a PR Line';
            }
            
            if (!this.lineModal.data.qty || this.lineModal.data.qty <= 0) {
                this.lineModal.errors.qty = 'Quantity must be greater than 0';
            } else if (this.lineModal.data.outstandingQty && this.lineModal.data.qty > this.lineModal.data.outstandingQty) {
                // Validate against outstanding quantity (partial fulfillment)
                this.lineModal.errors.qty = `Quantity cannot exceed outstanding quantity of ${this.lineModal.data.outstandingQty}`;
            }
            
            if (this.lineModal.data.price === null || this.lineModal.data.price === undefined || this.lineModal.data.price < 0) {
                this.lineModal.errors.price = 'Price must be 0 or greater';
            }
            
            return Object.keys(this.lineModal.errors).length === 0;
        },

        /**
         * Confirm delete line
         */
        confirmDeleteLine(line) {
            this.deleteModal.line = line;
            this.deleteModal.show = true;
        },

        /**
         * Delete line item
         */
        async deleteLine() {
            if (!this.deleteModal.line) return;

            this.deleteModal.isDeleting = true;

            try {
                const response = await fetch(`/api/PoLines/${this.deleteModal.line.poLineId}`, {
                    method: 'DELETE',
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                });

                if (response.ok) {
                    // Success - reload page to get updated data
                    window.location.reload();
                } else {
                    const errorMessage = await this.parseErrorResponse(response);
                    console.error('Delete line error:', errorMessage);
                    alert('Error deleting line item:\n\n' + errorMessage);
                }
            } catch (error) {
                console.error('Delete line failed:', error);
                alert('An error occurred while deleting the line item:\n\n' + error.message);
            } finally {
                this.deleteModal.isDeleting = false;
            }
        },

        /**
         * Calculate total amount
         */
        calculateTotal() {
            return this.lines.reduce((sum, line) => sum + (parseFloat(line.amount) || 0), 0);
        }
    };
}
