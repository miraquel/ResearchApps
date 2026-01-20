/*
Template Name: Velzon - Admin & Dashboard Template
Author: Themesbrand
Version: 4.0.0
Website: https://Themesbrand.com/
Contact: Themesbrand@gmail.com
File: Layout Js File
*/

/**
 * Immediate Layout Restoration
 * This file runs synchronously in the <head> to prevent layout flashing
 * It reads from localStorage and applies settings before page render
 */

(function () {
    'use strict';

    // List of layout-related keys stored in localStorage
    var LAYOUT_KEYS = [
        'data-layout',
        'data-sidebar-size',
        'data-bs-theme',
        'data-layout-width',
        'data-sidebar',
        'data-sidebar-image',
        'data-layout-direction',
        'data-layout-position',
        'data-layout-style',
        'data-topbar',
        'data-preloader',
        'data-body-image',
        'data-sidebar-visibility'
    ];
    
    // Apply stored layout attributes immediately from localStorage
    LAYOUT_KEYS.forEach(function(key) {
        var value = localStorage.getItem(key);
        if (value) {
            document.documentElement.setAttribute(key, value);
        }
    });
})();