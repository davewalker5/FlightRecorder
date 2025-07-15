using FlightRecorder.Client.Interfaces;

namespace FlightRecorder.Mvc.Api
{
    public class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        public const string TokenSessionKey = "FlightRecorder.Token";

        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<AuthenticationTokenProvider> _logger;

        public AuthenticationTokenProvider(IHttpContextAccessor accessor, ILogger<AuthenticationTokenProvider> logger)
        {
            _accessor = accessor;
            _logger = logger;
        }

        /// <summary>
        /// Return the current API token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            var token = _accessor.HttpContext.Session.GetString(TokenSessionKey);
            _logger.LogDebug($"Retrieved API token : {token?[..50]}...");
            return token;
        }

        /// <summary>
        /// Set the API token for the current session
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetToken(string token)
        {
            _logger.LogDebug($"Setting API token to : {token?[..50]}...");
            _accessor.HttpContext.Session.SetString(TokenSessionKey, token);
        }

        /// <summary>
        /// Clear the current API token from the current session
        /// </summary>
        public void ClearToken()
        {
            _logger.LogDebug($"Clearing API token");
            _accessor.HttpContext.Session.Clear();
        }
    }
}