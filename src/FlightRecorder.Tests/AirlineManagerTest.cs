﻿using System.Collections.Generic;
using System.Linq;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class AirlineManagerTest
    {
        private const string EntityName = "EasyJet";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Airlines.Add(EntityName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Airlines.Add(EntityName);
            Assert.AreEqual(1, _factory.Airlines.List().Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Airline entity = _factory.Airlines.Get(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Airline entity = _factory.Airlines.Get(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Airline> entities = _factory.Airlines.List();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Airline> entities = _factory.Airlines.List(e => e.Name == EntityName);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Airline> entities = _factory.Airlines.List(e => e.Name == "Missing");
            Assert.AreEqual(0, entities.Count());
        }
    }
}
