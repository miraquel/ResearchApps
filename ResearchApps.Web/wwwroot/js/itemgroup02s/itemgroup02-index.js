/**
 * ItemGroup02 Index Page Component
 * Handles listing, filtering, sorting, and pagination of item group 02 with HTMX
 */
function itemgroup02Index() {
    return {
        isLoading: false,
        sortBy: 'ItemGroup02Id',
        sortAsc: false,
        filters: {
            ItemGroup02Id: '',
            ItemGroup02Name: '',
            StatusId: ''
        },
        
        init() {
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'itemgroup02-list-container') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'itemgroup02-list-container') {
                    this.isLoading = false;
                }
            });
            
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'itemgroup02-list-container') {
                    this.isLoading = false;
                    const status = e.detail.xhr.status;
                    const statusText = e.detail.xhr.statusText || 'Error';
                    e.detail.target.innerHTML = `
                        <div class="text-center py-5">
                            <div class="mb-3">
                                <i class="ri-error-warning-line text-danger" style="font-size: 3rem;"></i>
                            </div>
                            <h5 class="text-danger">Failed to Load Data</h5>
                            <p class="text-muted">Server returned ${status} ${statusText}</p>
                            <button type="button" class="btn btn-primary" onclick="location.reload()">
                                <i class="ri-refresh-line me-1"></i> Reload Page
                            </button>
                        </div>
                    `;
                }
            });
            
            document.body.addEventListener('htmx:sendError', (e) => {
                if (e.detail.target && e.detail.target.id === 'itemgroup02-list-container') {
                    this.isLoading = false;
                    e.detail.target.innerHTML = `
                        <div class="text-center py-5">
                            <div class="mb-3">
                                <i class="ri-wifi-off-line text-warning" style="font-size: 3rem;"></i>
                            </div>
                            <h5 class="text-warning">Network Error</h5>
                            <p class="text-muted">Unable to connect to the server. Please check your connection.</p>
                            <button type="button" class="btn btn-primary" onclick="location.reload()">
                                <i class="ri-refresh-line me-1"></i> Retry
                            </button>
                        </div>
                    `;
                }
            });
            
            this.fetchList();
        },
        
        sort(column) {
            if (this.sortBy === column) {
                this.sortAsc = !this.sortAsc;
            } else {
                this.sortBy = column;
                this.sortAsc = true;
            }
            this.fetchList();
        },
        
        clearFilters() {
            this.filters = {
                ItemGroup02Id: '',
                ItemGroup02Name: '',
                StatusId: ''
            };
            this.sortBy = 'ItemGroup02Name';
            this.sortAsc = true;
            this.fetchList();
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('itemgroup02-list-container');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            Object.entries(this.filters).forEach(([key, value]) => {
                if (value && value.toString().trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/ItemGroup02s/List?${params.toString()}`;
            
            htmx.ajax('GET', url, {
                target: '#itemgroup02-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        }
    };
}

window.itemgroup02Index = itemgroup02Index;
