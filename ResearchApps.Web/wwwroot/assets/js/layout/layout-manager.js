/**
 * Layout Manager
 * Orchestrates layout changes, manages layout state and settings
 */

import { StorageService } from '../core/storage.js';
import { DOMUtils } from '../core/dom-utils.js';
import { eventBus } from '../core/event-bus.js';
import APP_CONFIG from '../core/config.js';
import MenuManager from './menu.js';
import SidebarManager from './sidebar.js';

export class LayoutManager {
    constructor(menuManager, sidebarManager) {
        this.menuManager = menuManager;
        this.sidebarManager = sidebarManager;
        this.currentLayout = StorageService.get('data-layout', APP_CONFIG.layout.defaultLayout);
    }
    
    /**
     * Initialize layout manager
     */
    init() {
        this.applyStoredLayout();
        this.initHamburgerToggle();
        this.initWindowResize();
        this.initVerticalOverlay();
        eventBus.emit('layout:initialized');
    }
    
    /**
     * Apply stored layout settings
     */
    applyStoredLayout() {
        this.setLayout(this.currentLayout, false);
    }
    
    /**
     * Set layout type
     * @param {string} layout - Layout type (vertical, horizontal, twocolumn, semibox)
     * @param {boolean} saveToStorage - Whether to save to storage
     */
    setLayout(layout, saveToStorage = true) {
        if (!APP_CONFIG.layout.availableLayouts.includes(layout)) {
            console.warn(`Invalid layout: ${layout}`);
            return;
        }
        
        this.currentLayout = layout;
        document.documentElement.setAttribute('data-layout', layout);
        
        if (saveToStorage) {
            StorageService.set('data-layout', layout);
        }
        
        this.applyLayoutSpecificSettings(layout);
        eventBus.emit('layout:changed', { layout });
    }
    
    /**
     * Apply layout-specific settings
     * @param {string} layout - Layout type
     */
    applyLayoutSpecificSettings(layout) {
        switch (layout) {
            case 'vertical':
                this.setupVerticalLayout();
                break;
            case 'horizontal':
                this.setupHorizontalLayout();
                break;
            case 'twocolumn':
                this.setupTwoColumnLayout();
                break;
            case 'semibox':
                this.setupSemiboxLayout();
                break;
        }
    }
    
    /**
     * Setup vertical layout
     */
    setupVerticalLayout() {
        document.getElementById("two-column-menu").innerHTML = "";
        this.menuManager.restoreNavbarMenu();
        
        document.getElementById("scrollbar").setAttribute("data-simplebar", "");
        document.getElementById("navbar-nav").setAttribute("data-simplebar", "");
        document.getElementById("scrollbar").classList.add("h-100");
        
        this.menuManager.initCollapseMenu();
        this.menuManager.initActiveMenu();
    }
    
    /**
     * Setup horizontal layout
     */
    setupHorizontalLayout() {
        this.updateHorizontalMenus();
        this.menuManager.initActiveMenu();
    }
    
    /**
     * Setup two-column layout
     */
    setupTwoColumnLayout() {
        document.getElementById("scrollbar").removeAttribute("data-simplebar");
        document.getElementById("scrollbar").classList.remove("h-100");
        
        this.generateTwoColumnMenu();
        this.initTwoColumnActiveMenu();
        this.menuManager.initCollapseMenu();
    }
    
    /**
     * Setup semibox layout
     */
    setupSemiboxLayout() {
        document.getElementById("two-column-menu").innerHTML = "";
        this.menuManager.restoreNavbarMenu();
        
        this.menuManager.initCollapseMenu();
        this.menuManager.initActiveMenu();
    }
    
    /**
     * Generate two-column menu structure
     */
    generateTwoColumnMenu() {
        // Implementation would mirror the twoColumnMenuGenerate function from app.js
        // This is a simplified version - full implementation would be more complex
        const navbarMenuHTML = this.menuManager.getNavbarMenuHTML();
        if (!navbarMenuHTML) return;
        
        eventBus.emit('layout:twoColumnMenuGenerated');
    }
    
    /**
     * Initialize two-column active menu
     */
    initTwoColumnActiveMenu() {
        const currentPath = location.pathname === "/" ? "/" : "/" + location.pathname.substring(1);
        if (!currentPath) return;
        
        const twoColumnMenu = document.getElementById("two-column-menu");
        if (twoColumnMenu) {
            const link = twoColumnMenu.querySelector(`[href="${currentPath}"]`);
            if (link) {
                link.classList.add("active");
            }
        }
        
        this.menuManager.initActiveMenu();
    }
    
    /**
     * Update horizontal menus
     */
    updateHorizontalMenus() {
        document.getElementById("two-column-menu").innerHTML = "";
        this.menuManager.restoreNavbarMenu();
        
        document.getElementById("scrollbar").removeAttribute("data-simplebar");
        document.getElementById("navbar-nav").removeAttribute("data-simplebar");
        document.getElementById("scrollbar").classList.remove("h-100");
        
        const splitMenu = APP_CONFIG.layout.horizontalMenuSplit;
        const extraMenuName = "More";
        const menuData = document.querySelectorAll("ul.navbar-nav > li.nav-item");
        let newMenus = "";
        let splitItem = null;
        
        Array.from(menuData).forEach((item, index) => {
            if (index + 1 === splitMenu) {
                splitItem = item;
            }
            if (index + 1 > splitMenu) {
                newMenus += item.outerHTML;
                item.remove();
            }
            
            if (index + 1 === menuData.length && splitItem) {
                splitItem.insertAdjacentHTML(
                    "afterend",
                    `<li class="nav-item">
                        <a class="nav-link" href="#sidebarMore" data-bs-toggle="collapse" role="button" aria-expanded="false" aria-controls="sidebarMore">
                            <i class="ri-briefcase-2-line"></i> <span data-key="t-more">${extraMenuName}</span>
                        </a>
                        <div class="collapse menu-dropdown" id="sidebarMore">
                            <ul class="nav nav-sm flex-column">${newMenus}</ul>
                        </div>
                    </li>`
                );
            }
        });
    }
    
    /**
     * Initialize hamburger menu toggle
     */
    initHamburgerToggle() {
        const hamburgerIcon = document.getElementById("topnav-hamburger-icon");
        if (!hamburgerIcon) return;
        
        hamburgerIcon.addEventListener("click", () => {
            this.toggleHamburgerMenu();
        });
    }
    
    /**
     * Toggle hamburger menu
     */
    toggleHamburgerMenu() {
        const windowSize = DOMUtils.getWindowSize();
        
        if (windowSize.width > 767) {
            document.querySelector(".hamburger-icon")?.classList.toggle("open");
        }
        
        const layout = document.documentElement.getAttribute("data-layout");
        
        switch (layout) {
            case "horizontal":
                document.body.classList.toggle("menu");
                break;
                
            case "vertical":
                this.handleVerticalToggle(windowSize);
                break;
                
            case "semibox":
                this.handleSemiboxToggle(windowSize);
                break;
                
            case "twocolumn":
                document.body.classList.toggle("twocolumn-panel");
                break;
        }
    }
    
    /**
     * Handle vertical layout toggle
     * @param {Object} windowSize - Window size object
     */
    handleVerticalToggle(windowSize) {
        if (windowSize.isTablet) {
            document.body.classList.remove("vertical-sidebar-enable");
            const currentSize = document.documentElement.getAttribute("data-sidebar-size");
            const newSize = currentSize === "sm" ? "" : "sm";
            document.documentElement.setAttribute("data-sidebar-size", newSize);
        } else if (windowSize.isDesktop) {
            document.body.classList.remove("vertical-sidebar-enable");
            const currentSize = document.documentElement.getAttribute("data-sidebar-size");
            const newSize = currentSize === "lg" ? "sm" : "lg";
            document.documentElement.setAttribute("data-sidebar-size", newSize);
        } else if (windowSize.isMobile) {
            document.body.classList.add("vertical-sidebar-enable");
            document.documentElement.setAttribute("data-sidebar-size", "lg");
        }
    }
    
    /**
     * Handle semibox layout toggle
     * @param {Object} windowSize - Window size object
     */
    handleSemiboxToggle(windowSize) {
        if (windowSize.width > 767) {
            const visibility = document.documentElement.getAttribute('data-sidebar-visibility');
            if (visibility === "show") {
                const currentSize = document.documentElement.getAttribute("data-sidebar-size");
                const newSize = currentSize === "lg" ? "sm" : "lg";
                document.documentElement.setAttribute("data-sidebar-size", newSize);
            } else {
                const showBtn = document.getElementById("sidebar-visibility-show");
                if (showBtn) showBtn.click();
            }
        } else {
            document.body.classList.add("vertical-sidebar-enable");
            document.documentElement.setAttribute("data-sidebar-size", "lg");
        }
    }
    
    /**
     * Initialize window resize handler
     */
    initWindowResize() {
        let resizeTimeout;
        
        window.addEventListener("resize", () => {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(() => {
                this.handleWindowResize();
            }, 100);
        });
    }
    
    /**
     * Handle window resize
     */
    handleWindowResize() {
        const windowSize = DOMUtils.getWindowSize();
        const layout = StorageService.get("data-layout");
        
        if (windowSize.isMobile) {
            document.body.classList.remove("vertical-sidebar-enable");
            document.body.classList.add("twocolumn-panel");
            
            if (layout === "twocolumn") {
                document.documentElement.setAttribute("data-layout", "vertical");
                this.setupVerticalLayout();
            }
            
            if (layout !== "horizontal") {
                document.documentElement.setAttribute("data-sidebar-size", "lg");
            }
        } else if (windowSize.isTablet) {
            document.body.classList.remove("twocolumn-panel");
            
            if (layout === "vertical" || layout === "semibox") {
                document.documentElement.setAttribute("data-sidebar-size", "sm");
            }
        } else if (windowSize.isDesktop) {
            document.body.classList.remove("twocolumn-panel");
            
            if (layout === "vertical" || layout === "semibox") {
                const storedSize = StorageService.get("data-sidebar-size");
                document.documentElement.setAttribute("data-sidebar-size", storedSize);
            }
        }
        
        eventBus.emit('layout:resized', windowSize);
    }
    
    /**
     * Initialize vertical overlay click handler
     */
    initVerticalOverlay() {
        const verticalOverlays = document.getElementsByClassName("vertical-overlay");
        if (!verticalOverlays.length) return;
        
        Array.from(verticalOverlays).forEach(overlay => {
            overlay.addEventListener("click", () => {
                document.body.classList.remove("vertical-sidebar-enable");
                
                const layout = StorageService.get("data-layout");
                if (layout === "twocolumn") {
                    document.body.classList.add("twocolumn-panel");
                } else {
                    const storedSize = StorageService.get("data-sidebar-size");
                    document.documentElement.setAttribute("data-sidebar-size", storedSize);
                }
            });
        });
    }
    
    /**
     * Get current layout
     * @returns {string} Current layout
     */
    getLayout() {
        return this.currentLayout;
    }
    
    /**
     * Set layout width
     * @param {string} width - Layout width (fluid, boxed)
     */
    setLayoutWidth(width) {
        document.documentElement.setAttribute('data-layout-width', width);
        StorageService.set('data-layout-width', width);
        eventBus.emit('layout:widthChanged', { width });
    }
    
    /**
     * Set layout position
     * @param {string} position - Layout position (fixed, scrollable)
     */
    setLayoutPosition(position) {
        document.documentElement.setAttribute('data-layout-position', position);
        StorageService.set('data-layout-position', position);
        eventBus.emit('layout:positionChanged', { position });
    }
}

export default LayoutManager;
