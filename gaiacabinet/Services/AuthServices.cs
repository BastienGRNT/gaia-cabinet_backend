using gaiacabinet_api.Database;
using Microsoft.EntityFrameworkCore;

namespace gaiacabinet_api.Services;

public class AuthServices
{
    private readonly AppDbContext _db;
    public AuthServices(AppDbContext db) => _db = db;
    
    public async Task<string> Lookup(string mail)
    {
        var TrimMail = mail.Trim().ToLowerInvariant();
        
        var existe = await _db.Users.AnyAsync(u => u.Mail.ToLower() == TrimMail);
        if (existe) return "existing_user";
        
        var pending = await _db.PendingUsers.AnyAsync(p => p.Mail.ToLower() == TrimMail);
        if (existe) return "pending_user";
        
        return "not_allowed";
    }
}