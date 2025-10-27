using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;
using System.Data;
using Dapper;

namespace ResearchApps.Repo
{
    public class StatusRepo : IStatusRepo
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDbTransaction _dbTransaction;
        public StatusRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            _dbConnection = dbConnection;
            _dbTransaction = dbTransaction;
        }
        public async Task<IEnumerable<Status>> StatusCboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
        {
            const string query = "StatusCbo";
            await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
            var parameters = new DynamicParameters();
            
            if (cboRequest.Id > 0)
            {
                parameters.Add("@Id", cboRequest.Id);
            }
            
            if (!string.IsNullOrEmpty(cboRequest.Term))
            {
                parameters.Add("@Term", cboRequest.Term);
            }
        
            var command = new CommandDefinition(
                query,
                parameters,
                _dbTransaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure);
        
            return await _dbConnection.QueryAsync<Status>(command);
        }
    }
}

