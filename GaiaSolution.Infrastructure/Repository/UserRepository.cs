using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Interfaces.IRepository;
using GaiaSolution.Infrastructure.Base;
using GaiaSolution.Infrastructure.Database;

namespace GaiaSolution.Infrastructure.Repository;

public class UserRepository : BaseRepository<User, CoreDbContext>, IUserRepository
{
    public UserRepository(CoreDbContext databaseContext) : base(databaseContext)
    {
    }
}
