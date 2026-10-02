using Dapper;
using RideHailingApi_Dapper.Data;
using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Repository.Interfaces;

namespace RideHailingApi_Dapper.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public UserRepository(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            WHERE Id = @Id
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            sql,
            new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            WHERE Email = @Email
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            sql,
            new { Email = email });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Users
            (
                FullName,
                Email,
                PhoneNumber,
                PasswordHash,
                Role,
                IsEmailVerified,
                IsPhoneNumberVerified,
                IsActive,
                CreatedAt,
                UpdatedAt
            )
            VALUES
            (
                @FullName,
                @Email,
                @PhoneNumber,
                @PasswordHash,
                @Role,
                @IsEmailVerified,
                @IsPhoneNumberVerified,
                @IsActive,
                @CreatedAt,
                @UpdatedAt
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE Users
            SET
                FullName = @FullName,
                PhoneNumber = @PhoneNumber,
                IsEmailVerified = @IsEmailVerified,
                IsPhoneNumberVerified = @IsPhoneNumberVerified,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, user);

        return rowsAffected > 0;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE Users
            SET
                IsActive = 0,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                UpdatedAt = DateTime.UtcNow
            });

        return rowsAffected > 0;
    }

}