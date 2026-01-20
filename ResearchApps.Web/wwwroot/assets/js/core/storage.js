/**
 * Storage Service
 * Centralized storage management for session and local storage
 */

export class StorageService {
    /**
     * List of layout-related keys that should be stored in localStorage for persistence
     */
    static LAYOUT_KEYS = [
        'data-layout',
        'data-sidebar-size',
        'data-bs-theme',
        'data-layout-width',
        'data-sidebar',
        'data-sidebar-image',
        'data-layout-direction',
        'data-layout-position',
        'data-layout-style',
        'data-topbar',
        'data-preloader',
        'data-body-image',
        'data-sidebar-visibility'
    ];
    
    /**
     * Get item from storage (localStorage for layout keys, sessionStorage for others)
     * @param {string} key - Storage key
     * @param {*} defaultValue - Default value if key doesn't exist
     * @returns {string|*} Stored value or default
     */
    static get(key, defaultValue = null) {
        // Use localStorage for layout-related keys to persist across pages
        if (this.LAYOUT_KEYS.includes(key)) {
            const value = localStorage.getItem(key);
            return value !== null ? value : defaultValue;
        }
        
        // Use sessionStorage for other keys
        const value = sessionStorage.getItem(key);
        return value !== null ? value : defaultValue;
    }
    
    /**
     * Set item in storage (localStorage for layout keys, sessionStorage for others)
     * @param {string} key - Storage key
     * @param {string} value - Value to store
     */
    static set(key, value) {
        // Use localStorage for layout-related keys to persist across pages
        if (this.LAYOUT_KEYS.includes(key)) {
            localStorage.setItem(key, value);
        } else {
            sessionStorage.setItem(key, value);
        }
    }
    
    /**
     * Remove item from sessionStorage
     * @param {string} key - Storage key
     */
    static remove(key) {
        sessionStorage.removeItem(key);
    }
    
    /**
     * Clear all sessionStorage
     */
    static clear() {
        sessionStorage.clear();
    }
    
    /**
     * Get all layout attributes from storage
     * @returns {Object} Layout attributes object
     */
    static getLayoutAttributes() {
        return {
            'data-layout': this.get('data-layout'),
            'data-sidebar-size': this.get('data-sidebar-size'),
            'data-bs-theme': this.get('data-bs-theme'),
            'data-layout-width': this.get('data-layout-width'),
            'data-sidebar': this.get('data-sidebar'),
            'data-sidebar-image': this.get('data-sidebar-image'),
            'data-layout-direction': this.get('data-layout-direction'),
            'data-layout-position': this.get('data-layout-position'),
            'data-layout-style': this.get('data-layout-style'),
            'data-topbar': this.get('data-topbar'),
            'data-preloader': this.get('data-preloader'),
            'data-body-image': this.get('data-body-image'),
            'data-sidebar-visibility': this.get('data-sidebar-visibility')
        };
    }
    
    /**
     * Set layout attributes in storage
     * @param {Object} attributes - Attributes object
     */
    static setLayoutAttributes(attributes) {
        Object.keys(attributes).forEach(key => {
            if (attributes[key] !== null && attributes[key] !== undefined) {
                this.set(key, attributes[key]);
            }
        });
    }
    
    /**
     * Get default attributes from DOM
     * @returns {Object} Default attributes object
     */
    static getDefaultAttributes() {
        const stored = this.get('defaultAttribute');
        if (stored) {
            return JSON.parse(stored);
        }
        return null;
    }
    
    /**
     * Save default attributes from current DOM state
     */
    static saveDefaultAttributes() {
        const attributesValue = document.documentElement.attributes;
        const isLayoutAttributes = {};
        
        Array.from(attributesValue).forEach(attr => {
            if (attr && attr.nodeName && attr.nodeName !== "undefined") {
                isLayoutAttributes[attr.nodeName] = attr.nodeValue;
                // Only save to session storage if not already in localStorage
                if (!this.LAYOUT_KEYS.includes(attr.nodeName)) {
                    sessionStorage.setItem(attr.nodeName, attr.nodeValue);
                } else if (!localStorage.getItem(attr.nodeName)) {
                    // Save to localStorage only if not already set (first time)
                    localStorage.setItem(attr.nodeName, attr.nodeValue);
                }
            }
        });
        
        sessionStorage.setItem('defaultAttribute', JSON.stringify(isLayoutAttributes));
        return isLayoutAttributes;
    }
    
    /**
     * Check if default attributes match current DOM attributes
     * @returns {boolean} True if attributes match
     */
    static checkDefaultAttributesMatch() {
        const storedDefault = this.get('defaultAttribute');
        if (!storedDefault) return false;
        
        const attributesValue = document.documentElement.attributes;
        const currentAttributes = {};
        
        Array.from(attributesValue).forEach(attr => {
            if (attr && attr.nodeName && attr.nodeName !== "undefined") {
                currentAttributes[attr.nodeName] = attr.nodeValue;
            }
        });
        
        return storedDefault === JSON.stringify(currentAttributes);
    }
    
    /**
     * Get from localStorage
     * @param {string} key - Storage key
     * @param {*} defaultValue - Default value if key doesn't exist
     * @returns {string|*} Stored value or default
     */
    static getLocal(key, defaultValue = null) {
        const value = localStorage.getItem(key);
        return value !== null ? value : defaultValue;
    }
    
    /**
     * Set in localStorage
     * @param {string} key - Storage key
     * @param {string} value - Value to store
     */
    static setLocal(key, value) {
        localStorage.setItem(key, value);
    }
    
    /**
     * Remove from localStorage
     * @param {string} key - Storage key
     */
    static removeLocal(key) {
        localStorage.removeItem(key);
    }
}

export default StorageService;
