/**
 * DOM Utility Functions
 * Common DOM manipulation and query helpers
 */

export class DOMUtils {
    /**
     * Check if element is in viewport
     * @param {HTMLElement} el - Element to check
     * @returns {boolean} True if element is in viewport
     */
    static isInViewport(el) {
        if (!el) return false;
        
        let top = el.offsetTop;
        let left = el.offsetLeft;
        const width = el.offsetWidth;
        const height = el.offsetHeight;
        
        let parent = el.offsetParent;
        while (parent) {
            top += parent.offsetTop;
            left += parent.offsetLeft;
            parent = parent.offsetParent;
        }
        
        return (
            top >= window.pageYOffset &&
            left >= window.pageXOffset &&
            top + height <= window.pageYOffset + window.innerHeight &&
            left + width <= window.pageXOffset + window.innerWidth
        );
    }
    
    /**
     * Get all siblings of an element
     * @param {HTMLElement} elem - Element
     * @returns {Array<HTMLElement>} Array of sibling elements
     */
    static getSiblings(elem) {
        const siblings = [];
        let sibling = elem.parentNode.firstChild;
        
        while (sibling) {
            if (sibling.nodeType === 1 && sibling !== elem) {
                siblings.push(sibling);
            }
            sibling = sibling.nextSibling;
        }
        
        return siblings;
    }
    
    /**
     * Add event listener with optional delegation
     * @param {string} selector - CSS selector
     * @param {string} event - Event name
     * @param {Function} handler - Event handler
     * @param {HTMLElement} context - Context element (default: document)
     */
    static on(selector, event, handler, context = document) {
        const elements = context.querySelectorAll(selector);
        Array.from(elements).forEach(elem => {
            elem.addEventListener(event, handler);
        });
    }
    
    /**
     * Remove event listener from elements
     * @param {string} selector - CSS selector
     * @param {string} event - Event name
     * @param {Function} handler - Event handler
     * @param {HTMLElement} context - Context element (default: document)
     */
    static off(selector, event, handler, context = document) {
        const elements = context.querySelectorAll(selector);
        Array.from(elements).forEach(elem => {
            elem.removeEventListener(event, handler);
        });
    }
    
    /**
     * Get window size information
     * @returns {Object} Window size object
     */
    static getWindowSize() {
        return {
            width: document.documentElement.clientWidth,
            height: document.documentElement.clientHeight,
            isMobile: document.documentElement.clientWidth <= 767,
            isTablet: document.documentElement.clientWidth > 767 && document.documentElement.clientWidth < 1025,
            isDesktop: document.documentElement.clientWidth >= 1025
        };
    }
    
    /**
     * Debounce function execution
     * @param {Function} func - Function to debounce
     * @param {number} delay - Delay in milliseconds
     * @returns {Function} Debounced function
     */
    static debounce(func, delay) {
        let timeout;
        return function(...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), delay);
        };
    }
    
    /**
     * Throttle function execution
     * @param {Function} func - Function to throttle
     * @param {number} limit - Time limit in milliseconds
     * @returns {Function} Throttled function
     */
    static throttle(func, limit) {
        let inThrottle;
        return function(...args) {
            if (!inThrottle) {
                func.apply(this, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }
    
    /**
     * Format number with commas
     * @param {number} x - Number to format
     * @returns {string} Formatted number
     */
    static numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
    
    /**
     * Get data attributes from element
     * @param {HTMLElement} element - Element to get data from
     * @param {string} prefix - Data attribute prefix (without 'data-')
     * @returns {Object} Object with data attributes
     */
    static getDataAttributes(element, prefix = '') {
        const data = {};
        const attributes = element.attributes;
        const searchPrefix = prefix ? `data-${prefix}-` : 'data-';
        
        Array.from(attributes).forEach(attr => {
            if (attr.name.startsWith(searchPrefix)) {
                const key = attr.name.replace(searchPrefix, '');
                data[key] = attr.value;
            }
        });
        
        return data;
    }
    
    /**
     * Set attributes on element
     * @param {HTMLElement} element - Target element
     * @param {Object} attributes - Attributes object
     */
    static setAttributes(element, attributes) {
        Object.keys(attributes).forEach(key => {
            if (attributes[key] !== null && attributes[key] !== undefined) {
                element.setAttribute(key, attributes[key]);
            }
        });
    }
    
    /**
     * Create element with attributes and content
     * @param {string} tag - HTML tag name
     * @param {Object} attributes - Attributes object
     * @param {string|HTMLElement} content - Element content
     * @returns {HTMLElement} Created element
     */
    static createElement(tag, attributes = {}, content = '') {
        const element = document.createElement(tag);
        this.setAttributes(element, attributes);
        
        if (typeof content === 'string') {
            element.innerHTML = content;
        } else if (content instanceof HTMLElement) {
            element.appendChild(content);
        }
        
        return element;
    }
    
    /**
     * Fade out element
     * @param {HTMLElement} element - Element to fade out
     * @param {number} duration - Duration in milliseconds
     */
    static fadeOut(element, duration = 300) {
        element.style.transition = `opacity ${duration}ms`;
        element.style.opacity = '0';
        
        setTimeout(() => {
            element.style.display = 'none';
        }, duration);
    }
    
    /**
     * Fade in element
     * @param {HTMLElement} element - Element to fade in
     * @param {string} display - Display value (default: 'block')
     * @param {number} duration - Duration in milliseconds
     */
    static fadeIn(element, display = 'block', duration = 300) {
        element.style.display = display;
        element.style.transition = `opacity ${duration}ms`;
        element.style.opacity = '0';
        
        setTimeout(() => {
            element.style.opacity = '1';
        }, 10);
    }
}

export default DOMUtils;
