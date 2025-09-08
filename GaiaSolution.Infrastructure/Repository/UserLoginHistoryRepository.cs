using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Interfaces.IRepository;
using GaiaSolution.Infrastructure.Base;
using GaiaSolution.Infrastructure.Database;

namespace GaiaSolution.Infrastructure.Repository;

public class UserLoginHistoryRepository : BaseRepository<UserLoginHistory, CoreDbContext>, IUserLoginHistoryRepository
{
    public UserLoginHistoryRepository(CoreDbContext databaseContext) : base(databaseContext)
    {
    }
}