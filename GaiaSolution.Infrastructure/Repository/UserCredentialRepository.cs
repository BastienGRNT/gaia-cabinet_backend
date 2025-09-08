using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Interfaces.IRepository;
using GaiaSolution.Infrastructure.Base;
using GaiaSolution.Infrastructure.Database;

namespace GaiaSolution.Infrastructure.Repository;

public class UserCredentialRepository : BaseRepository<UserCredential, CoreDbContext>, IUserCredentialRepository
{
    public UserCredentialRepository(CoreDbContext databaseContext) : base(databaseContext)
    {
    }
}