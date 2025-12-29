using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class NotificationRepo : INotificationRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public NotificationRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<int> NotificationInsert(Notification notification, CancellationToken cancellationToken)
    {
        const string query = "Notification_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", notification.UserId);
        parameters.Add("@Title", notification.Title);
        parameters.Add("@Message", notification.Message);
        parameters.Add("@Url", notification.Url);
        parameters.Add("@NotificationType", notification.NotificationType);
        parameters.Add("@RefId", notification.RefId);
        parameters.Add("@RefRecId", notification.RefRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QuerySingleAsync<int>(command);
        return result;
    }

    public async Task<IEnumerable<Notification>> NotificationSelectByUserId(string userId, int take, CancellationToken cancellationToken)
    {
        const string query = "Notification_SelectByUserId";
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@Take", take);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryAsync<Notification>(command);
        return result;
    }

    public async Task<IEnumerable<Notification>> NotificationSelectUnreadByUserId(string userId, CancellationToken cancellationToken)
    {
        const string query = "Notification_SelectUnreadByUserId";
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryAsync<Notification>(command);
        return result;
    }

    public async Task NotificationMarkAsRead(int notificationId, string userId, CancellationToken cancellationToken)
    {
        const string query = "Notification_MarkAsRead";
        var parameters = new DynamicParameters();
        parameters.Add("@NotificationId", notificationId);
        parameters.Add("@UserId", userId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task NotificationMarkAllAsRead(string userId, CancellationToken cancellationToken)
    {
        const string query = "Notification_MarkAllAsRead";
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<(int TotalCount, int UnreadCount)> NotificationGetCount(string userId, CancellationToken cancellationToken)
    {
        const string query = "Notification_GetCount";
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<(int TotalCount, int UnreadCount)>(command);
        return result;
    }

    public async Task NotificationDelete(int notificationId, string userId, CancellationToken cancellationToken)
    {
        const string query = "Notification_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@NotificationId", notificationId);
        parameters.Add("@UserId", userId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }
}

