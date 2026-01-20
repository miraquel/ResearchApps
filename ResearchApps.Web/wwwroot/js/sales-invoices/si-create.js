/**
 * Sales Invoice Create Page Alpine.js Component
 * Handles form validation, customer/DO selection, and outstanding lines management
 */
function siCreateForm() {
    return {
        // State
        errors: {},
        isSubmitting: false,
        showConfirmModal: false,
        isLoadingCustomers: false,
        isLoadingDos: false,
        
        // Form data
        customerId: null,
        customerName: '',
        
        // Outstanding DO lines
        deliveryOrders: [],
        selectedDoLines: [],
        lines: [],
        
        // Pre-selected values from URL params
        preSelectedCustomerId: '',
        
        // TomSelect instances
        customerSelect: null,
        
        // Flatpickr instance
        datePicker: null,
        
        get totalAmount() {
            // Recalculate based on selectedDoLines to ensure reactivity
            const selected = this.deliveryOrders.filter(d => this.selectedDoLines.includes(d.doLineId));
            return selected.reduce((sum, line) => sum + (line.qty * line.price), 0);
        },
        
        get selectedCustomerId() {
            return this.customerId;
        },
        
        /**
         * Initialize the component
         */
        init() {
            this.initializeDatePicker();
            this.initializeCustomerSelect();
            
            // Watch for changes in selectedDoLines array
            this.$watch('selectedDoLines', () => {
                this.updateLines();
            });
            
            // Check for pre-selected values passed from view
            const customerIdEl = document.getElementById('preSelectedCustomerId');
            
            if (customerIdEl) this.preSelectedCustomerId = customerIdEl.value;
            
            // Load pre-selected customer if exists
            if (this.preSelectedCustomerId) {
                this.loadPreSelectedCustomer();
            }
        },
        
        /**
         * Initialize Flatpickr date picker
         */
        initializeDatePicker() {
            this.datePicker = initFlatpickr('#Header_SiDate', {
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
            this.customerSelect = initTomSelect('#Header_CustomerId', {
                url: '/api/Customers/cbo',
                placeholder: 'Select Customer',
                maxOptions: 50,
                onChange: (value) => {
                    this.onCustomerChange(value);
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
                    
                    // Load delivery orders
                    await this.loadDeliveryOrders();
                }
            } catch (error) {
                console.error('Error loading pre-selected customer:', error);
            } finally {
                this.isLoadingCustomers = false;
            }
        },
        
        /**
         * Handle customer selection change
         */
        onCustomerChange(value) {
            this.customerId = value;
            this.deliveryOrders = [];
            this.selectedDoLines = [];
            this.lines = [];
            
            if (value) {
                this.loadDeliveryOrders();
            }
        },
        
        /**
         * Load outstanding DO lines for the selected customer
         */
        async loadDeliveryOrders() {
            if (!this.customerId) return;
            
            this.isLoadingDos = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            
            try {
                const response = await fetch(`/api/DeliveryOrders/outstanding?customerId=${this.customerId}`, {
                    headers: { 'RequestVerificationToken': token }
                });
                const result = await response.json();
                const data = result.data || result;
                
                if (data && Array.isArray(data) && data.length > 0) {
                    this.deliveryOrders = data.map(line => ({
                        doLineId: String(line.doLineId), // Convert to string to match checkbox value
                        doId: line.doId,
                        // format date as 'dd mmm yyyy'
                        doDateStr: new Date(line.doDate).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }),
                        itemId: line.itemId,
                        itemName: line.itemName,
                        qty: line.qtyOs || line.qtyDo || line.qty,
                        unitName: line.unitName,
                        unitId: line.unitId || 0,
                        price: line.price || 0
                    }));
                } else {
                    this.deliveryOrders = [];
                }
            } catch (error) {
                console.error('Error loading delivery orders:', error);
                this.deliveryOrders = [];
            } finally {
                this.isLoadingDos = false;
            }
        },
        
        /**
         * Update selected lines array
         */
        updateLines() {
            this.lines = this.deliveryOrders
                .filter(d => this.selectedDoLines.includes(d.doLineId))
                .map(d => ({
                    doLineId: parseInt(d.doLineId), // Convert back to number for form submission
                    doId: d.doId,
                    itemId: d.itemId,
                    itemName: d.itemName,
                    qty: parseFloat(d.qty),
                    unitName: d.unitName,
                    unitId: d.unitId || 0,
                    price: parseFloat(d.price)
                }));
        },
        
        /**
         * Select all visible DO lines
         */
        selectAllLines() {
            this.selectedDoLines = this.deliveryOrders.map(d => d.doLineId);
            this.updateLines();
        },
        
        /**
         * Deselect all DO lines
         */
        deselectAllLines() {
            this.selectedDoLines = [];
            this.updateLines();
        },
        
        /**
         * Remove line from selected lines
         */
        removeLine(index) {
            const line = this.lines[index];
            if (line) {
                this.selectedDoLines = this.selectedDoLines.filter(id => id !== line.doLineId);
                this.lines.splice(index, 1);
            }
        },
        
        /**
         * Validate form before submission
         */
        validateForm() {
            this.errors = {};
            
            if (!this.customerId) {
                this.errors.CustomerId = 'Please select a customer';
            }
            
            const siDate = document.getElementById('Header_SiDate')?.value;
            if (!siDate) {
                this.errors.SiDate = 'Please select an invoice date';
            }
            
            if (this.lines.length === 0) {
                this.errors.Lines = 'Please select at least one delivery order line';
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
            
            this.lines.forEach((line, index) => {
                container.innerHTML += `
                    <input type="hidden" name="Lines[${index}].DoLineId" value="${line.doLineId}" />
                    <input type="hidden" name="Lines[${index}].DoId" value="${line.doId}" />
                    <input type="hidden" name="Lines[${index}].ItemId" value="${line.itemId}" />
                    <input type="hidden" name="Lines[${index}].Qty" value="${line.qty}" />
                    <input type="hidden" name="Lines[${index}].UnitId" value="${line.unitId || 0}" />
                    <input type="hidden" name="Lines[${index}].Price" value="${line.price}" />
                `;
            });
        },
        
        /**
         * Submit the form
         */
        proceedSubmit() {
            this.showConfirmModal = false;
            this.isSubmitting = true;
            document.getElementById('si-form').submit();
        },
        
        /**
         * Format number with thousand separators
         */
        formatNumber(value) {
            if (value === null || value === undefined || value === '') return '';
            const num = parseFloat(String(value).replace(/,/g, ''));
            if (isNaN(num)) return value;
            return num.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
        },
        
        /**
         * Format decimal with thousand separators
         */
        formatDecimal(value) {
            if (value === null || value === undefined || value === '') return '';
            const num = parseFloat(String(value).replace(/,/g, ''));
            if (isNaN(num)) return value;
            return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
    };
}

// Make available globally
window.siCreateForm = siCreateForm;