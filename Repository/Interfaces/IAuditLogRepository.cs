using RideHailingApi_Dapper.Domain.Entities;

namespace RideHailingApi_Dapper.Repository.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog);
}