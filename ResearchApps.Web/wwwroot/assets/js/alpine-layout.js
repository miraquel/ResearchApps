/**
 * Alpine.js Layout Store
 * Replaces app-modular.js, MenuManager, SidebarManager, TopbarManager,
 * LayoutManager, StorageService, DOMUtils, SearchComponent, PluginManager,
 * and ComponentsManager — all consolidated into a single Alpine store.
 *
 * Loaded BEFORE Alpine.js (via defer), this file registers:
 *   Alpine.store('layout', { ... })
 *
 * The store self-initialises via Alpine's 'alpine:init' event.
 *
 * Usage in HTML:
 *   <body x-data @alpine:initialized="$store.layout.init()">
 *     — OR simply rely on alpine:init which triggers automatically.
 */
document.addEventListener('alpine:init', () => {

    // =========================================================================
    // Configuration (was core/config.js)
    // =========================================================================
    const CONFIG = {
        layout: {
            defaultLayout: 'vertical',
            horizontalMenuSplit: 7,
            availableLayouts: ['vertical', 'horizontal', 'twocolumn', 'semibox']
        },
        sidebar: {
            sizes: ['lg', 'sm', 'md', 'sm-hover', 'sm-hover-active'],
            defaultSize: 'lg',
            themes: ['light', 'dark', 'gradient', 'gradient-2', 'gradient-3', 'gradient-4'],
            defaultTheme: 'dark',
            images: ['none', 'img-1', 'img-2', 'img-3', 'img-4']
        },
        topbar: {
            themes: ['light', 'dark'],
            defaultTheme: 'light'
        },
        theme: { modes: ['light', 'dark'], defaultMode: 'light' },
        counter: { speed: 250 },
        preloader: { fadeDelay: 1000 },
        scrollOffset: 300,
        search: { minLength: 1, debounceDelay: 300 },
        breakpoints: { mobile: 767, tablet: 1025 }
    };

    // =========================================================================
    // Storage helpers (was core/storage.js)
    // =========================================================================
    const LAYOUT_KEYS = [
        'data-layout', 'data-sidebar-size', 'data-bs-theme',
        'data-layout-width', 'data-sidebar', 'data-sidebar-image',
        'data-layout-direction', 'data-layout-position', 'data-layout-style',
        'data-topbar', 'data-preloader', 'data-body-image', 'data-sidebar-visibility'
    ];

    const Storage = {
        get(key, defaultValue = null) {
            const store = LAYOUT_KEYS.includes(key) ? localStorage : sessionStorage;
            const val = store.getItem(key);
            return val !== null ? val : defaultValue;
        },
        set(key, value) {
            const store = LAYOUT_KEYS.includes(key) ? localStorage : sessionStorage;
            store.setItem(key, value);
        },
        getLayoutAttributes() {
            const attrs = {};
            LAYOUT_KEYS.forEach(k => { attrs[k] = this.get(k); });
            return attrs;
        },
        saveDefaultAttributes() {
            const current = {};
            Array.from(document.documentElement.attributes).forEach(a => {
                if (a.nodeName && a.nodeName !== 'undefined') {
                    current[a.nodeName] = a.nodeValue;
                    if (!LAYOUT_KEYS.includes(a.nodeName)) {
                        sessionStorage.setItem(a.nodeName, a.nodeValue);
                    } else if (!localStorage.getItem(a.nodeName)) {
                        localStorage.setItem(a.nodeName, a.nodeValue);
                    }
                }
            });
            sessionStorage.setItem('defaultAttribute', JSON.stringify(current));
            return current;
        },
        getDefaultAttributes() {
            const v = sessionStorage.getItem('defaultAttribute');
            return v ? JSON.parse(v) : null;
        },
        checkDefaultAttributesMatch() {
            const stored = sessionStorage.getItem('defaultAttribute');
            if (!stored) return false;
            const current = {};
            Array.from(document.documentElement.attributes).forEach(a => {
                if (a.nodeName && a.nodeName !== 'undefined') current[a.nodeName] = a.nodeValue;
            });
            return stored === JSON.stringify(current);
        }
    };

    // =========================================================================
    // DOM helpers (was core/dom-utils.js) — only the parts still needed
    // =========================================================================
    function getSiblings(elem) {
        const siblings = [];
        let sib = elem.parentNode.firstChild;
        while (sib) {
            if (sib.nodeType === 1 && sib !== elem) siblings.push(sib);
            sib = sib.nextSibling;
        }
        return siblings;
    }

    function getWindowSize() {
        const w = document.documentElement.clientWidth;
        return {
            width: w,
            height: document.documentElement.clientHeight,
            isMobile: w <= CONFIG.breakpoints.mobile,
            isTablet: w > CONFIG.breakpoints.mobile && w < CONFIG.breakpoints.tablet,
            isDesktop: w >= CONFIG.breakpoints.tablet
        };
    }

    function debounce(fn, delay) {
        let timer;
        return function (...args) {
            clearTimeout(timer);
            timer = setTimeout(() => fn.apply(this, args), delay);
        };
    }

    function fadeOut(el, duration = 300) {
        el.style.transition = `opacity ${duration}ms`;
        el.style.opacity = '0';
        setTimeout(() => { el.style.display = 'none'; }, duration);
    }

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }

    // =========================================================================
    // Alpine store: layout
    // =========================================================================
    Alpine.store('layout', {
        initialized: false,
        navbarMenuHTML: null,
        currentLayout: Storage.get('data-layout', CONFIG.layout.defaultLayout),
        currentSidebarSize: Storage.get('data-sidebar-size', CONFIG.sidebar.defaultSize),
        currentSidebarTheme: Storage.get('data-sidebar', CONFIG.sidebar.defaultTheme),
        currentTopbarTheme: Storage.get('data-topbar', CONFIG.topbar.defaultTheme),

        // ==========================
        // Bootstrap / Initialization
        // ==========================
        init() {
            if (this.initialized) return;

            // Restore default attributes
            this._initDefaultAttributes();

            // Apply stored layout attributes to <html>
            const attrs = Storage.getLayoutAttributes();
            Object.keys(attrs).forEach(k => {
                if (attrs[k]) document.documentElement.setAttribute(k, attrs[k]);
            });

            // DOM-ready initialization
            const onReady = () => {
                this._initMenu();
                this._initSidebar();
                this._initTopbar();
                this._initHamburgerToggle();
                this._initWindowResize();
                this._initVerticalOverlay();
                this._initScrollToTop();
                this._initPreloader();
                this._initCustomizer();
                this._initSearch();
                this._initUIComponents();
                this._initFeatherIcons();
                this._initWaves();
            };

            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', onReady);
            } else {
                onReady();
            }

            this.initialized = true;
        },

        // ==========================
        // Default Attributes
        // ==========================
        _initDefaultAttributes() {
            if (!Storage.getDefaultAttributes()) {
                Storage.saveDefaultAttributes();
            } else if (!Storage.checkDefaultAttributesMatch()) {
                sessionStorage.clear();
                window.location.reload();
            }
        },

        // ==========================
        // Menu (was MenuManager)
        // ==========================
        _initMenu() {
            const navbar = document.querySelector('.navbar-menu');
            if (navbar) this.navbarMenuHTML = navbar.innerHTML;

            this._initCollapseMenu();
            this._initActiveMenu();
            this._scrollToActiveItem();
        },

        _initCollapseMenu() {
            const collapses = document.querySelectorAll('.navbar-nav .collapse');
            if (!collapses.length) return;

            collapses.forEach(collapse => {
                const inst = new bootstrap.Collapse(collapse, { toggle: false });

                collapse.addEventListener('show.bs.collapse', (e) => {
                    e.stopPropagation();
                    this._handleCollapseShow(collapse, inst);
                });

                collapse.addEventListener('hide.bs.collapse', (e) => {
                    e.stopPropagation();
                    const children = collapse.querySelectorAll('.collapse');
                    children.forEach(c => {
                        const ci = bootstrap.Collapse.getInstance(c);
                        if (ci) ci.hide();
                    });
                });
            });
        },

        _handleCollapseShow(collapse, instance) {
            const parent = collapse.parentElement.closest('.collapse');

            if (parent) {
                parent.querySelectorAll('.collapse').forEach(sib => {
                    const si = bootstrap.Collapse.getInstance(sib);
                    if (si && si !== instance) si.hide();
                });
            } else {
                getSiblings(collapse.parentElement).forEach(item => {
                    if (item.childNodes.length > 2 && item.firstElementChild) {
                        item.firstElementChild.setAttribute('aria-expanded', 'false');
                    }
                    item.querySelectorAll('*[id]').forEach(sub => {
                        sub.classList.remove('show');
                        if (sub.childNodes.length > 2) {
                            sub.querySelectorAll('ul li a').forEach(a => {
                                if (a.hasAttribute('aria-expanded')) a.setAttribute('aria-expanded', 'false');
                            });
                        }
                    });
                });
            }
        },

        _initActiveMenu() {
            const path = location.pathname === '/' ? '/' : '/' + location.pathname.substring(1);
            const nav = document.getElementById('navbar-nav');
            if (!nav) return;

            // Try exact match first
            let link = nav.querySelector(`[href="${path}"]`);

            // Fall back to longest prefix match (e.g. /Items matches /Items/Details/1000)
            if (!link) {
                let bestMatch = null;
                let bestLength = 0;
                nav.querySelectorAll('[href]').forEach(el => {
                    const href = el.getAttribute('href');
                    if (href && href !== '/' && path.startsWith(href + '/') && href.length > bestLength) {
                        bestMatch = el;
                        bestLength = href.length;
                    }
                });
                link = bestMatch;
            }

            if (link) this._setActiveMenuItem(link);
        },

        _setActiveMenuItem(link) {
            link.classList.add('active');
            let parent = link.closest('.collapse.menu-dropdown');
            while (parent) {
                parent.classList.add('show');
                const parentLink = parent.parentElement.children[0];
                if (parentLink) {
                    parentLink.classList.add('active');
                    parentLink.setAttribute('aria-expanded', 'true');
                }
                parent = parent.parentElement.closest('.collapse.menu-dropdown');
            }
        },

        _scrollToActiveItem() {
            setTimeout(() => {
                const nav = document.getElementById('navbar-nav');
                if (!nav) return;
                const active = nav.querySelector('.nav-item .active');
                if (!active) return;
                const offset = active.offsetTop;
                if (offset > CONFIG.scrollOffset) {
                    const menu = document.getElementsByClassName('app-menu')[0];
                    const wrapper = menu?.querySelector('.simplebar-content-wrapper');
                    if (wrapper) {
                        setTimeout(() => { wrapper.scrollTop = offset === 330 ? offset + 85 : offset; }, 0);
                    }
                }
            }, 250);
        },

        _clearActiveMenuItems() {
            const nav = document.getElementById('navbar-nav');
            if (!nav) return;
            nav.querySelectorAll('.active').forEach(el => el.classList.remove('active'));
            nav.querySelectorAll('.show').forEach(el => el.classList.remove('show'));
            nav.querySelectorAll('[aria-expanded="true"]').forEach(el => el.setAttribute('aria-expanded', 'false'));
        },

        _restoreNavbarMenu() {
            const navbar = document.querySelector('.navbar-menu');
            if (navbar && this.navbarMenuHTML) navbar.innerHTML = this.navbarMenuHTML;
        },

        // ==========================
        // Sidebar (was SidebarManager)
        // ==========================
        _initSidebar() {
            document.documentElement.setAttribute('data-sidebar-size', this.currentSidebarSize);
            document.documentElement.setAttribute('data-sidebar', this.currentSidebarTheme);
            this._initSidebarHover();
        },

        _initSidebarHover() {
            const icon = document.getElementById('vertical-hover');
            if (!icon) return;
            icon.addEventListener('click', () => {
                const cur = document.documentElement.getAttribute('data-sidebar-size');
                this.setSidebarSize(cur === 'sm-hover' ? 'sm-hover-active' : 'sm-hover');
            });
        },

        setSidebarSize(size) {
            this.currentSidebarSize = size;
            document.documentElement.setAttribute('data-sidebar-size', size);
            Storage.set('data-sidebar-size', size);
        },

        setSidebarTheme(theme) {
            if (!CONFIG.sidebar.themes.includes(theme)) return;
            this.currentSidebarTheme = theme;
            document.documentElement.setAttribute('data-sidebar', theme);
            Storage.set('data-sidebar', theme);
        },

        toggleSidebar() {
            document.body.classList.toggle('vertical-sidebar-enable');
        },

        // ==========================
        // Topbar (was TopbarManager)
        // ==========================
        _initTopbar() {
            document.documentElement.setAttribute('data-topbar', this.currentTopbarTheme);
            this._initScrollEffect();
            this._initFullscreen();
            this._initThemeToggle();
        },

        setTopbarTheme(theme) {
            if (!CONFIG.topbar.themes.includes(theme)) return;
            this.currentTopbarTheme = theme;
            document.documentElement.setAttribute('data-topbar', theme);
            Storage.set('data-topbar', theme);
        },

        _initScrollEffect() {
            const topbar = document.getElementById('page-topbar');
            if (!topbar) return;
            document.addEventListener('scroll', () => {
                const st = document.body.scrollTop || document.documentElement.scrollTop;
                topbar.classList.toggle('topbar-shadow', st >= 50);
            });
        },

        _initFullscreen() {
            const btn = document.querySelector('[data-toggle="fullscreen"]');
            if (!btn) return;

            btn.addEventListener('click', (e) => {
                e.preventDefault();
                document.body.classList.toggle('fullscreen-enable');
                if (!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement) {
                    (document.documentElement.requestFullscreen || document.documentElement.mozRequestFullScreen || document.documentElement.webkitRequestFullscreen)?.call(document.documentElement);
                } else {
                    (document.cancelFullScreen || document.mozCancelFullScreen || document.webkitCancelFullScreen)?.call(document);
                }
            });

            const onFsChange = () => {
                if (!document.webkitIsFullScreen && !document.mozFullScreen && !document.msFullscreenElement) {
                    document.body.classList.remove('fullscreen-enable');
                }
            };
            document.addEventListener('fullscreenchange', onFsChange);
            document.addEventListener('webkitfullscreenchange', onFsChange);
            document.addEventListener('mozfullscreenchange', onFsChange);
        },

        _initThemeToggle() {
            document.querySelectorAll('.light-dark-mode').forEach(btn => {
                btn.addEventListener('click', () => {
                    const html = document.documentElement;
                    const current = html.getAttribute('data-bs-theme');
                    const next = current === 'dark' ? 'light' : 'dark';
                    html.setAttribute('data-bs-theme', next);
                    Storage.set('data-bs-theme', next);
                    window.dispatchEvent(new Event('resize'));
                });
            });
        },

        // ==========================
        // Layout management (was LayoutManager)
        // ==========================
        setLayout(layout, save = true) {
            if (!CONFIG.layout.availableLayouts.includes(layout)) return;
            this.currentLayout = layout;
            document.documentElement.setAttribute('data-layout', layout);
            if (save) Storage.set('data-layout', layout);
            this._applyLayoutSpecific(layout);
        },

        _applyLayoutSpecific(layout) {
            switch (layout) {
                case 'vertical':   this._setupVertical(); break;
                case 'horizontal': this._setupHorizontal(); break;
                case 'twocolumn':  this._setupTwoColumn(); break;
                case 'semibox':    this._setupSemibox(); break;
            }
        },

        _setupVertical() {
            const twocol = document.getElementById('two-column-menu');
            if (twocol) twocol.innerHTML = '';
            this._restoreNavbarMenu();

            const scrollbar = document.getElementById('scrollbar');
            const navbarNav = document.getElementById('navbar-nav');
            if (scrollbar) { scrollbar.setAttribute('data-simplebar', ''); scrollbar.classList.add('h-100'); }
            if (navbarNav) navbarNav.setAttribute('data-simplebar', '');

            this._initCollapseMenu();
            this._initActiveMenu();
        },

        _setupHorizontal() {
            const twocol = document.getElementById('two-column-menu');
            if (twocol) twocol.innerHTML = '';
            this._restoreNavbarMenu();

            const scrollbar = document.getElementById('scrollbar');
            const navbarNav = document.getElementById('navbar-nav');
            if (scrollbar) { scrollbar.removeAttribute('data-simplebar'); scrollbar.classList.remove('h-100'); }
            if (navbarNav) navbarNav.removeAttribute('data-simplebar');

            // Split menus for horizontal layout
            const splitMenu = CONFIG.layout.horizontalMenuSplit;
            const menuItems = document.querySelectorAll('ul.navbar-nav > li.nav-item');
            let newMenus = '';
            let splitItem = null;

            menuItems.forEach((item, i) => {
                if (i + 1 === splitMenu) splitItem = item;
                if (i + 1 > splitMenu) {
                    newMenus += item.outerHTML;
                    item.remove();
                }
                if (i + 1 === menuItems.length && splitItem && newMenus) {
                    splitItem.insertAdjacentHTML('afterend',
                        `<li class="nav-item">
                            <a class="nav-link" href="#sidebarMore" data-bs-toggle="collapse" role="button" aria-expanded="false" aria-controls="sidebarMore">
                                <i class="ri-briefcase-2-line"></i> <span data-key="t-more">More</span>
                            </a>
                            <div class="collapse menu-dropdown" id="sidebarMore">
                                <ul class="nav nav-sm flex-column">${newMenus}</ul>
                            </div>
                        </li>`);
                }
            });

            this._initActiveMenu();
        },

        _setupTwoColumn() {
            const scrollbar = document.getElementById('scrollbar');
            if (scrollbar) { scrollbar.removeAttribute('data-simplebar'); scrollbar.classList.remove('h-100'); }
            this._initCollapseMenu();
            this._initActiveMenu();
        },

        _setupSemibox() {
            const twocol = document.getElementById('two-column-menu');
            if (twocol) twocol.innerHTML = '';
            this._restoreNavbarMenu();
            this._initCollapseMenu();
            this._initActiveMenu();
        },

        // ==========================
        // Hamburger menu toggle
        // ==========================
        _initHamburgerToggle() {
            const icon = document.getElementById('topnav-hamburger-icon');
            if (!icon) return;
            icon.addEventListener('click', () => this._toggleHamburger());
        },

        _toggleHamburger() {
            const ws = getWindowSize();
            if (ws.width > CONFIG.breakpoints.mobile) {
                document.querySelector('.hamburger-icon')?.classList.toggle('open');
            }

            const layout = document.documentElement.getAttribute('data-layout');

            switch (layout) {
                case 'horizontal':
                    document.body.classList.toggle('menu');
                    break;
                case 'vertical':
                    this._handleVerticalToggle(ws);
                    break;
                case 'semibox':
                    this._handleSemiboxToggle(ws);
                    break;
                case 'twocolumn':
                    document.body.classList.toggle('twocolumn-panel');
                    break;
            }
        },

        _handleVerticalToggle(ws) {
            if (ws.isTablet) {
                document.body.classList.remove('vertical-sidebar-enable');
                const cur = document.documentElement.getAttribute('data-sidebar-size');
                document.documentElement.setAttribute('data-sidebar-size', cur === 'sm' ? '' : 'sm');
            } else if (ws.isDesktop) {
                document.body.classList.remove('vertical-sidebar-enable');
                const cur = document.documentElement.getAttribute('data-sidebar-size');
                document.documentElement.setAttribute('data-sidebar-size', cur === 'lg' ? 'sm' : 'lg');
            } else if (ws.isMobile) {
                document.body.classList.add('vertical-sidebar-enable');
                document.documentElement.setAttribute('data-sidebar-size', 'lg');
            }
        },

        _handleSemiboxToggle(ws) {
            if (ws.width > CONFIG.breakpoints.mobile) {
                const vis = document.documentElement.getAttribute('data-sidebar-visibility');
                if (vis === 'show') {
                    const cur = document.documentElement.getAttribute('data-sidebar-size');
                    document.documentElement.setAttribute('data-sidebar-size', cur === 'lg' ? 'sm' : 'lg');
                } else {
                    document.getElementById('sidebar-visibility-show')?.click();
                }
            } else {
                document.body.classList.add('vertical-sidebar-enable');
                document.documentElement.setAttribute('data-sidebar-size', 'lg');
            }
        },

        // ==========================
        // Window resize
        // ==========================
        _initWindowResize() {
            let timer;
            window.addEventListener('resize', () => {
                clearTimeout(timer);
                timer = setTimeout(() => this._handleResize(), 100);
            });
        },

        _handleResize() {
            const ws = getWindowSize();
            const layout = Storage.get('data-layout');

            if (ws.isMobile) {
                document.body.classList.remove('vertical-sidebar-enable');
                document.body.classList.add('twocolumn-panel');
                if (layout === 'twocolumn') {
                    document.documentElement.setAttribute('data-layout', 'vertical');
                    this._setupVertical();
                }
                if (layout !== 'horizontal') {
                    document.documentElement.setAttribute('data-sidebar-size', 'lg');
                }
            } else if (ws.isTablet) {
                document.body.classList.remove('twocolumn-panel');
                if (layout === 'vertical' || layout === 'semibox') {
                    document.documentElement.setAttribute('data-sidebar-size', 'sm');
                }
            } else if (ws.isDesktop) {
                document.body.classList.remove('twocolumn-panel');
                if (layout === 'vertical' || layout === 'semibox') {
                    const stored = Storage.get('data-sidebar-size');
                    document.documentElement.setAttribute('data-sidebar-size', stored);
                }
            }
        },

        // ==========================
        // Vertical overlay (click outside to close sidebar on mobile)
        // ==========================
        _initVerticalOverlay() {
            document.querySelectorAll('.vertical-overlay').forEach(overlay => {
                overlay.addEventListener('click', () => {
                    document.body.classList.remove('vertical-sidebar-enable');
                    const layout = Storage.get('data-layout');
                    if (layout === 'twocolumn') {
                        document.body.classList.add('twocolumn-panel');
                    } else {
                        const stored = Storage.get('data-sidebar-size');
                        document.documentElement.setAttribute('data-sidebar-size', stored);
                    }
                });
            });
        },

        // ==========================
        // Scroll-to-top button
        // ==========================
        _initScrollToTop() {
            const btn = document.getElementById('back-to-top');
            if (!btn) return;
            window.addEventListener('scroll', () => {
                const st = document.body.scrollTop || document.documentElement.scrollTop;
                btn.style.display = st > 100 ? 'block' : 'none';
            });
            btn.addEventListener('click', () => {
                document.body.scrollTop = 0;
                document.documentElement.scrollTop = 0;
            });
        },

        // ==========================
        // Preloader
        // ==========================
        _initPreloader() {
            const setting = Storage.get('data-preloader');
            document.documentElement.setAttribute('data-preloader', setting === 'enable' ? 'enable' : 'disable');

            window.addEventListener('load', () => {
                const preloader = document.getElementById('preloader');
                if (preloader && setting === 'enable') {
                    setTimeout(() => fadeOut(preloader, 1000), CONFIG.preloader.fadeDelay);
                }
            });
        },

        // ==========================
        // Customizer panel (settings radios)
        // ==========================
        _initCustomizer() {
            const radioGroups = [
                { name: 'data-layout', handler: (v) => this.setLayout(v, true) },
                { name: 'data-bs-theme', handler: (v) => { document.documentElement.setAttribute('data-bs-theme', v); Storage.set('data-bs-theme', v); } },
                { name: 'data-topbar', handler: (v) => this.setTopbarTheme(v) },
                { name: 'data-sidebar-size', handler: (v) => this.setSidebarSize(v) },
                { name: 'data-sidebar', handler: (v) => this.setSidebarTheme(v) },
                { name: 'data-layout-width', handler: (v) => { document.documentElement.setAttribute('data-layout-width', v); Storage.set('data-layout-width', v); } },
                { name: 'data-layout-position', handler: (v) => { document.documentElement.setAttribute('data-layout-position', v); Storage.set('data-layout-position', v); } },
                { name: 'data-sidebar-image', handler: (v) => { document.documentElement.setAttribute('data-sidebar-image', v); Storage.set('data-sidebar-image', v); } },
                { name: 'data-preloader', handler: (v) => { document.documentElement.setAttribute('data-preloader', v); Storage.set('data-preloader', v); } }
            ];

            radioGroups.forEach(({ name, handler }) => {
                const radios = document.querySelectorAll(`input[name="${name}"]`);
                const currentVal = document.documentElement.getAttribute(name);
                radios.forEach(radio => {
                    if (radio.value === currentVal) radio.checked = true;
                    radio.addEventListener('change', (e) => {
                        if (e.target.checked) handler(e.target.value);
                    });
                });
            });
        },

        // ==========================
        // Search (was SearchComponent)
        // ==========================
        _initSearch() {
            const input = document.getElementById('search-options');
            const dropdown = document.getElementById('search-dropdown');
            const closeBtn = document.getElementById('search-close-options');

            if (input && dropdown) {
                const debouncedFilter = debounce(() => {
                    const len = input.value.length;
                    dropdown.classList.toggle('show', len > 0);
                    closeBtn?.classList.toggle('d-none', len === 0);
                    if (len > 0) this._filterSearchResults(input.value.toLowerCase());
                }, CONFIG.search.debounceDelay);

                input.addEventListener('focus', () => {
                    const len = input.value.length;
                    dropdown.classList.toggle('show', len > 0);
                    closeBtn?.classList.toggle('d-none', len === 0);
                });
                input.addEventListener('keyup', debouncedFilter);

                if (closeBtn) {
                    closeBtn.addEventListener('click', () => {
                        input.value = '';
                        dropdown.classList.remove('show');
                        closeBtn.classList.add('d-none');
                        this._showAllSearchItems();
                    });
                }

                document.body.addEventListener('click', (e) => {
                    if (e.target.getAttribute('id') !== 'search-options') dropdown.classList.remove('show');
                });
            }

            // Mobile / responsive search
            const rInput = document.getElementById('search-options-reponsive');
            const rDropdown = document.getElementById('search-dropdown-reponsive');
            if (rInput && rDropdown) {
                const rClose = document.getElementById('search-close-options');

                rInput.addEventListener('focus', () => {
                    rDropdown.classList.toggle('show', rInput.value.length > 0);
                    rClose?.classList.toggle('d-none', rInput.value.length === 0);
                });
                rInput.addEventListener('keyup', () => {
                    rDropdown.classList.toggle('show', rInput.value.length > 0);
                    rClose?.classList.toggle('d-none', rInput.value.length === 0);
                });
                if (rClose) rClose.addEventListener('click', () => {
                    rInput.value = '';
                    rDropdown.classList.remove('show');
                    rClose.classList.add('d-none');
                });
                document.body.addEventListener('click', (e) => {
                    if (e.target.getAttribute('id') !== 'search-options-reponsive') {
                        rDropdown.classList.remove('show');
                        rClose?.classList.add('d-none');
                    }
                });
            }
        },

        _filterSearchResults(term) {
            document.querySelectorAll('.notify-item').forEach(item => {
                let text = '';
                if (item.querySelector('h6')) {
                    const span = item.getElementsByTagName('span')[0]?.innerText.toLowerCase() || '';
                    const name = item.querySelector('h6').innerText.toLowerCase();
                    text = name.includes(term) ? name : span;
                } else if (item.getElementsByTagName('span').length > 0) {
                    text = item.getElementsByTagName('span')[0].innerText.toLowerCase();
                }
                item.style.display = text.includes(term) ? 'block' : 'none';
            });
        },

        _showAllSearchItems() {
            document.querySelectorAll('.notify-item').forEach(item => {
                item.style.display = 'block';
            });
        },

        // ==========================
        // UI Components (was ComponentsManager)
        // ==========================
        _initUIComponents() {
            // Bootstrap tooltips
            document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
                new bootstrap.Tooltip(el);
            });

            // Bootstrap popovers
            document.querySelectorAll('[data-bs-toggle="popover"]').forEach(el => {
                new bootstrap.Popover(el);
            });

            // Dropdown tabs
            document.querySelectorAll('.dropdown-menu a[data-bs-toggle="tab"]').forEach(el => {
                el.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const tab = bootstrap.Tab.getInstance(e.target);
                    if (tab) tab.show();
                });
            });

            // Counter animations
            document.querySelectorAll('.counter-value').forEach(counter => {
                const target = +counter.getAttribute('data-target');
                const speed = CONFIG.counter.speed;
                const animate = () => {
                    const count = +counter.innerText;
                    const inc = target / speed;
                    if (count < target) {
                        counter.innerText = Math.ceil(count + inc);
                        setTimeout(animate, 1);
                    } else {
                        counter.innerText = numberWithCommas(target);
                    }
                };
                animate();
            });
        },

        /**
         * Reinitialize UI components after dynamic content load (e.g. HTMX swaps)
         */
        reinitUIComponents() {
            this._initUIComponents();
        },

        // ==========================
        // Feather icons
        // ==========================
        _initFeatherIcons() {
            if (typeof feather !== 'undefined') feather.replace();
        },

        // ==========================
        // Waves effect
        // ==========================
        _initWaves() {
            if (typeof Waves !== 'undefined') {
                Waves.init();
            }
        },

        // ==========================
        // Layout-level setters (for customizer panel usage)
        // ==========================
        setLayoutWidth(width) {
            document.documentElement.setAttribute('data-layout-width', width);
            Storage.set('data-layout-width', width);
        },

        setLayoutPosition(position) {
            document.documentElement.setAttribute('data-layout-position', position);
            Storage.set('data-layout-position', position);
        },

        /**
         * Reset all settings and reload.
         */
        reset() {
            sessionStorage.clear();
            LAYOUT_KEYS.forEach(k => localStorage.removeItem(k));
            window.location.reload();
        }
    });

    // =========================================================================
    // Expose Storage globally (used by layout.js in <head> and some pages)
    // =========================================================================
    window.StorageService = Storage;
});
