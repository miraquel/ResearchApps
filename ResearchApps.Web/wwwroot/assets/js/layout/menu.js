/**
 * Menu Manager
 * Handles menu navigation, collapse/expand, and active state management
 */

import { DOMUtils } from '../core/dom-utils.js';
import { eventBus } from '../core/event-bus.js';

export class MenuManager {
    constructor() {
        this.navbarMenuHTML = null;
        this.initialized = false;
    }
    
    /**
     * Initialize menu manager
     */
    init() {
        if (this.initialized) return;
        
        const navbarMenu = document.querySelector(".navbar-menu");
        if (navbarMenu) {
            this.navbarMenuHTML = navbarMenu.innerHTML;
        }
        
        this.initCollapseMenu();
        this.initActiveMenu();
        
        this.initialized = true;
        eventBus.emit('menu:initialized');
    }
    
    /**
     * Initialize collapsible menu functionality
     */
    initCollapseMenu() {
        const collapses = document.querySelectorAll(".navbar-nav .collapse");
        if (!collapses.length) return;
        
        Array.from(collapses).forEach(collapse => {
            const collapseInstance = new bootstrap.Collapse(collapse, {
                toggle: false,
            });
            
            // Handle show event
            collapse.addEventListener("show.bs.collapse", (e) => {
                e.stopPropagation();
                this.handleCollapseShow(collapse, collapseInstance);
            });
            
            // Handle hide event
            collapse.addEventListener("hide.bs.collapse", (e) => {
                e.stopPropagation();
                this.handleCollapseHide(collapse);
            });
        });
    }
    
    /**
     * Handle collapse show event
     * @param {HTMLElement} collapse - Collapse element
     * @param {bootstrap.Collapse} collapseInstance - Bootstrap collapse instance
     */
    handleCollapseShow(collapse, collapseInstance) {
        const closestCollapse = collapse.parentElement.closest(".collapse");
        
        if (closestCollapse) {
            // Hide sibling collapses
            const siblingCollapses = closestCollapse.querySelectorAll(".collapse");
            Array.from(siblingCollapses).forEach(siblingCollapse => {
                const siblingInstance = bootstrap.Collapse.getInstance(siblingCollapse);
                if (siblingInstance && siblingInstance !== collapseInstance) {
                    siblingInstance.hide();
                }
            });
        } else {
            // Hide all sibling collapses at root level
            const siblings = DOMUtils.getSiblings(collapse.parentElement);
            Array.from(siblings).forEach(item => {
                if (item.childNodes.length > 2) {
                    item.firstElementChild.setAttribute("aria-expanded", "false");
                }
                
                const ids = item.querySelectorAll("*[id]");
                Array.from(ids).forEach(item1 => {
                    item1.classList.remove("show");
                    if (item1.childNodes.length > 2) {
                        const val = item1.querySelectorAll("ul li a");
                        Array.from(val).forEach(subitem => {
                            if (subitem.hasAttribute("aria-expanded")) {
                                subitem.setAttribute("aria-expanded", "false");
                            }
                        });
                    }
                });
            });
        }
    }
    
    /**
     * Handle collapse hide event
     * @param {HTMLElement} collapse - Collapse element
     */
    handleCollapseHide(collapse) {
        const childCollapses = collapse.querySelectorAll(".collapse");
        Array.from(childCollapses).forEach(childCollapse => {
            const childInstance = bootstrap.Collapse.getInstance(childCollapse);
            if (childInstance) {
                childInstance.hide();
            }
        });
    }
    
    /**
     * Initialize active menu item based on current path
     */
    initActiveMenu() {
        let currentPath = location.pathname === "/" ? "/" : "/" + location.pathname.substring(1);
        
        if (!currentPath) return;
        
        const navbarNav = document.getElementById("navbar-nav");
        if (!navbarNav) return;
        
        const activeLink = navbarNav.querySelector(`[href="${currentPath}"]`);
        
        if (activeLink) {
            this.setActiveMenuItem(activeLink);
        }
    }
    
    /**
     * Set active menu item and expand parent collapses
     * @param {HTMLElement} link - Active link element
     */
    setActiveMenuItem(link) {
        link.classList.add("active");
        
        let parentCollapseDiv = link.closest('.collapse.menu-dropdown');
        
        while (parentCollapseDiv) {
            parentCollapseDiv.classList.add("show");
            
            const parentLink = parentCollapseDiv.parentElement.children[0];
            if (parentLink) {
                parentLink.classList.add("active");
                parentLink.setAttribute("aria-expanded", "true");
            }
            
            // Move to next parent collapse
            parentCollapseDiv = parentCollapseDiv.parentElement.closest('.collapse.menu-dropdown');
        }
    }
    
    /**
     * Scroll to active menu item
     * @param {number} offset - Scroll offset threshold
     */
    scrollToActiveItem(offset = 300) {
        setTimeout(() => {
            const sidebarMenu = document.getElementById("navbar-nav");
            if (!sidebarMenu) return;
            
            const activeMenu = sidebarMenu.querySelector(".nav-item .active");
            if (!activeMenu) return;
            
            const itemOffset = activeMenu.offsetTop;
            
            if (itemOffset > offset) {
                const verticalMenu = document.getElementsByClassName("app-menu")[0];
                if (verticalMenu && verticalMenu.querySelector(".simplebar-content-wrapper")) {
                    setTimeout(() => {
                        const scrollTop = itemOffset === 330 ? itemOffset + 85 : itemOffset;
                        verticalMenu.querySelector(".simplebar-content-wrapper").scrollTop = scrollTop;
                    }, 0);
                }
            }
        }, 250);
    }
    
    /**
     * Get navbar menu HTML
     * @returns {string} Menu HTML
     */
    getNavbarMenuHTML() {
        return this.navbarMenuHTML;
    }
    
    /**
     * Restore navbar menu HTML
     */
    restoreNavbarMenu() {
        const navbarMenu = document.querySelector(".navbar-menu");
        if (navbarMenu && this.navbarMenuHTML) {
            navbarMenu.innerHTML = this.navbarMenuHTML;
        }
    }
    
    /**
     * Clear active menu items
     */
    clearActiveItems() {
        const activeItems = document.querySelectorAll(".navbar-nav .active");
        Array.from(activeItems).forEach(item => {
            item.classList.remove("active");
        });
    }
}

export default MenuManager;
