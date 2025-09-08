using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Interfaces.IRepository;
using GaiaSolution.Infrastructure.Base;
using GaiaSolution.Infrastructure.Database;

namespace GaiaSolution.Infrastructure.Repository;

public class UserSessionRepository : BaseRepository<UserSession, CoreDbContext>, IUserSessionRepository
{
    public UserSessionRepository(CoreDbContext databaseContext) : base(databaseContext)
    {
    }
}