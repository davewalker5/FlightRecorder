using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class UserManagerTest
    {
        private const string UserName = "Some User";
        private const string AsyncUserName = "Some Other User";
        private const string Password = "password";
        private const string UpdatedPassword = "newpassword";

        private FlightRecorderFactory _factory;
        private int _userId;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);

            User user = _factory.Users.AddUser(UserName, Password);
            _factory.Context.SaveChanges();
            _userId = user.Id;
        }

        [TestMethod]
        public void AddUserTest()
        {
            // The  user has been added during test initialisation. All that needs
            // to be done here is to validate it
            Assert.AreEqual(1, _factory.Context.Users.Count());
            Assert.AreEqual(UserName, _factory.Context.Users.First().UserName);

            // Password is hashed and so should *not* equal the supplied password
            Assert.AreNotEqual(Password, _factory.Context.Users.First().Password);
        }

        [TestMethod]
        public async Task AddUserAsyncTest()
        {
            User user = await _factory.Users.AddUserAsync(AsyncUserName, Password);
            await _factory.Context.SaveChangesAsync();
            Assert.AreEqual(2, _factory.Context.Users.Count());
            Assert.AreEqual(AsyncUserName, user.UserName);
            Assert.AreNotEqual(Password, user.Password);
        }

        [TestMethod, ExpectedException(typeof(UserExistsException))]
        public void AddExistingUserTest()
        {
            _factory.Users.AddUser(UserName, Password);
        }

        [TestMethod]
        public void DeleteUserTest()
        {
            _factory.Users.DeleteUser(UserName);
            IEnumerable<User> users = _factory.Users.GetUsers();
            Assert.IsFalse(users.Any());
        }

        [TestMethod]
        public async Task DeleteUserAsyncTest()
        {
            await _factory.Users.DeleteUserAsync(UserName);
            List<User> users = await _factory.Users.GetUsersAsync().ToListAsync();
            Assert.IsFalse(users.Any());
        }

        [TestMethod]
        public void GetUserByIdTest()
        {
            User user = _factory.Users.GetUser(_userId);
            Assert.AreEqual(UserName, user.UserName);
            Assert.AreNotEqual(Password, user.Password);
        }

        [TestMethod]
        public async Task GetUserByIdAsyncTest()
        {
            User user = await _factory.Users.GetUserAsync(_userId);
            Assert.AreEqual(UserName, user.UserName);
            Assert.AreNotEqual(Password, user.Password);
        }

        [TestMethod, ExpectedException(typeof(UserNotFoundException))]
        public void GetMissingUserByIdTest()
        {
            _factory.Users.GetUser(-1);
        }

        [TestMethod]
        public void GetAllUsersTest()
        {
            IEnumerable<User> users = _factory.Users.GetUsers();
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual(UserName, _factory.Context.Users.First().UserName);
            Assert.AreNotEqual(Password, _factory.Context.Users.First().Password);
        }

        [TestMethod]
        public async Task GetAllUsersAsyncTest()
        {
            List<User> users = await _factory.Users.GetUsersAsync().ToListAsync();
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual(UserName, _factory.Context.Users.First().UserName);
            Assert.AreNotEqual(Password, _factory.Context.Users.First().Password);
        }

        [TestMethod]
        public void AuthenticateTest()
        {
            bool authenticated = _factory.Users.Authenticate(UserName, Password);
            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public async Task AuthenticateAsyncTest()
        {
            bool authenticated = await _factory.Users.AuthenticateAsync(UserName, Password);
            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public void FailedAuthenticationTest()
        {
            bool authenticated = _factory.Users.Authenticate(UserName, "the wrong password");
            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public void SetPassswordTest()
        {
            _factory.Users.SetPassword(UserName, UpdatedPassword);
            bool authenticated = _factory.Users.Authenticate(UserName, UpdatedPassword);
            Assert.IsTrue(authenticated);
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
