using APP.Base.Model.Base;
using APP.Data.Concrete;
using APP.Data.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APP.Common.Data.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private const string PropNameCreatedOn = "CreatedOn";
        private const string PropNameCreatedBy = "CreatedBy";
        private const string PropNameModifiedBy = "ModifiedBy";
        private const string PropNameModifiedOn = "ModifiedOn";
        private const string PropNameDeletedBy = "DeletedBy";
        private const string PropNameDeletedOn = "DeletedOn";
        private const string PropNameStatus = "Status";

        private bool _disposed;

        protected DbContext _dataContext;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected Dictionary<string, dynamic> _repositories;

        public UnitOfWork() { }

        public UnitOfWork(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _repositories = new Dictionary<string, dynamic>();
        }

        public virtual DbContext GetContext()
        {
            return _dataContext;
        }

        public Dictionary<string, dynamic> GetRepositories()
        {
            return _repositories;
        }

        public Task<int> SaveChangesAsync()
        {
            return _dataContext.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _dataContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (_dataContext != null)
                        {
                            _dataContext.Dispose();
                            _dataContext = null;
                        }
                    }
                    catch (ObjectDisposedException ex)
                    {
                        throw (ex);
                    }
                } 
                _disposed = true;
            }
        }
    }

    #region .
    //controller IUnitOfWork<AuthContext> ctor dan talep etti
    //startup scoped servici ile unitOfWork<AuthContext> generic classýnda üretmeye baþladý. Fakat;
    // UnitOfWork<AuthContext> ctor da bir AuthContext/(T) türünde dbcontext diye bir parametre istiyor.
    //Bunun için tekrar startup a gidip services.AddDbContext<AuthContext> den authcontextin nesnesini alýyor
    //dikkat edersen dbcontexoptionsbulide parametresini servise ek olarka veriyor. çünki auth context ctor da dbcontextoptions istiyor.
    #endregion

    public class UnitOfWork<T> : UnitOfWork, IUnitOfWork<T> where T : DbContext
    {
        public UnitOfWork(T dataContext, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _dataContext = dataContext;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseClass
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, dynamic>();
            }

            var type = typeof(TEntity).Name;
            if (_repositories.ContainsKey(type))
            {
                return (IRepository<TEntity>)_repositories[type];
            }

            var repositoryType = typeof(Repository<>);
            _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dataContext));
            return _repositories[type];
        }
    }
}
