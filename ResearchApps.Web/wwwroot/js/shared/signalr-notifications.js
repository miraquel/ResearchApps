/**
 * Shared SignalR Notification Utilities
 * Provides connection management, deduplication, and Alpine.js toast integration.
 * Used by both PR and CO notification managers.
 */
const SignalRNotifications = (function () {
    const notificationTTL = 2000;

    /**
     * Create and configure a SignalR hub connection.
     * @param {string} hubUrl - Hub endpoint (e.g. '/hubs/pr-notifications')
     * @returns {{ connection: signalR.HubConnection, state: { isConnected: boolean } }}
     */
    function createConnection(hubUrl) {
        if (typeof signalR === 'undefined') {
            console.error('SignalR library not loaded');
            return null;
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .withStatefulReconnect()
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        const state = { isConnected: false };

        connection.onreconnecting(() => {
            state.isConnected = false;
            console.log(`[SignalR] Reconnecting to ${hubUrl}...`);
        });

        connection.onreconnected(() => {
            state.isConnected = true;
            console.log(`[SignalR] Reconnected to ${hubUrl}`);
        });

        connection.onclose(() => {
            state.isConnected = false;
            console.log(`[SignalR] Connection to ${hubUrl} closed`);
        });

        return { connection, state };
    }

    /**
     * Start a connection with retry logic for initial connection failures.
     * @param {signalR.HubConnection} connection
     * @param {{ isConnected: boolean }} state
     * @param {string} label - Label for logging (e.g. 'PR')
     */
    async function startConnection(connection, state, label) {
        try {
            await connection.start();
            state.isConnected = true;
            console.log(`[SignalR] Connected to ${label} hub`);
        } catch (err) {
            console.error(`[SignalR] ${label} connection error:`, err);
            state.isConnected = false;
            setTimeout(() => startConnection(connection, state, label), 5000);
        }
    }

    /**
     * Deduplication tracker factory.
     * @returns {{ shouldShow: (key: string) => boolean }}
     */
    function createDeduplicator() {
        const shown = new Map();
        return {
            shouldShow(key) {
                const now = Date.now();
                for (const [k, ts] of shown.entries()) {
                    if (now - ts > notificationTTL) shown.delete(k);
                }
                if (shown.has(key)) return false;
                shown.set(key, now);
                return true;
            }
        };
    }

    /**
     * Build a deduplication key from a notification object.
     * @param {object} notification
     * @returns {string}
     */
    function notificationKey(notification) {
        return `${notification.type}_${notification.entityId}_${notification.recId}_${notification.timestamp}`;
    }

    /**
     * Show a toast via Alpine.js store (XSS-safe, text only).
     * Falls back to console.log if Alpine is not available.
     * @param {string} message - Plain text message
     * @param {'success'|'danger'|'warning'|'info'} type
     */
    function showToast(message, type) {
        if (window.Alpine && Alpine.store('toast')) {
            switch (type) {
                case 'success': Alpine.store('toast').success(message); break;
                case 'danger':  Alpine.store('toast').error(message);   break;
                case 'warning': Alpine.store('toast').warning(message); break;
                default:        Alpine.store('toast').show(message, type); break;
            }
        } else {
            console.log(`[${type.toUpperCase()}] ${message}`);
        }
    }

    /** Play notification sound. */
    function playSound() {
        try {
            const audio = new Audio('/assets/sounds/notification.mp3');
            audio.volume = 0.5;
            audio.play().catch(() => {});
        } catch { /* sound not available */ }
    }

    /** Refresh the notification bell if NotificationManager exists. */
    function refreshBell() {
        if (typeof NotificationManager !== 'undefined') {
            if (NotificationManager.loadUnreadNotifications) NotificationManager.loadUnreadNotifications();
            if (NotificationManager.loadNotifications) NotificationManager.loadNotifications();
            if (NotificationManager.loadNotificationCount) NotificationManager.loadNotificationCount();
        }
    }

    return {
        createConnection,
        startConnection,
        createDeduplicator,
        notificationKey,
        showToast,
        playSound,
        refreshBell
    };
})();
