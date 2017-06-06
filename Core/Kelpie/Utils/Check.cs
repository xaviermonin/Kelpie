using System;

namespace Kelpie.Utils
{
    internal static class Check
    {
        public static void NotNull(object param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(paramName);
        }

        public static void NotEmpty(string param, string paramName)
        {
            if (string.IsNullOrWhiteSpace(param))
                throw new ArgumentException($"{paramName} is null, empty or white space");
        }
    }
}
