using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HHSBoard.Data;
using HHSBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HHSBoard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UnitsController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;

        public UnitsController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(new UnitsViewModel
            {
                Units = _applicationDbContext.Units.ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddNewUnit(CreateUnitModel createUnitModel)
        {
            if (string.IsNullOrEmpty(createUnitModel.Name))
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json("Invalid unit name.");
            }

            var unit = (await _applicationDbContext.Units.AddAsync(new Unit { Name = createUnitModel.Name })).Entity;
            await _applicationDbContext.SaveChangesAsync();

            return Json(unit);
        }
    }
}