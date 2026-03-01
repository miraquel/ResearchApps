/**
 * Delivery Order Create Page Alpine.js Component
 * Handles form validation, customer/CO selection, and outstanding lines management
 */
function doCreateForm() {
    return {
        // State
        errors: {},
        errorLines: [],
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
            
            // Read pre-selected values passed from view (URL params)
            const customerIdEl = document.getElementById('preSelectedCustomerId');
            const coIdEl = document.getElementById('preSelectedCoId');
            
            if (customerIdEl) this.preSelectedCustomerId = customerIdEl.value;
            if (coIdEl) this.preSelectedCoId = coIdEl.value;
            
            // Load pre-selected customer if exists
            if (this.preSelectedCustomerId) {
                this.loadPreSelectedCustomer();
            }
            
            // Initialize warehouse selects when outstanding lines are rendered
            this.$watch('outstandingLines', () => {
                this.$nextTick(() => {
                    this.initializeWarehouseSelects();
                });
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
                defaultDate: 'today'
            });
        },
        
        /**
         * Initialize warehouse TomSelect for all outstanding lines
         */
        initializeWarehouseSelects() {
            const self = this;
            const warehouseSelects = document.querySelectorAll('.warehouse-select');
            
            warehouseSelects.forEach(selectElement => {
                // Skip if already initialized or if id not yet set by Alpine
                if (selectElement.tomselect || !selectElement.id) {
                    return;
                }
                
                const index = parseInt(selectElement.dataset.index);
                const line = this.outstandingLines[index];
                const isSelected = selectElement.dataset.selected === '1';
                
                const whSelect = initTomSelect(`#${selectElement.id}`, {
                    url: '/api/Warehouses/Cbo',
                    valueField: 'value',
                    labelField: 'text',
                    searchField: 'text',
                    preload: 'focus',
                    dropdownParent: 'body',
                    onChange: function(value) {
                        if (line) {
                            line.whId = value ? parseInt(value) : null;
                        }
                    }
                });
                
                // Set initial disabled state
                if (!isSelected) {
                    whSelect.disable();
                }
                
                // Pre-select warehouse if line has one
                if (line && line.whId) {
                    // TomSelect will load options from API and set value
                    whSelect.setValue(line.whId);
                }
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
                    showNotificationModal('Cannot load outstanding lines because the Delivery Order is not linked to a valid Customer Order.', 'error');
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
                        whId: item.whId || null, // Default to null, user must select
                        price: item.price || 0,
                        qty: item.qtyOs || item.qtyOutstanding || 0, // Default to full outstanding
                        notes: '', // Default empty notes
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
            
            // Enable/disable warehouse TomSelect
            const whSelectElement = document.getElementById(`whSelect_${index}`);
            if (whSelectElement && whSelectElement.tomselect) {
                if (this.outstandingLines[index].selected) {
                    whSelectElement.tomselect.enable();
                    // Force refresh the input state to update placeholder visibility
                    whSelectElement.tomselect.inputState();
                } else {
                    whSelectElement.tomselect.clear();
                    whSelectElement.tomselect.disable();
                    this.outstandingLines[index].whId = null;
                }
            }
        },
        
        /**
         * Select all visible lines
         */
        selectAllLines() {
            this.outstandingLines.forEach((line, index) => {
                if (line.qtyOs > 0) {
                    line.selected = true;
                    // Enable warehouse TomSelect
                    const whSelectElement = document.getElementById(`whSelect_${index}`);
                    if (whSelectElement && whSelectElement.tomselect) {
                        whSelectElement.tomselect.enable();
                    }
                }
            });
            this.updateSelectedLines();
        },
        
        /**
         * Deselect all lines
         */
        deselectAllLines() {
            this.outstandingLines.forEach((line, index) => {
                line.selected = false;
                // Disable warehouse TomSelect and clear value
                const whSelectElement = document.getElementById(`whSelect_${index}`);
                if (whSelectElement && whSelectElement.tomselect) {
                    whSelectElement.tomselect.clear();
                    whSelectElement.tomselect.disable();
                    line.whId = null;
                }
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
            this.errorLines = [];
            
            if (!this.customerId) {
                this.errors.CustomerId = 'Please select a customer';
            }
            
            const doDate = document.getElementById('Header_DoDate')?.value;
            if (!doDate) {
                this.errors.DoDate = 'Please select a delivery date';
            }
            
            if (!this.coId) {
                this.errors.CoId = 'Please select a customer order';
            }

            // Check if at least one line is selected
            if (this.selectedLines.length === 0) {
                this.errors.Lines = 'Please select at least one line item';
            }
            
            // Validate quantities, for loop with index
            for (const [index, line] of this.selectedLines.entries()) {
                if (!line.whId) {
                    this.errorLines[index] = `Please select a warehouse for ${line.itemName}`;
                    break;
                }
                if (!line.qty || line.qty <= 0) {
                    this.errorLines[index] = 'All selected lines must have a valid quantity';
                    break;
                }
                if (line.qty > line.qtyOs) {
                    this.errorLines[index] = `Quantity cannot exceed outstanding amount for ${line.itemName}`;
                }
            }
            
            return Object.keys(this.errors).length === 0 && this.errorLines.length === 0;
        },
        
        /**
         * Show confirmation modal
         */
        confirmCreate() {
            if (this.validateForm()) {
                this.showConfirmModal = true;
            }
        },
        
        /**
         * Submit the form via API
         */
        async proceedSubmit() {
            this.showConfirmModal = false;
            this.isSubmitting = true;
            this.errors = {};
            this.errorLines = [];
            
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            // Build the request payload
            const payload = {
                header: {
                    customerId: parseInt(this.customerId) || 0,
                    coRecId: parseInt(this.coRecId) || 0,
                    coId: this.coId || '',
                    doDate: document.getElementById('Header_DoDate')?.value || '',
                    refId: document.getElementById('Header_RefId')?.value || '',
                    descr: document.getElementById('Header_Descr')?.value || '',
                    dn: document.getElementById('Header_Dn')?.value || '',
                    notes: document.getElementById('Header_Notes')?.value || ''
                },
                lines: this.selectedLines.map(line => ({
                    coLineId: line.coLineId,
                    itemId: line.itemId,
                    itemName: line.itemName || '',
                    qty: line.qty,
                    unitId: line.unitId || 0,
                    whId: line.whId || 0,
                    notes: line.notes || ''
                }))
            };
            
            try {
                const response = await fetch('/api/DeliveryOrders', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify(payload)
                });
                
                // Try to parse JSON safely (500 errors may return non-JSON)
                let result = null;
                try {
                    result = await response.json();
                } catch (e) {
                    // Response body is not valid JSON
                }
                
                if (response.ok) {
                    // Success - redirect to edit page
                    const recId = result?.data;
                    const doId = result.data?.doId || result.data?.DoId || '';
                    
                    showNotificationModal(`Delivery Order ${doId} created successfully.`, 'success');
                    
                    // Short delay to show success then redirect
                    setTimeout(() => {
                        window.location.href = `/DeliveryOrders/Edit/${recId}`;
                    }, 800);
                } else {
                    // Extract error message from API response
                    const message = result?.detail || result?.message || result?.Message || result?.title
                        || `Failed to create delivery order. (HTTP ${response.status})`;
                    
                    showNotificationModal(message, 'error');
                    
                    // Scroll to top so user sees the error
                    window.scrollTo({ top: 0, behavior: 'smooth' });
                    this.isSubmitting = false;
                }
            } catch (error) {
                console.error('Error creating delivery order:', error);
                showNotificationModal('An unexpected error occurred. Please try again.', 'error');
                this.isSubmitting = false;
            }
        },
    };
}

// Make available globally
window.doCreateForm = doCreateForm;
