/**
 * Delivery Order Index Page Component
 * Handles listing, filtering, sorting, and pagination of delivery orders
 */
function doIndex() {
    return {
        isLoading: false,
        isExporting: false,
        sortBy: 'DoId',
        sortAsc: false,
        dateRangePicker: null,
        filters: {
            DoId: '',
            DoDateFrom: '',
            DoDateTo: '',
            CustomerName: '',
            CoId: '',
            RefId: '',
            Descr: '',
            DoStatusId: ''
        },
        
        init() {
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'do-list-container') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'do-list-container') {
                    this.isLoading = false;
                    // Initialize flatpickr after content is swapped in
                    this.initializeDatePicker();
                }
            });
            
            // Handle errors - response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'do-list-container') {
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
                if (e.detail.target && e.detail.target.id === 'do-list-container') {
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
        
        initializeDatePicker() {
            // Destroy existing instance if any
            if (this.dateRangePicker) {
                this.dateRangePicker.destroy();
            }
            
            // Initialize flatpickr for date range
            const dateInput = document.querySelector('#do-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                allowInput: false,
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        // Range selected - fetch results
                        this.filters.DoDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.DoDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 1) {
                        // First date selected - just store it, don't fetch yet
                        this.filters.DoDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.DoDateTo = '';
                    } else {
                        // Cleared - reset and fetch
                        this.filters.DoDateFrom = '';
                        this.filters.DoDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            // Restore previously selected dates if they exist
            if (this.filters.DoDateFrom && this.filters.DoDateTo) {
                this.dateRangePicker.setDate([this.filters.DoDateFrom, this.filters.DoDateTo], false);
            }
        },
        
        formatDateISO(date) {
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
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
                DoId: '',
                DoDateFrom: '',
                DoDateTo: '',
                CustomerName: '',
                CoId: '',
                RefId: '',
                Descr: '',
                DoStatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = 'DoId';
            this.sortAsc = false;
            this.fetchList();
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('do-list-container');
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
            
            const url = `/DeliveryOrders/List?${params.toString()}`;
            
            // Use htmx to fetch
            htmx.ajax('GET', url, {
                target: '#do-list-container',
                swap: 'innerHTML',
                indicator: '#list-loading'
            });
        },
        
        async exportToExcel() {
            this.isExporting = true;
            
            try {
                // Build query parameters from current filters and sorting
                const params = new URLSearchParams();
                
                // Add filters with proper format for PagedListRequestVm binding
                Object.keys(this.filters).forEach(key => {
                    if (this.filters[key]) {
                        params.append(`Filters[${key}]`, this.filters[key]);
                    }
                });
                
                // Add sorting
                params.append('SortBy', this.sortBy);
                params.append('IsSortAscending', this.sortAsc);
                
                // Add pagination (high page size to get all)
                params.append('PageNumber', 1);
                params.append('PageSize', 999999);
                
                const response = await fetch(`/api/DeliveryOrders/export?${params.toString()}`, {
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
                a.download = `DeliveryOrders_${new Date().toISOString().slice(0, 10)}.xlsx`;
                document.body.appendChild(a);
                a.click();
                
                // Cleanup
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                
                // Show success message
                if (window.showAlertModal) {
                    window.showAlertModal('Success', 'Excel file exported successfully', 'success');
                } else {
                    alert('Excel file exported successfully');
                }
                
            } catch (error) {
                console.error('Error exporting to Excel:', error);
                
                // Show error message
                if (window.showAlertModal) {
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
window.doIndex = doIndex;
