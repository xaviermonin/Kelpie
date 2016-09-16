using EntityCore.DynamicEntity.Event;
using EntityCore.Entity;
using EntityCore.Proxy;
using System;
using System.Collections.Generic;
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

        private void InitializeTypes()
        {
            foreach (var type in EntityTypeCache.GetEntitiesTypes(this.Database.Connection.ConnectionString))
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

        public BaseEntity Create(string entityName)
        {
            return Set(entityName).Create() as BaseEntity;
        }

        public T Create<T>(string entityName) where T : IBaseEntity
        {
            return (T)(Set(entityName).Create());
        }

        public DbSet Set(string entityName)
        {
            return Set(_tables[entityName]);
        }

        public IQueryable<T> SetAs<T>(string entityName) where T : IBaseEntity
        {
            return Set(_tables[entityName]) as IQueryable<T>;
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
