using System.Threading.Tasks;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    public class LoginController : Controller
    {
        public const string TokenSessionKey = "Token";
        public const string LoginPath = "/login";

        private readonly AuthenticationClient _client;

        public LoginController(AuthenticationClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the login page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            LoginViewModel model = new LoginViewModel();
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
                string token = await _client.AuthenticateAsync(model.UserName, model.Password);
                if (!string.IsNullOrEmpty(token))
                {
                    // Successful, so store the token in session and redirect to the home page
                    HttpContext.Session.SetString(TokenSessionKey, token);
                    result = RedirectToAction("Index", "Home");
                }
                else
                {
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
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
