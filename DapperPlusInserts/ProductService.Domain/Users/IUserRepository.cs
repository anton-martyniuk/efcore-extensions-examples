namespace ProductService.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    
    Task AddAsync(User user);
    
    Task UpdateAsync(User user);
    
    Task DeleteAsync(int id);
}