using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HHSBoard.Data;
using HHSBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HHSBoard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;

        public AuditController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}