using Dapper;
using RideHailingApi_Dapper.Data;
using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Repository.Interfaces;

namespace RideHailingApi_Dapper.Repository.Implementations;

public class AuthRepository : IAuthRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public AuthRepository(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
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

    public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            WHERE PhoneNumber = @PhoneNumber
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            sql,
            new { PhoneNumber = phoneNumber });
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            WHERE Id = @UserId
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            sql,
            new { UserId = userId });
    }

    public async Task<User> CreateUserAsync(User user)
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

            SELECT *
            FROM Users
            WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<User>(
            sql,
            user);
    }

    public async Task<EmailOtp?> GetActiveEmailOtpAsync(int userId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT TOP 1 *
            FROM EmailOtps
            WHERE UserId = @UserId
              AND IsUsed = 0
              AND ExpiresAt > @Now
            ORDER BY CreatedAt DESC
            """;

        return await connection.QuerySingleOrDefaultAsync<EmailOtp>(
            sql,
            new
            {
                UserId = userId,
                Now = DateTime.UtcNow
            });
    }

    public async Task<EmailOtp> CreateEmailOtpAsync(EmailOtp otp)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO EmailOtps
            (
                UserId,
                Code,
                ExpiresAt,
                IsUsed,
                VerifiedAt,
                CreatedAt
            )
            VALUES
            (
                @UserId,
                @Code,
                @ExpiresAt,
                @IsUsed,
                @VerifiedAt,
                @CreatedAt
            );

            SELECT *
            FROM EmailOtps
            WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<EmailOtp>(
            sql,
            otp);
    }

    public async Task UpdateEmailOtpAsync(EmailOtp otp)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE EmailOtps
            SET
                Code = @Code,
                ExpiresAt = @ExpiresAt,
                IsUsed = @IsUsed,
                VerifiedAt = @VerifiedAt
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, otp);
    }

    public async Task<PhoneOtp?> GetActivePhoneOtpAsync(int userId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT TOP 1 *
            FROM PhoneOtps
            WHERE UserId = @UserId
              AND IsUsed = 0
              AND ExpiresAt > @Now
            ORDER BY CreatedAt DESC
            """;

        return await connection.QuerySingleOrDefaultAsync<PhoneOtp>(
            sql,
            new
            {
                UserId = userId,
                Now = DateTime.UtcNow
            });
    }

    public async Task<PhoneOtp> CreatePhoneOtpAsync(PhoneOtp otp)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO PhoneOtps
            (
                UserId,
                Code,
                ExpiresAt,
                IsUsed,
                VerifiedAt,
                CreatedAt
            )
            VALUES
            (
                @UserId,
                @Code,
                @ExpiresAt,
                @IsUsed,
                @VerifiedAt,
                @CreatedAt
            );

            SELECT *
            FROM PhoneOtps
            WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<PhoneOtp>(
            sql,
            otp);
    }

    public async Task UpdatePhoneOtpAsync(PhoneOtp otp)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE PhoneOtps
            SET
                Code = @Code,
                ExpiresAt = @ExpiresAt,
                IsUsed = @IsUsed,
                VerifiedAt = @VerifiedAt
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, otp);
    }

    public async Task<PasswordResetOtp?> GetActivePasswordResetOtpAsync(
        int userId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT TOP 1 *
            FROM PasswordResetOtps
            WHERE UserId = @UserId
              AND IsUsed = 0
              AND ExpiresAt > @Now
            ORDER BY CreatedAt DESC
            """;

        return await connection.QuerySingleOrDefaultAsync<PasswordResetOtp>(
            sql,
            new
            {
                UserId = userId,
                Now = DateTime.UtcNow
            });
    }

    public async Task<PasswordResetOtp> CreatePasswordResetOtpAsync(
        PasswordResetOtp otp)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO PasswordResetOtps
            (
                UserId,
                Code,
                ExpiresAt,
                IsUsed,
                CreatedAt
            )
            VALUES
            (
                @UserId,
                @Code,
                @ExpiresAt,
                @IsUsed,
                @CreatedAt
            );

            SELECT *
            FROM PasswordResetOtps
            WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<PasswordResetOtp>(
            sql,
            otp);
    }

    public async Task UpdatePasswordResetOtpAsync(
        PasswordResetOtp otp)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE PasswordResetOtps
            SET
                Code = @Code,
                ExpiresAt = @ExpiresAt,
                IsUsed = @IsUsed
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, otp);
    }

    public async Task UpdateUserAsync(User user)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE Users
            SET
                FullName = @FullName,
                Email = @Email,
                PhoneNumber = @PhoneNumber,
                PasswordHash = @PasswordHash,
                Role = @Role,
                IsEmailVerified = @IsEmailVerified,
                IsPhoneNumberVerified = @IsPhoneNumberVerified,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, user);
    }

    public async Task<DriverProfile> CreateDriverProfileAsync(
        DriverProfile driverProfile)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO DriverProfiles
            (
                UserId,
                IsAvailable,
                Status,
                ApprovalReason,
                ApprovedAt
            )
            VALUES
            (
                @UserId,
                @IsAvailable,
                @Status,
                @ApprovalReason,
                @ApprovedAt
            );

            SELECT *
            FROM DriverProfiles
            WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<DriverProfile>(
            sql,
            driverProfile);
    }
    
}