using HHSBoard.Data;
using HHSBoard.Models;
using HHSBoard.Models.AdminViewModels;
using HHSBoard.Models.CelebrationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HHSBoard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public AdminController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ChangeRequestAmount = await _applicationDbContext.ChangeRequests.CountAsync();
            return View();
        }

        public async Task<IActionResult> UpdateUnitAccess(UpdateUnitAccessModel updateUnitAccessModel)
        {
            var user = await _applicationDbContext.Users.SingleOrDefaultAsync(u => u.Id.Equals(updateUnitAccessModel.UserID));
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("No user found");
            }

            var unit = await _applicationDbContext.Units.SingleOrDefaultAsync(u => u.ID.Equals(updateUnitAccessModel.UnitID));
            if (unit == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("No unit found");
            }

            if (updateUnitAccessModel.Adding)
            {
                var hasUnitAccess = _applicationDbContext.UnitAccesses.Any(u => u.UserID.Equals(user.Id) && u.UnitID.Equals(unit.ID));
                if(!hasUnitAccess)
                {
                    _applicationDbContext.UnitAccesses.Add(new UnitAccess
                    {
                        UnitID = unit.ID,
                        UserID = user.Id,
                    });
                }
            }
            else
            {
                _applicationDbContext.UnitAccesses.RemoveRange(_applicationDbContext.UnitAccesses.Where(u => u.UserID.Equals(user.Id) && u.UnitID.Equals(unit.ID)));
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Updated unit access");
        }

        public async Task<IActionResult> UpdateAdminRole(UpdateRoleModel updateAdminRoleModel)
        {
            var user = await _applicationDbContext.Users.SingleOrDefaultAsync(u => u.Id.Equals(updateAdminRoleModel.ID));
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("No user found");
            }

            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            
            var isAlreadyAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));
            if (isAlreadyAdmin)
            {
                _applicationDbContext.UserRoles.RemoveRange(_applicationDbContext.UserRoles.Where(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID)));
            }
            else
            {
                await _applicationDbContext.UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = adminRoleID
                });
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Updated role.");
        }

        public async Task<IActionResult> UpdateStaffRole(UpdateRoleModel updateRoleModel)
        {
            var user = await _applicationDbContext.Users.SingleOrDefaultAsync(u => u.Id.Equals(updateRoleModel.ID));
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("No user found");
            }

            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;

            var isAlreadyStaff = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(staffRoleID));
            if (isAlreadyStaff)
            {
                _applicationDbContext.UserRoles.RemoveRange(_applicationDbContext.UserRoles.Where(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(staffRoleID)));
            }
            else
            {
                await _applicationDbContext.UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = staffRoleID
                });
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Updated role.");
        }

        public async Task<IActionResult> GetUserData(BoardTableModel boardTableViewModel)
        {
            var search = boardTableViewModel.Search?.ToUpper().Trim();
            var table = _applicationDbContext.Users;
            var total = await table.CountAsync();
            var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;

            List<UserViewModel> userViewModels = new List<UserViewModel>();
            foreach (var applicationUser in table)
            {
                List<AdminUnitViewModel> adminUnitViewModels = new List<AdminUnitViewModel>();
                foreach(var unit in _applicationDbContext.Units)
                {
                    adminUnitViewModels.Add(new AdminUnitViewModel
                    {
                        ID = unit.ID,
                        Name = unit.Name,
                        HasAccess = _applicationDbContext.UnitAccesses.Any(u => u.UserID.Equals(applicationUser.Id) && u.UnitID.Equals(unit.ID))
                    });
                }

                userViewModels.Add(new UserViewModel
                {
                    ID = applicationUser.Id,
                    Username = applicationUser.UserName,
                    IsAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(applicationUser.Id) && r.RoleId.Equals(adminRoleID)),
                    IsStaff = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(applicationUser.Id) && r.RoleId.Equals(staffRoleID)),
                    AdminUnitViewModels = adminUnitViewModels
                });
            }

            return Json(new UsersViewModel
            {
                Total = total,
                UserViewModels = userViewModels
            });
        }
    }
}