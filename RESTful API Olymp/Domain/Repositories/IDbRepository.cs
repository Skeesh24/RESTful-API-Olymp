using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using RESTful_API_Olymp.Domain.Entities;
using System.Linq.Expressions;

namespace RESTful_API_Olymp.Data.Repositories
{
    public interface IDbRepository
    {
        public IQueryable<T> Select<T>(Expression<Func<T, bool>> selector) where T : class, IEntity;

        public IQueryable<T> SelectAll<T>() where T : class, IEntity;


        public Task<long> Add<T>(T newEntity) where T : class, IEntity;

        public Task AddRange<T>(IEnumerable<T> entities) where T : class, IEntity;


        public Task Delete<T>(T entities) where T : class, IEntity;

        public Task DeleteRange<T>(IEnumerable<T> entities) where T : class, IEntity;


        public Task Update<T>(T entity) where T : class, IEntity;

        public Task UpdateRange<T>(IEnumerable<T> entities) where T : class, IEntity;


        public Task<int> SaveChangesAsync();
    }
}
