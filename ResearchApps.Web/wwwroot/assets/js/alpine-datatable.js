/**
 * Alpine.js DataTable Component
 * Replaces jQuery DataTables with Alpine.js + fetch for server-side
 * paginated, sortable, and filterable tables.
 *
 * Compatible with the existing PagedListVm<T> response format:
 *   { Items: [...], TotalCount, PageNumber, PageSize, TotalPages }
 *
 * Usage:
 *   <div x-data="dataTable({
 *       url: '/api/Reports',
 *       columns: [
 *          { field: 'Id', label: '#', sortable: true },
 *          { field: 'Name', label: 'Report Name', sortable: true },
 *          { field: 'CreatedDate', label: 'Date', sortable: true }
 *       ],
 *       pageSize: 10,
 *       defaultSort: 'Id',
 *       defaultSortAsc: false
 *   })">
 *       <!-- Search -->
 *       <input type="text" x-model.debounce.500ms="search" placeholder="Search..." />
 *
 *       <!-- Table -->
 *       <table class="table">
 *           <thead>
 *               <tr>
 *                   <template x-for="col in columns" :key="col.field">
 *                       <th @click="col.sortable && sort(col.field)"
 *                           :class="{ 'cursor-pointer': col.sortable }"
 *                           x-text="col.label">
 *                       </th>
 *                   </template>
 *               </tr>
 *           </thead>
 *           <tbody>
 *               <template x-if="isLoading">
 *                   <tr><td :colspan="columns.length" class="text-center py-4">
 *                       <span class="spinner-border spinner-border-sm me-2"></span> Loading...
 *                   </td></tr>
 *               </template>
 *               <template x-if="!isLoading && items.length === 0">
 *                   <tr><td :colspan="columns.length" class="text-center py-4 text-muted">
 *                       No records found.
 *                   </td></tr>
 *               </template>
 *               <template x-for="item in items" :key="item[primaryKey] || JSON.stringify(item)">
 *                   <tr>
 *                       <!-- Render via slot or default -->
 *                   </tr>
 *               </template>
 *           </tbody>
 *       </table>
 *
 *       <!-- Pagination -->
 *       <nav x-show="totalPages > 1">
 *           <ul class="pagination">
 *               <li class="page-item" :class="{ disabled: page === 1 }">
 *                   <a class="page-link" href="#" @click.prevent="goToPage(page - 1)">Previous</a>
 *               </li>
 *               <template x-for="p in pageNumbers" :key="p">
 *                   <li class="page-item" :class="{ active: p === page }">
 *                       <a class="page-link" href="#" @click.prevent="goToPage(p)" x-text="p"></a>
 *                   </li>
 *               </template>
 *               <li class="page-item" :class="{ disabled: page === totalPages }">
 *                   <a class="page-link" href="#" @click.prevent="goToPage(page + 1)">Next</a>
 *               </li>
 *           </ul>
 *       </nav>
 *   </div>
 */
document.addEventListener('alpine:init', () => {

    Alpine.data('dataTable', (config = {}) => ({
        // Configuration
        url: config.url || '',
        method: (config.method || 'GET').toUpperCase(),
        columns: config.columns || [],
        primaryKey: config.primaryKey || 'Id',
        pageSize: config.pageSize || 10,
        maxVisiblePages: config.maxVisiblePages || 5,

        // State
        items: [],
        isLoading: false,
        hasError: false,
        errorMessage: '',

        // Pagination
        page: 1,
        totalCount: 0,
        totalPages: 0,

        // Sorting
        sortBy: config.defaultSort || '',
        sortAsc: config.defaultSortAsc !== undefined ? config.defaultSortAsc : true,

        // Search
        search: '',

        // Column filters (for advanced filtering)
        filters: config.filters || {},

        init() {
            // Watch search changes — debounce is handled by x-model.debounce
            this.$watch('search', () => {
                this.page = 1;
                this.fetchData();
            });

            // Initial load
            this.fetchData();
        },

        /**
         * Fetch data from server.
         */
        async fetchData() {
            this.isLoading = true;
            this.hasError = false;

            try {
                const params = new URLSearchParams();
                params.set('pageNumber', this.page);
                params.set('pageSize', this.pageSize);

                if (this.search) {
                    params.set('search', this.search);
                }

                if (this.sortBy) {
                    params.set('sortBy', this.sortBy);
                    params.set('isSortAscending', this.sortAsc);
                }

                // Column-specific filters
                Object.entries(this.filters).forEach(([key, value]) => {
                    if (value !== '' && value != null) {
                        params.set(`filters[${key}]`, value);
                    }
                });

                let response;
                if (this.method === 'POST') {
                    // Include CSRF token for POST requests
                    const tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
                    if (tokenEl) params.set('__RequestVerificationToken', tokenEl.value);

                    response = await fetch(this.url, {
                        method: 'POST',
                        headers: { 'Accept': 'application/json' },
                        body: params
                    });
                } else {
                    response = await fetch(`${this.url}?${params.toString()}`, {
                        headers: { 'Accept': 'application/json' }
                    });
                }

                if (!response.ok) {
                    throw new Error(`Server returned ${response.status}`);
                }

                const data = await response.json();

                // Support both PagedListVm and ServiceResponse<PagedListVm> formats
                const paged = data.Data || data.data || data;
                this.items = paged.Items || paged.items || [];
                this.totalCount = paged.TotalCount || paged.totalCount || 0;
                this.totalPages = paged.TotalPages || paged.totalPages || 0;
                this.page = paged.PageNumber || paged.pageNumber || this.page;

            } catch (error) {
                console.error('DataTable fetch error:', error);
                this.hasError = true;
                this.errorMessage = error.message || 'Failed to load data';
                this.items = [];

                if (window.Alpine && Alpine.store('toast')) {
                    Alpine.store('toast').error('Failed to load data: ' + this.errorMessage);
                }
            } finally {
                this.isLoading = false;
            }
        },

        /**
         * Sort by column.
         * @param {string} field
         */
        sort(field) {
            if (this.sortBy === field) {
                this.sortAsc = !this.sortAsc;
            } else {
                this.sortBy = field;
                this.sortAsc = true;
            }
            this.page = 1;
            this.fetchData();
        },

        /**
         * Get sort icon class for a column.
         * @param {string} field
         * @returns {string}
         */
        sortIcon(field) {
            if (this.sortBy !== field) return 'ri-arrow-up-down-line text-muted';
            return this.sortAsc ? 'ri-arrow-up-s-line' : 'ri-arrow-down-s-line';
        },

        /**
         * Navigate to a page.
         * @param {number} p
         */
        goToPage(p) {
            if (p < 1 || p > this.totalPages || p === this.page) return;
            this.page = p;
            this.fetchData();
        },

        /**
         * Computed page numbers for pagination.
         * Returns an array like [1, 2, 3, 4, 5] centered around current page.
         */
        get pageNumbers() {
            const total = this.totalPages;
            const current = this.page;
            const max = this.maxVisiblePages;

            if (total <= max) {
                return Array.from({ length: total }, (_, i) => i + 1);
            }

            let start = Math.max(1, current - Math.floor(max / 2));
            let end = start + max - 1;

            if (end > total) {
                end = total;
                start = Math.max(1, end - max + 1);
            }

            return Array.from({ length: end - start + 1 }, (_, i) => start + i);
        },

        /**
         * Update a single filter and re-fetch.
         * @param {string} key
         * @param {*} value
         */
        setFilter(key, value) {
            this.filters[key] = value;
            this.page = 1;
            this.fetchData();
        },

        /**
         * Clear all filters and search.
         */
        clearFilters() {
            this.search = '';
            Object.keys(this.filters).forEach(key => {
                this.filters[key] = '';
            });
            this.sortBy = config.defaultSort || '';
            this.sortAsc = config.defaultSortAsc !== undefined ? config.defaultSortAsc : true;
            this.page = 1;
            this.fetchData();
        },

        /**
         * Refresh current page data.
         */
        refresh() {
            this.fetchData();
        },

        /**
         * Get display range text, e.g. "Showing 1 to 10 of 123 entries"
         */
        get displayRange() {
            if (this.totalCount === 0) return 'No entries';
            const start = (this.page - 1) * this.pageSize + 1;
            const end = Math.min(this.page * this.pageSize, this.totalCount);
            return `Showing ${start} to ${end} of ${this.totalCount} entries`;
        }
    }));
});
