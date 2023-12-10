using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class CacheManagementController : Controller
    {
        private readonly ICacheWrapper _cacheWrapper;

        public CacheManagementController(ICacheWrapper cacheWrapper)
        {
            _cacheWrapper = cacheWrapper;
        }

        /// <summary>
        /// Cache management home page
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index(string filter = "")
        {
            CacheManagementViewModel model = new CacheManagementViewModel
            {
                Filter = filter
            };
            return View(model);
        }

        /// <summary>
        /// Handle POST events to clear the cache
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CacheManagementViewModel model)
        {
            if (ModelState.IsValid)
            {
                _cacheWrapper.Clear();
                ModelState.Clear();
                model.Clear();
                model.Message = $"The cache has been cleared";
            }

            return View(model);
        }

        /// <summary>
        /// Return an (optionally) filtered list of cached data keys
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult List(string filter)
        {
            List<string> model = _cacheWrapper.GetFilteredKeys(filter).ToList();
            return PartialView(model);
        }

        /// <summary>
        /// Handle POST events to remove individual items from the cache
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Remove(string key)
        {
            _cacheWrapper.Remove(key);
            return Json(new
            {
                message = $"'{key}' has been removed from the cache"
            });
        }
    }
}
