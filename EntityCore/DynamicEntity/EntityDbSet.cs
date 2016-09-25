using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.DynamicEntity
{
    public class EntityDbSet<TEntity> : IDbAsyncEnumerable<TEntity>, IDbAsyncEnumerable, IDbSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, IQueryable, IEnumerable where TEntity : class
    {
        DbSet dbSet;

        public EntityDbSet(DbSet dbSet)
        {
            this.dbSet = dbSet;
        }

        public DbQuery<TEntity> AsNoTracking()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Include(string path)
        {
            return dbSet.Include(path) as IQueryable<TEntity>;
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

        public ObservableCollection<TEntity> Local
        {
            get
            {
                return (dbSet as IDbSet<TEntity>).Local;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return (dbSet as IQueryable).Provider;
            }
        }

        public TEntity Add(TEntity entity)
        {
            return (TEntity)dbSet.Add(entity);
        }

        public TEntity Attach(TEntity entity)
        {
            return (TEntity)dbSet.Attach(entity);
        }

        public TEntity Create()
        {
            return (TEntity)dbSet.Create();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
        {
            return (TDerivedEntity)dbSet.Create();
        }

        public TEntity Find(params object[] keyValues)
        {
            return (TEntity)dbSet.Find(keyValues);
        }

        public IDbAsyncEnumerator GetAsyncEnumerator()
        {
            return (dbSet as IDbAsyncEnumerable).GetAsyncEnumerator();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return (dbSet as IEnumerable<TEntity>).GetEnumerator();
        }

        public TEntity Remove(TEntity entity)
        {
            return dbSet.Remove(entity) as TEntity;
        }

        IDbAsyncEnumerator<TEntity> IDbAsyncEnumerable<TEntity>.GetAsyncEnumerator()
        {
            return (dbSet as IDbAsyncEnumerable<TEntity>).GetAsyncEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (dbSet as IEnumerable<TEntity>).GetEnumerator();
        }
    }
}
