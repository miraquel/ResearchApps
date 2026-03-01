/**
 * Penyesuaian Stock Edit Page Alpine.js Component
 * Handles form editing and line management
 */
function psEditForm() {
    return {
        // State
        errors: {},
        isSubmitting: false,
        
        // Line form state
        editingLineId: 0,
        lineQty: 0,
        lineNotes: '',
        
        // TomSelect instances
        lineItemSelect: null,
        lineWhSelect: null,
        
        // Bootstrap modal instance
        addLineModal: null,
        
        /**
         * Initialize the component
         */
        init() {
            // Initialize Bootstrap modal
            const modalEl = document.getElementById('addLineModal');
            if (modalEl) {
                this.addLineModal = new bootstrap.Modal(modalEl);
                
                // Reset form when modal is hidden
                modalEl.addEventListener('hidden.bs.modal', () => {
                    this.resetLineForm();
                });
            }
            
            // Watch for HTMX line reload events
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'ps-lines-container') {
                    // Lines reloaded
                }
            });
        },
        
        /**
         * Show add line modal
         */
        showAddLineModal() {
            this.editingLineId = 0;
            this.resetLineForm();
            this.initializeLineSelects();
            if (this.addLineModal) {
                this.addLineModal.show();
            }
        },
        
        /**
         * Initialize line TomSelect dropdowns
         */
        initializeLineSelects() {
            // Destroy existing instances
            if (this.lineItemSelect) {
                this.lineItemSelect.destroy();
                this.lineItemSelect = null;
            }
            if (this.lineWhSelect) {
                this.lineWhSelect.destroy();
                this.lineWhSelect = null;
            }
            
            // Small delay to ensure modal is shown
            setTimeout(() => {
                // Item select
                this.lineItemSelect = initTomSelect('#LineItemId', {
                    url: '/api/Items/cbo',
                    placeholder: 'Select Item',
                    maxOptions: 50
                });
                
                // Warehouse select
                this.lineWhSelect = initTomSelect('#LineWhId', {
                    url: '/api/Warehouses/cbo',
                    placeholder: 'Select Warehouse',
                    maxOptions: 50
                });
            }, 100);
        },
        
        /**
         * Reset line form
         */
        resetLineForm() {
            this.editingLineId = 0;
            this.lineQty = 0;
            this.lineNotes = '';
            
            // Clear select values
            if (this.lineItemSelect) {
                this.lineItemSelect.clear();
            }
            if (this.lineWhSelect) {
                this.lineWhSelect.clear();
            }
        },
        
        /**
         * Edit existing line
         */
        async editLine(lineId) {
            try {
                const response = await fetch(`/Pss/GetLineForEdit/${lineId}`);
                if (response.ok) {
                    const line = await response.json();
                    
                    this.editingLineId = line.psLineId;
                    this.lineQty = line.qty;
                    this.lineNotes = line.notes || '';
                    
                    this.initializeLineSelects();
                    
                    // Set select values after a delay
                    setTimeout(() => {
                        if (this.lineItemSelect && line.itemId) {
                            this.lineItemSelect.addOption({
                                value: line.itemId.toString(),
                                text: line.itemName
                            });
                            this.lineItemSelect.setValue(line.itemId.toString());
                        }
                        if (this.lineWhSelect && line.whId) {
                            this.lineWhSelect.addOption({
                                value: line.whId.toString(),
                                text: line.whName
                            });
                            this.lineWhSelect.setValue(line.whId.toString());
                        }
                    }, 200);
                    
                    if (this.addLineModal) {
                        this.addLineModal.show();
                    }
                }
            } catch (error) {
                console.error('Error loading line:', error);
                toastr.error('Failed to load line data');
            }
        },
        
        /**
         * Format number for display
         */
        formatNumber(value) {
            return parseFloat(value || 0).toLocaleString('en-US', {
                minimumFractionDigits: 0,
                maximumFractionDigits: 0
            });
        },
        
        /**
         * Format decimal for display
         */
        formatDecimal(value) {
            return parseFloat(value || 0).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
        }
    };
}
