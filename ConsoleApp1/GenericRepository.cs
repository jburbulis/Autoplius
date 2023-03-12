using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Autoplius.Repository.Repositories;

public interface IGenericRepository<T> where T : class
{
    T Add(T entity);

    T AddWithSave(T entity);

    Task<T> AddWithSaveAsync(T entity);

    bool Delete(T entity);

    bool DeleteWithSave(T entity);

    Task<bool> DeleteWithSaveAsync(T entity);

    T Get(Expression<Func<T, bool>> whereCondition);

    T Get(Expression<Func<T, bool>> whereCondition, Func<IQueryable<T>, IQueryable<T>> includeFunction);

    List<T> GetAll();

    List<T> GetAll(Expression<Func<T, bool>> whereCondition);

    List<T> GetAll(Expression<Func<T, bool>> whereCondition, Func<IQueryable<T>, IQueryable<T>> includeFunction);

    Task<List<T>> GetAllAsync();

    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition);

    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition, Func<IQueryable<T>, IQueryable<T>> includeFunction);

    Task<T> GetAsync(Expression<Func<T, bool>> whereCondition, Func<IQueryable<T>, IQueryable<T>> includeFunction);

    Task<T> GetAsync(Expression<Func<T, bool>> whereCondition);

    IQueryable<T> GetQueryable();

    bool Update(T entity);

    void UpdateWithSave(T entity);

    Task UpdateWithSaveAsync(T entity);
}

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected AutopliusDatabase _context;
    private readonly DbSet<T> dbSet;

    public GenericRepository(AutopliusDatabase context)
    {
        _context = context;
        dbSet = _context.Set<T>();
    }

    public virtual T Add(T entity)
    {
        dbSet.Add(entity);
        _context.SaveChanges();
        _context.Entry(entity)
            .State = EntityState.Detached;

        return entity;
    }

    public virtual T AddWithSave(T entity)
    {
        dbSet.Add(entity);

        _context.SaveChanges();
        _context.Entry(entity)
            .State = EntityState.Detached;

        return entity;
    }

    public virtual async Task<T> AddWithSaveAsync(T entity)
    {
        dbSet.Add(entity);

        await _context.SaveChangesAsync();
        _context.Entry(entity)
            .State = EntityState.Detached;

        return entity;
    }

    public virtual bool Delete(T entity)
    {
        dbSet.Remove(entity);

        return true;
    }

    public virtual bool DeleteWithSave(T entity)
    {
        dbSet.Remove(entity);
        _context.SaveChanges();

        return true;
    }

    public virtual async Task<bool> DeleteWithSaveAsync(T entity)
    {
        dbSet.Remove(entity);

        await _context.SaveChangesAsync();

        return true;
    }

    public virtual T Get(Expression<Func<T, bool>> whereCondition)
    {
        return dbSet.AsNoTracking()
            .Where(whereCondition)
            .FirstOrDefault();
    }

    public virtual T Get(Expression<Func<T, bool>> whereCondition, Func<IQueryable<T>, IQueryable<T>> includeFunction)
    {
        var result = dbSet;
        var resultWithEagerLoading = includeFunction(result);

        return resultWithEagerLoading.AsNoTracking()
            .Where(whereCondition)
            .FirstOrDefault();
    }

    public virtual List<T> GetAll()
    {
        return dbSet.AsNoTracking()
            .ToList();
    }

    public virtual List<T> GetAll(Expression<Func<T, bool>> whereCondition)
    {
        return dbSet.AsNoTracking()
            .Where(whereCondition)
            .ToList();
    }

    public virtual List<T> GetAll(Expression<Func<T, bool>> whereCondition, Func<IQueryable<T>, IQueryable<T>> includeFunction)
    {
        var result = dbSet;
        var resultWithEagerLoading = includeFunction(result);

        return resultWithEagerLoading.AsNoTracking()
            .Where(whereCondition)
            .ToList();
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await dbSet.AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition)
    {
        return await dbSet.AsNoTracking()
            .Where(whereCondition)
            .ToListAsync();
    }

    public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition, Func<IQueryable<T>, IQueryable<T>> includeFunction)
    {
        var result = dbSet;
        var resultWithEagerLoading = includeFunction(result);

        return await resultWithEagerLoading.AsNoTracking()
            .Where(whereCondition)
            .ToListAsync();
    }

    public virtual async Task<T> GetAsync(Expression<Func<T, bool>> whereCondition)
    {
        return await dbSet.AsNoTracking()
            .Where(whereCondition)
            .FirstOrDefaultAsync();
    }

    public virtual async Task<T> GetAsync(Expression<Func<T, bool>> whereCondition, Func<IQueryable<T>, IQueryable<T>> includeFunction)
    {
        var result = dbSet;
        var resultWithEagerLoading = includeFunction(result);

        return await resultWithEagerLoading.AsNoTracking()
            .Where(whereCondition)
            .FirstOrDefaultAsync();
    }

    public virtual IQueryable<T> GetQueryable()
    {
        return dbSet.AsNoTracking()
            .AsQueryable();
    }

    public virtual bool Update(T entity)
    {
        dbSet.Attach(entity);
        var entry = _context.Entry(entity);
        entry.State = EntityState.Modified;

        _context.SaveChanges();
        _context.Entry(entity)
            .State = EntityState.Detached;

        return true;
    }

    public virtual void UpdateWithSave(T entity)
    {
        dbSet.Attach(entity);
        var entry = _context.Entry(entity);
        entry.State = EntityState.Modified;
        _context.SaveChanges();
        _context.Entry(entity)
            .State = EntityState.Detached;
    }

    public virtual async Task UpdateWithSaveAsync(T entity)
    {
        dbSet.Attach(entity);
        var entry = _context.Entry(entity);
        entry.State = EntityState.Modified;

        await _context.SaveChangesAsync();
        _context.Entry(entity)
            .State = EntityState.Detached;
    }
}