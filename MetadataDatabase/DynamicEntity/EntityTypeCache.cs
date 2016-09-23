using EntityCore.DynamicEntity.Construction;
using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Reflection.Emit;

namespace EntityCore.DynamicEntity
{
    class EntityTypeCache
    {
        static private Dictionary<string, IEnumerable<Type>> typesByDatabase = new Dictionary<string, IEnumerable<Type>>();

        static public IEnumerable<Type> GetEntitiesTypes(string connectionString)
        {
            if (typesByDatabase.ContainsKey(connectionString))
                return typesByDatabase[connectionString];

            var entitiesTypes = CreateEntitiesTypes(connectionString);
            typesByDatabase.Add(connectionString, entitiesTypes);

            return entitiesTypes;
        }

        static IEnumerable<Type> CreateEntitiesTypes(string connectionString)
        {
            using (MetadataContext context = new MetadataContext(connectionString))
            {
                DynamicAssemblyBuilder assemblyBuilder = new DynamicAssemblyBuilder();

                var entities = context.Entities.Include(c => c.Attributes)
                                               .Include(c => c.Proxies)
                                               .Include("Attributes.Type")
                                               .Include(c => c.ManyToOneRelationships)
                                               .Include(c => c.OneToManyRelationships).ToArray();

                return assemblyBuilder.BuildTypes(entities);
            }
        }
    }
}
