using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Enums;
using GaiaSolution.Domain.Exceptions;
using GaiaSolution.Domain.ValueObjects;
using GaiaSolution.Infrastructure.Repository;
using GaiaSolution.Tests.Infrastructure.Persistence;

namespace GaiaSolution.Tests.Infrastructure.Repository;

public class BaseRepositoryTests : IClassFixture<DbFixture>
{
    private readonly DbFixture _fixture;

    public BaseRepositoryTests(DbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetById_Returns_Inserted_Entity()
    {
        await using var db = _fixture.CreateDb();
        var repo = new UserRepository(db);

        var role = new Role { RoleName = "Admin" };
        db.Roles.Add(role);
        await db.SaveChangesAsync();

        var user = new User
        {
            UserStatus = UserStatus.Active,
            FirstName = "John",
            LastName = "Doe",
            EmailNormalized = EmailNormalized.From("JOHN@EXAMPLE.COM"),
            PhoneNormalized = PhoneNormalized.From("06 01 02 03 04"),
            RoleId = role.Id
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var fetched = await repo.GetById(user.Id);
        Assert.Equal("john@example.com", fetched.EmailNormalized.Value);
        Assert.Equal("+33601020304", fetched.PhoneNormalized.Value);
        Assert.Equal(role.Id, fetched.RoleId);
    }

    [Fact]
    public async Task GetById_NotFound_Throws_InvalidAddException()
    {
        await using var db = _fixture.CreateDb();
        var repo = new UserRepository(db);
        await Assert.ThrowsAsync<InvalidAddException>(() => repo.GetById(999));
    }

    [Fact]
    public async Task GetAllAsync_WithPredicate_Filters()
    {
        await using var db = _fixture.CreateDb();
        var repo = new UserRepository(db);

        var role = new Role { RoleName = "User" };
        db.Roles.Add(role);
        await db.SaveChangesAsync();

        db.Users.AddRange(
            new User { UserStatus = UserStatus.Active,  EmailNormalized = EmailNormalized.From("a@x.fr"), PhoneNormalized = PhoneNormalized.From("06 00 00 00 01"), RoleId = role.Id },
            new User { UserStatus = UserStatus.Banned, EmailNormalized = EmailNormalized.From("b@x.fr"), PhoneNormalized = PhoneNormalized.From("06 00 00 00 02"), RoleId = role.Id }
        );
        await db.SaveChangesAsync();

        var actives = await repo.GetAllAsync(u => u.UserStatus == UserStatus.Active);
        Assert.Single(actives);
        Assert.Equal("a@x.fr", actives[0].EmailNormalized.Value);
    }
}
