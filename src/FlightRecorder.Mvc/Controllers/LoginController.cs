using System.Threading.Tasks;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Models;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    public class LoginController : Controller
    {
        public const string TokenSessionKey = "FlightRecorder.Token";
        public const string LoginPath = "/login";

        private readonly AuthenticationClient _client;
        private readonly AddSightingWizard _wizard;

        public LoginController(AuthenticationClient client, AddSightingWizard wizard)
        {
            _client = client;
            _wizard = wizard;
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
                string token = await _client.AuthenticateAsync(model.UserName, model.Password);
                if (!string.IsNullOrEmpty(token))
                {
                    // Successful, so store the token in session, clear any cached user attributes (as the
                    // logged in user may have changed) and the location cached by the Wizard, then redirect to
                    // the home page
                    HttpContext.Session.SetString(TokenSessionKey, token);
                    _client.ClearCachedUserAttributes();
                    _wizard.ClearCachedLocation(model.UserName);
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
