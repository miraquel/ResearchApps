/**
 * Purchase Requisition Index Page Component
 * Handles listing, filtering, sorting, and pagination of purchase requisitions
 */
function prIndex() {
    return {
        isLoading: false,
        isExporting: false,
        sortBy: 'PrId',
        sortAsc: false,
        dateRangePicker: null,
        filters: {
            PrId: '',
            PrDateFrom: '',
            PrDateTo: '',
            PrName: '',
            TotalOperator: '=',
            Total: '',
            PrStatusId: ''
        },
        
        init() {
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'pr-list-container') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'pr-list-container') {
                    this.isLoading = false;
                    // Initialize flatpickr after content is swapped in
                    this.initializeDatePicker();
                }
            });
            
            // Handle errors - response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'pr-list-container') {
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
                if (e.detail.target && e.detail.target.id === 'pr-list-container') {
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
            const dateInput = document.querySelector('#pr-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                allowInput: false,
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        // Range selected - fetch results
                        this.filters.PrDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.PrDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 1) {
                        // First date selected - just store it, don't fetch yet
                        this.filters.PrDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.PrDateTo = '';
                    } else {
                        // Cleared - reset and fetch
                        this.filters.PrDateFrom = '';
                        this.filters.PrDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            // Restore previously selected dates if they exist
            if (this.filters.PrDateFrom && this.filters.PrDateTo) {
                this.dateRangePicker.setDate([this.filters.PrDateFrom, this.filters.PrDateTo], false);
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
                PrId: '',
                PrDateFrom: '',
                PrDateTo: '',
                PrName: '',
                TotalOperator: '=',
                Total: '',
                PrStatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = 'PrId';
            this.sortAsc = false;
            this.fetchList();
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('pr-list-container');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            // Add column filters
            Object.entries(this.filters).forEach(([key, value]) => {
                // Skip TotalOperator and Total individually, handle them together below
                if (key === 'TotalOperator' || key === 'Total') {
                    return;
                }
                if (value && value.trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            // Only add Total filters if both operator and amount are provided
            if (this.filters.Total && this.filters.Total.trim() !== '') {
                params.set('filters[TotalOperator]', this.filters.TotalOperator);
                params.set('filters[Total]', this.filters.Total);
            }
            
            // Add sort parameters
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/Prs/List?${params.toString()}`;
            
            // Use htmx to fetch
            htmx.ajax('GET', url, {
                target: '#pr-list-container',
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
                
                const response = await fetch(`/api/Prs/export?${params.toString()}`, {
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
                a.download = `PurchaseRequisitions_${new Date().toISOString().slice(0, 10)}.xlsx`;
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
        },
        
        // Helper functions for number formatting in filters
        formatNumber(value) {
            if (!value || value === '') return '';
            const num = parseFloat(value) || 0;
            return num.toLocaleString('en-US');
        },
        
        parseNumber(value) {
            if (!value || value === '') return '';
            // Remove all non-numeric characters except decimal point
            return value.replace(/[^\d.]/g, '');
        }
    };
}

// Make available globally
window.prIndex = prIndex;
