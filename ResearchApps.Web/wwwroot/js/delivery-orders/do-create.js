/**
 * Delivery Order Create Page Alpine.js Component
 * Handles form validation, customer/CO selection, and outstanding lines management
 */
function doCreateForm() {
    return {
        // State
        errors: {},
        isSubmitting: false,
        showConfirmModal: false,
        isLoadingCustomers: false,
        isLoadingCos: false,
        isLoadingOutstanding: false,
        
        // Form data
        customerId: null,
        coRecId: null,
        coId: '',
        customerName: '',
        
        // Outstanding lines
        outstandingLines: [],
        selectedLines: [],
        
        // TomSelect instances
        customerSelect: null,
        coSelect: null,
        
        // Flatpickr instance
        datePicker: null,
        
        // Pre-selected values from URL params
        preSelectedCustomerId: '',
        preSelectedCoId: '',
        
        /**
         * Initialize the component
         */
        init() {
            this.initializeDatePicker();
            this.initializeCustomerSelect();
            
            // Check for pre-selected values passed from view
            const customerIdEl = document.getElementById('preSelectedCustomerId');
            const coIdEl = document.getElementById('preSelectedCoId');
            
            if (customerIdEl) this.preSelectedCustomerId = customerIdEl.value;
            if (coIdEl) this.preSelectedCoId = coIdEl.value;
            
            // Load pre-selected customer if exists
            if (this.preSelectedCustomerId) {
                this.loadPreSelectedCustomer();
            }
        },
        
        /**
         * Initialize Flatpickr date picker
         */
        initializeDatePicker() {
            this.datePicker = initFlatpickr('#Header_DoDate', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: 'today'
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
        },
        
        /**
         * Initialize CO TomSelect (called after customer is selected)
         */
        initializeCoSelect() {
            if (this.coSelect) {
                this.coSelect.destroy();
            }
            
            const self = this;
            const coElement = document.getElementById('Header_CoId');
            if (!coElement) return;
            
            // Enable the element
            coElement.disabled = false;
            
            this.coSelect = initTomSelect('#Header_CoId', {
                url: `/api/CustomerOrders/Cbo?customerId=${this.customerId}`,
                valueField: 'value',
                labelField: 'text',
                searchField: 'text',
                preload: 'focus',
                onChange: function(value) {
                    self.onCoChange(value);
                }
            });
        },
        
        /**
         * Load pre-selected customer from URL param
         */
        async loadPreSelectedCustomer() {
            this.isLoadingCustomers = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                const response = await fetch(`/api/Customers/cbo?customerId=${this.preSelectedCustomerId}`, {
                    headers: {
                        'RequestVerificationToken': token,
                        'X-TomSelect': 'true'
                    }
                });
                const data = await response.json();
                
                if (data && data.length > 0) {
                    const customer = data[0];
                    this.customerId = customer.value;
                    this.customerName = customer.text;
                    
                    // Add option and set value
                    this.customerSelect.addOption(customer);
                    this.customerSelect.setValue(customer.value);
                    
                    // Initialize CO select
                    this.initializeCoSelect();
                    
                    // Load pre-selected CO if exists
                    if (this.preSelectedCoId) {
                        await this.loadPreSelectedCo();
                    }
                }
            } catch (error) {
                console.error('Error loading pre-selected customer:', error);
            } finally {
                this.isLoadingCustomers = false;
            }
        },
        
        /**
         * Load pre-selected CO from URL param
         */
        async loadPreSelectedCo() {
            this.isLoadingCos = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                const response = await fetch(`/api/CustomerOrders/Cbo?customerId=${this.customerId}`, {
                    headers: {
                        'RequestVerificationToken': token,
                        'X-TomSelect': 'true'
                    }
                });
                const data = await response.json();
                
                if (data && data.length > 0) {
                    const co = data.find(item => item.value === this.preSelectedCoId);
                    if (co) {
                        this.coRecId = co.value;
                        this.coId = co.text;
                        
                        // Add option and set value
                        this.coSelect.addOption(co);
                        this.coSelect.setValue(co.value);
                        
                        // Load outstanding orders
                        await this.loadOutstandingOrders();
                    }
                }
            } catch (error) {
                console.error('Error loading pre-selected CO:', error);
            } finally {
                this.isLoadingCos = false;
            }
        },
        
        /**
         * Handle customer selection change
         */
        onCustomerChange(value) {
            this.customerId = value;
            this.coRecId = null;
            this.coId = '';
            this.outstandingLines = [];
            this.selectedLines = [];
            
            if (this.coSelect) {
                this.coSelect.clear();
                this.coSelect.destroy();
                this.coSelect = null;
            }
            
            const coElement = document.getElementById('Header_CoId');
            if (coElement) coElement.disabled = true;
            
            if (value) {
                this.initializeCoSelect();
            }
        },
        
        /**
         * Handle CO selection change
         */
        onCoChange(value) {
            this.coRecId = value;
            this.selectedLines = [];
            
            // Get the text (CO ID) from TomSelect
            if (this.coSelect && value) {
                const option = this.coSelect.options[value];
                this.coId = option ? option.text : '';
            } else {
                this.coId = '';
            }
            
            if (value && this.customerId) {
                this.loadOutstandingOrders();
            } else {
                this.outstandingLines = [];
            }
        },
        
        /**
         * Load outstanding CO lines for the selected customer
         */
        async loadOutstandingOrders() {
            if (!this.customerId) return;
            
            this.isLoadingOutstanding = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                if (this.coRecId && this.coRecId > 0) {
                    url = `/api/CustomerOrders/${this.coRecId}/outstanding`;
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
                    this.outstandingLines = data.map(item => ({
                        coId: item.coId,
                        coLineId: item.coLineId,
                        coLineRecId: item.coLineRecId || item.coRecId,
                        itemId: item.itemId,
                        itemName: item.itemName,
                        qtyCo: item.qtyCo || 0,
                        qtyDo: item.qtyDo || 0,
                        qtyOs: item.qtyOs || item.qtyOutstanding || 0,
                        unitId: item.unitId || 0,
                        unitName: item.unitName,
                        whId: item.whId || 0,
                        price: item.price || 0,
                        qty: item.qtyOs || item.qtyOutstanding || 0, // Default to full outstanding
                        selected: false
                    }));
                } else {
                    this.outstandingLines = [];
                }
            } catch (error) {
                console.error('Error loading outstanding orders:', error);
                this.outstandingLines = [];
            } finally {
                this.isLoadingOutstanding = false;
            }
        },
        
        /**
         * Toggle line selection
         */
        toggleLineSelection(index) {
            this.outstandingLines[index].selected = !this.outstandingLines[index].selected;
            this.updateSelectedLines();
        },
        
        /**
         * Select all visible lines
         */
        selectAllLines() {
            this.outstandingLines.forEach(line => {
                if (line.qtyOs > 0) {
                    line.selected = true;
                }
            });
            this.updateSelectedLines();
        },
        
        /**
         * Deselect all lines
         */
        deselectAllLines() {
            this.outstandingLines.forEach(line => {
                line.selected = false;
            });
            this.updateSelectedLines();
        },
        
        /**
         * Update selected lines array
         */
        updateSelectedLines() {
            this.selectedLines = this.outstandingLines.filter(line => line.selected);
        },
        
        /**
         * Validate form before submission
         */
        validateForm() {
            this.errors = {};
            
            if (!this.customerId) {
                this.errors.CustomerId = 'Please select a customer';
            }
            
            const doDate = document.getElementById('Header_DoDate')?.value;
            if (!doDate) {
                this.errors.DoDate = 'Please select a delivery date';
            }
            
            // Check if at least one line is selected
            if (this.selectedLines.length === 0) {
                this.errors.Lines = 'Please select at least one line item';
            }
            
            // Validate quantities
            for (const line of this.selectedLines) {
                if (!line.qty || line.qty <= 0) {
                    this.errors.Lines = 'All selected lines must have a valid quantity';
                    break;
                }
                if (line.qty > line.qtyOs) {
                    this.errors.Lines = `Quantity cannot exceed outstanding amount for ${line.itemName}`;
                    break;
                }
            }
            
            return Object.keys(this.errors).length === 0;
        },
        
        /**
         * Show confirmation modal
         */
        confirmCreate() {
            if (this.validateForm()) {
                this.collectLinesForForm();
                this.showConfirmModal = true;
            }
        },
        
        /**
         * Collect selected lines and add to form as hidden fields
         */
        collectLinesForForm() {
            const container = document.getElementById('lines-container');
            if (!container) return;
            
            container.innerHTML = '';
            
            this.selectedLines.forEach((line, index) => {
                container.innerHTML += `
                    <input type="hidden" name="Lines[${index}].CoLineId" value="${line.coLineId}" />
                    <input type="hidden" name="Lines[${index}].ItemId" value="${line.itemId}" />
                    <input type="hidden" name="Lines[${index}].Qty" value="${line.qty}" />
                    <input type="hidden" name="Lines[${index}].UnitId" value="${line.unitId || 0}" />
                `;
            });
        },
        
        /**
         * Submit the form
         */
        proceedSubmit() {
            this.showConfirmModal = false;
            this.isSubmitting = true;
            document.getElementById('do-form').submit();
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
window.doCreateForm = doCreateForm;
