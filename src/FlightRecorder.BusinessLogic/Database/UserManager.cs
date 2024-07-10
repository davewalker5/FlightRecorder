using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Database
{
    public class UserManager : IUserManager
    {
        private readonly Lazy<PasswordHasher<string>> _hasher;
        private readonly FlightRecorderDbContext _context;

        public UserManager(FlightRecorderDbContext context)
        {
            _hasher = new Lazy<PasswordHasher<string>>(() => new PasswordHasher<string>());
            _context = context;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(int userId)
        {
            User user = await _context.Users
                                      .Include(x => x.Attributes)
                                      .ThenInclude(x => x.UserAttribute)
                                      .FirstOrDefaultAsync(u => u.Id == userId);
            ThrowIfUserNotFound(user, userId);
            return user;
        }

        /// <summary>
        /// Return the user with the specified Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(string userName)
        {
            User user = await _context.Users
                                      .Include(x => x.Attributes)
                                      .ThenInclude(x => x.UserAttribute)
                                      .FirstOrDefaultAsync(u => u.UserName == userName);
            ThrowIfUserNotFound(user, userName);
            return user;
        }

        /// <summary>
        /// Get all the current user details
        /// </summary>
        public IAsyncEnumerable<User> GetUsersAsync() =>
            _context.Users.AsAsyncEnumerable();

        /// <summary>
        /// Add a new user, given their details
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AddUserAsync(string userName, string password)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            ThrowIfUserFound(user, userName);

            user = new User
            {
                UserName = userName,
                Password = _hasher.Value.HashPassword(userName, password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Authenticate the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> AuthenticateAsync(string userName, string password)
        {
            User user = await GetUserAsync(userName);
            PasswordVerificationResult result = _hasher.Value.VerifyHashedPassword(userName, user.Password, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.Password = _hasher.Value.HashPassword(userName, password);
                await _context.SaveChangesAsync();
            }
            return result != PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public async Task SetPasswordAsync(string userName, string password)
        {
            User user = await GetUserAsync(userName);
            user.Password = _hasher.Value.HashPassword(userName, password);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete the specified user
        /// </summary>
        /// <param name="userName"></param>
        public async Task DeleteUserAsync(string userName)
        {
            User user = await GetUserAsync(userName);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Throw an exception if a user doesn't exist
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        [ExcludeFromCodeCoverage]
        private static void ThrowIfUserNotFound(User user, object userId)
        {
            if (user == null)
            {
                string message = $"User {userId} not found";
                throw new UserNotFoundException(message);
            }
        }

        /// <summary>
        /// Throw an exception if a user already exists
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        [ExcludeFromCodeCoverage]
        private static void ThrowIfUserFound(User user, object userId)
        {
            if (user != null)
            {
                throw new UserExistsException($"User {userId} already exists");
            }
        }
    }
}
