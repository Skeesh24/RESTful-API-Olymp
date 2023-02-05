using RESTful_API_Olymp.Domain.Entities;
using System.Linq.Expressions;

namespace RESTful_API_Olymp.Data.Repositories
{
    public class DbRepository : IDbRepository
    {
        private readonly DataContext _context;

        public DbRepository(DataContext context, string id)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        async Task<long> IDbRepository.Add<T>(T newEntity)
        {
            var entity = await _context.Set<T>().AddAsync(newEntity);
            return entity.Entity.Id;
        }

        async Task IDbRepository.AddRange<T>(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        async Task IDbRepository.Delete<T>(T entity)
        {
            await Task.Run(() => _context.Set<T>().Remove(entity));
        }

        async Task IDbRepository.DeleteRange<T>(IEnumerable<T> entities)
        {
            await Task.Run(() => _context.Set<T>().RemoveRange(entities));
        }

        IQueryable<T> IDbRepository.Select<T>(Expression<Func<T, bool>> selector)
        {
            return _context.Set<T>().Where(selector);
        }

        IQueryable<T> IDbRepository.SelectAll<T>()
        {
            return _context.Set<T>();
        }   

        async Task IDbRepository.Update<T>(T entity)
        {
            await Task.Run(() => _context.Update(entity));
        }

        async Task IDbRepository.UpdateRange<T>(IEnumerable<T> entities)
        {
            await Task.Run(() => _context.UpdateRange(entities));
        }
    }
}
