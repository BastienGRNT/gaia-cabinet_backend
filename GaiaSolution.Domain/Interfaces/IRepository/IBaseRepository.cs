using System.Linq.Expressions;
using GaiaSolution.Domain.Base;

namespace GaiaSolution.Domain.Interfaces.IRepository;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T> GetById(int id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> where);
}