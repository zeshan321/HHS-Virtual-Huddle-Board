using HHSBoard.Data;
using HHSBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            ViewBag.ChangeRequestAmount = await _applicationDbContext.ChangeRequests.CountAsync();

            return View(new UnitsViewModel
            {
                Units = await _applicationDbContext.Units.ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddNewUnit(CreateUnitModel createUnitModel)
        {
            if (string.IsNullOrEmpty(createUnitModel.Name))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Invalid unit name.");
            }

            var unit = (await _applicationDbContext.Units.AddAsync(new Unit { Name = createUnitModel.Name })).Entity;
            await _applicationDbContext.SaveChangesAsync();

            return Json(unit);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUnit(ModifyUnitModel modifyUnitModel)
        {
            var unit = await _applicationDbContext.Units.SingleOrDefaultAsync(u => u.ID == modifyUnitModel.UnitId);
            if (unit == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Unit not found!");
            }

            _applicationDbContext.Units.Remove(unit);
            await _applicationDbContext.SaveChangesAsync();

            return Json("Deleted unit!");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateName(ModifyUnitModel modifyUnitModel)
        {
            var unit = await _applicationDbContext.Units.SingleOrDefaultAsync(u => u.ID == modifyUnitModel.UnitId);
            if (unit == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Unit not found!");
            }

            if (string.IsNullOrEmpty(modifyUnitModel.Name))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Invalid unit name.");
            }

            unit.Name = modifyUnitModel.Name;
            await _applicationDbContext.SaveChangesAsync();

            return Json("Updated unit name!");
        }
    }
}