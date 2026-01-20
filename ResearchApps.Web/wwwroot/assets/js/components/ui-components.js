/**
 * UI Components Manager
 * Handles Bootstrap components initialization (tooltips, popovers, etc.)
 */

import { DOMUtils } from '../core/dom-utils.js';
import APP_CONFIG from '../core/config.js';

export class ComponentsManager {
    constructor() {
        this.initialized = false;
    }
    
    /**
     * Initialize all UI components
     */
    init() {
        if (this.initialized) return;
        
        this.initTooltips();
        this.initPopovers();
        this.initDropdownTabs();
        this.initCodeSwitcher();
        this.initCounter();
        
        this.initialized = true;
    }
    
    /**
     * Initialize Bootstrap tooltips
     */
    initTooltips() {
        const tooltipTriggerList = [].slice.call(
            document.querySelectorAll('[data-bs-toggle="tooltip"]')
        );
        
        tooltipTriggerList.map(tooltipTriggerEl => {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
    
    /**
     * Initialize Bootstrap popovers
     */
    initPopovers() {
        const popoverTriggerList = [].slice.call(
            document.querySelectorAll('[data-bs-toggle="popover"]')
        );
        
        popoverTriggerList.map(popoverTriggerEl => {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    }
    
    /**
     * Initialize dropdown tabs
     */
    initDropdownTabs() {
        const dropdownTabs = document.querySelectorAll('.dropdown-menu a[data-bs-toggle="tab"]');
        
        Array.from(dropdownTabs).forEach(element => {
            element.addEventListener("click", (e) => {
                e.stopPropagation();
                const tab = bootstrap.Tab.getInstance(e.target);
                if (tab) tab.show();
            });
        });
    }
    
    /**
     * Initialize code preview/code switcher
     */
    initCodeSwitcher() {
        const codeSwitchers = document.getElementsByClassName("code-switcher");
        
        Array.from(codeSwitchers).forEach(switcher => {
            switcher.addEventListener("change", function() {
                const card = this.closest(".card");
                const preview = card.querySelector(".live-preview");
                const code = card.querySelector(".code-view");
                
                if (this.checked) {
                    preview?.classList.add("d-none");
                    code?.classList.remove("d-none");
                } else {
                    preview?.classList.remove("d-none");
                    code?.classList.add("d-none");
                }
            });
        });
    }
    
    /**
     * Initialize counter animations
     */
    initCounter() {
        const counters = document.querySelectorAll(".counter-value");
        if (!counters.length) return;
        
        Array.from(counters).forEach(counter => {
            this.animateCounter(counter);
        });
    }
    
    /**
     * Animate single counter
     * @param {HTMLElement} counter - Counter element
     */
    animateCounter(counter) {
        const target = +counter.getAttribute("data-target");
        const speed = APP_CONFIG.counter.speed;
        
        const updateCount = () => {
            const count = +counter.innerText;
            const inc = target / speed;
            
            if (count < target) {
                counter.innerText = Math.ceil(count + inc);
                setTimeout(updateCount, 1);
            } else {
                counter.innerText = DOMUtils.numberWithCommas(target);
            }
        };
        
        updateCount();
    }
    
    /**
     * Reinitialize components (useful after dynamic content load)
     */
    reinit() {
        this.initialized = false;
        this.init();
    }
}

export default ComponentsManager;
