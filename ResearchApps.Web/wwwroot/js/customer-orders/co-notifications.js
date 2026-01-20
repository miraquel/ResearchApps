/**
 * CO Notification Hub Client
 * Provides real-time notifications for Customer Order workflow actions using SignalR
 */

// CO Notification Manager
const CoNotificationManager = (function () {
    let connection = null;
    let isConnected = false;
    let reconnectAttempts = 0;
    const maxReconnectAttempts = 5;
    
    // Notification deduplication - track recently shown notifications
    const shownNotifications = new Map();
    const notificationTTL = 2000; // 2 seconds

    // Initialize the SignalR connection
    function init() {
        if (typeof signalR === 'undefined') {
            console.error('SignalR library not loaded');
            return;
        }

        connection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/co-notifications')
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Set up event handlers
        setupEventHandlers();

        // Start the connection
        startConnection();
    }

    // Start the SignalR connection
    async function startConnection() {
        try {
            await connection.start();
            isConnected = true;
            reconnectAttempts = 0;
            console.log('SignalR Connected to CO Notification Hub');
            
            // Dispatch connected event
            document.dispatchEvent(new CustomEvent('coNotificationConnected'));
        } catch (err) {
            console.error('SignalR Connection Error:', err);
            isConnected = false;
            
            // Retry connection
            if (reconnectAttempts < maxReconnectAttempts) {
                reconnectAttempts++;
                setTimeout(startConnection, 5000);
            }
        }
    }

    // Set up SignalR event handlers
    function setupEventHandlers() {
        // Handle reconnecting
        connection.onreconnecting((error) => {
            isConnected = false;
            console.log('SignalR Reconnecting...', error);
        });

        // Handle reconnected
        connection.onreconnected((connectionId) => {
            isConnected = true;
            console.log('SignalR Reconnected:', connectionId);
        });

        // Handle close
        connection.onclose((error) => {
            isConnected = false;
            console.log('SignalR Connection Closed:', error);
        });

        // Handle pending approval notifications
        connection.on('ReceivePendingApproval', (notification) => {
            handlePendingApproval(notification);
        });

        // Handle CO rejected notifications
        connection.on('ReceiveCoRejected', (notification) => {
            handleCoRejected(notification);
        });

        // Handle general notifications
        connection.on('ReceiveNotification', (notification) => {
            handleGeneralNotification(notification);
        });

        // Handle CO status changed (broadcast to all viewing the CO)
        connection.on('CoStatusChanged', (notification) => {
            handleCoStatusChanged(notification);
        });
    }

    // Handle pending approval notification
    function handlePendingApproval(notification) {
        console.log('CO Pending Approval:', notification);
        
        // Create a unique key for this notification
        const notificationKey = `${notification.Type}_${notification.CoId}_${notification.RecId}_${notification.Timestamp}`;
        
        // Check if we've already shown this notification recently (prevent duplicates)
        if (shouldShowNotification(notificationKey)) {
            let title = 'Pending Approval';
            
            // Determine title based on notification type
            if (notification.Type === 'CoSubmitted') {
                title = 'New Customer Order Submitted';
            } else if (notification.Type === 'CoApproved') {
                title = 'Customer Order Approved - Your Turn';
            }
            
            // Show toast notification
            showNotification(notification.Message, 'warning', title);
            
            // Play notification sound
            playNotificationSound();
        }
        
        // Dispatch event for UI updates (notification bell refresh)
        document.dispatchEvent(new CustomEvent('coPendingApproval', { detail: notification }));
    }

    // Handle CO rejected notification
    function handleCoRejected(notification) {
        console.log('CO Rejected:', notification);
        
        // Create a unique key for this notification
        const notificationKey = `${notification.Type}_${notification.CoId}_${notification.RecId}_${notification.Timestamp}`;
        
        // Check if we've already shown this notification recently
        if (shouldShowNotification(notificationKey)) {
            // Show toast notification for rejection
            showNotification(notification.Message, 'danger', 'Customer Order Rejected');
            
            // Play notification sound
            playNotificationSound();
        }
        
        // Dispatch event for UI updates
        document.dispatchEvent(new CustomEvent('coRejected', { detail: notification }));
    }

    // Handle general notification
    function handleGeneralNotification(notification) {
        console.log('General Notification:', notification);
        showNotification(notification.Message, 'info', 'Notification');
        
        // Dispatch custom event
        document.dispatchEvent(new CustomEvent('generalNotification', { detail: notification }));
    }

    // Handle CO status changed (for updating UI across all users viewing the same CO)
    function handleCoStatusChanged(notification) {
        console.log('CO Status Changed:', notification);
        
        // Don't show toast here - only dispatch event for UI updates
        document.dispatchEvent(new CustomEvent('coStatusChanged', { detail: notification }));
        
        // Trigger page refresh or specific UI update if needed
        if (typeof refreshCoTable === 'function') {
            refreshCoTable();
        }
    }
    
    // Check if notification should be shown (deduplication)
    function shouldShowNotification(notificationKey) {
        const now = Date.now();
        
        // Clean up old entries
        for (const [key, timestamp] of shownNotifications.entries()) {
            if (now - timestamp > notificationTTL) {
                shownNotifications.delete(key);
            }
        }
        
        // Check if this notification was recently shown
        if (shownNotifications.has(notificationKey)) {
            return false;
        }
        
        // Mark as shown
        shownNotifications.set(notificationKey, now);
        return true;
    }

    // Play notification sound
    function playNotificationSound() {
        try {
            const audio = new Audio('/assets/sounds/notification.mp3');
            audio.volume = 0.5;
            audio.play().catch(err => {
                // Ignore audio play errors (user hasn't interacted with page)
                console.log('Could not play notification sound:', err.message);
            });
        } catch (e) {
            console.log('Notification sound not available');
        }
    }

    // Show toast notification
    function showNotification(message, type = 'info', title = 'Notification') {
        // Check if Toastify is available (Velzon template)
        if (typeof Toastify !== 'undefined') {
            let backgroundColor = '#0ab39c'; // success
            switch (type) {
                case 'danger':
                case 'error':
                    backgroundColor = '#f06548';
                    break;
                case 'warning':
                    backgroundColor = '#f7b84b';
                    break;
                case 'info':
                    backgroundColor = '#299cdb';
                    break;
            }
            
            Toastify({
                text: `<strong>${title}</strong><br/>${message}`,
                duration: 5000,
                close: true,
                gravity: 'top',
                position: 'right',
                escapeMarkup: false,
                style: {
                    background: backgroundColor
                }
            }).showToast();
        }
        // Fallback to Bootstrap toast if available
        else if (typeof bootstrap !== 'undefined' && document.getElementById('notification-toast-container')) {
            const toastHtml = `
                <div class="toast" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="toast-header bg-${type === 'error' ? 'danger' : type}">
                        <strong class="me-auto text-white">${title}</strong>
                        <small class="text-white">just now</small>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                    <div class="toast-body">
                        ${message}
                    </div>
                </div>
            `;
            
            const container = document.getElementById('notification-toast-container');
            container.insertAdjacentHTML('beforeend', toastHtml);
            const toastElement = container.lastElementChild;
            const toast = new bootstrap.Toast(toastElement);
            toast.show();
            
            // Remove toast element after it's hidden
            toastElement.addEventListener('hidden.bs.toast', () => {
                toastElement.remove();
            });
        }
        // Fallback to console
        else {
            console.log(`[${type.toUpperCase()}] ${title}: ${message}`);
        }
    }

    // Join a specific CO notification group
    async function joinCoGroup(coId) {
        if (connection && isConnected) {
            try {
                await connection.invoke('JoinCoGroup', coId);
                console.log(`Joined CO group: ${coId}`);
            } catch (err) {
                console.error('Error joining CO group:', err);
            }
        }
    }

    // Leave a specific CO notification group
    async function leaveCoGroup(coId) {
        if (connection && isConnected) {
            try {
                await connection.invoke('LeaveCoGroup', coId);
                console.log(`Left CO group: ${coId}`);
            } catch (err) {
                console.error('Error leaving CO group:', err);
            }
        }
    }

    // Check connection status
    function getConnectionStatus() {
        return isConnected;
    }

    // Public API
    return {
        init: init,
        joinCoGroup: joinCoGroup,
        leaveCoGroup: leaveCoGroup,
        isConnected: getConnectionStatus,
        showNotification: showNotification
    };
})();

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    // Only initialize if SignalR library is loaded
    if (typeof signalR !== 'undefined') {
        CoNotificationManager.init();
    }
});

// Listen for notification events to refresh notification bell
document.addEventListener('coPendingApproval', function() {
    // Trigger notification bell refresh if NotificationManager exists
    if (typeof NotificationManager !== 'undefined' && NotificationManager.loadNotifications) {
        NotificationManager.loadNotifications();
    }
});

document.addEventListener('coRejected', function() {
    // Trigger notification bell refresh if NotificationManager exists
    if (typeof NotificationManager !== 'undefined' && NotificationManager.loadNotifications) {
        NotificationManager.loadNotifications();
    }
});
