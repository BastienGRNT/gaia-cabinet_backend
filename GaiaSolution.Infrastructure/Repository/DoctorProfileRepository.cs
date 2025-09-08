using GaiaSolution.Domain.Base;
using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Interfaces.IRepository;
using GaiaSolution.Infrastructure.Base;
using GaiaSolution.Infrastructure.Database;

namespace GaiaSolution.Infrastructure.Repository;

public class DoctorProfileRepository : BaseRepository<DoctorProfile, CoreDbContext>, IDoctorProfileRepository
{
    public DoctorProfileRepository(CoreDbContext databaseContext) : base(databaseContext)
    {
    }
}