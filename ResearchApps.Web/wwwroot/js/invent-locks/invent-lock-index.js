/**
 * Inventory Lock Index Page Component
 * Handles listing and management of inventory closing periods
 */
function inventLockIndex() {
    return {
        isLoading: false,
        selectedYear: new Date().getFullYear(),
        
        // Modal data
        modalRecId: 0,
        modalYear: 0,
        modalMonth: 0,
        modalPeriod: '',
        
        // Modal instances
        closeModal: null,
        openModal: null,
        
        init() {
            // Initialize modals
            const closeModalEl = document.getElementById('closeInventoryModal');
            const openModalEl = document.getElementById('openInventoryModal');
            
            if (closeModalEl) {
                this.closeModal = new bootstrap.Modal(closeModalEl);
            }
            
            if (openModalEl) {
                this.openModal = new bootstrap.Modal(openModalEl);
            }
            
            // Watch for htmx events
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target && e.detail.target.id === 'invent-lock-list-container') {
                    this.isLoading = true;
                }
            });
            
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target && e.detail.target.id === 'invent-lock-list-container') {
                    this.isLoading = false;
                }
            });
            
            // Handle errors - response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target && e.detail.target.id === 'invent-lock-list-container') {
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
                if (e.detail.target && e.detail.target.id === 'invent-lock-list-container') {
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
            
            // Listen for custom event to open unlock modal
            window.addEventListener('open-unlock-modal', (e) => {
                this.showOpenModal(e.detail.recId, e.detail.year, e.detail.month, e.detail.period);
            });
        },
        
        fetchList() {
            const container = document.getElementById('invent-lock-list-container');
            if (container) {
                // Update the URL with the new year parameter and trigger HTMX request
                const url = `/InventLocks/List?year=${this.selectedYear}`;
                htmx.ajax('GET', url, {target: '#invent-lock-list-container', swap: 'innerHTML'});
            }
        },
        
        showCloseModal() {
            if (this.closeModal) {
                this.closeModal.show();
            }
        },
        
        showOpenModal(recId, year, month, period) {
            this.modalRecId = recId;
            this.modalYear = year;
            this.modalMonth = month;
            this.modalPeriod = period;
            
            if (this.openModal) {
                this.openModal.show();
            }
        }
    };
}
