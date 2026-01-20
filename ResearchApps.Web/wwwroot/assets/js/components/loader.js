/**
 * Loader Component - Reusable Loading Overlay
 * Provides a fullscreen loading overlay with spinner for AJAX operations
 * 
 * Usage:
 * LoaderHelper.show(); // Show loader
 * LoaderHelper.hide(); // Hide loader
 * 
 * Or for automatic handling:
 * LoaderHelper.wrap(async () => {
 *     // Your async operation
 *     await fetch(...);
 * });
 */

(function () {
    'use strict';

    var LoaderHelper = {
        /**
         * Initialize and create loader element in DOM
         */
        init: function() {
            if (document.getElementById('global-loader')) {
                return; // Already initialized
            }

            var loaderHTML = `
                <div id="global-loader" class="loader-overlay" style="display: none;">
                    <div class="loader-content">
                        <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                        <p class="mt-3 text-muted loader-message">Loading...</p>
                    </div>
                </div>
            `;

            // Append to body
            document.body.insertAdjacentHTML('beforeend', loaderHTML);

            // Add CSS
            this._addStyles();
        },

        /**
         * Add loader styles to document
         */
        _addStyles: function() {
            if (document.getElementById('loader-styles')) {
                return; // Styles already added
            }

            var style = document.createElement('style');
            style.id = 'loader-styles';
            style.textContent = `
                .loader-overlay {
                    position: fixed;
                    top: 0;
                    left: 0;
                    width: 100%;
                    height: 100%;
                    background-color: rgba(0, 0, 0, 0.5);
                    z-index: 9999;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                }
                
                .loader-content {
                    text-align: center;
                    background: white;
                    padding: 2rem;
                    border-radius: 0.5rem;
                    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
                }
                
                .loader-message {
                    margin-bottom: 0;
                    font-size: 0.875rem;
                }
            `;
            document.head.appendChild(style);
        },

        /**
         * Show the loader
         * @param {string} message - Optional custom message
         */
        show: function(message) {
            this.init();
            var loader = document.getElementById('global-loader');
            var messageEl = loader.querySelector('.loader-message');
            
            if (message && messageEl) {
                messageEl.textContent = message;
            } else if (messageEl) {
                messageEl.textContent = 'Loading...';
            }
            
            loader.style.display = 'flex';
        },

        /**
         * Hide the loader
         */
        hide: function() {
            var loader = document.getElementById('global-loader');
            if (loader) {
                loader.style.display = 'none';
            }
        },

        /**
         * Wrap an async function with loader show/hide
         * @param {Function} asyncFn - Async function to execute
         * @param {string} message - Optional loading message
         * @returns {Promise} - Result of the async function
         */
        wrap: async function(asyncFn, message) {
            try {
                this.show(message);
                return await asyncFn();
            } finally {
                this.hide();
            }
        },

        /**
         * Wrap a fetch call with loader
         * @param {string} url - URL to fetch
         * @param {object} options - Fetch options
         * @param {string} message - Optional loading message
         * @returns {Promise} - Fetch response
         */
        fetch: async function(url, options, message) {
            return this.wrap(async () => {
                return await fetch(url, options);
            }, message);
        }
    };

    // Expose to window for global access
    window.LoaderHelper = LoaderHelper;

    // Auto-initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            LoaderHelper.init();
        });
    } else {
        LoaderHelper.init();
    }

})();
