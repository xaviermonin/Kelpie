using EntityCore.DynamicEntity.Construction.Helper.Reflection;
using EntityCore.Entity;
using EntityCore.Entity.Event;
using EntityCore.Proxy;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;

namespace EntityCore.DynamicEntity
{
    public class DynamicEntityContext : BaseEntityContext
    {
        private Dictionary<string, Type> _tables = new Dictionary<string, Type>();
        private MetadataRepository _metadataRepository;

        internal DynamicEntityContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            InitializeTypes();
        }

        internal DynamicEntityContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            InitializeTypes();
        }

        private void InitializeTypes()
        {
            foreach (var type in EntityTypeCache.GetEntitiesTypes(Database.Connection))
                _tables.Add(type.Name, type);
            
            _metadataRepository = new MetadataRepository(this);
        }

        #region Events

        public override int SaveChanges()
        {
            if (!ChangeTracker.HasChanges())
                return base.SaveChanges();

            var entriesByState = ChangeTracker.Entries().GroupBy(c => c.State);

            IEnumerable<DbEntityEntry> createdEntries = null;
            IEnumerable<DbEntityEntry> updatedEntries = null;
            IEnumerable<DbEntityEntry> deletedEntries = null;

            foreach (var entries in entriesByState)
            {
                switch (entries.Key)
                {
                    case EntityState.Added:
                        createdEntries = entries;
                        break;
                    case EntityState.Deleted:
                        deletedEntries = entries;
                        break;
                    case EntityState.Modified:
                        updatedEntries = entries;
                        break;
                    default: System.Diagnostics.Debug.WriteLine("Entry State {0} not supported", entries.Key);
                        break;
                }
            }

            // Asynchrone ?

            PreCreate(createdEntries);
            PreDelete(deletedEntries);
            PreUpdate(deletedEntries);

            int result = base.SaveChanges();

            PostCreate(createdEntries);
            PostDelete(deletedEntries);
            PostUpdate(updatedEntries);

            return result;
        }

        private void PostUpdate(IEnumerable<DbEntityEntry> updatedEntries)
        {
            if (updatedEntries == null)
                return;

            foreach (var entry in updatedEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostUpdate, entry.Entity.GetType(), this, entry);
        }

        private void PostDelete(IEnumerable<DbEntityEntry> deletedEntries)
        {
            if (deletedEntries == null)
                return;

            foreach (var entry in deletedEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostDelete, entry.Entity.GetType(), this, entry);
        }

        private void PostCreate(IEnumerable<DbEntityEntry> createdEntries)
        {
            if (createdEntries == null)
                return;

            foreach (var entry in createdEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostCreate, entry.Entity.GetType(), this, entry);
        }

        private void PreUpdate(IEnumerable<DbEntityEntry> updatingEntries)
        {
            if (updatingEntries == null)
                return;

            foreach (var entry in updatingEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PreUpdate, entry.Entity.GetType(), this, entry);
        }

        private void PreDelete(IEnumerable<DbEntityEntry> deletingEntries)
        {
            if (deletingEntries == null)
                return;

            foreach (var entry in deletingEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PreDelete, entry.Entity.GetType(), this, entry);
        }

        private void PreCreate(IEnumerable<DbEntityEntry> creatingEntries)
        {
            if (creatingEntries == null)
                return;

            foreach (var entry in creatingEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PreCreate, entry.Entity.GetType(), this, entry);
        }

        #endregion

        /// <summary>
        /// Create a new instance of an entity
        /// from the given entity name.
        /// Note that this instance is NOT added or attached to the set.
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <returns></returns>
        public BaseEntity Create(string entityName)
        {
            return Set(entityName).Create() as BaseEntity;
        }

        /// <summary>
        /// Create a new instance of an entity
        /// casted to the given proxy type.
        /// Note that this instance is NOT added or attached to the set.
        /// </summary>
        /// <typeparam name="TProxy">Proxy type</typeparam>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public TProxy Create<TProxy>(string entityName) where TProxy : IBaseEntity
        {
            return (TProxy)(Set(entityName).Create());
        }

        /// <summary>
        /// Create an entity record.
        /// The entity name is got from
        /// the <see cref="BindedEntityAttribute"/> of the proxy type.
        /// Note that this instance is NOT added or attached to the set.
        /// </summary>
        /// <typeparam name="TProxy">Proxy type</typeparam>
        /// <returns></returns>
        public TProxy Create<TProxy>() where TProxy : IBaseEntity
        {
            var bindedName = TypeHelper.GetCustomAttribute<BindedEntityAttribute>(typeof(TProxy));
            return Create<TProxy>(bindedName.Name);
        }

        /// <summary>
        /// Create a new instance of an entity
        /// from the given entity name,
        /// set its state to Added and attach it to the set.
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <returns></returns>
        public BaseEntity New(string entityName)
        {
            var baseEntity = Create(entityName);
            Set(entityName).Add(baseEntity);

            return baseEntity;
        }

        /// <summary>
        /// Create a new instance of an entity
        /// from the given entity name,
        /// set it state to Added and attach it to the set.
        /// </summary>
        /// <typeparam name="TProxy">Proxy type</typeparam>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public TProxy New<TProxy>(string entityName) where TProxy : IBaseEntity
        {
            var entity = Create<TProxy>(entityName);
            Set(entityName).Add(entity);

            return entity;
        }

        /// <summary>
        /// Create an entity record and
        /// set it state to Added and attach it to the set.
        /// The entity name is got from
        /// the <see cref="BindedEntityAttribute"/> of the proxy type.
        /// </summary>
        /// <typeparam name="TProxy">Proxy type</typeparam>
        /// <returns></returns>
        public TProxy New<TProxy>() where TProxy : IBaseEntity
        {
            var bindedName = TypeHelper.GetCustomAttribute<BindedEntityAttribute>(typeof(TProxy));
            return New<TProxy>(bindedName.Name);
        }

        /// <summary>
        /// Return a non-generic <see cref="ProxyDbSet{TEntity}"/> instance
        /// for access to entities of the given entity name.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public DbSet Set(string entityName)
        {
            if (!_tables.ContainsKey(entityName))
                throw new ArgumentException($"The '{entityName}' entity doesn't exist", "entityName");

            return Set(_tables[entityName]);
        }

        /// <summary>
        /// Return a <see cref="ProxyDbSet{TEntity}"/> instance
        /// for access to entities via a proxy inherited from <see cref="IBaseEntity"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityName">Entity name</param>
        /// <returns></returns>
        public ProxyDbSet<T> ProxySet<T>(string entityName) where T : class, IBaseEntity
        {
            return new ProxyDbSet<T>(Set(entityName));
        }

        /// <summary>
        /// Return a <see cref="ProxyDbSet{TEntity}"/> instance
        /// for access to entities via a proxy inherited from <see cref="IBaseEntity"/>.
        /// The entity name is got from
        /// the <see cref="BindedEntityAttribute"/> of the proxy type.
        /// </summary>
        /// <typeparam name="T">Proxy type inherited from <seealso cref="IBaseEntity"/></typeparam>
        /// <returns></returns>
        public ProxyDbSet<T> ProxySet<T>() where T : class, IBaseEntity
        {
            var bindedName = TypeHelper.GetCustomAttribute<BindedEntityAttribute>(typeof(T));
            return new ProxyDbSet<T>(Set(bindedName.Name));
        }

        public MetadataRepository Metadata
        {
            get { return _metadataRepository;  }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            foreach (var table in _tables)
                modelBuilder.RegisterEntityType(table.Value);

            if (!(Database.Connection is SqlConnection))
                modelBuilder.Conventions.Remove<ColumnAttributeConvention>();
        }
    }
}
