using FlightRecorder.Api.Interfaces;
using FlightRecorder.BusinessLogic.Config;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FlightRecorder.Api.Services
{
    public class UserService : IUserService
    {
        private readonly FlightRecorderFactory _factory;
        private readonly FlightRecorderApplicationSettings _settings;

        public UserService(FlightRecorderFactory factory, IOptions<FlightRecorderApplicationSettings> settings)
        {
            _factory = factory;
            _settings = settings.Value;
        }

        /// <summary>
        /// Authenticate the specified user and, if successful, return the serialized JWT token
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> AuthenticateAsync(string userName, string password)
        {
            string serializedToken = null;

            bool authenticated = await _factory.Users.AuthenticateAsync(userName, password);
            if (authenticated)
            {
                // The user ID is used to construct the claim
                User user = await _factory.Users.GetUserAsync(userName);

                // Construct the information needed to populate the token descriptor
                byte[] key = Encoding.ASCII.GetBytes(_settings.Secret);
                var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
                DateTime expiry = DateTime.UtcNow.AddMinutes(_settings.TokenLifespanMinutes);

                // Create the descriptor containing the information used to create the JWT token
                var descriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.Name, user.UserName)
                    }),
                    Expires = expiry,
                    SigningCredentials = credentials
                };

                // Use the descriptor to create the JWT token then serialize it to
                // a string
                var handler = new JwtSecurityTokenHandler();
                SecurityToken token = handler.CreateToken(descriptor);
                serializedToken = handler.WriteToken(token);
            }

            return serializedToken;
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> AddUserAsync(string userName, string password)
        {
            return await _factory.Users.AddUserAsync(userName, password);
        }

        /// <summary>
        /// Set the password for the specified user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task SetUserPasswordAsync(string userName, string password)
        {
            await _factory.Users.SetPasswordAsync(userName, password);
        }
    }
}
