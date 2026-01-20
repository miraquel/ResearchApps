/**
 * Search Component
 * Handles search dropdown functionality in topbar
 */

import { DOMUtils } from '../core/dom-utils.js';
import APP_CONFIG from '../core/config.js';

export class SearchComponent {
    constructor() {
        this.searchInput = null;
        this.searchDropdown = null;
        this.searchCloseBtn = null;
        this.initialized = false;
    }
    
    /**
     * Initialize search component
     */
    init() {
        if (this.initialized) return;
        
        this.searchInput = document.getElementById("search-options");
        this.searchDropdown = document.getElementById("search-dropdown");
        this.searchCloseBtn = document.getElementById("search-close-options");
        
        if (this.searchInput) {
            this.initDesktopSearch();
        }
        
        this.initMobileSearch();
        
        this.initialized = true;
    }
    
    /**
     * Initialize desktop search
     */
    initDesktopSearch() {
        // Focus event
        this.searchInput.addEventListener("focus", () => {
            this.handleSearchFocus();
        });
        
        // Keyup event with debouncing
        const debouncedSearch = DOMUtils.debounce(() => {
            this.handleSearchInput();
        }, APP_CONFIG.search.debounceDelay);
        
        this.searchInput.addEventListener("keyup", debouncedSearch);
        
        // Close button
        if (this.searchCloseBtn) {
            this.searchCloseBtn.addEventListener("click", () => {
                this.clearSearch();
            });
        }
        
        // Click outside to close
        document.body.addEventListener("click", (e) => {
            if (e.target.getAttribute("id") !== "search-options") {
                this.hideDropdown();
            }
        });
    }
    
    /**
     * Initialize mobile/responsive search
     */
    initMobileSearch() {
        const searchInputResponsive = document.getElementById("search-options-reponsive");
        const dropdownResponsive = document.getElementById("search-dropdown-reponsive");
        const searchCloseBtn = document.getElementById("search-close-options");
        
        if (!searchInputResponsive || !dropdownResponsive) return;
        
        // Focus event
        searchInputResponsive.addEventListener("focus", () => {
            const inputLength = searchInputResponsive.value.length;
            if (inputLength > 0) {
                dropdownResponsive.classList.add("show");
                if (searchCloseBtn) searchCloseBtn.classList.remove("d-none");
            } else {
                dropdownResponsive.classList.remove("show");
                if (searchCloseBtn) searchCloseBtn.classList.add("d-none");
            }
        });
        
        // Keyup event
        searchInputResponsive.addEventListener("keyup", () => {
            const inputLength = searchInputResponsive.value.length;
            if (inputLength > 0) {
                dropdownResponsive.classList.add("show");
                if (searchCloseBtn) searchCloseBtn.classList.remove("d-none");
            } else {
                dropdownResponsive.classList.remove("show");
                if (searchCloseBtn) searchCloseBtn.classList.add("d-none");
            }
        });
        
        // Close button
        if (searchCloseBtn) {
            searchCloseBtn.addEventListener("click", () => {
                searchInputResponsive.value = "";
                dropdownResponsive.classList.remove("show");
                searchCloseBtn.classList.add("d-none");
            });
        }
        
        // Click outside to close
        document.body.addEventListener("click", (e) => {
            if (e.target.getAttribute("id") !== "search-options-reponsive") {
                dropdownResponsive.classList.remove("show");
                if (searchCloseBtn) searchCloseBtn.classList.add("d-none");
            }
        });
    }
    
    /**
     * Handle search input focus
     */
    handleSearchFocus() {
        const inputLength = this.searchInput.value.length;
        
        if (inputLength > 0) {
            this.showDropdown();
            this.searchCloseBtn?.classList.remove("d-none");
        } else {
            this.hideDropdown();
            this.searchCloseBtn?.classList.add("d-none");
        }
    }
    
    /**
     * Handle search input
     */
    handleSearchInput() {
        const inputLength = this.searchInput.value.length;
        
        if (inputLength > 0) {
            this.showDropdown();
            this.searchCloseBtn?.classList.remove("d-none");
            this.filterResults(this.searchInput.value.toLowerCase());
        } else {
            this.hideDropdown();
            this.searchCloseBtn?.classList.add("d-none");
        }
    }
    
    /**
     * Filter search results
     * @param {string} searchTerm - Search term
     */
    filterResults(searchTerm) {
        const notifyItems = document.getElementsByClassName("notify-item");
        
        Array.from(notifyItems).forEach(item => {
            let searchText = '';
            
            if (item.querySelector("h6")) {
                const spanText = item.getElementsByTagName("span")[0]?.innerText.toLowerCase() || '';
                const nameText = item.querySelector("h6").innerText.toLowerCase();
                searchText = nameText.includes(searchTerm) ? nameText : spanText;
            } else if (item.getElementsByTagName("span").length > 0) {
                searchText = item.getElementsByTagName("span")[0].innerText.toLowerCase();
            }
            
            item.style.display = searchText.includes(searchTerm) ? "block" : "none";
        });
    }
    
    /**
     * Show dropdown
     */
    showDropdown() {
        if (this.searchDropdown) {
            this.searchDropdown.classList.add("show");
        }
    }
    
    /**
     * Hide dropdown
     */
    hideDropdown() {
        if (this.searchDropdown) {
            this.searchDropdown.classList.remove("show");
        }
    }
    
    /**
     * Clear search
     */
    clearSearch() {
        if (this.searchInput) {
            this.searchInput.value = "";
        }
        this.hideDropdown();
        this.searchCloseBtn?.classList.add("d-none");
        
        // Show all items
        const notifyItems = document.getElementsByClassName("notify-item");
        Array.from(notifyItems).forEach(item => {
            item.style.display = "block";
        });
    }
}

export default SearchComponent;
