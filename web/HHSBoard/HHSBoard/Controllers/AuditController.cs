﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HHSBoard.Data;
using HHSBoard.Models;
using HHSBoard.Models.AuditViewModels;
using HHSBoard.Models.CelebrationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            ViewBag.ChangeRequestAmount = await _applicationDbContext.ChangeRequests.CountAsync();
            return View();
        }

        public async Task<IActionResult> GetAuditData(BoardTableModel boardTableViewModel)
        {
            var search = boardTableViewModel.Search?.ToUpper().Trim();
            var table = _applicationDbContext.Audits.OrderByDescending(a => a.ID);
            var total = await table.CountAsync();
            var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(a => a.KeyValues.ToUpper().Contains(search)
                || a.NewValues.ToUpper().Contains(search)
                || a.OldValues.ToUpper().Contains(search)
                || a.State.ToUpper().Contains(search)
                || a.TableName.ToUpper().Contains(search)
                || a.Username.ToUpper().Contains(search)
                || a.DateTime.ToString().ToUpper().Contains(search));
            }

            var list = await data.ToListAsync();

            foreach (var audit in list)
            {
                audit.OldValues = HttpUtility.HtmlEncode(audit.OldValues);
                audit.NewValues = HttpUtility.HtmlEncode(audit.NewValues);
            }

            return Json(new AuditViewModel
            {
                Total = total,
                Audits = list
            });
        }
    }
}