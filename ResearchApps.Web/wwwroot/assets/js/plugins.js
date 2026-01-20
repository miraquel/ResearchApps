/*
Template Name: Velzon - Admin & Dashboard Template
Author: Themesbrand
Version: 4.3.0
Website: https://Themesbrand.com/
Contact: Themesbrand@gmail.com
File: Common Plugins Js File
*/

// Common plugins - load asynchronously instead of using document.write
if(document.querySelectorAll("[toast-list]") || document.querySelectorAll('[data-choices]') || document.querySelectorAll("[data-provider]")){ 
  // Toastify
  if (!window.Toastify && document.querySelectorAll("[toast-list]").length) {
    const toastifyScript = document.createElement('script');
    toastifyScript.src = 'https://cdn.jsdelivr.net/npm/toastify-js';
    toastifyScript.async = true;
    document.head.appendChild(toastifyScript);
  }
  
  // Choices.js - Note: We're not using this in our refactored components
  // Flatpickr - Note: Loaded per-page in @section scripts where needed
}