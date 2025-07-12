using System.Security.Authentication;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    public class LoginController : Controller
    {
        public const string LoginPath = "/login";

        private readonly IAuthenticationClient _client;
        private readonly IAuthenticationTokenProvider _tokenProvider;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IAuthenticationClient client, IAuthenticationTokenProvider provider, ILogger<LoginController> logger)
        {
            _client = client;
            _tokenProvider = provider;
            _logger = logger;
        }

        /// <summary>
        /// Serve the login page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle a request to login
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                // Authenticate with the sevice
                string token = "";
                try
                {
                    token = await _client.AuthenticateAsync(model.UserName, model.Password);
                }
                catch (AuthenticationException)
                {
                    // Authentication failures raise an AuthenticationException from the client
                }

                // Validate the token
                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogDebug($"Successfully authenticated as user {model.UserName}");

                    // Successful, so store the token in session, and redirect to the home page
                    _tokenProvider.SetToken(token);
                    result = RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogDebug($"Failed to authenticate as user {model.UserName}");
                    model.Message = "Incorrect username or password";
                    result = View(model);
                }
            }
            else
            {
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle a request to log out
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult LogOut()
        {
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }
    }
}
