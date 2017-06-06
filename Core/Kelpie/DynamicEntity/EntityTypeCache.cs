using Kelpie.DynamicEntity.Construction;
using Kelpie.Initialization.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;

namespace Kelpie.DynamicEntity
{
    static class EntityTypeCache
    {
        static private Dictionary<string, IEnumerable<Type>> typesByDatabase = new Dictionary<string, IEnumerable<Type>>();

        static public IEnumerable<Type> GetEntitiesTypes(DbConnection dbConnection)
        {
            if (typesByDatabase.ContainsKey(dbConnection.ConnectionString))
                return typesByDatabase[dbConnection.ConnectionString];

            var entitiesTypes = CreateEntitiesTypes(dbConnection);
            typesByDatabase.Add(dbConnection.ConnectionString, entitiesTypes);

            return entitiesTypes;
        }

        static public Type GetEntityType(DbConnection dbConnection, string entityName)
        {
            return GetEntitiesTypes(dbConnection).Where(e => e.Name == entityName).Single();
        }

        static IEnumerable<Type> CreateEntitiesTypes(DbConnection existingConnection)
        {
            using (MetadataInitializationContext context = new MetadataInitializationContext(existingConnection, false))
            {
                DynamicAssemblyBuilder assemblyBuilder = new DynamicAssemblyBuilder();

                var entities = context.Entities.Include(c => c.Attributes)
                                               .Include(c => c.Listeners)
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
