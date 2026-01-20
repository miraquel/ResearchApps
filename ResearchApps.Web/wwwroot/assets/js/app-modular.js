/**
 * Modular Application Entry Point
 * This is the new modular version of app.js
 * Import and initialize all application modules
 */

// Core utilities
import { StorageService } from './core/storage.js';
import { DOMUtils } from './core/dom-utils.js';
import { eventBus } from './core/event-bus.js';
import APP_CONFIG from './core/config.js';

// Layout modules
import MenuManager from './layout/menu.js';
import SidebarManager from './layout/sidebar.js';
import TopbarManager from './layout/topbar.js';
import LayoutManager from './layout/layout-manager.js';

// Component modules
import SearchComponent from './components/search.js';
import ComponentsManager from './components/ui-components.js';

// Plugin manager
import PluginManager from './plugins/plugin-manager.js';

/**
 * Main Application Class
 */
class App {
    constructor() {
        this.modules = {};
        this.initialized = false;
    }
    
    /**
     * Initialize the application
     */
    async init() {
        if (this.initialized) return;
        
        console.log('🚀 Initializing modular application...');
        
        // Initialize core modules
        this.initializeDefaultAttributes();
        
        // Create module instances
        this.modules.menu = new MenuManager();
        this.modules.sidebar = new SidebarManager();
        this.modules.topbar = new TopbarManager();
        this.modules.layout = new LayoutManager(this.modules.menu, this.modules.sidebar);
        this.modules.search = new SearchComponent();
        this.modules.components = new ComponentsManager();
        this.modules.plugins = new PluginManager();
        
        // Set up event listeners
        this.setupEventListeners();
        
        // Wait for DOM to be ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.onDOMReady());
        } else {
            this.onDOMReady();
        }
        
        this.initialized = true;
    }
    
    /**
     * Initialize or restore default attributes
     */
    initializeDefaultAttributes() {
        if (!StorageService.getDefaultAttributes()) {
            StorageService.saveDefaultAttributes();
        } else if (!StorageService.checkDefaultAttributesMatch()) {
            // Attributes changed, reset
            sessionStorage.clear();
            window.location.reload();
            return;
        }
        
        // Apply stored layout attributes
        const layoutAttributes = StorageService.getLayoutAttributes();
        Object.keys(layoutAttributes).forEach(key => {
            if (layoutAttributes[key]) {
                document.documentElement.setAttribute(key, layoutAttributes[key]);
            }
        });
    }
    
    /**
     * Handle DOM ready event
     */
    onDOMReady() {
        console.log('📄 DOM Ready - Initializing modules...');
        
        // Initialize modules in order
        this.modules.plugins.init();
        this.modules.menu.init();
        this.modules.sidebar.init();
        this.modules.topbar.init();
        this.modules.layout.init();
        this.modules.search.init();
        this.modules.components.init();
        
        // Scroll to active menu item
        this.modules.menu.scrollToActiveItem(APP_CONFIG.scrollOffset);
        
        // Initialize preloader
        this.initPreloader();
        
        // Initialize customizer
        this.initCustomizer();
        
        console.log('✅ Application initialized successfully');
        eventBus.emit('app:ready');
    }
    
    /**
     * Initialize theme customizer
     */
    initCustomizer() {
        // Layout radio buttons
        const layoutRadios = document.querySelectorAll('input[name="data-layout"]');
        layoutRadios.forEach(radio => {
            // Set initial checked state based on current layout
            if (radio.value === this.modules.layout.currentLayout) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    this.modules.layout.setLayout(e.target.value, true);
                }
            });
        });
        
        // Color scheme (theme) radio buttons
        const themeRadios = document.querySelectorAll('input[name="data-bs-theme"]');
        themeRadios.forEach(radio => {
            const currentTheme = document.documentElement.getAttribute('data-bs-theme') || 'light';
            if (radio.value === currentTheme) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    document.documentElement.setAttribute('data-bs-theme', e.target.value);
                    StorageService.set('data-bs-theme', e.target.value);
                }
            });
        });
        
        // Topbar color radio buttons
        const topbarRadios = document.querySelectorAll('input[name="data-topbar"]');
        topbarRadios.forEach(radio => {
            const currentTopbar = document.documentElement.getAttribute('data-topbar') || 'light';
            if (radio.value === currentTopbar) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    this.modules.topbar.setTheme(e.target.value);
                }
            });
        });
        
        // Sidebar size radio buttons
        const sidebarSizeRadios = document.querySelectorAll('input[name="data-sidebar-size"]');
        sidebarSizeRadios.forEach(radio => {
            const currentSize = document.documentElement.getAttribute('data-sidebar-size') || 'lg';
            if (radio.value === currentSize) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    this.modules.sidebar.setSize(e.target.value);
                }
            });
        });
        
        // Sidebar color radio buttons
        const sidebarColorRadios = document.querySelectorAll('input[name="data-sidebar"]');
        sidebarColorRadios.forEach(radio => {
            const currentColor = document.documentElement.getAttribute('data-sidebar') || 'dark';
            if (radio.value === currentColor) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    this.modules.sidebar.setTheme(e.target.value);
                }
            });
        });
        
        // Layout width radio buttons
        const layoutWidthRadios = document.querySelectorAll('input[name="data-layout-width"]');
        layoutWidthRadios.forEach(radio => {
            const currentWidth = document.documentElement.getAttribute('data-layout-width') || 'fluid';
            if (radio.value === currentWidth) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    this.modules.layout.setLayoutWidth(e.target.value);
                }
            });
        });
        
        // Layout position radio buttons
        const layoutPositionRadios = document.querySelectorAll('input[name="data-layout-position"]');
        layoutPositionRadios.forEach(radio => {
            const currentPosition = document.documentElement.getAttribute('data-layout-position') || 'fixed';
            if (radio.value === currentPosition) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    this.modules.layout.setLayoutPosition(e.target.value);
                }
            });
        });
        
        // Sidebar image radio buttons
        const sidebarImageRadios = document.querySelectorAll('input[name="data-sidebar-image"]');
        sidebarImageRadios.forEach(radio => {
            const currentImage = document.documentElement.getAttribute('data-sidebar-image') || 'none';
            if (radio.value === currentImage) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    document.documentElement.setAttribute('data-sidebar-image', e.target.value);
                    StorageService.set('data-sidebar-image', e.target.value);
                }
            });
        });
        
        // Preloader radio buttons
        const preloaderRadios = document.querySelectorAll('input[name="data-preloader"]');
        preloaderRadios.forEach(radio => {
            const currentPreloader = document.documentElement.getAttribute('data-preloader') || 'disable';
            if (radio.value === currentPreloader) {
                radio.checked = true;
            }
            
            radio.addEventListener('change', (e) => {
                if (e.target.checked) {
                    document.documentElement.setAttribute('data-preloader', e.target.value);
                    StorageService.set('data-preloader', e.target.value);
                }
            });
        });
    }
    
    /**
     * Setup global event listeners
     */
    setupEventListeners() {
        // Listen for layout changes
        eventBus.on('layout:changed', (data) => {
            console.log('Layout changed to:', data.layout);
        });
        
        // Listen for theme changes
        eventBus.on('app:themeChanged', (data) => {
            console.log('Theme changed to:', data.theme);
        });
        
        // Handle scroll to top button
        this.initScrollToTop();
        
        // Handle preloader on window load
        window.addEventListener('load', () => {
            this.handlePreloaderOnLoad();
        });
    }
    
    /**
     * Initialize preloader
     */
    initPreloader() {
        const preloaderSetting = StorageService.get('data-preloader');
        
        if (preloaderSetting === 'enable') {
            document.documentElement.setAttribute('data-preloader', 'enable');
        } else {
            document.documentElement.setAttribute('data-preloader', 'disable');
        }
    }
    
    /**
     * Handle preloader on window load
     */
    handlePreloaderOnLoad() {
        const preloader = document.getElementById('preloader');
        if (!preloader) return;
        
        const preloaderSetting = StorageService.get('data-preloader');
        
        if (preloaderSetting === 'enable') {
            setTimeout(() => {
                DOMUtils.fadeOut(preloader, 1000);
            }, APP_CONFIG.preloader.fadeDelay);
        }
    }
    
    /**
     * Initialize scroll to top button
     */
    initScrollToTop() {
        const scrollToTopBtn = document.getElementById('back-to-top');
        if (!scrollToTopBtn) return;
        
        // Show/hide button based on scroll position
        window.addEventListener('scroll', () => {
            const scrollTop = document.body.scrollTop || document.documentElement.scrollTop;
            scrollToTopBtn.style.display = scrollTop > 100 ? 'block' : 'none';
        });
        
        // Scroll to top on click
        scrollToTopBtn.addEventListener('click', () => {
            document.body.scrollTop = 0;
            document.documentElement.scrollTop = 0;
        });
    }
    
    /**
     * Get module instance
     * @param {string} moduleName - Module name
     * @returns {Object} Module instance
     */
    getModule(moduleName) {
        return this.modules[moduleName];
    }
    
    /**
     * Reset application to defaults
     */
    reset() {
        sessionStorage.clear();
        window.location.reload();
    }
}

// Create and initialize application
const app = new App();
app.init();

// Expose app instance globally for debugging and external access
window.App = app;
window.StorageService = StorageService;
window.DOMUtils = DOMUtils;
window.eventBus = eventBus;

// Export for module usage
export default app;
