using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APP.Base.Model.Base;
using Microsoft.EntityFrameworkCore;

namespace APP.Data.Interface
{
    public interface IUnitOfWork
    {
        int SaveChanges();      
        Task<int> SaveChangesAsync();
        DbContext GetContext();
        Dictionary<string, dynamic> GetRepositories();
    }

    public interface IUnitOfWork<T> : IUnitOfWork where T : DbContext, IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : BaseClass;
    }
}
