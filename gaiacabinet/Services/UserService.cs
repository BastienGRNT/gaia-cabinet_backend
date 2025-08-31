using gaiacabinet_api.Contracts;
using gaiacabinet_api.Database;
using Microsoft.EntityFrameworkCore;

namespace gaiacabinet_api.Services;

public interface IUserService
{
    Task<UserDto> GetCurrentUserAsync(int userId, CancellationToken ct);
}

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<UserDto> GetCurrentUserAsync(int userId, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);
        
        if (user is null) throw new UnauthorizedAccessException("user_not_found");
        
        return new UserDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = new RoleDto
            {
                RoleId = user.Role.RoleId,
                Label = user.Role.Label,
            }
        };
    }
}