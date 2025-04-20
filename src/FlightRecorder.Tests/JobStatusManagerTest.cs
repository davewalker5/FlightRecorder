using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class JobStatusManagerTest
    {
        private const string Name = "SightingsExport";
        private const string Parameters = "2023-10-24 Export.csv";
        private const string Error = "Some error message";

        private FlightRecorderFactory _factory;
        private long _statusId;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _statusId = Task.Run(() => _factory.JobStatuses.AddAsync(Name, Parameters)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            var status = await _factory.JobStatuses.GetAsync(x => x.Id == _statusId);

            Assert.IsNotNull(status);
            Assert.AreEqual(_statusId, status.Id);
            Assert.AreEqual(Name, status.Name);
            Assert.AreEqual(Parameters, status.Parameters);
            Assert.IsNotNull(status.Start);
            Assert.IsNull(status.End);
            Assert.IsNull(status.Error);
        }

        [TestMethod]
        public async Task ListAsyncTest()
        {
            var statuses = await _factory.JobStatuses.ListAsync(x => true, 1, 10).ToListAsync();

            Assert.IsNotNull(statuses);
            Assert.AreEqual(1, statuses.Count);
            Assert.AreEqual(_statusId, statuses[0].Id);
            Assert.AreEqual(Name, statuses[0].Name);
            Assert.AreEqual(Parameters, statuses[0].Parameters);
            Assert.IsNotNull(statuses[0].Start);
            Assert.IsNull(statuses[0].End);
            Assert.IsNull(statuses[0].Error);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            var statuses = await _factory.JobStatuses.ListAsync(null, 1, 10).ToListAsync();

            Assert.IsNotNull(statuses);
            Assert.AreEqual(1, statuses.Count);
            Assert.AreEqual(_statusId, statuses[0].Id);
            Assert.AreEqual(Name, statuses[0].Name);
            Assert.AreEqual(Parameters, statuses[0].Parameters);
            Assert.IsNotNull(statuses[0].Start);
            Assert.IsNull(statuses[0].End);
            Assert.IsNull(statuses[0].Error);
        }

        [TestMethod]
        public async Task UpdateAsyncTest()
        {
            var status = await _factory.JobStatuses.UpdateAsync(_statusId, Error);

            Assert.IsNotNull(status);
            Assert.AreEqual(_statusId, status.Id);
            Assert.AreEqual(Name, status.Name);
            Assert.AreEqual(Parameters, status.Parameters);
            Assert.IsNotNull(status.Start);
            Assert.IsNotNull(status.End);
            Assert.AreEqual(Error, status.Error);
        }
    }
}
