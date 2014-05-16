using System;
using System.Linq;
using TimeTraveller.Services.Data.CouchDB;
using TimeTraveller.General.Logging;
using TimeTraveller.General.Patterns.Range;

using NUnit.Framework;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Data.Manual.Test
{
    /// <summary>
    /// Summary description for ObjectValueTest
    /// </summary>
    [TestFixture]
    public class ObjectValueTest
    {
        [Test]
        public void TestAdd()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject obj = ds.CreateBaseObject(type);
            obj.Id = Guid.NewGuid();
            obj.ExtId = string.Format("test_{0}", DateTime.Now.Ticks);
            ds.InsertBaseObject(obj);

            IBaseObjectValue ov = ds.CreateBaseObjectValue();
            ov.Parent = obj;
            ov.Text = "test";
            ds.SaveChanges();

            IBaseObjectValue savedObjectValue = ds.GetBaseObjectValue(ov.Id);

            Assert.IsNotNull(savedObjectValue);
            Assert.AreEqual("test", savedObjectValue.Text);
            Assert.IsTrue(savedObjectValue.Parent.ExtId.StartsWith("test"));
        }

        [Test]
        public void TestAddMulti()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject obj = ds.CreateBaseObject(type);
            obj.Id = Guid.NewGuid();
            obj.ExtId = string.Format("test_{0}", DateTime.Now.Ticks);
            ds.InsertBaseObject(obj);
            ds.SaveChanges();

            IBaseObjectValue ov = ds.CreateBaseObjectValue();
            ov.Parent = obj;
            ov.Text = "test";
            ds.SaveChanges();

            IBaseObjectValue ov2 = ds.CreateBaseObjectValue();
            ov2.Parent = obj;
            ov2.Text = "test2";
            ds.SaveChanges();

            IBaseObjectValue savedObjectValue = ds.GetBaseObjectValue(ov.Id);
            Assert.IsNotNull(savedObjectValue);
            Assert.IsNotNull(ds.GetBaseObjectValue(ov2.Id));

            obj = ds.GetBaseObject(obj.Id);

            Assert.AreEqual(2, obj.Values.Count());
        }

        [Test]
        public void TestAddSequence()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            string extId = string.Format("test_{0}", DateTime.Now.Ticks);
            TimePoint timestamp = TimePoint.Now;

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObjectValue v1 = ds.InsertValue("test", timestamp.AddSeconds(-1), Guid.NewGuid(), extId, type);
            IBaseObjectValue v2 = ds.InsertValue("updated test", timestamp, v1.Parent);

            ds.SaveChanges();

            IBaseObject obj = ds.GetBaseObject(extId, type);

            Assert.AreNotEqual(Guid.Empty, obj.Id);
            Assert.AreEqual(2, obj.Values.Count());
            Assert.AreEqual("test", ds.GetValue(extId, type, timestamp.AddSeconds(-1)).Text);
            Assert.AreEqual("updated test", ds.GetValue(extId, type).Text);
        }

        [Test]
        public void TestAddWithReference()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject obj1 = ds.CreateBaseObject(type);
            obj1.Id = Guid.NewGuid();
            obj1.ExtId = string.Format("obj1_{0}", DateTime.Now.Ticks);
            ds.InsertBaseObject(obj1);
            ds.SaveChanges();

            IBaseObjectValue ov1 = ds.CreateBaseObjectValue();
            ov1.Parent = obj1;
            ov1.ContentType = string.Empty;
            ds.SaveChanges();

            IBaseObject obj2 = ds.CreateBaseObject(type);
            obj2.Id = Guid.NewGuid();
            obj2.ExtId = string.Format("obj2_{0}", DateTime.Now.Ticks);
            obj2.Reference = obj1;
            ds.InsertBaseObject(obj2);
            ds.SaveChanges();

            IBaseObjectValue ov2 = ds.CreateBaseObjectValue();
            ov2.Parent = obj2;
            ov2.Reference = ov1;
            ov2.ContentType = string.Empty;
            ds.SaveChanges();

            IBaseObjectValue savedObjectValue = ds.GetBaseObjectValue(ov2.Id);
            Assert.IsNotNull(savedObjectValue);

            Assert.AreEqual(savedObjectValue.Reference.Id, ov1.Id);
        }

        [Test]
        public void TestAddWithRelations()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType entityType = ds.GetBaseObjectType("entity");
            IEntityObject entity1 = ds.CreateBaseObject(entityType) as IEntityObject;
            entity1.Id = Guid.NewGuid();
            entity1.ExtId = string.Format("entity1_{0}", DateTime.Now.Ticks);
            ds.InsertBaseObject(entity1);
            ds.SaveChanges();

            IBaseObjectValue ov1 = ds.CreateBaseObjectValue();
            ov1.Parent = entity1;
            ov1.ContentType = string.Empty;
            ds.SaveChanges();

            IEntityObject entity2 = ds.CreateBaseObject(entityType) as IEntityObject;
            entity2.Id = Guid.NewGuid();
            entity2.ExtId = string.Format("entity2_{0}", DateTime.Now.Ticks);
            ds.InsertBaseObject(entity2);
            ds.SaveChanges();

            IBaseObjectValue ov2 = ds.CreateBaseObjectValue();
            ov2.Parent = entity2;
            ov2.Reference = ov1;
            ov2.ContentType = string.Empty;
            ds.SaveChanges();

            IBaseObjectType relationType = ds.GetBaseObjectType("relation");
            IRelationObject relation = ds.CreateBaseObject(relationType) as IRelationObject;
            relation.Id = Guid.NewGuid();
            relation.ExtId = "relation";
            relation.Relation1 = entity1;
            relation.Relation2 = entity2;

            IBaseObjectValue ov3 = ds.CreateBaseObjectValue();
            ov3.Parent = relation;
            ov3.ContentType = string.Empty;
            ds.SaveChanges();

            IBaseObjectValue savedObjectValue = ds.GetBaseObjectValue(ov3.Id);
            Assert.IsNotNull(savedObjectValue);

            Assert.AreEqual(savedObjectValue.Parent.Id, relation.Id);
            IRelationObject savedObject = savedObjectValue.Parent as IRelationObject;
            Assert.AreEqual(savedObject.Relation1.Id, entity1.Id);
            Assert.AreEqual(savedObject.Relation2.Id, entity2.Id);
        }

        [Test]
        public void TestGetObjectModel()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("objectmodel");
            IBaseObject baseObject = ds.GetBaseObject("myobjectmodel", type);
            if (baseObject != null)
            {
                Console.WriteLine(baseObject.ExtId);

                var values = baseObject.Values.ToArray();

                foreach (IBaseObjectValue bov in values)
                {
                    Console.WriteLine(bov.Text);
                }
            }
        }

        //[Test]
        public void TestInsertValues()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("casefilespecification");
            IBaseObject cfsBaseObject = ds.GetBaseObject("MyObjectmodel/MySpecification", type);
            IBaseObjectValue caseFileSpecification = ds.GetValue(cfsBaseObject.Id);
            IBaseObjectValue objectModel = ds.GetBaseObjectValue(caseFileSpecification.ReferenceId);

            Guid house1Guid = Guid.NewGuid();
            Guid house2Guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            Guid ownsHouse1Guid = Guid.NewGuid();
            Guid ownsHouse2Guid = Guid.NewGuid();

            ds.GetBaseObjectValue(objectModel.Id);

            ds.GetBaseObject(house1Guid);
            IBaseObjectType entityType = ds.GetBaseObjectType("entity");
            IBaseObjectValue house1 = ds.InsertValue("Patersstraat", TimePoint.Now, house1Guid, "House", entityType, objectModel);

            ds.GetBaseObject(house2Guid);
            IBaseObjectValue house2 = ds.InsertValue("Lindelaan", TimePoint.Now, house2Guid, "House", entityType, objectModel);

            ds.GetBaseObject(personGuid);
            IBaseObjectValue person = ds.InsertValue("Alex.Harbers", TimePoint.Now, personGuid, "Person", entityType, objectModel);

            ds.GetBaseObject(ownsHouse1Guid);
            IBaseObjectType relationType = ds.GetBaseObjectType("relation");
            IBaseObjectValue ownshouse1 = ds.InsertValue("", TimePoint.Now, ownsHouse1Guid, "OwnsHouse", relationType, objectModel);

            ds.GetBaseObject(ownsHouse2Guid);
            IBaseObjectValue ownshouse2 = ds.InsertValue("", TimePoint.Now, ownsHouse2Guid, "OwnsHouse", relationType, objectModel);

            IRelationObject ownsHouse1Relation = ownshouse1.Parent as IRelationObject;
            ownsHouse1Relation.Relation1 = person.Parent as IEntityObject;
            ownsHouse1Relation.Relation2 = house1.Parent as IEntityObject;

            IRelationObject ownsHouse2Relation = ownshouse2.Parent as IRelationObject;
            ownsHouse2Relation.Relation1 = person.Parent as IEntityObject;
            ownsHouse2Relation.Relation2 = house2.Parent as IEntityObject;

            ds.SaveChanges();
        }
    }
}
