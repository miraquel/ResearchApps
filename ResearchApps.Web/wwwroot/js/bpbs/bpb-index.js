/**
 * BPB (Bon Pengambilan Barang) Index Page Component
 * Handles listing, filtering, sorting, and pagination of BPB records
 */
function bpbIndex() {
    return {
        isLoading: false,
        isExporting: false,
        sortBy: 'BpbId',
        sortAsc: false,
        dateRangePicker: null,
        filters: {
            BpbId: '',
            BpbDateFrom: '',
            BpbDateTo: '',
            RefId: '',
            Descr: '',
            AmountOperator: '=',
            Amount: '',
            BpbStatusId: ''
        },
        
        // Report generation
        showReportModal: false,
        isGeneratingReport: false,
        reportType: 'summary', // 'summary' or 'detail'
        reportStartDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0], // First day of current month
        reportEndDate: new Date().toISOString().split('T')[0], // Today
        
        init() {
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'bpbListContainer') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'bpbListContainer') {
                    this.isLoading = false;
                    // Initialize flatpickr after content is swapped in
                    this.initializeDatePicker();
                }
            });
            
            // Handle errors - response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'bpbListContainer') {
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
            
            // Handle network errors
            document.body.addEventListener('htmx:sendError', (e) => {
                if (e.detail.target && e.detail.target.id === 'bpbListContainer') {
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
            
            const dateInput = document.querySelector('#bpb-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                allowInput: false,
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        this.filters.BpbDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.BpbDateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 1) {
                        this.filters.BpbDateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.BpbDateTo = '';
                    } else {
                        this.filters.BpbDateFrom = '';
                        this.filters.BpbDateTo = '';
                        this.fetchList();
                    }
                }
            });
            
            if (this.filters.BpbDateFrom && this.filters.BpbDateTo) {
                this.dateRangePicker.setDate([this.filters.BpbDateFrom, this.filters.BpbDateTo], false);
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
                BpbId: '',
                BpbDateFrom: '',
                BpbDateTo: '',
                RefId: '',
                Descr: '',
                AmountOperator: '=',
                Amount: '',
                BpbStatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = 'BpbId';
            this.sortAsc = false;
            this.fetchList();
        },
        
        fetchList(page = 1) {
            const container = document.getElementById('bpbListContainer');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            Object.entries(this.filters).forEach(([key, value]) => {
                if (key === 'AmountOperator' || key === 'Amount') {
                    return;
                }
                if (value && value.toString().trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            if (this.filters.Amount && this.filters.Amount.toString().trim() !== '') {
                params.set('filters[AmountOperator]', this.filters.AmountOperator);
                params.set('filters[Amount]', this.filters.Amount);
            }
            
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/Bpbs/List?${params.toString()}`;
            
            htmx.ajax('GET', url, {
                target: '#bpbListContainer',
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
                    if (key === 'AmountOperator' || key === 'Amount') {
                        return;
                    }
                    if (value && value.toString().trim() !== '') {
                        params.set(`filters[${key}]`, value);
                    }
                });
                
                if (this.filters.Amount && this.filters.Amount.toString().trim() !== '') {
                    params.set('filters[AmountOperator]', this.filters.AmountOperator);
                    params.set('filters[Amount]', this.filters.Amount);
                }
                
                const response = await fetch(`/api/Bpbs/export?${params.toString()}`, {
                    method: 'GET'
                });
                
                if (response.ok) {
                    const blob = await response.blob();
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = `BPB_${new Date().toISOString().split('T')[0]}.xlsx`;
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
        },
        
        async generateReport() {
            if (!this.reportStartDate || !this.reportEndDate) {
                alert('Please select both start and end dates');
                return;
            }
            
            this.isGeneratingReport = true;
            
            try {
                const params = new URLSearchParams({
                    startDate: this.reportStartDate,
                    endDate: this.reportEndDate
                });
                
                // Determine endpoint based on report type
                const endpoint = this.reportType === 'detail' 
                    ? `/api/Bpbs/report/detail?${params.toString()}`
                    : `/api/Bpbs/report/summary?${params.toString()}`;
                
                const response = await fetch(endpoint, {
                    method: 'GET',
                    headers: {
                        'Accept': 'application/pdf'
                    }
                });
                
                if (!response.ok) {
                    const errorData = await response.json().catch(() => null);
                    const errorMessage = errorData?.message || `Server error: ${response.status} ${response.statusText}`;
                    throw new Error(errorMessage);
                }
                
                // Get the PDF blob
                const blob = await response.blob();
                
                // Create download link
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                const reportTypeLabel = this.reportType === 'detail' ? 'Detail' : 'Summary';
                a.download = `BPB_${reportTypeLabel}_${this.reportStartDate}_${this.reportEndDate}.pdf`;
                document.body.appendChild(a);
                a.click();
                
                // Cleanup
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                
                // Close modal and show success message
                this.showReportModal = false;
                toastr.success('Report generated successfully');
                
            } catch (error) {
                console.error('Error generating report:', error);
                toastr.error(error.message || 'Failed to generate report');
            } finally {
                this.isGeneratingReport = false;
            }
        }
    };
}

// Make available globally
window.bpbIndex = bpbIndex;
