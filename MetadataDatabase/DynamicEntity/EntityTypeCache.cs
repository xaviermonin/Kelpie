using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                EntityFactory entityFactory = new EntityFactory();

                foreach (var entity in context.Entities.Include(c => c.Attributes).Include(c => c.Proxies).Include("Attributes.Type"))
                    yield return entityFactory.CreateDynamicType<BaseEntity>(entity);
            }
        }
    }
}
