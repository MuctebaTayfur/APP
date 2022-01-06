using APP.Base.Model.Base;
using APP.Base.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace APP.Data.Interface
{
    public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : BaseClass
    {
        void Insert(TEntity entity);
        void InsertAsync(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void InsertOrUpdate(TEntity entity);
        void Update(TEntity entity);
        void UpdateWithStatus(TEntity entity, Status status);
        void UpdateRange(IEnumerable<TEntity> entities);
        void UpdateRangeWithStatus(IEnumerable<TEntity> entities, Status status);
        void DeleteNew(params TEntity[] entities);
        void Delete(params TEntity[] entities);
        void Delete(object id);
        void Delete(Expression<Func<TEntity, bool>> where);
        void HardDelete(object Id);
        TEntity GetById(object Id);
        IQueryable<TEntity> FromSqlRaw(string sql);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllWithoutFilter(string[] include);

        TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate = null);
        Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null);
        IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where);
        IQueryable<TEntity> GetPagination(int pageSize, int pageNumber);


    }
}
