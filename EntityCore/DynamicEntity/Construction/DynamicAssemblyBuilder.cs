using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            var entitiesFactories = CreateEntitiesBuilders(entities).ToArray();
            var typesBuilders = entitiesFactories.Select(c => c.TypeBuilder);

            foreach (var entityFactory in entitiesFactories)
                entityFactory.AddProperties();

            foreach (var entityFactory in entitiesFactories)
                entityFactory.AddNavigationProperties(typesBuilders);

            foreach (var entityFactory in entitiesFactories)
                entityFactory.AddBindedNavigationProperties(typesBuilders);

            foreach (var entityFactory in entitiesFactories)
                yield return entityFactory.TypeBuilder.CreateType();
        }

        private IEnumerable<EntityBuilder> CreateEntitiesBuilders(IEnumerable<Models.Entity> entities)
        {
            foreach (var entity in entities)
            {
                EntityBuilder entityFactory = new EntityBuilder(entity, _moduleBuilder);
                yield return entityFactory;
            }
        }

        public string Name { get; private set; }

        public void SaveAssembly()
        {
            _assemblyBuilder.Save(_assemblyBuilder.GetName().Name + ".dll");
        }
    }
}
