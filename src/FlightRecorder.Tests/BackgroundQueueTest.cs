using FlightRecorder.BusinessLogic.Logic;
using FlightRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class BackgroundQueueTest
    {
        private const long EntityId = 6532;
        private const string EntityName = "British Airways";

        private BackgroundQueue<Airline> _queue;

        [TestInitialize]
        public void Initialise()
        {
            _queue = new BackgroundQueue<Airline>();
        }

        [TestMethod]
        public void EnqueueDequeueTest()
        {
            var airline = new Airline { Id = EntityId, Name = EntityName };
            _queue.Enqueue(airline);
            var dequeued = _queue.Dequeue();

            Assert.IsNotNull(dequeued);
            Assert.AreEqual(EntityId, dequeued.Id);
            Assert.AreEqual(EntityName, dequeued.Name);
        }

        [TestMethod]
        public void EmptyQueueTest()
        {
            var dequeued = _queue.Dequeue();
            Assert.IsNull(dequeued);
        }
    }
}
