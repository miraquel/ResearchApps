/**
 * Reusable PO Line Selector Component
 * Provides PO line search and selection functionality with filters and pagination
 */
function createPoSelector(config = {}) {
    return {
        show: false,
        isLoading: false,
        items: [],
        filters: {
            poId: '',
            itemName: '',
            deliveryDate: ''
        },
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
        
        // Configuration
        config: {
            supplierId: null,
            onSelect: null, // Callback when PO line is selected
            ...config
        },
        
        /**
         * Open the PO selector modal
         */
        open(supplierId) {
            if (!supplierId) {
                showNotificationModal('Please select a supplier first', 'warning');
                return;
            }
            
            this.config.supplierId = supplierId;
            this.show = true;
            this.filters = { poId: '', itemName: '', deliveryDate: '' };
            this.pageNumber = 1;
            this.search();
        },
        
        /**
         * Reset all filters and refresh search
         */
        resetFilters() {
            this.filters = { poId: '', itemName: '', deliveryDate: '' };
            this.search();
        },
        
        /**
         * Search for PO lines with current filters
         */
        async search() {
            if (!this.config.supplierId) {
                return;
            }
            
            this.isLoading = true;
            try {
                const response = await fetch(`/api/GoodsReceipts/outstanding/${this.config.supplierId}`);
                const result = await response.json();
                
                if (result.data) {
                    let items = result.data || [];
                    
                    // Client-side filtering
                    if (this.filters.poId) {
                        items = items.filter(po => 
                            po.poId.toLowerCase().includes(this.filters.poId.toLowerCase())
                        );
                    }
                    if (this.filters.itemName) {
                        items = items.filter(po => 
                            po.itemName.toLowerCase().includes(this.filters.itemName.toLowerCase())
                        );
                    }
                    if (this.filters.deliveryDate) {
                        items = items.filter(po => 
                            po.deliveryDateStr && po.deliveryDateStr.includes(this.filters.deliveryDate)
                        );
                    }
                    
                    // Client-side pagination
                    this.totalCount = items.length;
                    this.totalPages = Math.ceil(items.length / this.pageSize);
                    
                    const start = (this.pageNumber - 1) * this.pageSize;
                    const end = start + this.pageSize;
                    this.items = items.slice(start, end);
                } else {
                    this.items = [];
                    this.totalCount = 0;
                    this.totalPages = 0;
                }
            } catch (error) {
                console.error('Error searching PO lines:', error);
                this.items = [];
                showNotificationModal('Failed to load PO lines', true);
            } finally {
                this.isLoading = false;
            }
        },
        
        /**
         * Handle PO line selection
         */
        select(po) {
            if (this.config.onSelect) {
                this.config.onSelect(po);
            }
            this.show = false;
        },
        
        /**
         * Handle search on Enter key
         */
        handleSearchEnter() {
            this.pageNumber = 1;
            this.search();
        },
        
        /**
         * Navigate to specific page
         */
        goToPage(page) {
            if (page < 1 || page > this.totalPages) return;
            this.pageNumber = page;
            this.search();
        },
        
        /**
         * Get visible page numbers for pagination
         */
        getVisiblePages() {
            const pages = [];
            const maxVisible = 5;
            let start = Math.max(1, this.pageNumber - Math.floor(maxVisible / 2));
            let end = Math.min(this.totalPages, start + maxVisible - 1);
            if (end - start + 1 < maxVisible) {
                start = Math.max(1, end - maxVisible + 1);
            }
            for (let i = start; i <= end; i++) {
                pages.push(i);
            }
            return pages;
        }
    };
}
