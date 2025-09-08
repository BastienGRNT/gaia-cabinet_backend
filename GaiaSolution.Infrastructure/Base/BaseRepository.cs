using System.Linq.Expressions;
using GaiaSolution.Domain.Base;
using GaiaSolution.Domain.Exceptions;
using GaiaSolution.Domain.Interfaces.IRepository;
using GaiaSolution.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace GaiaSolution.Infrastructure.Base;

public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity> 
    where TEntity : BaseEntity
    where TContext : CoreDbContext
{
    private readonly TContext _dbContext;

    public BaseRepository(TContext context)
    {
        _dbContext = context;
    }

    public async Task<TEntity> GetById(int id)
    {
        try
        {
            var rep = await _dbContext.Set<TEntity>().FindAsync(id);
            if (rep != null)
                return rep;
            throw new InvalidAddException();
        }
        catch (Exception ex)
        {
            throw new InvalidAddException(inner:ex);
        }
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        try
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidAddException(inner:ex);
        }
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where)
    {
        try
        {
            return await _dbContext.Set<TEntity>().Where(where).AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidAddException(inner:ex);
        }
    }
}