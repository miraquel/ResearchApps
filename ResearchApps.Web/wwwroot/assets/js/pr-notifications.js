/**
 * PR Notification Hub Client
 * Provides real-time notifications for PR workflow actions using SignalR
 */

// PR Notification Manager
const PrNotificationManager = (function () {
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
            .withUrl('/hubs/pr-notifications')
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
            console.log('SignalR Connected to PR Notification Hub');
            
            // Dispatch connected event
            document.dispatchEvent(new CustomEvent('prNotificationConnected'));
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
            showNotification('Connection lost. Reconnecting...', 'warning');
        });

        // Handle reconnected
        connection.onreconnected((connectionId) => {
            isConnected = true;
            console.log('SignalR Reconnected:', connectionId);
            showNotification('Connection restored', 'success');
        });

        // Handle close
        connection.onclose((error) => {
            isConnected = false;
            console.log('SignalR Connection Closed:', error);
        });

        // Handle PR notifications
        connection.on('ReceivePrNotification', (notification) => {
            handlePrNotification(notification);
        });

        // Handle pending approval notifications
        connection.on('ReceivePendingApproval', (notification) => {
            handlePendingApproval(notification);
        });

        // Handle PR rejected notifications
        connection.on('ReceivePrRejected', (notification) => {
            handlePrRejected(notification);
        });

        // Handle general notifications
        connection.on('ReceiveNotification', (notification) => {
            handleGeneralNotification(notification);
        });

        // Handle PR status changed (broadcast to all)
        connection.on('PrStatusChanged', (notification) => {
            handlePrStatusChanged(notification);
        });
    }

    // Handle PR notification
    function handlePrNotification(notification) {
        console.log('PR Notification:', notification);
        
        // Create a unique key for this notification
        const notificationKey = `${notification.Type}_${notification.PrId}_${notification.RecId}_${notification.Timestamp}`;
        
        // Check if we've already shown this notification recently
        if (shouldShowNotification(notificationKey)) {
            let notificationType = 'info';
            let title = 'PR Notification';
            
            switch (notification.Type) {
                case 'PrSubmitted':
                    notificationType = 'info';
                    title = 'PR Submitted';
                    break;
                case 'PrApproved':
                    notificationType = 'success';
                    title = 'PR Approved';
                    break;
                case 'PrFullyApproved':
                    notificationType = 'success';
                    title = 'PR Fully Approved';
                    break;
                case 'PrRejected':
                    notificationType = 'danger';
                    title = 'PR Rejected';
                    break;
                case 'PrRecalled':
                    notificationType = 'warning';
                    title = 'PR Recalled';
                    break;
            }
            
            showNotification(notification.Message, notificationType, title);
        }
        
        // Dispatch custom event for page-specific handling
        document.dispatchEvent(new CustomEvent('prNotification', { detail: notification }));
    }

    // Handle pending approval notification
    function handlePendingApproval(notification) {
        console.log('Pending Approval:', notification);
        
        // Create a unique key for this notification
        const notificationKey = `${notification.Type}_${notification.PrId}_${notification.RecId}_${notification.Timestamp}`;
        
        // Check if we've already shown this notification recently (prevent duplicates)
        if (shouldShowNotification(notificationKey)) {
            let title = 'Pending Approval';
            
            // Determine title based on notification type
            if (notification.Type === 'PrSubmitted') {
                title = 'New PR Submitted';
            } else if (notification.Type === 'PrApproved') {
                title = 'PR Approved - Your Turn';
            }
            
            // Show toast notification
            showNotification(notification.Message, 'info', title);
        }
        
        // Dispatch event for UI updates
        document.dispatchEvent(new CustomEvent('prPendingApproval', { detail: notification }));
    }

    // Handle PR rejected notification
    function handlePrRejected(notification) {
        console.log('PR Rejected:', notification);
        
        // Create a unique key for this notification
        const notificationKey = `${notification.Type}_${notification.PrId}_${notification.RecId}_${notification.Timestamp}`;
        
        // Check if we've already shown this notification recently
        if (shouldShowNotification(notificationKey)) {
            // Show toast notification for rejection
            showNotification(notification.Message, 'danger', 'PR Rejected');
        }
        
        // Dispatch event for UI updates
        document.dispatchEvent(new CustomEvent('prRejected', { detail: notification }));
    }

    // Handle general notification
    function handleGeneralNotification(notification) {
        console.log('General Notification:', notification);
        showNotification(notification.Message, 'info', 'Notification');
        
        // Dispatch custom event
        document.dispatchEvent(new CustomEvent('generalNotification', { detail: notification }));
    }

    // Handle PR status changed (for updating UI across all users)
    function handlePrStatusChanged(notification) {
        console.log('PR Status Changed:', notification);
        
        // Don't show toast here - only dispatch event for UI updates
        document.dispatchEvent(new CustomEvent('prStatusChanged', { detail: notification }));
        
        // Refresh the table if on the PR index page
        if (typeof refreshPrTable === 'function') {
            refreshPrTable();
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

    // Join a specific PR notification group
    async function joinPrGroup(prId) {
        if (connection && isConnected) {
            try {
                await connection.invoke('JoinPrGroup', prId);
                console.log(`Joined PR group: ${prId}`);
            } catch (err) {
                console.error('Error joining PR group:', err);
            }
        }
    }

    // Leave a specific PR notification group
    async function leavePrGroup(prId) {
        if (connection && isConnected) {
            try {
                await connection.invoke('LeavePrGroup', prId);
                console.log(`Left PR group: ${prId}`);
            } catch (err) {
                console.error('Error leaving PR group:', err);
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
        joinPrGroup: joinPrGroup,
        leavePrGroup: leavePrGroup,
        isConnected: getConnectionStatus,
        showNotification: showNotification
    };
})();

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    // Only initialize if SignalR library is loaded
    if (typeof signalR !== 'undefined') {
        PrNotificationManager.init();
    }
});

