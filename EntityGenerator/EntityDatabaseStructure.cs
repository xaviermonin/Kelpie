using MetadataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityGenerator
{
    public class EntityDatabaseStructure
    {
        public static string GenerateCreateTableSqlQuery(Entity entity)
        {
            var createTable = new StringBuilder();
            createTable.AppendFormat("CREATE TABLE [{0}] (Id int IDENTITY(1,1) NOT NULL, ", entity.Name);

            foreach (var attribute in entity.Attributes)
            {
                createTable.AppendFormat("{0} {1}{2} {3} {4}, ", attribute.Name,
                                                                   attribute.Type.SqlServerName,
                                                                   attribute.Length == null ? string.Empty : "(" + attribute.Length.ToString() + ")",
                                                                   attribute.IsNullable ? "NULL" : string.Empty,
                                                                   attribute.DefaultValue != null ? "DEFAULT(" + attribute.DefaultValue + ")" : string.Empty);
            }

            createTable.AppendFormat("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED", entity.Name);
            createTable.AppendFormat("(Id ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF," +
                                     "ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]");

            return createTable.ToString();
        }
    }
}
