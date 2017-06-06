using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace Kelpie.Model
{
    static class ModelCache
    {
        static private Dictionary<string, DbCompiledModel> cache = new Dictionary<string, DbCompiledModel>();

        static void RegisterModel(DbCompiledModel model, string connectionString)
        {
            if (!IsInCache(connectionString))
                throw new InvalidOperationException("Model already in cache");

            cache.Add(connectionString, model);
        }

        static DbCompiledModel GetModel(string connectionString)
        {
            return cache[connectionString];
        }

        static bool IsInCache(string connectionString)
        {
            return cache.ContainsKey(connectionString);
        }
    }
}
