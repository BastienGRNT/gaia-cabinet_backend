using gaiacabinet_api.Models;

namespace gaiacabinet_api.Database;

public static class DataSeeder
{
    public static void SeedDevelopmentData(AppDbContext db)
    {
        string[] roleNames = { "Admin", "Ménage", "Medecin" };
        foreach (var name in roleNames)
            if (!db.Roles.Any(r => r.Label == name))
                db.Roles.Add(new Role { Label = name });

        if (db.ChangeTracker.HasChanges()) db.SaveChanges();

        string Hash(string pwd) => BCrypt.Net.BCrypt.HashPassword(pwd);

        var adminRoleId  = db.Roles.Single(r => r.Label == "Admin").RoleId;
        var menageRoleId = db.Roles.Single(r => r.Label == "Ménage").RoleId;
        var medecinRoleId= db.Roles.Single(r => r.Label == "Medecin").RoleId;

        var usersToEnsure = new[]
        {
            new { Mail="admin@gaia.local",   Phone="0600000001", First="Admin",  Last="Root",    RoleId=adminRoleId,  Pwd="Admin!123" },
            new { Mail="menage@gaia.local",  Phone="0600000002", First="Alice",  Last="Menage",  RoleId=menageRoleId, Pwd="Menage!123" },
            new { Mail="medecin@gaia.local", Phone="0600000003", First="Marc",   Last="Medecin", RoleId=medecinRoleId, Pwd="Medecin!123" }
        };

        foreach (var u in usersToEnsure)
        {
            if (!db.Users.Any(x => x.Mail == u.Mail))
            {
                db.Users.Add(new User
                {
                    FirstName = u.First,
                    LastName  = u.Last,
                    Mail      = u.Mail,
                    Phone     = u.Phone,
                    PasswordHash = Hash(u.Pwd),
                    RoleId    = u.RoleId,
                    Authorized = true,
                    DaysAdvance = null
                });
            }
        }

        if (db.ChangeTracker.HasChanges()) db.SaveChanges();
    }
}
