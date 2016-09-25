using EntityCore.DynamicEntity.Construction;
using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Reflection.Emit;

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
                DynamicAssemblyBuilder assemblyBuilder = new DynamicAssemblyBuilder();

                var entities = context.Entities.Include(c => c.Attributes)
                                               .Include(c => c.Proxies)
                                               .Include("Attributes.Type")
                                               .Include(c => c.ManyToOneRelationships)
                                               .Include(c => c.OneToManyRelationships).ToArray();

                var types = assemblyBuilder.BuildTypes(entities).ToArray();

                assemblyBuilder.SaveAssembly();

                return types;
            }
        }
    }
}
