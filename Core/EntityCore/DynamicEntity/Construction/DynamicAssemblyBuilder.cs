using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction
{
    internal class DynamicAssemblyBuilder
    {
        private AppDomain _appDomain;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;

        public DynamicAssemblyBuilder()
            : this("EntityCore.DynamicEntity.Objects")
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

            if (_assemblyBuilder == null)
                _assemblyBuilder = _appDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);

            //vital to ensure the namespace of the assembly is the same as the module name, else IL inspectors will fail
            if (_moduleBuilder == null)
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
        }

        internal IEnumerable<Type> BuildTypes(IEnumerable<Models.Entity> entities)
        {
            EntityFactory factory = new EntityFactory(_moduleBuilder);
            return factory.Build(entities);
        }

        public string Name { get; private set; }

        public void SaveAssembly()
        {
            _assemblyBuilder.Save(_assemblyBuilder.GetName().Name + ".dll");
        }
    }
}
