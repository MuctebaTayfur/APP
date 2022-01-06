using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using APP.Data.Interface;
using APP.Base.Model.Base;
using Microsoft.EntityFrameworkCore;
using APP.Base.Model.Entity;
using APP.Base.Model.Enum;

namespace APP.Data.Concrete
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseClass
    {
        private DbSet<TEntity> _dbSet;

        public Repository(object context)
        {
            var dbContext = context as DbContext;
            if (dbContext != null)
            {
                _dbSet = dbContext.Set<TEntity>();
            }
        }

        public virtual void Delete(params TEntity[] entities)
        {
            foreach (TEntity a in entities)
                Delete(GetObjectPrimaryKey(a));
        }

        public virtual void DeleteNew(params TEntity[] entities)
        {
            foreach (TEntity a in entities)
            {
                var prop = a.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(a, Status.Deleted, null);
                }

                _dbSet.Update(a);
            }    
        }

        public virtual void Delete(object id)
        {
            var result = GetById(id);
            if (result == null)
            {
                return;
            }
            if (GetRowStatus(typeof(TEntity)))
            {
                var prop = result.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(result, Status.Deleted, null);
                }
            }
            _dbSet.Update(result);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> where)
        {
            IQueryable<TEntity> result = _dbSet.Where<TEntity>(where);
            foreach (TEntity a in result)
                Delete(GetObjectPrimaryKey(a));
        }

        public virtual void HardDelete(object Id)
        {
            var entity = GetById(Id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual IQueryable<TEntity> GetAllWithoutFilter(string[] include)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking().IgnoreQueryFilters();

            if (include != null)
            {
                query = include.Aggregate(query, (current, includePath) => current.Include(includePath));
            }

            return query;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            if (GetRowStatus(typeof(TEntity)))
            {
                if (typeof(TEntity).BaseType == typeof(BaseEntity))
                    return _dbSet.Cast<BaseEntity>().Where(x => x.Status != Status.Deleted).Select(x => x as TEntity);
                else
                    return _dbSet;
            }
            else
            {
                return _dbSet;
            }
        }

        public virtual TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                if (typeof(TEntity).BaseType == typeof(BaseEntity))
                    return _dbSet.Cast<BaseEntity>().OrderByDescending(x => x.Id).FirstOrDefault() as TEntity;
                else
                    return _dbSet.FirstOrDefault();
            }
            else
            {
                if (typeof(TEntity).BaseType == typeof(BaseEntity))
                    return _dbSet.Where(predicate).Cast<BaseEntity>().OrderByDescending(x => x.Id).FirstOrDefault() as TEntity;
                else
                    return _dbSet.Where(predicate).FirstOrDefault();
            }
        }

        public virtual async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                if (typeof(TEntity).BaseType == typeof(BaseEntity))
                    return await _dbSet.Cast<BaseEntity>().OrderByDescending(x => x.Id).FirstOrDefaultAsync() as TEntity;
                else
                    return await _dbSet.FirstOrDefaultAsync();
            }
            else
            {
                if (typeof(TEntity).BaseType == typeof(BaseEntity))
                    return await _dbSet.Where(predicate).Cast<BaseEntity>().OrderByDescending(x => x.Id).FirstOrDefaultAsync() as TEntity;
                else
                    return await _dbSet.Where(predicate).FirstOrDefaultAsync();
            }
        }

        public virtual TEntity GetById(object Id)
        {
            return _dbSet.Find(Id);
        }

        public virtual IQueryable<TEntity> FromSqlRaw(string sql)
        {
            return null;
        }

        public virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Where(where);
        }

        public virtual IQueryable<TEntity> GetPagination(int pageSize, int pageNumber)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 1 : pageSize;
            return _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (GetRowStatus(typeof(TEntity)))
            {
                var prop = entity.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(entity, Status.Active, null);
                }
            }

            _dbSet.Add(entity);
        }

        public virtual void InsertAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (GetRowStatus(typeof(TEntity)))
            {
                var prop = entity.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(entity, Status.Active, null);
                }
            }

            _dbSet.AddAsync(entity);
        }

        public virtual void InsertOrUpdate(TEntity entity)
        {
            var result = GetById(GetObjectPrimaryKey(entity));
            if (result != null)
            {
                if (GetRowStatus(typeof(TEntity)))
                {
                    var prop = entity.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(entity, Status.Modified, null);
                    }
                }
            }
            else
            {
                if (GetRowStatus(typeof(TEntity)))
                {
                    var prop = entity.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(entity, Status.Active, null);
                    }
                }
            }
            _dbSet.Attach(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (GetRowStatus(typeof(TEntity)))
            {
                var prop = entity.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(entity, Status.Modified, null);
                }
            }
            _dbSet.Update(entity);
        }

        public virtual void UpdateWithStatus(TEntity entity, Status status)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (GetRowStatus(typeof(TEntity)))
            {
                var prop = entity.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(entity, status, null);
                }
            }
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void UpdateRangeWithStatus(IEnumerable<TEntity> entities, Status status)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entity");
            }

            foreach (var entity in entities) {
                if (GetRowStatus(typeof(TEntity)))
                {
                    var prop = entity.GetType().GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(entity, status, null);
                    }
                }
            }

            _dbSet.UpdateRange(entities);

        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _dbSet.AsNoTracking().AsEnumerable<TEntity>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dbSet.AsNoTracking().AsEnumerable().GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TEntity); }
        }

        public Expression Expression
        {
            get { return _dbSet.AsNoTracking().AsQueryable<TEntity>().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _dbSet.AsNoTracking().AsQueryable<TEntity>().Provider; }
        }

        private object GetObjectPrimaryKey(TEntity entity)
        {
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo info in properties)
            {
                string displayName = GetDisplayName(entity.GetType(), info, false);
                if (displayName == "PrimaryKey")
                {
                    return info.GetValue(entity, null);
                }
            }
            return null;
        }

        private bool GetRowStatus(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                string displayName = GetDisplayName(type, info, false);
                if (displayName == "Status")
                {
                    return true;
                }
            }
            return false;
        }

        private string GetDisplayName(Type type, PropertyInfo info, bool hasMetaDataAttribute)
        {
            if (!hasMetaDataAttribute)
            {
                object[] attributes = info.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    var displayName = (DisplayNameAttribute)attributes[0];
                    return displayName.DisplayName;
                }
                return info.Name;
            }
            PropertyDescriptor propDesc = TypeDescriptor.GetProperties(type).Find(info.Name, true);
            DisplayNameAttribute displayAttribute =
                propDesc.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            return displayAttribute != null ? displayAttribute.DisplayName : null;
        }

    }
}
