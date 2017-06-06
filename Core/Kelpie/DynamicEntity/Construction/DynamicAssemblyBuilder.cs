using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Models = Kelpie.Initialization.Metadata.Models;

namespace Kelpie.DynamicEntity.Construction
{
    internal class DynamicAssemblyBuilder
    {
        private AppDomain _appDomain;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;

        public DynamicAssemblyBuilder()
            : this("Kelpie.DynamicEntity.Objects")
        {
        }

        public DynamicAssemblyBuilder(string assemblyName)
            : this(assemblyName, Thread.GetDomain())
        {

        }

        public DynamicAssemblyBuilder(string assemblyName, AppDomain appDomain)
        {
            _appDomain = appDomain;
            Name = assemblyName;

            _assemblyBuilder = _appDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName);
        }

        internal IEnumerable<Type> BuildTypes(IEnumerable<Models.Entity> entities)
        {
            EntityFactory factory = new EntityFactory(_moduleBuilder);
            return factory.Build(entities);
        }

        public string Name { get; private set; }

        public void SaveAssembly()
        {
            _assemblyBuilder.Save(Name + ".dll");
        }
    }
}
