using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Add(TEntity item);

        TEntity FindById(object id);

        IEnumerable<TEntity> GetAll();

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        void Remove(TEntity item);

        void Update(TEntity item);
        IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> GetWithInclude(Func<TEntity, bool> predicate,
                    params Expression<Func<TEntity, object>>[] includeProperties);

        IEnumerable<TEntity> GetWithInclude(params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
