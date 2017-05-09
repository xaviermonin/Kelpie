using EntityCore.DynamicEntity.Construction.Workshops;
using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction
{
    class EntityFactory
    {
        List<IEntityWorkshop> workshops = new List<IEntityWorkshop>();
        List<JobBag> _jobBags = new List<JobBag>();
        ModuleBuilder _moduleBuilder;

        public EntityFactory(ModuleBuilder moduleBuilder)
        {
            _moduleBuilder = moduleBuilder;

            workshops.Add(new InitializationWorkshop(this));
            workshops.Add(new PropertyWorkshop(this));
            workshops.Add(new NavigationPropertyWorkshop(this));
            workshops.Add(new ProxyNavigationPropertyWorkshop(this));
            workshops.Add(new ListenerWorkshop(this));
        }

        public IEnumerable<JobBag> JobBags
        {
            get
            {
                return _jobBags;
            }
        }

        public IEnumerable<Type> Build(IEnumerable<Models.Entity> entities)
        {
            IEnumerable<JobBag> jobsBags = entities.Select(e => new JobBag()
            {
                Entity = e,
                Type = CreateDynamicTypeBuilder<BaseEntity>(e)
            }).ToArray();

            _jobBags.AddRange(jobsBags);

            foreach (var w in workshops)
                w.DoWork(jobsBags);

            return jobsBags.Select(jb => jb.Type.CreateType()).ToArray();
        }

        /// <summary>
        /// Exposes a TypeBuilder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private TypeBuilder CreateDynamicTypeBuilder<T>(Models.Entity entity)
            where T : BaseEntity
        {
            TypeBuilder typeBuilder = _moduleBuilder.DefineType(_moduleBuilder.ScopeName + "." + entity.Name,
                                                              TypeAttributes.Public
                                                            | TypeAttributes.Class
                                                            | TypeAttributes.AutoClass
                                                            | TypeAttributes.AnsiClass
                                                            | TypeAttributes.Serializable
                                                            | TypeAttributes.BeforeFieldInit, typeof(T));

            return typeBuilder;
        }

        public TWorkshop GetEntityWorkshop<TWorkshop>() where TWorkshop : IEntityWorkshop
        {
            return workshops.OfType<TWorkshop>().Single();
        }
    }
}
