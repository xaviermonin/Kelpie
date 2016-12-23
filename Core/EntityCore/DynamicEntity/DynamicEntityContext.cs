using EntityCore.DynamicEntity.Construction.Helper.Reflection;
using EntityCore.DynamicEntity.Event;
using EntityCore.Entity;
using EntityCore.Proxy;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace EntityCore.DynamicEntity
{
    public partial class DynamicEntityContext : BaseEntityContext
    {
        private Dictionary<string, Type> _tables = new Dictionary<string, Type>();

        public DynamicEntityContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            InitializeTypes();
        }

        public DynamicEntityContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            InitializeTypes();
        }

        private void InitializeTypes()
        {
            foreach (var type in EntityTypeCache.GetEntitiesTypes(Database.Connection))
                _tables.Add(type.Name, type);
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
                    case System.Data.Entity.EntityState.Added:
                        createdEntries = entries;
                        break;
                    case System.Data.Entity.EntityState.Deleted:
                        deletedEntries = entries;
                        break;
                    case System.Data.Entity.EntityState.Modified:
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
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostUpdate, entry.Entity.GetType().Name, this, entry);
        }

        private void PostDelete(IEnumerable<DbEntityEntry> deletedEntries)
        {
            if (deletedEntries == null)
                return;

            foreach (var entry in deletedEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostUpdate, entry.Entity.GetType().Name, this, entry);
        }

        private void PostCreate(IEnumerable<DbEntityEntry> createdEntries)
        {
            if (createdEntries == null)
                return;

            foreach (var entry in createdEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostUpdate, entry.Entity.GetType().Name, this, entry);
        }

        private void PreUpdate(IEnumerable<DbEntityEntry> updatingEntries)
        {
            if (updatingEntries == null)
                return;

            foreach (var entry in updatingEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostUpdate, entry.Entity.GetType().Name, this, entry);
        }

        private void PreDelete(IEnumerable<DbEntityEntry> deletingEntries)
        {
            if (deletingEntries == null)
                return;

            foreach (var entry in deletingEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostUpdate, entry.Entity.GetType().Name, this, entry);
        }

        private void PreCreate(IEnumerable<DbEntityEntry> creatingEntries)
        {
            if (creatingEntries == null)
                return;

            foreach (var entry in creatingEntries)
                EventDispatcher.DispatchEvent(EventDispatcher.EntityEvent.PostUpdate, entry.Entity.GetType().Name, this, entry);
        }

#endregion

        /// <summary>
        /// Create a new instance of an entity
        /// from the given entity name.
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
        /// </summary>
        /// <typeparam name="TProxy">Proxy type</typeparam>
        /// <returns></returns>
        public TProxy Create<TProxy>() where TProxy : IBaseEntity
        {
            var bindedName = TypeHelper.GetCustomAttribute<BindedEntityAttribute>(typeof(TProxy));
            return (TProxy)(Set(bindedName.Name).Create());
        }

        /// <summary>
        /// Return a non-generic <see cref="EntityDbSet{TEntity}"/> instance
        /// for access to entities of the given entity name.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public DbSet Set(string entityName)
        {
            return Set(_tables[entityName]);
        }

        /// <summary>
        /// Return a <see cref="EntityDbSet{TEntity}"/> instance
        /// for access to entities via a proxy inherited from <see cref="IBaseEntity"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityName">Entity name</param>
        /// <returns></returns>
        public EntityDbSet<T> ProxySet<T>(string entityName) where T : class, IBaseEntity
        {
            return new EntityDbSet<T>(Set(entityName));
        }

        /// <summary>
        /// Return a <see cref="EntityDbSet{TEntity}"/> instance
        /// for access to entities via a proxy inherited from <see cref="IBaseEntity"/>.
        /// The entity name is got from
        /// the <see cref="BindedEntityAttribute"/> of the proxy type.
        /// </summary>
        /// <typeparam name="T">Proxy type inherited from <seealso cref="IBaseEntity"/></typeparam>
        /// <returns></returns>
        public EntityDbSet<T> ProxySet<T>() where T : class, IBaseEntity
        {
            var bindedName = TypeHelper.GetCustomAttribute<BindedEntityAttribute>(typeof(T));
            return new EntityDbSet<T>(Set(bindedName.Name));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var entityMethod = modelBuilder.GetType().GetMethod("Entity");

            foreach (var table in _tables)
            {
                //modelBuilder.Entity(table.Value);
                entityMethod.MakeGenericMethod(table.Value).Invoke(modelBuilder, new object[] { });

                /*foreach (var pi in (table.Value).GetProperties())
                {
                    if (pi.Name == "Id")
                        modelBuilder.Entity(table.Value).HasKey(typeof(int), "Id");
                    else
                        modelBuilder.Entity(table.Value).StringProperty(pi.Name);
                }*/
            }
        }
    }
}
