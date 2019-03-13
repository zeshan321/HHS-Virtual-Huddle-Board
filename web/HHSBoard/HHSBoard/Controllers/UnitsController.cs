using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HHSBoard.Data;
using HHSBoard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HHSBoard.Controllers
{
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


        public async Task<IActionResult> SeedDb()
        {
            await _applicationDbContext.Units.AddAsync
            (
                new Unit { Name = "Unit A"}
            );
            
            await _applicationDbContext.SaveChangesAsync();

            return View("Index", _applicationDbContext.Units.ToList());
        }
    }
}