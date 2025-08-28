using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class UserManagerTest
    {
        private const string UserName = "Some User";
        private const string Password = "password";
        private const string UpdatedPassword = "newpassword";

        private FlightRecorderFactory _factory;
        private User _user;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context, new MockFileLogger());

            _user = Task.Run(() => _factory.Users.AddUserAsync(UserName, Password)).Result;
        }

        [TestMethod, ExpectedException(typeof(UserExistsException))]
        public async Task AddExistingUserAsyncTest()
        {
            await _factory.Users.AddUserAsync(UserName, Password);
        }

        [TestMethod]
        public async Task DeleteUserAsyncTest()
        {
            await _factory.Users.DeleteUserAsync(UserName);
            List<User> users = await _factory.Users.GetUsersAsync().ToListAsync();
            Assert.IsFalse(users.Any());
        }

        [TestMethod]
        public async Task GetUserByIdAsyncTest()
        {
            User user = await _factory.Users.GetUserAsync(_user.Id);
            Assert.AreEqual(UserName, user.UserName);
            Assert.AreNotEqual(Password, user.Password);
        }

        [TestMethod, ExpectedException(typeof(UserNotFoundException))]
        public async Task GetMissingUserByIdAsyncTest()
        {
            await _factory.Users.GetUserAsync(-1);
        }

        [TestMethod]
        public async Task GetAllUsersAsyncTest()
        {
            List<User> users = await _factory.Users.GetUsersAsync().ToListAsync();
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual(UserName, _factory.Context.Users.First().UserName);
            Assert.AreNotEqual(Password, _factory.Context.Users.First().Password);
        }

        [TestMethod]
        public async Task AuthenticateAsyncTest()
        {
            bool authenticated = await _factory.Users.AuthenticateAsync(UserName, Password);
            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public async Task FailedAuthenticationTest()
        {
            bool authenticated = await _factory.Users.AuthenticateAsync(UserName, "the wrong password");
            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public async Task SetPassswordAsyncTest()
        {
            await _factory.Users.SetPasswordAsync(UserName, UpdatedPassword);
            bool authenticated = await _factory.Users.AuthenticateAsync(UserName, UpdatedPassword);
            Assert.IsTrue(authenticated);
        }
    }
}
