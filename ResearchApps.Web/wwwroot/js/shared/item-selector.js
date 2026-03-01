/**
 * Reusable Item Selector Component
 * Provides item search and selection functionality with filters and pagination
 */
function createItemSelector(config = {}) {
    return {
        show: false,
        isLoading: false,
        items: [],
        filters: {
            itemId: '',
            itemName: '',
            itemTypeName: '',
            itemDeptName: '',
            statusId: ''
        },
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
        itemTypeFilterSelect: null,
        itemDeptFilterSelect: null,
        
        // Configuration
        config: {
            useTypeFilter: true,
            useDeptFilter: true,
            onSelect: null, // Callback when item is selected
            priceField: 'purchasePrice', // 'purchasePrice' or 'salesPrice'
            ...config
        },
        
        /**
         * Open the item selector modal
         */
        open(itemTypeFilterRef, itemDeptFilterRef, $nextTick) {
            this.show = true;
            this.filters = { itemId: '', itemName: '', itemTypeName: '', itemDeptName: '', statusId: '' };
            this.pageNumber = 1;
            
            // Initialize filter TomSelects after DOM update (only for CO module)
            if (this.config.useTypeFilter || this.config.useDeptFilter) {
                $nextTick(() => {
                    if (this.config.useTypeFilter && itemTypeFilterRef && !this.itemTypeFilterSelect) {
                        this.itemTypeFilterSelect = initTomSelect(itemTypeFilterRef, {
                            url: '/api/ItemTypes/cbo',
                            placeholder: 'All Types',
                            onChange: () => this.handleSearchEnter()
                        });
                    }
                    if (this.config.useDeptFilter && itemDeptFilterRef && !this.itemDeptFilterSelect) {
                        this.itemDeptFilterSelect = initTomSelect(itemDeptFilterRef, {
                            url: '/api/ItemDepts/cbo',
                            placeholder: 'All Departments',
                            onChange: () => this.handleSearchEnter()
                        });
                    }
                });
            }
            
            this.search();
        },
        
        /**
         * Reset all filters and refresh search
         */
        resetFilters() {
            this.filters = { itemId: '', itemName: '', itemTypeName: '', itemDeptName: '', statusId: '' };
            if (this.itemTypeFilterSelect) this.itemTypeFilterSelect.clear();
            if (this.itemDeptFilterSelect) this.itemDeptFilterSelect.clear();
            this.search();
        },
        
        /**
         * Search for items with current filters
         */
        async search() {
            this.isLoading = true;
            try {
                const params = new URLSearchParams({
                    pageNumber: this.pageNumber,
                    pageSize: this.pageSize
                });
                
                if (this.filters.itemId) params.append('filters.itemId', this.filters.itemId);
                if (this.filters.itemName) params.append('filters.itemName', this.filters.itemName);
                
                // Use appropriate filter field names based on configuration
                if (this.config.useTypeFilter) {
                    if (this.filters.itemTypeId) params.append('filters.itemTypeId', this.filters.itemTypeId);
                    if (this.filters.itemTypeName) params.append('filters.itemTypeName', this.filters.itemTypeName);
                }
                
                if (this.config.useDeptFilter) {
                    if (this.filters.itemDeptId) params.append('filters.itemDeptId', this.filters.itemDeptId);
                    if (this.filters.itemDeptName) params.append('filters.itemDeptName', this.filters.itemDeptName);
                }
                
                if (this.filters.statusId) params.append('filters.statusId', this.filters.statusId);
                
                const response = await fetch(`/api/Items?${params}`);
                const result = await response.json();
                
                if (result.data) {
                    this.items = result.data.items || [];
                    this.totalCount = result.data.totalCount || 0;
                    this.totalPages = result.data.totalPages || 0;
                } else {
                    this.items = [];
                    this.totalCount = 0;
                    this.totalPages = 0;
                }
            } catch (error) {
                console.error('Error searching items:', error);
                this.items = [];
            } finally {
                this.isLoading = false;
            }
        },
        
        /**
         * Handle item selection
         */
        select(item, lineModalData, lineModalQty) {
            if (this.config.onSelect) {
                // Use custom callback if provided
                this.config.onSelect(item);
            } else {
                // Default selection behavior
                lineModalData.itemId = item.itemId;
                lineModalData.itemName = item.itemName;
                lineModalData.unitId = item.unitId;
                lineModalData.unitName = item.unitName;
                lineModalData.price = item[this.config.priceField] || 0;
                lineModalData.amount = lineModalQty * lineModalData.price;
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
