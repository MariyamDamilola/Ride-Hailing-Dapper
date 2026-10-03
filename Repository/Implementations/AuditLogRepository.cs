using Dapper;
using RideHailingApi_Dapper.Data;
using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Repository.Interfaces;

namespace RideHailingApi_Dapper.Repository.Implementations;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public AuditLogRepository(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
                                              INSERT INTO AuditLogs
                                              (    UserId,
                                                   UserRole,
                                                   Action,
                                                   Status,
                                                   EntityType,
                                                   EntityId,
                                                   Description,
                                                   CreatedAt
                                               )
                                               VALUES
                                                   (
                                                       @UserId,
                                                       @UserRole,
                                                       @Action,
                                                       @Status,
                                                           @EntityType,
                                                       @EntityId,
                                                       @Description,
                                                       @CreatedAt
                                                   );
                           SELECT *
                               FROM AuditLogs
                           WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
                           """;

        return await connection.QuerySingleAsync<AuditLog>(
            sql,
            auditLog);
    }
}
                              