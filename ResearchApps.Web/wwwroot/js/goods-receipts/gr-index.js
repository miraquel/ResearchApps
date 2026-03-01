/**
 * Goods Receipt Index Page Component
 * Handles listing, filtering, sorting, and pagination of goods receipts
 */
function grIndex() {
    return {
        isLoading: false,
        isExporting: false,
        sortBy: 'GrId',
        sortAsc: false,
        dateRangePicker: null,
        filters: {
            GrId: '',
            GrDateFrom: '',
            GrDateTo: '',
            SupplierName: '',
            TotalOperator: '=',
            Total: '',
            GrStatusId: ''
        },
        
        init() {
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'grListContainer') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'grListContainer') {
                    this.isLoading = false;
                    this.initializeDatePicker();
                }
            });
            
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'grListContainer') {
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
                if (e.detail.target && e.detail.target.id === 'grListContainer') {
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
            if (this.dateRangePicker) {
                this.dateRangePicker.destroy();
            }
            
            const dateInput = document.querySelector('#gr-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                allowInput: false,
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        this.filters.GrDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.GrDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 1) {
                        this.filters.GrDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.GrDateTo = '';
                    } else {
                        this.filters.GrDateFrom = '';
                        this.filters.GrDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            if (this.filters.GrDateFrom && this.filters.GrDateTo) {
                this.dateRangePicker.setDate([this.filters.GrDateFrom, this.filters.GrDateTo], false);
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
                GrId: '',
                GrDateFrom: '',
                GrDateTo: '',
                SupplierName: '',
                TotalOperator: '=',
                Total: '',
                GrStatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = 'GrId';
            this.sortAsc = false;
            this.fetchList();
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('grListContainer');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            Object.entries(this.filters).forEach(([key, value]) => {
                if (key === 'TotalOperator' || key === 'Total') {
                    return;
                }
                if (value && value.trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            if (this.filters.Total && this.filters.Total.trim() !== '') {
                params.set('filters[TotalOperator]', this.filters.TotalOperator);
                params.set('filters[Total]', this.filters.Total);
            }
            
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/GoodsReceipts/List?${params.toString()}`;
            
            htmx.ajax('GET', url, {
                target: '#grListContainer',
                swap: 'innerHTML'
            });
        },
        
        formatNumber(value) {
            if (!value && value !== 0) return '';
            return parseFloat(value).toLocaleString('en-US', { maximumFractionDigits: 2 });
        },
        
        parseNumber(value) {
            if (!value) return '';
            return value.replace(/,/g, '');
        },
        
        async exportToExcel() {
            this.isExporting = true;
            try {
                const params = new URLSearchParams();
                
                Object.entries(this.filters).forEach(([key, value]) => {
                    if (key === 'TotalOperator' || key === 'Total') {
                        return;
                    }
                    if (value && value.trim() !== '') {
                        params.set(`filters[${key}]`, value);
                    }
                });
                
                if (this.filters.Total && this.filters.Total.trim() !== '') {
                    params.set('filters[TotalOperator]', this.filters.TotalOperator);
                    params.set('filters[Total]', this.filters.Total);
                }
                
                const response = await fetch(`/api/GoodsReceipts/export?${params.toString()}`, {
                    method: 'GET'
                });
                
                if (response.ok) {
                    const blob = await response.blob();
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = `GoodsReceipts_${new Date().toISOString().split('T')[0]}.xlsx`;
                    document.body.appendChild(a);
                    a.click();
                    window.URL.revokeObjectURL(url);
                    a.remove();
                    toastr.success('Export completed successfully');
                } else {
                    toastr.error('Failed to export data');
                }
            } catch (error) {
                console.error('Export error:', error);
                toastr.error('An error occurred during export');
            } finally {
                this.isExporting = false;
            }
        }
    };
}
