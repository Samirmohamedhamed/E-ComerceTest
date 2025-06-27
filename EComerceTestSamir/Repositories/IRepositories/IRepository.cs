using EComerceTestSamir.Data;
using EComerceTestSamir.Models;
using System.Linq.Expressions;

namespace EComerceTestSamir.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<bool> CreateAsync(T entites);

         bool Update(T entites);

         bool Delete(T entites);

         T? GetOne(Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? Includes = null, bool traked = true, int skip = 0, int take = 0, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

         IQueryable<T> Get(Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? Includes = null, bool traked = true, int skip = 0, int take = 0, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        T? GetFirstOrDefault(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? Includes = null,bool traked = true);
        public  Task<bool> CommitAsync();
        
    }
}
