using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Interfaces.IRepository;
using GaiaSolution.Infrastructure.Base;
using GaiaSolution.Infrastructure.Database;

namespace GaiaSolution.Infrastructure.Repository;

public class RoleRepository : BaseRepository<Role, CoreDbContext>, IRoleRepository
{
    public RoleRepository(CoreDbContext databaseContext) : base(databaseContext)
    {
    }
}