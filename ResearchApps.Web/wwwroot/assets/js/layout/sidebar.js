/**
 * Sidebar Manager
 * Manages sidebar state, size, theme, and interactions
 */

import { StorageService } from '../core/storage.js';
import { eventBus } from '../core/event-bus.js';
import APP_CONFIG from '../core/config.js';

export class SidebarManager {
    constructor() {
        this.currentSize = StorageService.get('data-sidebar-size', APP_CONFIG.sidebar.defaultSize);
        this.currentTheme = StorageService.get('data-sidebar', APP_CONFIG.sidebar.defaultTheme);
    }
    
    /**
     * Initialize sidebar
     */
    init() {
        this.applyStoredSettings();
        this.initHoverListener();
        eventBus.emit('sidebar:initialized');
    }
    
    /**
     * Apply stored sidebar settings
     */
    applyStoredSettings() {
        document.documentElement.setAttribute('data-sidebar-size', this.currentSize);
        document.documentElement.setAttribute('data-sidebar', this.currentTheme);
    }
    
    /**
     * Set sidebar size
     * @param {string} size - Sidebar size (lg, sm, md, sm-hover)
     */
    setSize(size) {
        if (!APP_CONFIG.sidebar.sizes.includes(size)) {
            console.warn(`Invalid sidebar size: ${size}`);
            return;
        }
        
        this.currentSize = size;
        document.documentElement.setAttribute('data-sidebar-size', size);
        StorageService.set('data-sidebar-size', size);
        eventBus.emit('sidebar:sizeChanged', { size });
    }
    
    /**
     * Get current sidebar size
     * @returns {string} Current sidebar size
     */
    getSize() {
        return this.currentSize;
    }
    
    /**
     * Set sidebar theme
     * @param {string} theme - Sidebar theme
     */
    setTheme(theme) {
        if (!APP_CONFIG.sidebar.themes.includes(theme)) {
            console.warn(`Invalid sidebar theme: ${theme}`);
            return;
        }
        
        this.currentTheme = theme;
        document.documentElement.setAttribute('data-sidebar', theme);
        StorageService.set('data-sidebar', theme);
        eventBus.emit('sidebar:themeChanged', { theme });
    }
    
    /**
     * Get current sidebar theme
     * @returns {string} Current sidebar theme
     */
    getTheme() {
        return this.currentTheme;
    }
    
    /**
     * Set sidebar image
     * @param {string} image - Image identifier
     */
    setImage(image) {
        if (!APP_CONFIG.sidebar.images.includes(image)) {
            console.warn(`Invalid sidebar image: ${image}`);
            return;
        }
        
        document.documentElement.setAttribute('data-sidebar-image', image);
        StorageService.set('data-sidebar-image', image);
        eventBus.emit('sidebar:imageChanged', { image });
    }
    
    /**
     * Toggle sidebar visibility
     */
    toggleVisibility() {
        const currentVisibility = document.documentElement.getAttribute('data-sidebar-visibility');
        const newVisibility = currentVisibility === 'hidden' ? 'show' : 'hidden';
        
        document.documentElement.setAttribute('data-sidebar-visibility', newVisibility);
        StorageService.set('data-sidebar-visibility', newVisibility);
        eventBus.emit('sidebar:visibilityChanged', { visibility: newVisibility });
    }
    
    /**
     * Initialize hover listener for small hover menu
     */
    initHoverListener() {
        const hoverIcon = document.getElementById("vertical-hover");
        if (!hoverIcon) return;
        
        hoverIcon.addEventListener("click", () => {
            const currentSize = document.documentElement.getAttribute("data-sidebar-size");
            
            if (currentSize === "sm-hover") {
                this.setSize("sm-hover-active");
            } else if (currentSize === "sm-hover-active") {
                this.setSize("sm-hover");
            } else {
                this.setSize("sm-hover");
            }
        });
    }
    
    /**
     * Toggle sidebar for mobile
     */
    toggleMobile() {
        document.body.classList.toggle("vertical-sidebar-enable");
        eventBus.emit('sidebar:mobileToggled');
    }
    
    /**
     * Show sidebar
     */
    show() {
        document.body.classList.add("vertical-sidebar-enable");
    }
    
    /**
     * Hide sidebar
     */
    hide() {
        document.body.classList.remove("vertical-sidebar-enable");
    }
    
    /**
     * Check if sidebar is visible on mobile
     * @returns {boolean} True if visible
     */
    isMobileVisible() {
        return document.body.classList.contains("vertical-sidebar-enable");
    }
}

export default SidebarManager;
