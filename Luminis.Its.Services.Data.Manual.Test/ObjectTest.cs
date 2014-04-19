using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using Luminis.Its.Services.Data;
using Luminis.Its.Services.Data.Impl;
using Luminis.Logging;
using Luminis.Patterns.Range;

namespace Luminis.Its.Services.Data.Manual.Test
{
    /// <summary>
    /// Summary description for ObjectTest
    /// </summary>
    [TestFixture]
    public class ObjectTest
    {
        [Test]
        public void TestClean()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);
            ds.Clean();
        }

        [Test]
        public void TestAdd()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject obj = ds.CreateBaseObject(type);
            obj.Id = Guid.NewGuid();
            obj.ExtId = string.Format("test_{0}", DateTime.Now.Ticks);
            ds.InsertBaseObject(obj);
            ds.SaveChanges();

            IBaseObject savedObject = ds.GetBaseObject(obj.Id);

            Assert.IsNotNull(savedObject);
        }

        [Test]
        public void TestUpdate()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject obj = ds.CreateBaseObject(type);
            obj.Id = Guid.NewGuid();
            obj.ExtId = string.Format("test_{0}", DateTime.Now.Ticks);

            ds.InsertBaseObject(obj);
            ds.SaveChanges();

            IBaseObject savedObject = ds.GetBaseObject(obj.Id);

            savedObject.ExtId = string.Format("updated_{0}", DateTime.Now.Ticks);

            ds.SaveChanges();
            IBaseObject updatedObject = ds.GetBaseObject(obj.Id);


            Assert.IsTrue(updatedObject.ExtId.StartsWith("updated"));
        }

        [Test]
        public void TestDelete()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject obj = ds.CreateBaseObject(type);
            obj.Id = Guid.NewGuid();
            obj.ExtId = string.Format("test_{0}", DateTime.Now.Ticks);

            ds.InsertBaseObject(obj);
            ds.SaveChanges();

            IBaseObject savedObject = ds.GetBaseObject(obj.Id);
            ds.DeleteBaseObject(savedObject);
            ds.SaveChanges();

            IBaseObject deletedObject = ds.GetBaseObject(obj.Id);
            Assert.IsNull(deletedObject);
        }

        [Test]
        public void TestReference()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject refObj = ds.CreateBaseObject(type);
            refObj.Id = Guid.NewGuid();
            refObj.ExtId = string.Format("ref_{0}", DateTime.Now.Ticks);

            ds.InsertBaseObject(refObj);
            ds.SaveChanges();

            IBaseObject specObj = ds.CreateBaseObject(type);
            specObj.Id = Guid.NewGuid();
            specObj.ExtId = string.Format("spec_{0}", DateTime.Now.Ticks);
            specObj.Reference = refObj;

            ds.InsertBaseObject(specObj);
            ds.SaveChanges();

            IBaseObject savedObject = ds.GetBaseObject(specObj.Id);

            Assert.AreEqual(refObj.Id, savedObject.Reference.Id);
            Assert.IsNull(savedObject.Reference.Reference);
        }

        [Test]
        public void TestReferenceNoContext()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject refObj = ds.CreateBaseObject(type); // no insert
            refObj.Id = Guid.NewGuid();
            refObj.ExtId = string.Format("ref_{0}", DateTime.Now.Ticks);

            IBaseObject specObj = ds.CreateBaseObject(type);
            specObj.Id = Guid.NewGuid();
            specObj.ExtId = string.Format("spec_{0}", DateTime.Now.Ticks);
            specObj.Reference = refObj;                 // implicit context

            ds.InsertBaseObject(specObj);
            ds.SaveChanges();

            IBaseObject savedObject = ds.GetBaseObject(specObj.Id);
            Assert.AreEqual(refObj.Id, savedObject.Reference.Id);
        }

        [Test]
        public void TestAddJournalEntries()
        {
            DataService ds = new DataService(ConsoleLogger.Instance);

            IBaseObjectType type = ds.GetBaseObjectType("entity");
            IBaseObject obj = ds.CreateBaseObject(type);
            obj.Id = Guid.NewGuid();
            obj.ExtId = string.Format("journal_{0}", DateTime.Now.Ticks);

            IBaseObjectValue value1 = ds.CreateBaseObjectValue();
            value1.Parent = obj;
            value1.End = new TimePoint(DateTime.Now.AddSeconds(-1));
            value1.Text = "value1";

            IBaseObjectValue value2 = ds.CreateBaseObjectValue();
            value2.Parent = obj;
            value2.Start = TimePoint.Now;
            value2.Text = "value2";

            IBaseObjectJournal journalEntry = ds.CreateBaseObjectJournal();
            journalEntry.Parent = obj;
            journalEntry.When = TimePoint.Now;
            journalEntry.Username = "Alex Harbers";
            journalEntry.Before = value1;
            journalEntry.After = value2;

            ds.InsertBaseObject(obj);
            ds.SaveChanges();
        }
    }
}
