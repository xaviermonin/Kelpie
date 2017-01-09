using EntityCore;
using EntityCore.DynamicEntity;
using EntityCore.Entity;
using EntityCore.Entity.Event;
using EntityCore.Initialization.Metadata;
using EntityCore.Proxy.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class EntityListenerUnitTest
    {
        [TestMethod]
        [TestCategory("Entity Listener")]
        public void AddEntityListener()
        {
            var dbConnection = Effort.DbConnectionFactory.CreateTransient();
            string testListenerFqtn = typeof(TestListener).AssemblyQualifiedName;

            // Prerequisite : Adding listener

            using (var metadataContext = new MetadataInitializationContext(dbConnection, false))
            {
                var entity = metadataContext.Entities
                                            .Where(e => e.Name == "Entity")
                                            .Single();

                var listener = new EntityCore.Initialization.Metadata.Models.Listener()
                {
                    FullyQualifiedTypeName = testListenerFqtn,
                    Entity = entity,
                };

                metadataContext.Listeners.Add(listener);
                metadataContext.SaveChanges();
            }

            using (var context = Context.New(dbConnection))
            {
                context.ProxySet<IListener>()
                       .Where(i => i.FullyQualifiedTypeName == testListenerFqtn)
                       .Single();

                var entity = context.Create<IEntity>();
                entity.Name = "This name must be altered";

                context.ProxySet<IEntity>()
                       .Add(entity);

                context.SaveChanges();

                Assert.AreEqual(entity.Name, "ModifiedByTestListener");

                context.ProxySet<IEntity>()
                       .Where(e => e.Name == "ModifiedByTestListener").Single();

                context.ProxySet<IAttribute>()
                       .Where(e => e.Name == "AttributeCreatedByTestListener" &&
                                   e.Entity.Id == entity.Id)
                       .Single();
            }
        }

        private class TestListener : EntityListener
        {
            public override void PreCreate(BaseEntityContext context, DbEntityEntry creatingEntries)
            {
                var entity = (IEntity)creatingEntries.Entity;
                entity.Name = "ModifiedByTestListener";
            }

            public override void PostCreate(BaseEntityContext context, DbEntityEntry createdEntries)
            {
                DynamicEntityContext dynamicContext = (DynamicEntityContext)context;
                var entity = (IEntity)createdEntries.Entity;
                var attribute = dynamicContext.Create<IAttribute>();
                attribute.Name = "AttributeCreatedByTestListener";

                entity.Attributes.Add(attribute);

                context.SaveChanges();
            }

            public override void PostDelete(BaseEntityContext context, DbEntityEntry deletedEntries)
            {
                throw new NotImplementedException();
            }

            public override void PostUpdate(BaseEntityContext context, DbEntityEntry updatedEntries)
            {
                throw new NotImplementedException();
            }

            public override void PreDelete(BaseEntityContext context, DbEntityEntry deletingEntries)
            {
                throw new NotImplementedException();
            }

            public override void PreUpdate(BaseEntityContext context, DbEntityEntry updatingEntries)
            {
                throw new NotImplementedException();
            }
        }
    }
}
