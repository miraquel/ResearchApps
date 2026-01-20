/**
 * Supplier Index Page Component
 * Handles listing, filtering, sorting, and pagination of suppliers with HTMX
 */
function supplierIndex() {
    return {
        isLoading: false,
        isExporting: false,
        sortBy: 'SupplierName',
        sortAsc: true,
        filters: {
            SupplierName: '',
            Address: '',
            City: '',
            Telp: '',
            Email: '',
            StatusId: '',
            IsPpn: ''
        },
        
        /**
         * Initialize component and setup HTMX event listeners
         * @returns {void}
         */
        init() {
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'supplier-list-container') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'supplier-list-container') {
                    this.isLoading = false;
                }
            });
            
            // Handle errors - response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'supplier-list-container') {
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
            
            // Handle network errors (connection issues, timeouts)
            document.body.addEventListener('htmx:sendError', (e) => {
                if (e.detail.target && e.detail.target.id === 'supplier-list-container') {
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
        },
        
        /**
         * Sort table by column
         * Toggles sort direction if same column, otherwise sets new column and ascending order
         * @param {string} column - Column name to sort by
         * @returns {void}
         */
        sort(column) {
            if (this.sortBy === column) {
                this.sortAsc = !this.sortAsc;
            } else {
                this.sortBy = column;
                this.sortAsc = true;
            }
            this.fetchList();
        },
        
        /**
         * Clear all filters and reset sort to default
         * @returns {void}
         */
        clearFilters() {
            this.filters = {
                SupplierName: '',
                Address: '',
                City: '',
                Telp: '',
                Email: '',
                StatusId: '',
                IsPpn: ''
            };
            this.sortBy = 'SupplierName';
            this.sortAsc = true;
            this.fetchList();
        },
        
        /**
         * Fetch supplier list with current filters, sorting, and pagination
         * Uses HTMX to load partial view into container
         * @param {number} page - Page number to fetch (default: 1)
         * @returns {void}
         */
        fetchList(page = 1) {
            const container = document.getElementById('supplier-list-container');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            // Add column filters
            Object.entries(this.filters).forEach(([key, value]) => {
                if (value && value.toString().trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            // Add sort parameters
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/Suppliers/List?${params.toString()}`;
            
            // Use htmx to fetch
            htmx.ajax('GET', url, {
                target: '#supplier-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        },
        
        /**
         * Export suppliers to Excel with current filters and sorting
         * @returns {Promise<void>}
         */
        async exportToExcel() {
            this.isExporting = true;
            
            try {
                // Build query parameters from current filters and sorting
                const params = new URLSearchParams();
                
                // Add filters with proper format for PagedListRequestVm binding
                Object.keys(this.filters).forEach(key => {
                    if (this.filters[key] && this.filters[key].toString().trim() !== '') {
                        params.append(`Filters[${key}]`, this.filters[key]);
                    }
                });
                
                // Add sorting
                params.append('SortBy', this.sortBy);
                params.append('IsSortAscending', this.sortAsc);
                
                // Add pagination (high page size to get all)
                params.append('PageNumber', 1);
                params.append('PageSize', 999999);
                
                const response = await fetch(`/api/Suppliers/export?${params.toString()}`, {
                    method: 'GET',
                    headers: {
                        'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                    }
                });
                
                if (!response.ok) {
                    const errorData = await response.json().catch(() => null);
                    const errorMessage = errorData?.message || `Server error: ${response.status} ${response.statusText}`;
                    throw new Error(errorMessage);
                }
                
                // Get the Excel blob
                const blob = await response.blob();
                
                // Create download link
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `Suppliers_${new Date().toISOString().slice(0, 10)}.xlsx`;
                document.body.appendChild(a);
                a.click();
                
                // Cleanup
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                
                // Show success message
                if (window.showSuccess) {
                    window.showSuccess('Excel file exported successfully');
                } else if (window.showAlertModal) {
                    window.showAlertModal('Success', 'Excel file exported successfully', 'success');
                } else {
                    alert('Excel file exported successfully');
                }
                
            } catch (error) {
                console.error('[Supplier Index] Export error:', error);
                
                // Show error message
                if (window.showError) {
                    window.showError(error.message || 'Failed to export to Excel');
                } else if (window.showAlertModal) {
                    window.showAlertModal('Error', error.message || 'Failed to export to Excel', 'error');
                } else {
                    alert(error.message || 'Failed to export to Excel');
                }
            } finally {
                this.isExporting = false;
            }
        }
    };
}

// Make available globally
window.supplierIndex = supplierIndex;
