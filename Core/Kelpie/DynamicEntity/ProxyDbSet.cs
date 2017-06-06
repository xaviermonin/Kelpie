using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Kelpie.DynamicEntity
{
    public class ProxyDbSet<TProxy> : IDbAsyncEnumerable<TProxy>, IDbAsyncEnumerable, IDbSet<TProxy>, IQueryable<TProxy>, IEnumerable<TProxy>, IQueryable, IEnumerable where TProxy : class
    {
        DbSet dbSet;

        public ProxyDbSet(DbSet dbSet)
        {
            this.dbSet = dbSet;
        }

        /*public DbQuery<TProxy> AsNoTracking()
        {
            throw new NotImplementedException();
        }*/

        public IQueryable<TProxy> Include(string path)
        {
            return dbSet.Include(path) as IQueryable<TProxy>;
        }

        public Type ElementType
        {
            get
            {
                return dbSet.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return (dbSet as IQueryable).Expression;
            }
        }

        public ObservableCollection<TProxy> Local
        {
            get
            {
                return (dbSet as IDbSet<TProxy>).Local;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return (dbSet as IQueryable).Provider;
            }
        }

        public TProxy Add(TProxy entity)
        {
            return (TProxy)dbSet.Add(entity);
        }

        public TProxy Attach(TProxy entity)
        {
            return (TProxy)dbSet.Attach(entity);
        }

        public TProxy Create()
        {
            return (TProxy)dbSet.Create();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TProxy
        {
            return (TDerivedEntity)dbSet.Create();
        }

        public TProxy Find(params object[] keyValues)
        {
            return (TProxy)dbSet.Find(keyValues);
        }

        public IDbAsyncEnumerator GetAsyncEnumerator()
        {
            return (dbSet as IDbAsyncEnumerable).GetAsyncEnumerator();
        }

        public IEnumerator<TProxy> GetEnumerator()
        {
            return (dbSet as IEnumerable<TProxy>).GetEnumerator();
        }

        public TProxy Remove(TProxy entity)
        {
            return dbSet.Remove(entity) as TProxy;
        }

        IDbAsyncEnumerator<TProxy> IDbAsyncEnumerable<TProxy>.GetAsyncEnumerator()
        {
            return (dbSet as IDbAsyncEnumerable<TProxy>).GetAsyncEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (dbSet as IEnumerable<TProxy>).GetEnumerator();
        }
    }
}
