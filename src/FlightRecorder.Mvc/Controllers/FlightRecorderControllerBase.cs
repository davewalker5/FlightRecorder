using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlightRecorder.Mvc.Controllers
{
    public abstract class FlightRecorderControllerBase : Controller
    {
        public static readonly string Version = Assembly.GetExecutingAssembly()
                                                        .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                                                        .Version;

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
        /// Log model state errors
        /// </summary>
        /// <param name="logger"></param>
        protected void LogModelStateErrors(ILogger logger)
        {
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    logger.LogDebug(error.ErrorMessage);
                }
            }
        }
    }
}