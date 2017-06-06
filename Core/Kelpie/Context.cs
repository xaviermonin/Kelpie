using Kelpie.DynamicEntity;
using Kelpie.Utils;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;

namespace Kelpie
{
    public static class Context
    {
        public static DynamicEntityContext New(string nameOrConnectionString)
        {
            Check.NotEmpty(nameOrConnectionString, "nameOrConnectionString");
            var connection = DbHelpers.GetDbConnection(nameOrConnectionString);
            DbCompiledModel model = GetCompiledModel(connection);

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

            if (!(connection is SqlConnection)) // Compatible Effort. See https://effort.codeplex.com/workitem/678
                builder.Conventions.Remove<ColumnAttributeConvention>();

            var model = builder.Build(connection);

            return model.Compile();
        }
    }
}
