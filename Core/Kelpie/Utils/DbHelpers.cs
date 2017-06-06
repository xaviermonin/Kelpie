using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kelpie.Utils
{
    static class DbHelpers
    {
        public static bool TryGetConnectionName(string nameOrConnectionString, out string name)
        {
            int index = nameOrConnectionString.IndexOf('=');
            if (index < 0)
            {
                name = nameOrConnectionString;
                return true;
            }

            if (nameOrConnectionString.IndexOf('=', index + 1) >= 0)
            {
                name = null;
                return false;
            }

            if (nameOrConnectionString.Substring(0, index).Trim().Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                name = nameOrConnectionString.Substring(index + 1).Trim();
                return true;
            }
            name = null;
            return false;
        }

        public static DbConnection GetDbConnection(string nameOrConnectionString)
        {
            string name;
            string connectionString;
            DbProviderFactory dbProviderFactory;
            if (TryGetConnectionName(nameOrConnectionString, out name))
            {
                var configuration = ConfigurationManager.ConnectionStrings[name];
                connectionString = configuration.ConnectionString;
                dbProviderFactory = DbProviderFactories.GetFactory(configuration.ProviderName);
            }
            else
            {
                connectionString = name;
                dbProviderFactory = SqlClientFactory.Instance;
            }

            var connection = dbProviderFactory.CreateConnection();
            connection.ConnectionString = connectionString;

            return connection;
        }
    }
}
