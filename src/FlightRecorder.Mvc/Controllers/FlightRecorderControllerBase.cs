using System.Reflection;
using FlightRecorder.Mvc.Interfaces;
using HealthTracker.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlightRecorder.Mvc.Controllers
{
    public abstract class FlightRecorderControllerBase : Controller
    {
        public static readonly string Version = Assembly.GetExecutingAssembly()
                                                        .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                                                        .Version;

        private readonly IPartialViewToStringRenderer _renderer;
        protected readonly ILogger _logger;

        public FlightRecorderControllerBase(IPartialViewToStringRenderer renderer, ILogger logger)
        {
            _renderer = renderer;
            _logger = logger;
        }

        /// <summary>
        /// Add the version to the view data on each request
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewData["Version"] = Version;
        }

        /// <summary>
        /// Log the contents of model state
        /// </summary>
        protected void LogModelState()
        {
            LogModelStateValues();
            LogModelStateErrors();
        }

        /// <summary>
        /// Log model state keys and values
        /// </summary>
        private void LogModelStateValues()
        {
            foreach (var kvp in ModelState)
            {
                var entry = kvp.Value;
                var attemptedValue = entry?.AttemptedValue;
                var rawValue = entry?.RawValue?.ToString();
                var isValid = entry?.Errors?.Count == 0;

                _logger.LogDebug($"Model State Key {kvp.Key}: Attempted = {attemptedValue}, Raw = {rawValue}, Valid =  {isValid}");
            }
        }

        /// <summary>
        /// Log model state errors
        /// </summary>
        private void LogModelStateErrors()
        {
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogDebug(error.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Render a specified partial view using a model and show the results in a modal dialog
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> LoadModalContent(string viewName, object model, string title)
        {
            _logger.LogDebug($"Rendering view {viewName} with model {model}");

            var html = await _renderer.RenderPartialViewToStringAsync(viewName, model);

            _logger.LogDebug($"HTML markup = {html}");

            var result = new AjaxModalResponse
            {
                Title = title,
                HtmlContent = html
            };

            _logger.LogDebug($"Modal model = {result}");

            return Json(result);
        }
    }
}