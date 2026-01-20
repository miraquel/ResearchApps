/**
 * Event Bus
 * Simple pub/sub pattern for decoupled component communication
 */

export class EventBus {
    constructor() {
        this.events = {};
    }
    
    /**
     * Subscribe to an event
     * @param {string} event - Event name
     * @param {Function} handler - Event handler
     * @returns {Function} Unsubscribe function
     */
    on(event, handler) {
        if (!this.events[event]) {
            this.events[event] = [];
        }
        this.events[event].push(handler);
        
        // Return unsubscribe function
        return () => this.off(event, handler);
    }
    
    /**
     * Unsubscribe from an event
     * @param {string} event - Event name
     * @param {Function} handler - Event handler to remove
     */
    off(event, handler) {
        if (!this.events[event]) return;
        
        const index = this.events[event].indexOf(handler);
        if (index > -1) {
            this.events[event].splice(index, 1);
        }
    }
    
    /**
     * Emit an event
     * @param {string} event - Event name
     * @param {*} data - Data to pass to handlers
     */
    emit(event, data) {
        if (!this.events[event]) return;
        
        this.events[event].forEach(handler => {
            try {
                handler(data);
            } catch (error) {
                console.error(`Error in event handler for ${event}:`, error);
            }
        });
    }
    
    /**
     * Subscribe to an event once
     * @param {string} event - Event name
     * @param {Function} handler - Event handler
     */
    once(event, handler) {
        const onceHandler = (data) => {
            handler(data);
            this.off(event, onceHandler);
        };
        this.on(event, onceHandler);
    }
    
    /**
     * Remove all event listeners
     * @param {string} event - Optional event name, if not provided removes all
     */
    clear(event = null) {
        if (event) {
            delete this.events[event];
        } else {
            this.events = {};
        }
    }
    
    /**
     * Get list of registered events
     * @returns {Array<string>} Array of event names
     */
    getEvents() {
        return Object.keys(this.events);
    }
    
    /**
     * Get handler count for an event
     * @param {string} event - Event name
     * @returns {number} Number of handlers
     */
    getHandlerCount(event) {
        return this.events[event] ? this.events[event].length : 0;
    }
}

// Create and export a singleton instance
export const eventBus = new EventBus();

export default eventBus;
