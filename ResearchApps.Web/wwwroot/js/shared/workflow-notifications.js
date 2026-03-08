/**
 * Unified Workflow Notification Manager
 * Handles real-time SignalR notifications for all workflow entities (PR, CO, PO, etc.)
 * Single hub connection to /hubs/workflow — extensible for any future entity.
 */
const WorkflowNotificationManager = (function () {
    const S = SignalRNotifications;
    let conn, state, dedup;
    const joinedGroups = new Set();
    const pendingJoins = new Set();

    async function init() {
        const result = S.createConnection('/hubs/workflow');
        if (!result) return;

        conn = result.connection;
        state = result.state;
        dedup = S.createDeduplicator();

        setupHandlers();
        await S.startConnection(conn, state, 'Workflow');

        // Flush any groups that were queued before the connection was ready
        pendingJoins.forEach(function (group) {
            const parts = group.split('_');
            if (parts.length >= 2) {
                conn.invoke('JoinEntityGroup', parts[0], parts.slice(1).join('_')).catch(function () {});
            }
        });
        pendingJoins.clear();

        conn.onreconnected(function () {
            joinedGroups.forEach(function (group) {
                const parts = group.split('_');
                if (parts.length >= 2) {
                    conn.invoke('JoinEntityGroup', parts[0], parts.slice(1).join('_')).catch(function () {});
                }
            });
        });
    }

    function setupHandlers() {
        conn.on('ReceivePendingApproval', function (notification) {
            if (dedup.shouldShow(S.notificationKey(notification))) {
                S.showToast('Pending Approval: ' + notification.message, 'warning');
                S.playSound();
            }
            S.refreshBell();
            document.dispatchEvent(new CustomEvent('workflow:pendingApproval', { detail: notification }));
        });

        conn.on('ReceiveRejected', function (notification) {
            if (dedup.shouldShow(S.notificationKey(notification))) {
                S.showToast('Rejected: ' + notification.message, 'danger');
                S.playSound();
            }
            S.refreshBell();
            document.dispatchEvent(new CustomEvent('workflow:rejected', { detail: notification }));
        });

        conn.on('EntityStatusChanged', function (notification) {
            document.dispatchEvent(new CustomEvent('workflow:statusChanged', { detail: notification }));
        });

        conn.on('ReceiveNotification', function (notification) {
            S.showToast(notification.message, 'info');
            document.dispatchEvent(new CustomEvent('workflow:notification', { detail: notification }));
        });
    }

    function joinEntityGroup(entityType, entityId) {
        var groupKey = entityType + '_' + entityId;
        joinedGroups.add(groupKey);
        if (!conn || !state.isConnected) {
            // Connection not ready yet — queue until init() finishes connecting
            pendingJoins.add(groupKey);
            return;
        }
        conn.invoke('JoinEntityGroup', entityType, entityId).catch(function (err) {
            console.error('Error joining ' + entityType + ' group:', err);
        });
    }

    function leaveEntityGroup(entityType, entityId) {
        if (!conn || !state.isConnected) return;
        var groupKey = entityType + '_' + entityId;
        joinedGroups.delete(groupKey);
        conn.invoke('LeaveEntityGroup', entityType, entityId).catch(function (err) {
            console.error('Error leaving ' + entityType + ' group:', err);
        });
    }

    return {
        init: init,
        joinEntityGroup: joinEntityGroup,
        leaveEntityGroup: leaveEntityGroup,
        isConnected: function () { return state ? state.isConnected : false; }
    };
})();

// Auto-initialize when loaded
document.addEventListener('DOMContentLoaded', function () {
    WorkflowNotificationManager.init();
});
