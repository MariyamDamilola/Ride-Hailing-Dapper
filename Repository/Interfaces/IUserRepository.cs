using RideHailingApi_Dapper.Domain.Entities;

namespace RideHailingApi_Dapper.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<int> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeactivateAsync(int id);
}