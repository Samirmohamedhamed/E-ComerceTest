using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using System.Linq.Expressions;
using EComerceTestSamir.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace EComerceTestSamir.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;

            _dbSet = _context.Set<T>();
        }

        public async Task<bool> CreateAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool Update(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool Delete(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public T? GetOne(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? Includes = null, bool traked = true, int skip = 0, int take = 0, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            return Get(expression, Includes, traked, skip, take, orderBy).FirstOrDefault();
        }
        public IQueryable<T> Get(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? Includes = null, bool traked = true, int skip = 0, int take = 0, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> entities = _dbSet;

            if (expression != null)
            {
                entities = entities.Where(expression);
            }

            if (Includes != null)
            {
                foreach (var include in Includes)
                {
                    entities = entities.Include(include);
                }
            }

            if (!traked)
            {
                entities = entities.AsNoTracking();
            }

            if (orderBy != null)
            {
                entities = orderBy(entities); // ✅ Apply ordering
            }

            if (skip != 0)
            {
                entities = entities.Skip(skip);
            }

            if (take != 0)
            {
                entities = entities.Take(take);
            }

            return entities;
        }
        public T? GetFirstOrDefault(
    Expression<Func<T, bool>>? expression = null,
    Expression<Func<T, object>>[]? Includes = null,
    bool traked = true)
        {
            IQueryable<T> entities = _dbSet;

            if (expression != null)
            {
                entities = entities.Where(expression);
            }

            if (Includes != null)
            {
                foreach (var include in Includes)
                {
                    entities = entities.Include(include);
                }
            }

            if (!traked)
            {
                entities = entities.AsNoTracking();
            }

            return entities.FirstOrDefault();
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        
    }
}
