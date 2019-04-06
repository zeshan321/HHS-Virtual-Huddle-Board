using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using HHSBoard.Data;
using HHSBoard.Helpers;
using HHSBoard.Models;
using HHSBoard.Models.CelebrationViewModels;
using HHSBoard.Models.ChangeRequestViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace HHSBoard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ChangeRequestController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;

        public ChangeRequestController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ApproveChange(int changeRequestID)
        {
            var changeRequest = await _applicationDbContext.ChangeRequests.SingleOrDefaultAsync(c => c.ID == changeRequestID);

            switch(changeRequest.ChangeRequestType)
            {
                case ChangeRequestType.DELETE:
                    if (changeRequest.TableName == TableType.CELEBRATION)
                    {
                        var toRemove = _applicationDbContext.Celebrations.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        _applicationDbContext.Celebrations.Remove(toRemove);
                    }
                    break;

                case ChangeRequestType.UPDATE:
                    if (changeRequest.TableName == TableType.CELEBRATION)
                    {
                        var celebration = await _applicationDbContext.Celebrations.Where(c => c.ID == changeRequest.AssociatedID).FirstOrDefaultAsync();
                        var proptery = celebration.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var memberType = proptery.PropertyType;
                        var nonNullType = Nullable.GetUnderlyingType(memberType);
                        if (nonNullType != null)
                            memberType = nonNullType;
                        var converted = ConvertHelper.ConvertType(memberType, JObject.Parse(changeRequest.Values).GetValue("Value").ToString());

                        if (converted != null)
                        {
                            proptery.SetValue(celebration, Convert.ChangeType(converted, memberType), null);
                        }
                    }
                    break;
            }

            _applicationDbContext.ChangeRequests.Remove(changeRequest);
            await _applicationDbContext.SaveChangesAsync();
            return Json("Approved.");
        }

        public async Task<IActionResult> RejectChange(int changeRequestID)
        {
            var changeRequest = await _applicationDbContext.ChangeRequests.SingleOrDefaultAsync(c => c.ID == changeRequestID);

            _applicationDbContext.ChangeRequests.Remove(changeRequest);
            await _applicationDbContext.SaveChangesAsync();
            return Json("Rejected.");
        }

        public async Task<IActionResult> GetChangeRequestData(BoardTableModel boardTableViewModel)
        {
            var search = boardTableViewModel.Search?.ToUpper().Trim();
            var table = _applicationDbContext.ChangeRequests;
            var total = await table.CountAsync();
            var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

            List<ApproveViewModel> approveViewModels = new List<ApproveViewModel>();
            foreach (var changeRequest in data)
            {
                var previousValues = "";
                if (changeRequest.ChangeRequestType == Helpers.ChangeRequestType.UPDATE)
                {
                    dynamic json = new JObject();
                    json.Name = changeRequest.AssociatedName;

                    if (changeRequest.TableName == TableType.CELEBRATION)
                    {
                        var celebration = _applicationDbContext.Celebrations.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        var property = celebration.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        
                        json.Value = property.GetValue(celebration, null) ?? "";
                    }

                    previousValues = json.ToString();
                }

                approveViewModels.Add(new ApproveViewModel
                {
                    ID = changeRequest.ID,
                    Username = changeRequest.Username,
                    ChangeRequestType = changeRequest.ChangeRequestType,
                    TableName = changeRequest.TableName.ToString(),
                    AssociatedID = changeRequest.AssociatedID,
                    AssociatedName = changeRequest.AssociatedName,
                    Values = HttpUtility.HtmlDecode(changeRequest.Values),
                    Board = changeRequest.Board,
                    PreviousValues = HttpUtility.HtmlDecode(previousValues)
                });
            }

            return Json(new ChangeRequestViewModel
            {
                Total = total,
                ChangeRequests = approveViewModels
            });
        }
    }
}