using EntityCore.DynamicEntity.Factory;
using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EntityCore.DynamicEntity
{
    static class EntityTypeCache
    {
        static private Dictionary<string, IEnumerable<Type>> typesByDatabase = new Dictionary<string, IEnumerable<Type>>();

        static public IEnumerable<Type> GetEntitiesTypes(string nameOrConnectionString)
        {
            if (typesByDatabase.ContainsKey(nameOrConnectionString))
                return typesByDatabase[nameOrConnectionString];

            var entitiesTypes = CreateEntitiesTypes(nameOrConnectionString);
            typesByDatabase.Add(nameOrConnectionString, entitiesTypes);

            return entitiesTypes;
        }

        static public Type GetEntityType(string nameOrConnectionString, string entityName)
        {
            return GetEntitiesTypes(nameOrConnectionString).Where(e => e.Name == entityName).Single();
        }

        static IEnumerable<Type> CreateEntitiesTypes(string nameOrConnectionString)
        {
            using (MetadataContext context = new MetadataContext(nameOrConnectionString))
            {
                EntityFactory entityFactory = new EntityFactory();

                foreach (var entity in context.Entities.Include(c => c.Attributes).Include(c => c.Proxies).Include("Attributes.Type"))
                    yield return entityFactory.CreateDynamicType<BaseEntity>(entity);
            }
        }
    }
}
