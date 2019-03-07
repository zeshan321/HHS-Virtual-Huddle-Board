using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HHSBoard.Data;
using HHSBoard.Models;
using HHSBoard.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HHSBoard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var applicationUsers = _applicationDbContext.ApplicationUsers.ToList();
            // Example of getting user with the id 'test'
            ////_applicationDbContext.ApplicationUsers.Where(u => u.Id == "test").First();
            // Example of checking if user with the id 'test' exists
            ////_applicationDbContext.ApplicationUsers.Where(u => u.Id == "test").Any();

            return View(new AdminIndexViewModel
            {
                ApplicationUsers = applicationUsers
            });
        }
    }
}