using ApiVeiculos.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiVeiculos.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().AsNoTracking().ToList();
    }
    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().AsNoTracking().FirstOrDefault(predicate);
    }
    public T Create(T entity)
    {
        _context.Set<T>().Add(entity);
        return entity;
    }
    public T Update(T entity)
    {
        _context.Set<T>().Update(entity);
        return entity;
    }
    public T Delete(T entity)
    {
        _context.Set<T>().Update(entity);
        return entity;
    }
}

