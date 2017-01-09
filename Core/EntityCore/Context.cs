using EntityCore.DynamicEntity;
using EntityCore.Utils;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EntityCore
{
    public static class Context
    {
        public static DynamicEntityContext New(string nameOrConnectionString)
        {
            Check.NotEmpty(nameOrConnectionString, "nameOrConnectionString");
            var connnection = DbHelpers.GetDbConnection(nameOrConnectionString);
            DbCompiledModel model = GetCompiledModel(connnection);

            return new DynamicEntityContext(nameOrConnectionString, model);
        }

        public static DynamicEntityContext New(DbConnection existingConnection)
        {
            Check.NotNull(existingConnection, "existingConnection");
            DbCompiledModel model = GetCompiledModel(existingConnection);

            return new DynamicEntityContext(existingConnection, model, false);
        }

        private static DbCompiledModel GetCompiledModel(DbConnection connection)
        {
            var builder = new DbModelBuilder(DbModelBuilderVersion.Latest);

            foreach (var entity in EntityTypeCache.GetEntitiesTypes(connection))
                builder.RegisterEntityType(entity);

            var model = builder.Build(connection);

            return model.Compile();
        }
    }
}
