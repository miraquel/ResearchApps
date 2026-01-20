/**
 * Alpine.js Components for ResearchApps
 * Reusable components for Customer Order and other features
 */

// Wait for Alpine to be ready
document.addEventListener('alpine:init', () => {
    
    // ============================================
    // Global Alpine Stores
    // ============================================
    
    /**
     * Toast notification store
     */
    Alpine.store('toast', {
        messages: [],
        
        show(message, type = 'info', duration = 5000) {
            const id = Date.now();
            this.messages.push({ id, message, type });
            
            if (duration > 0) {
                setTimeout(() => this.dismiss(id), duration);
            }
            return id;
        },
        
        success(message, duration = 5000) {
            return this.show(message, 'success', duration);
        },
        
        error(message, duration = 8000) {
            return this.show(message, 'danger', duration);
        },
        
        warning(message, duration = 6000) {
            return this.show(message, 'warning', duration);
        },
        
        dismiss(id) {
            this.messages = this.messages.filter(m => m.id !== id);
        }
    });

    // ============================================
    // Alpine Data Components
    // ============================================
    
    /**
     * Modal component - reusable modal state management
     * Usage: x-data="modal()"
     */
    Alpine.data('modal', () => ({
        isOpen: false,
        isLoading: false,
        
        open() {
            this.isOpen = true;
            document.body.classList.add('overflow-hidden');
        },
        
        close() {
            this.isOpen = false;
            document.body.classList.remove('overflow-hidden');
        },
        
        toggle() {
            this.isOpen ? this.close() : this.open();
        }
    }));

    /**
     * Confirmation dialog component
     * Usage: x-data="confirmDialog()"
     */
    Alpine.data('confirmDialog', () => ({
        isOpen: false,
        title: '',
        message: '',
        confirmText: 'Confirm',
        cancelText: 'Cancel',
        confirmClass: 'btn-primary',
        onConfirm: null,
        isLoading: false,
        
        show(options = {}) {
            this.title = options.title || 'Confirm';
            this.message = options.message || 'Are you sure?';
            this.confirmText = options.confirmText || 'Confirm';
            this.cancelText = options.cancelText || 'Cancel';
            this.confirmClass = options.confirmClass || 'btn-primary';
            this.onConfirm = options.onConfirm || null;
            this.isOpen = true;
        },
        
        async confirm() {
            if (this.onConfirm) {
                this.isLoading = true;
                try {
                    await this.onConfirm();
                } finally {
                    this.isLoading = false;
                }
            }
            this.isOpen = false;
        },
        
        cancel() {
            this.isOpen = false;
        }
    }));

    /**
     * Form validation component
     * Usage: x-data="formValidation()"
     */
    Alpine.data('formValidation', () => ({
        errors: {},
        isSubmitting: false,
        
        setError(field, message) {
            this.errors[field] = message;
        },
        
        clearError(field) {
            delete this.errors[field];
        },
        
        clearAllErrors() {
            this.errors = {};
        },
        
        hasError(field) {
            return !!this.errors[field];
        },
        
        getError(field) {
            return this.errors[field] || '';
        },
        
        setErrorsFromResponse(response) {
            if (response.errors) {
                this.errors = response.errors;
            }
        }
    }));
});

// ============================================
// TomSelect Helpers
// ============================================

/**
 * Initialize TomSelect with AJAX support for ServiceResponse format
 * @param {string} selector - CSS selector for the input
 * @param {object} options - TomSelect options
 * @returns {TomSelect} TomSelect instance
 */
function initTomSelect(selector, options = {}) {
    const element = document.querySelector(selector);
    if (!element) return null;
    
    // Destroy existing instance if present
    if (element.tomselect) {
        element.tomselect.destroy();
    }
    
    const defaultOptions = {
        valueField: 'value',
        labelField: 'text',
        searchField: 'text',
        maxOptions: 50,
        preload: 'focus',
        plugins: ['dropdown_input'],
        
        load: function(query, callback) {
            const url = element.dataset.url;
            if (!url) {
                callback([]);
                return;
            }
            
            const searchUrl = new URL(url, window.location.origin);
            if (query) {
                searchUrl.searchParams.set('term', query);
            }
            
            fetch(searchUrl, {
                headers: {
                    'X-TomSelect': 'true',
                    'Accept': 'application/json'
                }
            })
            .then(response => response.json())
            .then(data => {
                callback(data);
            })
            .catch(error => {
                console.error('TomSelect load error:', error);
                callback([]);
            });
        },
        
        render: {
            option: function(data, escape) {
                return `<div class="option">${escape(data.text)}</div>`;
            },
            item: function(data, escape) {
                return `<div class="item">${escape(data.text)}</div>`;
            },
            no_results: function(data, escape) {
                return `<div class="no-results">No results found</div>`;
            }
        }
    };
    
    // Merge options
    const mergedOptions = { ...defaultOptions, ...options };
    
    // Override load function if custom URL provided in options
    if (options.url) {
        mergedOptions.load = function(query, callback) {
            const searchUrl = new URL(options.url, window.location.origin);
            if (query) {
                searchUrl.searchParams.set('term', query);
            }
            
            // Add any additional params
            if (options.params) {
                Object.entries(options.params).forEach(([key, value]) => {
                    if (value !== null && value !== undefined) {
                        searchUrl.searchParams.set(key, value);
                    }
                });
            }
            
            fetch(searchUrl, {
                headers: {
                    'X-TomSelect': 'true',
                    'Accept': 'application/json'
                }
            })
            .then(response => response.json())
            .then(data => {
                callback(data);
            })
            .catch(error => {
                console.error('TomSelect load error:', error);
                callback([]);
            });
        };
    }
    
    return new TomSelect(element, mergedOptions);
}

/**
 * Initialize TomSelect with static options (no AJAX)
 * @param {string} selector - CSS selector for the input
 * @param {array} items - Array of {value, text} objects
 * @param {object} options - Additional TomSelect options
 * @returns {TomSelect} TomSelect instance
 */
function initTomSelectStatic(selector, items = [], options = {}) {
    const element = document.querySelector(selector);
    if (!element) return null;
    
    // Destroy existing instance if present
    if (element.tomselect) {
        element.tomselect.destroy();
    }
    
    const defaultOptions = {
        valueField: 'value',
        labelField: 'text',
        searchField: 'text',
        options: items,
        plugins: ['dropdown_input']
    };
    
    return new TomSelect(element, { ...defaultOptions, ...options });
}

// ============================================
// Flatpickr Helpers
// ============================================

/**
 * Initialize Flatpickr date picker
 * @param {string} selector - CSS selector for the input
 * @param {object} options - Flatpickr options
 * @returns {flatpickr} Flatpickr instance
 */
function initFlatpickr(selector, options = {}) {
    const element = document.querySelector(selector);
    if (!element) return null;
    
    // Destroy existing instance if present
    if (element._flatpickr) {
        element._flatpickr.destroy();
    }
    
    const defaultOptions = {
        dateFormat: 'd M Y',
        allowInput: true,
        defaultDate: null
    };
    
    const picker = flatpickr(element, { ...defaultOptions, ...options });
    element._flatpickr = picker;
    
    return picker;
}

/**
 * Set date on a Flatpickr instance
 * @param {string} selector - CSS selector for the input
 * @param {string|Date} date - Date to set
 */
function setFlatpickrDate(selector, date) {
    const element = document.querySelector(selector);
    if (element && element._flatpickr) {
        element._flatpickr.setDate(date);
    }
}

// ============================================
// htmx Event Handlers
// ============================================

// Show loading indicator on htmx requests
document.addEventListener('htmx:beforeRequest', (event) => {
    const target = event.detail.elt;
    const indicator = target.querySelector('.htmx-indicator') || 
                      document.getElementById(target.dataset.indicator);
    if (indicator) {
        indicator.classList.remove('d-none');
    }
});

// Hide loading indicator after htmx requests
document.addEventListener('htmx:afterRequest', (event) => {
    const target = event.detail.elt;
    const indicator = target.querySelector('.htmx-indicator') || 
                      document.getElementById(target.dataset.indicator);
    if (indicator) {
        indicator.classList.add('d-none');
    }
});

// Handle htmx errors
document.addEventListener('htmx:responseError', (event) => {
    const response = event.detail.xhr;
    let message = 'An error occurred. Please try again.';
    
    try {
        const data = JSON.parse(response.responseText);
        message = data.message || data.Message || message;
    } catch (e) {
        // Use default message
    }
    
    if (window.Alpine && Alpine.store('toast')) {
        Alpine.store('toast').error(message);
    } else {
        alert(message);
    }
});

// Handle successful htmx responses that return JSON with message
document.addEventListener('htmx:afterSwap', (event) => {
    // Re-initialize TomSelect/Flatpickr on swapped content if needed
    const target = event.detail.target;
    
    // Auto-init TomSelect elements
    target.querySelectorAll('[data-tomselect]').forEach(el => {
        if (!el.tomselect) {
            initTomSelect(`#${el.id}`);
        }
    });
    
    // Auto-init Flatpickr elements
    target.querySelectorAll('[data-flatpickr]').forEach(el => {
        if (!el._flatpickr) {
            initFlatpickr(`#${el.id}`);
        }
    });
});

// ============================================
// Utility Functions
// ============================================

/**
 * Format number with thousand separators
 * @param {number|string} value - Number to format
 * @returns {string} Formatted number string with commas
 */
function formatNumber(value) {
    if (value === null || value === undefined || value === '') return '';
    // Remove any existing formatting
    const num = parseFloat(String(value).replace(/,/g, ''));
    if (isNaN(num)) return value;
    // Format with thousand separators
    return num.toLocaleString('en-US');
}

/**
 * Parse formatted number string to plain number string
 * @param {string} value - Formatted number string with commas
 * @returns {string} Plain number string without commas
 */
function parseNumber(value) {
    if (value === null || value === undefined || value === '') return '';
    // Remove thousand separators and return plain number string
    return String(value).replace(/,/g, '');
}

/**
 * Format number as currency
 * @param {number} value - Number to format
 * @param {string} locale - Locale string (default: 'id-ID')
 * @returns {string} Formatted currency string
 */
function formatCurrency(value, locale = 'id-ID') {
    return new Intl.NumberFormat(locale, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(value || 0);
}

/**
 * Format date for display
 * @param {string|Date} date - Date to format
 * @param {string} _format - Format string (default: 'DD MMM YYYY')
 * @returns {string} Formatted date string
 */
function formatDate(date, _format = 'DD MMM YYYY') {
    if (!date) return '';
    const d = new Date(date);
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    
    const day = String(d.getDate()).padStart(2, '0');
    const month = months[d.getMonth()];
    const year = d.getFullYear();
    
    return `${day} ${month} ${year}`;
}

/**
 * Parse date from display format to ISO
 * @param {string} dateStr - Date string in DD MMM YYYY format
 * @returns {string} ISO date string
 */
function parseDateToISO(dateStr) {
    if (!dateStr) return '';
    const months = {
        'Jan': '01', 'Feb': '02', 'Mar': '03', 'Apr': '04',
        'May': '05', 'Jun': '06', 'Jul': '07', 'Aug': '08',
        'Sep': '09', 'Oct': '10', 'Nov': '11', 'Dec': '12'
    };
    
    const parts = dateStr.split(' ');
    if (parts.length !== 3) return dateStr;
    
    const day = parts[0].padStart(2, '0');
    const month = months[parts[1]] || '01';
    const year = parts[2];
    
    return `${year}-${month}-${day}`;
}

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        initTomSelect,
        initTomSelectStatic,
        initFlatpickr,
        setFlatpickrDate,
        formatNumber,
        parseNumber,
        formatCurrency,
        formatDate,
        parseDateToISO,
        createWorkflowComponent
    };
}

/**
 * Create module-agnostic Workflow Component
 * Reusable Alpine.js component for workflow actions (Submit, Approve, Reject, Recall, Close)
 * Works with CustomerOrders, DeliveryOrders, Prs, etc.
 * 
 * @param {Object} config - Configuration object
 * @param {number} config.recId - Record ID
 * @param {string} config.refId - Reference ID (e.g., CoId, DoId, PrId)
 * @param {string} config.baseUrl - Base URL for the module (e.g., '/CustomerOrders', '/DeliveryOrders')
 * @param {string} [config.redirectUrl] - URL to redirect after successful action (optional)
 * @param {Object} [config.actions] - Custom action URL overrides (optional)
 * @returns {Object} Alpine.js component
 * 
 * @example
 * // For Customer Orders
 * workflow: createWorkflowComponent({
 *     recId: @Model.Header.RecId,
 *     refId: '@Model.Header.CoId',
 *     baseUrl: '/CustomerOrders'
 * })
 * 
 * @example
 * // For Delivery Orders
 * workflow: createWorkflowComponent({
 *     recId: @Model.Header.RecId,
 *     refId: '@Model.Header.DoId',
 *     baseUrl: '/DeliveryOrders'
 * })
 */
function createWorkflowComponent(configOrRecId, coId = null, redirectUrl = null) {
    // Support both old API (recId, coId, redirectUrl) and new API (config object)
    let config;
    if (typeof configOrRecId === 'object' && configOrRecId !== null) {
        config = configOrRecId;
    } else {
        // Legacy API support - convert to config object
        config = {
            recId: configOrRecId,
            refId: coId,
            baseUrl: '/CustomerOrders',
            redirectUrl: redirectUrl
        };
    }
    
    const { recId, refId, baseUrl = '/CustomerOrders', actions, customActionUrls } = config;
    const finalRedirectUrl = config.redirectUrl || `${baseUrl}/Details/${recId}`;
    
    // Default action URLs based on baseUrl
    const defaultActions = {
        'submit': `${baseUrl}/Submit`,
        'approve': `${baseUrl}/Approve`,
        'reject': `${baseUrl}/Reject`,
        'recall': `${baseUrl}/Recall`,
        'close': `${baseUrl}/Close`
    };
    
    // Merge with custom action URLs if provided (not enabled flags)
    const actionUrls = { ...defaultActions, ...customActionUrls };
    
    return {
        modal: {
            show: false,
            action: null,
            notes: '',
            notesError: '',
            isProcessing: false
        },
        
        showModal(action) {
            this.modal.show = true;
            this.modal.action = action;
            this.modal.notes = '';
            this.modal.notesError = '';
        },
        
        closeModal() {
            if (!this.modal.isProcessing) {
                this.modal.show = false;
                this.modal.action = null;
                this.modal.notes = '';
                this.modal.notesError = '';
            }
        },
        
        validateNotes() {
            if ((this.modal.action === 'reject' || this.modal.action === 'approve') && !this.modal.notes.trim()) {
                this.modal.notesError = 'Please enter a reason';
                return false;
            }
            this.modal.notesError = '';
            return true;
        },
        
        async execute() {
            //if (!this.validateNotes()) return;
            
            this.modal.isProcessing = true;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            
            const url = actionUrls[this.modal.action];
            if (!url) {
                console.error('Unknown workflow action:', this.modal.action);
                alert('Unknown action');
                this.modal.isProcessing = false;
                return;
            }
            
            const body = new URLSearchParams();
            body.append('id', recId);
            body.append('RecId', recId);
            // Use generic field names that work across modules
            body.append('CoId', refId); // Legacy support for CO
            body.append('DoId', refId); // Legacy support for DO
            body.append('PrId', refId); // Legacy support for PR
            body.append('RefId', refId); // Generic reference ID
            body.append('Notes', this.modal.notes);
            body.append('__RequestVerificationToken', token);
            
            try {
                const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                    },
                    body: body.toString()
                });
                
                if (response.redirected) {
                    window.location.href = response.url;
                } else if (response.ok) {
                    window.location.href = finalRedirectUrl;
                } else {
                    const text = await response.text();
                    throw new Error(text || 'Operation failed');
                }
            } catch (error) {
                console.error('Workflow action error:', error);
                alert(error.message || 'An error occurred');
            } finally {
                this.modal.isProcessing = false;
            }
        }
    };
}
