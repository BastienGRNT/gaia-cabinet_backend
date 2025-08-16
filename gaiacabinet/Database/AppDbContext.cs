using Microsoft.EntityFrameworkCore;
using gaiacabinet_api.Models;


namespace gaiacabinet_api.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    // Tables SQL
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PendingUser> PendingUsers => Set<PendingUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Table Role
        modelBuilder.Entity<Role>(e =>
        {
            e.HasKey(x => x.RoleId);
            e.Property(x => x.Label).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Label).IsUnique();
            
            // Relation Role <-0:N-> <-1:1-> User
            e.HasMany(x => x.Users).WithOne(x => x.Role).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });
        
        // Table User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.UserId);
            e.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(50).IsRequired();
            e.Property(x => x.Mail).HasMaxLength(250).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
            e.Property(x => x.OrdreRegistrationNumber).HasMaxLength(20);
            e.Property(x => x.Authorized).HasDefaultValue(true);
            
            e.HasIndex(x => x.Mail).IsUnique();
            e.HasIndex(x => x.Phone).IsUnique();
            e.HasIndex(x => x.OrdreRegistrationNumber).IsUnique();
        });
        
        // Table PendingUser
        modelBuilder.Entity<PendingUser>(e =>
        {
            e.HasKey(x => x.PendingUserId);
            e.Property(x => x.Mail).HasMaxLength(250).IsRequired();
            e.Property(x => x.VerificationCodeHash).HasMaxLength(128);
            e.Property(x => x.IsActive).HasDefaultValue(true);

            e.HasIndex(x => new { x.Mail, x.IsActive }).IsUnique();

            // Relation PendingUser <-1:1-> <-0:N-> Role
            e.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation PendingUser <-1:1-> <-0:N-> User
            e.HasOne(x => x.InvitedByUser)
                .WithMany()
                .HasForeignKey(x => x.InvitedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}