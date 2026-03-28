/**
 * Warehouse Edit Page Component
 * @param {number|null} initialStatusId - Pre-selected Status ID
 * @returns {Object} Alpine.js component
 */
function warehouseEdit(initialStatusId = null) {
    return {
        statusSelect: null,
        initialStatusId: initialStatusId,

        init() {
            this.initStatusSelect();
        },

        initStatusSelect() {
            const self = this;
            try {
                this.statusSelect = new TomSelect('#StatusId', {
                    valueField: 'value',
                    labelField: 'text',
                    searchField: ['text'],
                    load: async (query, callback) => {
                        try {
                            const response = await fetch('/api/Status/cbo', {
                                headers: { 'X-TomSelect': 'true' }
                            });
                            if (!response.ok) throw new Error(`HTTP ${response.status}`);
                            const data = await response.json();
                            callback(data);
                            if (self.initialStatusId !== null && self.statusSelect) {
                                setTimeout(() => self.statusSelect.setValue(self.initialStatusId.toString()), 100);
                            }
                        } catch (error) {
                            console.error('[Warehouse Edit] Error loading Status:', error);
                            callback();
                        }
                    },
                    placeholder: '-- Select Status --',
                    allowEmptyOption: false,
                    create: false,
                    onInitialize: function() { this.load(''); }
                });
            } catch (error) {
                console.error('[Warehouse Edit] Error initializing Status TomSelect:', error);
            }
        }
    };
}

// Make available globally
window.warehouseEdit = warehouseEdit;
