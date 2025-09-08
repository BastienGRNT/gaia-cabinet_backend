using Microsoft.EntityFrameworkCore;
using GaiaSolution.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GaiaSolution.Infrastructure.Database;

public class CoreDbContext : DbContext
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
    { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserCredential> UserCredentials { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<UserLoginHistory> UserLoginHistories { get; set; }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<DoctorProfile> DoctorProfiles { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
    
    /* Potentielement ce code
     * protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
     */
    
    
    
    
}