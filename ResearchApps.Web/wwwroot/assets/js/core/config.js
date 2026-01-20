/**
 * Application Configuration
 * Centralized configuration for the entire application
 */

export const APP_CONFIG = {
    layout: {
        defaultLayout: 'vertical',
        horizontalMenuSplit: 7, // after this number all horizontal menus will be moved in More menu options
        availableLayouts: ['vertical', 'horizontal', 'twocolumn', 'semibox']
    },
    
    language: {
        default: 'en',
        available: ['en', 'sp', 'gr', 'it', 'ru', 'ch', 'fr', 'ar'],
        flagMap: {
            'en': '/assets/images/flags/us.svg',
            'sp': '/assets/images/flags/spain.svg',
            'gr': '/assets/images/flags/germany.svg',
            'it': '/assets/images/flags/italy.svg',
            'ru': '/assets/images/flags/russia.svg',
            'ch': '/assets/images/flags/china.svg',
            'fr': '/assets/images/flags/french.svg',
            'ar': '/assets/images/flags/ae.svg'
        },
        translationPath: '/assets/lang/'
    },
    
    notifications: {
        ttl: 2000, // Time to live for deduplication (milliseconds)
        maxReconnectAttempts: 5,
        reconnectDelays: [0, 2000, 5000, 10000, 30000]
    },
    
    sidebar: {
        sizes: ['lg', 'sm', 'md', 'sm-hover'],
        defaultSize: 'lg',
        themes: ['light', 'dark', 'gradient', 'gradient-2', 'gradient-3', 'gradient-4'],
        defaultTheme: 'dark',
        images: ['none', 'img-1', 'img-2', 'img-3', 'img-4']
    },
    
    theme: {
        modes: ['light', 'dark'],
        defaultMode: 'light'
    },
    
    topbar: {
        themes: ['light', 'dark'],
        defaultTheme: 'light'
    },
    
    scrollOffset: 300, // Offset for menu item scroll activation
    
    search: {
        minLength: 1,
        debounceDelay: 300
    },
    
    counter: {
        speed: 250 // Lower = slower animation
    },
    
    preloader: {
        fadeDelay: 1000
    },
    
    breakpoints: {
        mobile: 767,
        tablet: 1025,
        desktop: 1848
    }
};

export default APP_CONFIG;
