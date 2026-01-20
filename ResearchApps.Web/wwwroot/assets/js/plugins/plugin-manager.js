/**
 * Plugin Manager
 * Initializes and manages third-party plugins (Choices, Flatpickr, Toastify, etc.)
 */

import { DOMUtils } from '../core/dom-utils.js';

export class PluginManager {
    constructor() {
        this.initialized = false;
    }
    
    /**
     * Initialize all plugins
     */
    init() {
        if (this.initialized) return;
        
        this.initToastify();
        this.initChoices();
        this.initFlatpickr();
        this.initWaves();
        this.initFeather();
        
        this.initialized = true;
    }
    
    /**
     * Initialize Toastify notifications
     */
    initToastify() {
        if (typeof Toastify === 'undefined') return;
        
        const toastExamples = document.querySelectorAll("[data-toast]");
        
        Array.from(toastExamples).forEach(element => {
            element.addEventListener("click", () => {
                const toastData = this.getToastData(element);
                
                Toastify({
                    newWindow: true,
                    text: toastData.text,
                    gravity: toastData.gravity,
                    position: toastData.position,
                    className: "bg-" + toastData.className,
                    stopOnFocus: true,
                    offset: {
                        x: toastData.offset ? 50 : 0,
                        y: toastData.offset ? 10 : 0,
                    },
                    duration: toastData.duration,
                    close: toastData.close === "close",
                    style: toastData.style === "style" ? {
                        background: "linear-gradient(to right, #0AB39C, #405189)"
                    } : "",
                }).showToast();
            });
        });
    }
    
    /**
     * Get toast data from element attributes
     * @param {HTMLElement} element - Toast trigger element
     * @returns {Object} Toast configuration
     */
    getToastData(element) {
        const attrs = element.attributes;
        return {
            text: attrs["data-toast-text"]?.value || '',
            gravity: attrs["data-toast-gravity"]?.value || 'top',
            position: attrs["data-toast-position"]?.value || 'right',
            className: attrs["data-toast-className"]?.value || 'primary',
            duration: attrs["data-toast-duration"]?.value || 3000,
            close: attrs["data-toast-close"]?.value || '',
            style: attrs["data-toast-style"]?.value || '',
            offset: attrs["data-toast-offset"]?.value || false
        };
    }
    
    /**
     * Initialize Choices.js select plugin
     */
    initChoices() {
        if (typeof Choices === 'undefined') return;
        
        const choicesElements = document.querySelectorAll("[data-choices]");
        
        Array.from(choicesElements).forEach(item => {
            const choiceData = this.getChoicesData(item);
            
            if (item.attributes["data-choices-text-disabled-true"]) {
                new Choices(item, choiceData).disable();
            } else {
                new Choices(item, choiceData);
            }
        });
    }
    
    /**
     * Get Choices.js configuration from element attributes
     * @param {HTMLElement} element - Select element
     * @returns {Object} Choices configuration
     */
    getChoicesData(element) {
        const attrs = element.attributes;
        const config = {};
        
        if (attrs["data-choices-groups"]) {
            config.placeholderValue = "This is a placeholder set in the config";
        }
        if (attrs["data-choices-search-false"]) {
            config.searchEnabled = false;
        }
        if (attrs["data-choices-search-true"]) {
            config.searchEnabled = true;
        }
        if (attrs["data-choices-removeItem"] || attrs["data-choices-multiple-remove"]) {
            config.removeItemButton = true;
        }
        if (attrs["data-choices-sorting-false"]) {
            config.shouldSort = false;
        }
        if (attrs["data-choices-sorting-true"]) {
            config.shouldSort = true;
        }
        if (attrs["data-choices-limit"]) {
            config.maxItemCount = parseInt(attrs["data-choices-limit"].value);
        }
        if (attrs["data-choices-editItem-true"]) {
            config.editItems = true;
        }
        if (attrs["data-choices-editItem-false"]) {
            config.editItems = false;
        }
        if (attrs["data-choices-text-unique-true"]) {
            config.duplicateItemsAllowed = false;
        }
        if (attrs["data-choices-text-disabled-true"]) {
            config.addItems = false;
        }
        
        return config;
    }
    
    /**
     * Initialize Flatpickr date/time picker
     */
    initFlatpickr() {
        if (typeof flatpickr === 'undefined') return;
        
        const flatpickrElements = document.querySelectorAll("[data-provider]");
        
        Array.from(flatpickrElements).forEach(item => {
            const provider = item.getAttribute("data-provider");
            
            if (provider === "flatpickr") {
                const config = this.getFlatpickrConfig(item);
                flatpickr(item, config);
            } else if (provider === "timepickr") {
                const config = this.getTimepickrConfig(item);
                flatpickr(item, config);
            }
        });
    }
    
    /**
     * Get Flatpickr configuration from element attributes
     * @param {HTMLElement} element - Input element
     * @returns {Object} Flatpickr configuration
     */
    getFlatpickrConfig(element) {
        const attrs = element.attributes;
        const config = { disableMobile: true };
        
        if (attrs["data-date-format"]) {
            config.dateFormat = attrs["data-date-format"].value;
        }
        if (attrs["data-enable-time"]) {
            config.enableTime = true;
            config.dateFormat = (attrs["data-date-format"]?.value || 'Y-m-d') + " H:i";
        }
        if (attrs["data-altFormat"]) {
            config.altInput = true;
            config.altFormat = attrs["data-altFormat"].value;
        }
        if (attrs["data-minDate"]) {
            config.minDate = attrs["data-minDate"].value;
        }
        if (attrs["data-maxDate"]) {
            config.maxDate = attrs["data-maxDate"].value;
        }
        if (attrs["data-deafult-date"]) {
            config.defaultDate = attrs["data-deafult-date"].value;
        }
        if (attrs["data-multiple-date"]) {
            config.mode = "multiple";
        }
        if (attrs["data-range-date"]) {
            config.mode = "range";
        }
        if (attrs["data-inline-date"]) {
            config.inline = true;
            if (attrs["data-deafult-date"]) {
                config.defaultDate = attrs["data-deafult-date"].value;
            }
        }
        if (attrs["data-disable-date"]) {
            config.disable = attrs["data-disable-date"].value.split(",");
        }
        if (attrs["data-week-number"]) {
            config.weekNumbers = true;
        }
        
        return config;
    }
    
    /**
     * Get Timepickr configuration from element attributes
     * @param {HTMLElement} element - Input element
     * @returns {Object} Flatpickr time configuration
     */
    getTimepickrConfig(element) {
        const attrs = element.attributes;
        const config = {
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i"
        };
        
        if (attrs["data-time-hrs"]) {
            config.time_24hr = true;
        }
        if (attrs["data-min-time"]) {
            config.minTime = attrs["data-min-time"].value;
        }
        if (attrs["data-max-time"]) {
            config.maxTime = attrs["data-max-time"].value;
        }
        if (attrs["data-default-time"]) {
            config.defaultDate = attrs["data-default-time"].value;
        }
        if (attrs["data-time-inline"]) {
            config.inline = true;
            config.defaultDate = attrs["data-time-inline"].value;
        }
        
        return config;
    }
    
    /**
     * Initialize Waves effect
     */
    initWaves() {
        if (typeof Waves !== 'undefined') {
            Waves.init();
        }
    }
    
    /**
     * Initialize Feather icons
     */
    initFeather() {
        if (typeof feather !== 'undefined') {
            feather.replace();
        }
    }
    
    /**
     * Reinitialize plugins after dynamic content
     */
    reinit() {
        this.initChoices();
        this.initFlatpickr();
        this.initFeather();
    }
}

export default PluginManager;
