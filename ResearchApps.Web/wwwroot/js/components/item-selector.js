/**
 * Item Selector Modal Component
 * Reusable component for selecting items with pagination and search
 * Usage: Include _ItemSelectorModal.cshtml partial and add x-data="itemSelector()" to parent
 */
document.addEventListener('alpine:init', () => {
    Alpine.data('itemSelector', () => ({
        // State
        show: false,
        isLoading: false,
        items: [],
        
        // Filters
        filters: {
            itemName: '',
            itemTypeName: '',
            statusId: ''
        },
        
        // Pagination
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
        
        // Callback for selection
        onSelectCallback: null,
    
    /**
     * Open the item selector modal
     * @param {Function} onSelect - Callback function when item is selected, receives item object
     */
    open(onSelect) {
        this.onSelectCallback = onSelect;
        this.resetFilters();
        this.show = true;
        this.search();
    },
    
    /**
     * Close the modal
     */
    close() {
        this.show = false;
        this.onSelectCallback = null;
    },
    
    /**
     * Reset filters to default
     */
    resetFilters() {
        this.filters = {
            itemName: '',
            itemTypeName: '',
            statusId: ''
        };
        this.pageNumber = 1;
    },
    
    /**
     * Search items with current filters
     */
    async search() {
        this.isLoading = true;
        
        try {
            const params = new URLSearchParams({
                pageNumber: this.pageNumber,
                pageSize: this.pageSize
            });
            
            // Add filters if present
            if (this.filters.itemName) {
                params.append('itemName', this.filters.itemName);
            }
            if (this.filters.itemTypeName) {
                params.append('itemTypeName', this.filters.itemTypeName);
            }
            if (this.filters.statusId) {
                params.append('statusId', this.filters.statusId);
            }
            
            const response = await fetch(`/api/Items?${params.toString()}`);
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
            this.totalCount = 0;
            this.totalPages = 0;
        } finally {
            this.isLoading = false;
        }
    },
    
    /**
     * Handle search on Enter key
     */
    handleSearchEnter() {
        this.pageNumber = 1;
        this.search();
    },
    
    /**
     * Go to a specific page
     * @param {number} page - Page number
     */
    goToPage(page) {
        if (page < 1 || page > this.totalPages) return;
        this.pageNumber = page;
        this.search();
    },
    
    /**
     * Go to previous page
     */
    prevPage() {
        if (this.pageNumber > 1) {
            this.pageNumber--;
            this.search();
        }
    },
    
    /**
     * Go to next page
     */
    nextPage() {
        if (this.pageNumber < this.totalPages) {
            this.pageNumber++;
            this.search();
        }
    },
    
    /**
     * Select an item and close modal
     * @param {Object} item - Selected item object
     */
    selectItem(item) {
        if (this.onSelectCallback && typeof this.onSelectCallback === 'function') {
            this.onSelectCallback(item);
        }
        this.close();
    },
    
    /**
     * Format number for display
     * @param {number} value - Number to format
     * @returns {string} Formatted number
     */
    formatNumber(value) {
        if (value === null || value === undefined) return '0';
        return new Intl.NumberFormat('id-ID').format(value);
    },
    
    /**
     * Get visible page numbers for pagination
     * @returns {Array} Array of page numbers to display
     */
    getVisiblePages() {
        const pages = [];
        const maxVisible = 5;
        let start = Math.max(1, this.pageNumber - Math.floor(maxVisible / 2));
        let end = Math.min(this.totalPages, start + maxVisible - 1);
        
        // Adjust start if we're near the end
        if (end - start + 1 < maxVisible) {
            start = Math.max(1, end - maxVisible + 1);
        }
        
        for (let i = start; i <= end; i++) {
            pages.push(i);
        }
        
        return pages;
    }
    }));
});
