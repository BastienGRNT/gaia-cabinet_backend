using GaiaSolution.Domain.Entities;
using GaiaSolution.Domain.Interfaces.IRepository;
using GaiaSolution.Infrastructure.Base;
using GaiaSolution.Infrastructure.Database;

namespace GaiaSolution.Infrastructure.Repository;

public class EmailVerificationRepository : BaseRepository<EmailVerification, CoreDbContext>, IEmailVerificationRepository
{
    public EmailVerificationRepository(CoreDbContext databaseContext) : base(databaseContext)
    {
    }
}