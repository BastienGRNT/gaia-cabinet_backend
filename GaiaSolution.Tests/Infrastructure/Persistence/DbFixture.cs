using GaiaSolution.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace GaiaSolution.Tests.Infrastructure.Persistence;

public class DbFixture : IDisposable
{
    public CoreDbContext Context { get; }

    public DbFixture()
    {
        var options = new DbContextOptionsBuilder<CoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new CoreDbContext(options);
        Seed();
    }
    public CoreDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<CoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // DB unique par test
            .Options;

        var context = new CoreDbContext(options);
        context.Database.EnsureCreated(); // init le schéma
        return context;
    }
    

    private void Seed()
    {
        // Ex. ajouter des rôles ou users par défaut
        // Context.Roles.Add(new Role { Name = "Admin" });
        // Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}