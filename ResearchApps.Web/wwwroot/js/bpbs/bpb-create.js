/**
 * BPB (Bon Pengambilan Barang) Create Page Component
 * Handles creation of BPB header with Production selection modal
 */
function bpbCreate() {
    return {
        showConfirmModal: false,
        showProdModal: false,
        isSubmitting: false,
        loadingProds: false,
        errors: {},
        refType: 'Production',
        selectedProdId: '',
        selectedProdName: '',
        prodSearch: '',
        productions: [],
        datePicker: null,
        
        init() {
            this.initDatePicker();
            
            // Watch for modal open and fetch productions
            this.$watch('showProdModal', (value) => {
                if (value && this.productions.length === 0) {
                    this.fetchProductions();
                }
            });
        },
        
        initDatePicker() {
            if (this.$refs.bpbDatePicker) {
                this.datePicker = flatpickr(this.$refs.bpbDatePicker, {
                    dateFormat: 'Y-m-d',
                    altInput: true,
                    altFormat: 'd M Y',
                    defaultDate: 'today',
                    allowInput: true
                });
            }
        },
        
        async fetchProductions() {
            this.loadingProds = true;
            try {
                const params = new URLSearchParams();
                if (this.prodSearch) {
                    params.set('search', this.prodSearch);
                }
                params.set('pageSize', '50');
                
                const response = await fetch(`/api/Prods/cbo?${params.toString()}`);
                if (response.ok) {
                    const data = await response.json();
                    this.productions = data.data.items || [];
                } else {
                    window.showNotificationModal('Failed to load production order', true);
                    this.productions = [];
                }
            } catch (error) {
                console.error('Error fetching productions:', error);
                window.showNotificationModal('An error occurred while loading production orders', true);
                this.productions = [];
            } finally {
                this.loadingProds = false;
            }
        },
        
        selectProduction(prod) {
            this.selectedProdId = prod.prodId;
            this.selectedProdName = `${prod.text || prod.itemName || ''} (${prod.prodDate || ''})`;
            this.showProdModal = false;
        },
        
        validateForm() {
            this.errors = {};
            
            const dateInput = document.getElementById('BpbDate');
            if (!dateInput || !dateInput.value) {
                this.errors.BpbDate = 'BPB Date is required';
            }
            
            const refType = document.getElementById('RefType').value;
            if (!refType) {
                this.errors.RefType = 'Reference Type is required';
            }
            
            if (!this.selectedProdId || this.selectedProdId.trim() === '') {
                this.errors.RefId = 'Reference ID is required';
            }
            
            return Object.keys(this.errors).length === 0;
        },
        
        submitForm() {
            if (!this.validateForm()) {
                window.showNotificationModal('Failed to create a prod order, please check the form for errors.', true);
                return;
            }
            
            this.isSubmitting = true;
            
            // Submit the form
            const form = document.getElementById('bpbForm');
            form.submit();
        }
    };
}

// Make available globally
window.bpbCreate = bpbCreate;
