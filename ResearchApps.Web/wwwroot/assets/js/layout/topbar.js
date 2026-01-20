/**
 * Topbar Manager
 * Manages topbar components including search, notifications, and theme toggle
 */

import { StorageService } from '../core/storage.js';
import { eventBus } from '../core/event-bus.js';
import APP_CONFIG from '../core/config.js';

export class TopbarManager {
    constructor() {
        this.currentTheme = StorageService.get('data-topbar', APP_CONFIG.topbar.defaultTheme);
        this.scrollThreshold = 50;
    }
    
    /**
     * Initialize topbar
     */
    init() {
        this.applyStoredSettings();
        this.initScrollEffect();
        this.initFullscreen();
        this.initThemeToggle();
        this.initCartDropdown();
        this.initNotificationDropdown();
        eventBus.emit('topbar:initialized');
    }
    
    /**
     * Apply stored topbar settings
     */
    applyStoredSettings() {
        document.documentElement.setAttribute('data-topbar', this.currentTheme);
    }
    
    /**
     * Set topbar theme
     * @param {string} theme - Topbar theme (light, dark)
     */
    setTheme(theme) {
        if (!APP_CONFIG.topbar.themes.includes(theme)) {
            console.warn(`Invalid topbar theme: ${theme}`);
            return;
        }
        
        this.currentTheme = theme;
        document.documentElement.setAttribute('data-topbar', theme);
        StorageService.set('data-topbar', theme);
        eventBus.emit('topbar:themeChanged', { theme });
    }
    
    /**
     * Initialize scroll effect for topbar shadow
     */
    initScrollEffect() {
        const pageTopbar = document.getElementById("page-topbar");
        if (!pageTopbar) return;
        
        const handleScroll = () => {
            const scrollTop = document.body.scrollTop || document.documentElement.scrollTop;
            
            if (scrollTop >= this.scrollThreshold) {
                pageTopbar.classList.add("topbar-shadow");
            } else {
                pageTopbar.classList.remove("topbar-shadow");
            }
        };
        
        document.addEventListener("scroll", handleScroll);
    }
    
    /**
     * Initialize fullscreen toggle
     */
    initFullscreen() {
        const fullscreenBtn = document.querySelector('[data-toggle="fullscreen"]');
        if (!fullscreenBtn) return;
        
        fullscreenBtn.addEventListener("click", (e) => {
            e.preventDefault();
            this.toggleFullscreen();
        });
        
        // Listen for fullscreen change events
        document.addEventListener("fullscreenchange", () => this.handleFullscreenChange());
        document.addEventListener("webkitfullscreenchange", () => this.handleFullscreenChange());
        document.addEventListener("mozfullscreenchange", () => this.handleFullscreenChange());
    }
    
    /**
     * Toggle fullscreen mode
     */
    toggleFullscreen() {
        document.body.classList.toggle("fullscreen-enable");
        
        if (!document.fullscreenElement && 
            !document.mozFullScreenElement && 
            !document.webkitFullscreenElement) {
            // Enter fullscreen
            if (document.documentElement.requestFullscreen) {
                document.documentElement.requestFullscreen();
            } else if (document.documentElement.mozRequestFullScreen) {
                document.documentElement.mozRequestFullScreen();
            } else if (document.documentElement.webkitRequestFullscreen) {
                document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
            }
        } else {
            // Exit fullscreen
            if (document.cancelFullScreen) {
                document.cancelFullScreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitCancelFullScreen) {
                document.webkitCancelFullScreen();
            }
        }
    }
    
    /**
     * Handle fullscreen change event
     */
    handleFullscreenChange() {
        if (!document.webkitIsFullScreen && 
            !document.mozFullScreen && 
            !document.msFullscreenElement) {
            document.body.classList.remove("fullscreen-enable");
        }
    }
    
    /**
     * Initialize theme toggle button
     */
    initThemeToggle() {
        const lightDarkBtns = document.querySelectorAll(".light-dark-mode");
        if (!lightDarkBtns.length) return;
        
        Array.from(lightDarkBtns).forEach(btn => {
            btn.addEventListener("click", () => {
                this.toggleAppTheme();
            });
        });
    }
    
    /**
     * Toggle application theme (light/dark)
     */
    toggleAppTheme() {
        const html = document.documentElement;
        const currentTheme = html.getAttribute("data-bs-theme");
        const newTheme = currentTheme === "dark" ? "light" : "dark";
        
        html.setAttribute("data-bs-theme", newTheme);
        StorageService.set("data-bs-theme", newTheme);
        
        // Trigger resize event for charts/components that need to adjust
        window.dispatchEvent(new Event('resize'));
        
        eventBus.emit('app:themeChanged', { theme: newTheme });
    }
    
    /**
     * Initialize cart dropdown functionality
     */
    initCartDropdown() {
        const removeButtons = document.querySelectorAll("#page-topbar .dropdown-menu-cart .remove-item-btn");
        if (!removeButtons.length) return;
        
        let itemCount = document.querySelectorAll(".dropdown-item-cart").length;
        
        Array.from(removeButtons).forEach(btn => {
            btn.addEventListener("click", (e) => {
                itemCount--;
                btn.closest(".dropdown-item-cart").remove();
                
                // Update badge
                Array.from(document.getElementsByClassName("cartitem-badge")).forEach(badge => {
                    badge.innerHTML = itemCount;
                });
                
                this.updateCartTotal();
                this.updateCartVisibility(itemCount);
            });
        });
        
        // Set initial badge count
        Array.from(document.getElementsByClassName("cartitem-badge")).forEach(badge => {
            badge.innerHTML = itemCount;
        });
        
        this.updateCartVisibility(itemCount);
    }
    
    /**
     * Update cart total price
     */
    updateCartTotal() {
        let subtotal = 0;
        Array.from(document.getElementsByClassName("cart-item-price")).forEach(priceElem => {
            subtotal += parseFloat(priceElem.innerHTML);
        });
        
        const totalElem = document.getElementById("cart-item-total");
        if (totalElem) {
            totalElem.innerHTML = "$" + subtotal.toFixed(2);
        }
    }
    
    /**
     * Update cart visibility based on item count
     * @param {number} itemCount - Number of items in cart
     */
    updateCartVisibility(itemCount) {
        const emptyCart = document.getElementById("empty-cart");
        const checkoutElem = document.getElementById("checkout-elem");
        
        if (emptyCart) {
            emptyCart.style.display = itemCount === 0 ? "block" : "none";
        }
        if (checkoutElem) {
            checkoutElem.style.display = itemCount === 0 ? "none" : "block";
        }
    }
    
    /**
     * Initialize notification dropdown functionality
     */
    initNotificationDropdown() {
        const notificationChecks = document.querySelectorAll(".notification-check input");
        if (!notificationChecks.length) return;
        
        Array.from(notificationChecks).forEach(checkbox => {
            checkbox.addEventListener("change", (e) => {
                this.handleNotificationCheck(e);
            });
        });
        
        // Handle delete notifications
        const removeModal = document.getElementById('removeNotificationModal');
        if (removeModal) {
            removeModal.addEventListener('show.bs.modal', () => {
                this.setupNotificationDelete();
            });
        }
        
        // Reset checkboxes on dropdown close
        const notificationDropdown = document.getElementById('notificationDropdown');
        if (notificationDropdown) {
            notificationDropdown.addEventListener('hide.bs.dropdown', () => {
                this.resetNotificationChecks();
            });
        }
        
        this.updateEmptyNotificationState();
    }
    
    /**
     * Handle notification checkbox change
     * @param {Event} e - Change event
     */
    handleNotificationCheck(e) {
        const notificationItem = e.target.closest(".notification-item");
        notificationItem.classList.toggle("active");
        
        const checkedCount = document.querySelectorAll('.notification-check input:checked').length;
        const actionsElem = document.getElementById("notification-actions");
        
        if (actionsElem) {
            actionsElem.style.display = checkedCount > 0 ? 'block' : 'none';
        }
        
        const selectContent = document.getElementById("select-content");
        if (selectContent) {
            selectContent.innerHTML = checkedCount;
        }
    }
    
    /**
     * Setup notification delete functionality
     */
    setupNotificationDelete() {
        const deleteBtn = document.getElementById("delete-notification");
        if (!deleteBtn) return;
        
        deleteBtn.addEventListener("click", () => {
            Array.from(document.querySelectorAll(".notification-item.active")).forEach(item => {
                item.remove();
            });
            
            this.updateEmptyNotificationState();
            
            const closeBtn = document.getElementById("NotificationModalbtn-close");
            if (closeBtn) closeBtn.click();
        });
    }
    
    /**
     * Reset notification checkboxes
     */
    resetNotificationChecks() {
        document.querySelectorAll('.notification-check input').forEach(checkbox => {
            checkbox.checked = false;
        });
        document.querySelectorAll('.notification-item').forEach(item => {
            item.classList.remove("active");
        });
        
        const actionsElem = document.getElementById('notification-actions');
        if (actionsElem) {
            actionsElem.style.display = '';
        }
    }
    
    /**
     * Update empty notification state display
     */
    updateEmptyNotificationState() {
        Array.from(document.querySelectorAll("#notificationItemsTabContent .tab-pane")).forEach(pane => {
            const hasItems = pane.querySelectorAll(".notification-item").length > 0;
            const viewAll = pane.querySelector(".view-all");
            
            if (viewAll) {
                viewAll.style.display = hasItems ? "block" : "none";
            }
            
            if (!hasItems && !pane.querySelector(".empty-notification-elem")) {
                pane.innerHTML += `
                    <div class="empty-notification-elem">
                        <div class="w-25 w-sm-50 pt-3 mx-auto">
                            <img src="/assets/images/svg/bell.svg" class="img-fluid" alt="user-pic">
                        </div>
                        <div class="text-center pb-5 mt-2">
                            <h6 class="fs-18 fw-semibold lh-base">Hey! You have no any notifications</h6>
                        </div>
                    </div>`;
            }
        });
    }
}

export default TopbarManager;
