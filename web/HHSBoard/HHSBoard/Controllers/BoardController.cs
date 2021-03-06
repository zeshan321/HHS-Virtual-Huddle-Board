﻿using HHSBoard.Data;
using HHSBoard.Extensions;
using HHSBoard.Helpers;
using HHSBoard.Models;
using HHSBoard.Models.BoardViewModels;
using HHSBoard.Models.CelebrationViewModels;
using HHSBoard.Models.ImpIdeasImplementedViewModels;
using HHSBoard.Models.NewImpOpViewModels;
using HHSBoard.Models.PurposeViewModels;
using HHSBoard.Models.WipViewModels;
using HHSBoard.Models.WIPViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace HHSBoard.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostEnvironment;

        public BoardController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, IHostingEnvironment hostEnvironment)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(int boardID, TableType tableType)
        {
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var isAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var isStaff = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(staffRoleID));

            ViewBag.BoardID = boardID;
            ViewBag.TableType = tableType;
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsStaff = isStaff;
            ViewBag.ChangeRequestAmount = await _applicationDbContext.ChangeRequests.CountAsync();

            var board = _applicationDbContext.Boards.Where(b => b.ID == boardID).FirstOrDefault();
            return View(board);
        }

        public async Task<IActionResult> GetTableData(BoardTableModel boardTableViewModel)
        {
            switch (boardTableViewModel.TableType)
            {
                case TableType.CELEBRATION:
                    var celebrationData = await GetViewModel<CelebrationViewModel>(boardTableViewModel);

                    return Json(celebrationData);

                case TableType.WIP:
                    var wipData = await GetViewModel<WIPViewModel>(boardTableViewModel);

                    return Json(wipData);

                case TableType.NEWIMPOP:
                    var newImpOpData = await GetViewModel<NewImpOpViewModel>(boardTableViewModel);

                    return Json(newImpOpData);

                case TableType.IMPIDEAS:
                    var impIdeasData = await GetViewModel<ImpIdeasImplementedViewModel>(boardTableViewModel);

                    return Json(impIdeasData);
            }

            return Json("No table found.");
        }

        [HttpPost]
        public async Task<IActionResult> AddCeleration(CreateCelebrationModel createCelebrationModel)
        {
            createCelebrationModel.EncodeUserHtml();

            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));

            var board = _applicationDbContext.Boards.SingleOrDefault(b => b.ID == createCelebrationModel.BoardID);
            if (board == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json($"No board found.");
            }

            if (!createCelebrationModel.Date.HasValue)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Date is required.");
            }

            if (bypassChangeRequest)
            {
                _applicationDbContext.Celebrations.Add(new Celebration
                {
                    Who = createCelebrationModel.Who,
                    What = createCelebrationModel.What,
                    Why = createCelebrationModel.Why,
                    Date = createCelebrationModel.Date.Value,
                    BoardID = createCelebrationModel.BoardID
                });
            }
            else
            {
                dynamic json = new JObject();
                json.id = Guid.NewGuid().ToString();
                json.who = createCelebrationModel.Who;
                json.what = createCelebrationModel.What;
                json.why = createCelebrationModel.Why;
                json.date = createCelebrationModel.Date.Value;
                json.BoardID = createCelebrationModel.BoardID;

                _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                {
                    Username = user.UserName,
                    ChangeRequestType = ChangeRequestType.ADD,
                    TableName = TableType.CELEBRATION,
                    AssociatedID = -1,
                    AssociatedName = null,
                    BoardID = board.ID,
                    Values = json.ToString()
                });
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Created");
        }

        [HttpPost]
        public async Task<IActionResult> AddWIP(CreateWipModel createWipModel)
        {
            createWipModel.EncodeUserHtml();

            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));

            var board = _applicationDbContext.Boards.SingleOrDefault(b => b.ID == createWipModel.BoardID);
            if (board == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json($"No board found.");
            }

            if (!createWipModel.Date.HasValue)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Date is required.");
            }

            if (bypassChangeRequest)
            {
                _applicationDbContext.WIPs.Add(new WIP
                {
                    BoardID = createWipModel.BoardID,
                    Saftey = createWipModel.Saftey,
                    Name = createWipModel.Name,
                    Date = createWipModel.Date.Value,
                    Problem = createWipModel.Problem,
                    EightWs = createWipModel.EightWs,
                    StrategicGoals = createWipModel.StrategicGoals,
                    IsPtFamilyInvovlmentOpportunity = createWipModel.IsPtFamilyInvovlmentOpportunity,
                    PickChart = createWipModel.PickChart,
                    DateAssigned = createWipModel.DateAssigned,
                    StaffWorkingOnOpportunity = createWipModel.StaffWorkingOnOpportunity,
                    Why = createWipModel.Why,
                    JustDoIt = createWipModel.JustDoIt,
                    Updates = createWipModel.Updates
                });
            }
            else
            {
                dynamic json = new JObject();
                json.id = Guid.NewGuid().ToString();
                json.saftey = createWipModel.Saftey;
                json.name = createWipModel.Name;
                json.date = createWipModel.Date.Value;
                json.problem = createWipModel.Problem;
                json.eightWs = createWipModel.EightWs;
                json.strategicGoals = createWipModel.StrategicGoals;
                json.isPtFamilyInvovlmentOpportunity = createWipModel.IsPtFamilyInvovlmentOpportunity;
                json.pickChart = createWipModel.PickChart;
                json.dateAssigned = createWipModel.DateAssigned;
                json.staffWorkingOnOpportunity = createWipModel.StaffWorkingOnOpportunity;
                json.why = createWipModel.Why;
                json.justDoIt = createWipModel.JustDoIt;
                json.updates = createWipModel.Updates;

                _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                {
                    Username = user.UserName,
                    ChangeRequestType = ChangeRequestType.ADD,
                    TableName = TableType.WIP,
                    AssociatedID = -1,
                    AssociatedName = null,
                    BoardID = board.ID,
                    Values = json.ToString()
                });
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Created");
        }

        [HttpPost]
        public async Task<IActionResult> AddNewImpOp(CreateNewImpOp createNewImpOp)
        {
            createNewImpOp.EncodeUserHtml();

            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));

            var board = _applicationDbContext.Boards.SingleOrDefault(b => b.ID == createNewImpOp.BoardID);
            if (board == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json($"No board found.");
            }

            if (!createNewImpOp.DateIdentified.HasValue)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Date is required.");
            }

            if (bypassChangeRequest)
            {
                _applicationDbContext.NewImpOps.Add(new NewImpOp
                {
                    BoardID = createNewImpOp.BoardID,
                    Legend = createNewImpOp.Legend,
                    PersonIdentifyingOpportunity = createNewImpOp.PersonIdentifyingOpportunity,
                    DateIdentified = createNewImpOp.DateIdentified.Value,
                    Problem = createNewImpOp.Problem,
                    StaffWorkingOnOpportunity = createNewImpOp.StaffWorkingOnOpportunity,
                    StrategicGoals = createNewImpOp.StrategicGoals,
                    IsPtFamilyInvovlmentOpportunity = createNewImpOp.IsPtFamilyInvovlmentOpportunity,
                    EightWs = createNewImpOp.EightWs,
                    PickChart = createNewImpOp.PickChart,
                    JustDoIt = createNewImpOp.JustDoIt
                });
            }
            else
            {
                dynamic json = new JObject();
                json.id = Guid.NewGuid().ToString();
                json.legend = createNewImpOp.Legend;
                json.personIdentifyingOpportunity = createNewImpOp.PersonIdentifyingOpportunity;
                json.dateIdentified = createNewImpOp.DateIdentified.Value;
                json.problem = createNewImpOp.Problem;
                json.staffWorkingOnOpportunity = createNewImpOp.StaffWorkingOnOpportunity;
                json.strategicGoals = createNewImpOp.StrategicGoals;
                json.isPtFamilyInvovlmentOpportunity = createNewImpOp.IsPtFamilyInvovlmentOpportunity;
                json.eightWs = createNewImpOp.EightWs;
                json.pickChart = createNewImpOp.PickChart;
                json.justDoIt = createNewImpOp.JustDoIt;

                _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                {
                    Username = user.UserName,
                    ChangeRequestType = ChangeRequestType.ADD,
                    TableName = TableType.NEWIMPOP,
                    AssociatedID = -1,
                    AssociatedName = null,
                    BoardID = board.ID,
                    Values = json.ToString()
                });
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Created");
        }

        [HttpPost]
        public async Task<IActionResult> AddImprovement(CreateImpIdeasImplemented createImpIdeasImplemented)
        {
            createImpIdeasImplemented.EncodeUserHtml();

            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));

            var board = _applicationDbContext.Boards.SingleOrDefault(b => b.ID == createImpIdeasImplemented.BoardID);
            if (board == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json($"No board found.");
            }

            if (!createImpIdeasImplemented.Date.HasValue)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Date is required.");
            }

            if (!createImpIdeasImplemented.DateComplete.HasValue)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Date is required.");
            }

            if (bypassChangeRequest)
            {
                _applicationDbContext.ImpIdeasImplemented.Add(new ImpIdeasImplemented
                {
                    BoardID = createImpIdeasImplemented.BoardID,
                    Name = createImpIdeasImplemented.Name,
                    Date = createImpIdeasImplemented.Date.Value,
                    Problem = createImpIdeasImplemented.Problem,
                    Owner = createImpIdeasImplemented.Owner,
                    Pillar = createImpIdeasImplemented.Pillar,
                    IsPtFamilyInvovlmentOpportunity = createImpIdeasImplemented.IsPtFamilyInvovlmentOpportunity,
                    EightWs = createImpIdeasImplemented.EightWs,
                    PickChart = createImpIdeasImplemented.PickChart,
                    JustDoIt = createImpIdeasImplemented.JustDoIt,
                    Solution = createImpIdeasImplemented.Solution,
                    DateComplete = createImpIdeasImplemented.DateComplete.Value,
                    WorkCreated = createImpIdeasImplemented.WorkCreated,
                    ProcessObservationCreated = createImpIdeasImplemented.ProcessObservationCreated,
                    DateEnterIntoDatabase = createImpIdeasImplemented.DateEnterIntoDatabase,
                });
            }
            else
            {
                dynamic json = new JObject();
                json.id = Guid.NewGuid().ToString();
                json.name = createImpIdeasImplemented.Name;
                json.date = createImpIdeasImplemented.Date.Value;
                json.problem = createImpIdeasImplemented.Problem;
                json.owner = createImpIdeasImplemented.Owner;
                json.pillar = createImpIdeasImplemented.Pillar;
                json.isPtFamilyInvovlmentOpportunity = createImpIdeasImplemented.IsPtFamilyInvovlmentOpportunity;
                json.eightWs = createImpIdeasImplemented.EightWs;
                json.pickChart = createImpIdeasImplemented.PickChart;
                json.justDoIt = createImpIdeasImplemented.JustDoIt;
                json.solution = createImpIdeasImplemented.Solution;
                json.dateComplete = createImpIdeasImplemented.DateComplete.Value;
                json.workCreated = createImpIdeasImplemented.WorkCreated;
                json.processObservationCreated = createImpIdeasImplemented.ProcessObservationCreated;
                json.dateEnterIntoDatabase = createImpIdeasImplemented.DateEnterIntoDatabase;

                _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                {
                    Username = user.UserName,
                    ChangeRequestType = ChangeRequestType.ADD,
                    TableName = TableType.IMPIDEAS,
                    AssociatedID = -1,
                    AssociatedName = null,
                    BoardID = board.ID,
                    Values = json.ToString()
                });
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Created");
        }

        public async Task<IActionResult> DeleteFields(FieldDeleteModel fieldDeleteModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));

            if (fieldDeleteModel.Delete == null || !fieldDeleteModel.Delete.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("No fields sent to be deleted.");
            }

            // UUIDs in the list are change requests. Remvoe them and delete from change requests table.
            List<string> toRemove = new List<string>();
            foreach (var changeRequestDelete in fieldDeleteModel.Delete)
            {
                if (Guid.TryParse(changeRequestDelete, out var guid))
                {
                    toRemove.Add(changeRequestDelete);

                    var changeRequests = _applicationDbContext.ChangeRequests.Where(c => c.Values.Contains(guid.ToString()));
                    _applicationDbContext.ChangeRequests.RemoveRange(changeRequests);
                    await _applicationDbContext.SaveChangesAsync();
                }
            }
            foreach (var remove in toRemove)
            {
                fieldDeleteModel.Delete.Remove(remove);
            }

            if (bypassChangeRequest)
            {
                if (fieldDeleteModel.TableType == TableType.CELEBRATION)
                {
                    var celebrations = _applicationDbContext.Celebrations.Where(c => fieldDeleteModel.Delete.Contains(c.ID.ToString()));
                    _applicationDbContext.Celebrations.RemoveRange(celebrations);

                    await _applicationDbContext.SaveChangesAsync();
                }
                else if (fieldDeleteModel.TableType == TableType.WIP)
                {
                    var wip = _applicationDbContext.WIPs.Where(c => fieldDeleteModel.Delete.Contains(c.ID.ToString()));
                    _applicationDbContext.WIPs.RemoveRange(wip);

                    await _applicationDbContext.SaveChangesAsync();
                }
                else if (fieldDeleteModel.TableType == TableType.NEWIMPOP)
                {
                    var newImpOp = _applicationDbContext.NewImpOps.Where(c => fieldDeleteModel.Delete.Contains(c.ID.ToString()));
                    _applicationDbContext.NewImpOps.RemoveRange(newImpOp);

                    await _applicationDbContext.SaveChangesAsync();
                }
                else if (fieldDeleteModel.TableType == TableType.IMPIDEAS)
                {
                    var impIdeasImplemented = _applicationDbContext.ImpIdeasImplemented.Where(c => fieldDeleteModel.Delete.Contains(c.ID.ToString()));
                    _applicationDbContext.ImpIdeasImplemented.RemoveRange(impIdeasImplemented);

                    await _applicationDbContext.SaveChangesAsync();
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("No table found.");
                }
            }
            else
            {
                var previousChangeRequests = _applicationDbContext.ChangeRequests.Where(c => fieldDeleteModel.Delete.Contains(c.AssociatedID.ToString()));
                _applicationDbContext.ChangeRequests.RemoveRange(previousChangeRequests);

                foreach (var idToDelete in fieldDeleteModel.Delete)
                {
                    var id = int.Parse(idToDelete);

                    _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                    {
                        Username = user.UserName,
                        ChangeRequestType = ChangeRequestType.DELETE,
                        TableName = fieldDeleteModel.TableType,
                        AssociatedID = id,
                        AssociatedName = null,
                        BoardID = fieldDeleteModel.BoardID,
                        Values = ""
                    });
                }
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Deleted.");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateField(FieldUpdateModel fieldUpdateModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));

            if (bypassChangeRequest)
            {
                if (fieldUpdateModel.TableType == TableType.CELEBRATION)
                {
                    var celebration = await _applicationDbContext.Celebrations.Where(c => c.ID == fieldUpdateModel.Pk).FirstOrDefaultAsync();
                    var proptery = celebration.GetType().GetProperty(fieldUpdateModel.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    var memberType = proptery.PropertyType;
                    var nonNullType = Nullable.GetUnderlyingType(memberType);
                    if (nonNullType != null)
                        memberType = nonNullType;
                    var converted = ConvertHelper.ConvertType(memberType, fieldUpdateModel.Value);

                    if (converted != null)
                    {
                        proptery.SetValue(celebration, Convert.ChangeType(converted, memberType), null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json($"Invalid value format for {TableType.CELEBRATION.ToString()}");
                    }
                }
                else if (fieldUpdateModel.TableType == TableType.WIP)
                {
                    var wip = await _applicationDbContext.WIPs.Where(c => c.ID == fieldUpdateModel.Pk).FirstOrDefaultAsync();
                    var proptery = wip.GetType().GetProperty(fieldUpdateModel.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    var memberType = proptery.PropertyType;
                    var nonNullType = Nullable.GetUnderlyingType(memberType);
                    if (nonNullType != null)
                        memberType = nonNullType;
                    var converted = ConvertHelper.ConvertType(memberType, fieldUpdateModel.Value);

                    if (converted != null)
                    {
                        proptery.SetValue(wip, Convert.ChangeType(converted, memberType), null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json($"Invalid value format for {TableType.WIP.ToString()}");
                    }
                }
                else if (fieldUpdateModel.TableType == TableType.NEWIMPOP)
                {
                    var newImpOp = await _applicationDbContext.NewImpOps.Where(c => c.ID == fieldUpdateModel.Pk).FirstOrDefaultAsync();
                    var proptery = newImpOp.GetType().GetProperty(fieldUpdateModel.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    var memberType = proptery.PropertyType;
                    var nonNullType = Nullable.GetUnderlyingType(memberType);
                    if (nonNullType != null)
                        memberType = nonNullType;
                    var converted = ConvertHelper.ConvertType(memberType, fieldUpdateModel.Value);

                    if (converted != null)
                    {
                        proptery.SetValue(newImpOp, Convert.ChangeType(converted, memberType), null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json($"Invalid value format for {TableType.NEWIMPOP.ToString()}");
                    }
                }
                else if (fieldUpdateModel.TableType == TableType.IMPIDEAS)
                {
                    var impIdeasImplemented = await _applicationDbContext.ImpIdeasImplemented.Where(c => c.ID == fieldUpdateModel.Pk).FirstOrDefaultAsync();
                    var proptery = impIdeasImplemented.GetType().GetProperty(fieldUpdateModel.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    var memberType = proptery.PropertyType;
                    var nonNullType = Nullable.GetUnderlyingType(memberType);
                    if (nonNullType != null)
                        memberType = nonNullType;
                    var converted = ConvertHelper.ConvertType(memberType, fieldUpdateModel.Value);

                    if (converted != null)
                    {
                        proptery.SetValue(impIdeasImplemented, Convert.ChangeType(converted, memberType), null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json($"Invalid value format for {TableType.IMPIDEAS.ToString()}");
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("No table found.");
                }
            }
            else
            {
                dynamic jObject = new JObject();
                jObject.Name = fieldUpdateModel.Name;
                jObject.Value = fieldUpdateModel.Value ?? HttpUtility.HtmlEncode(fieldUpdateModel.Value);

                var changeRequestType = ChangeRequestType.MODIFY;
                var changeRequest = await _applicationDbContext.ChangeRequests.SingleOrDefaultAsync(c => c.AssociatedID.Equals(fieldUpdateModel.Pk) && c.AssociatedName.Equals(fieldUpdateModel.Name) && c.TableName.Equals(TableType.CELEBRATION));
                if (changeRequest != null)
                {
                    changeRequest.Values = jObject.ToString();
                }
                else
                {
                    _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                    {
                        Username = user.UserName,
                        ChangeRequestType = changeRequestType,
                        TableName = fieldUpdateModel.TableType,
                        AssociatedID = fieldUpdateModel.Pk,
                        AssociatedName = fieldUpdateModel.Name,
                        BoardID = fieldUpdateModel.BoardID,
                        Values = jObject.ToString()
                    });
                }

                await _applicationDbContext.SaveChangesAsync();
                return Json("Change requested.");
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Updated.");
        }

        public PartialViewResult CelebrationTable()
        {
            return PartialView();
        }

        public PartialViewResult Purpose()
        {
            return PartialView();
        }

        public PartialViewResult WIPTable()
        {
            return PartialView();
        }

        public PartialViewResult NewImpOpsTable()
        {
            return PartialView();
        }

        public PartialViewResult ImpIdeasImplementedTable()
        {
            return PartialView();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePurpose(PurposeUpdateModel purposeUpdateModel)
        {
            var board = await _applicationDbContext.Boards.SingleOrDefaultAsync(b => b.ID == purposeUpdateModel.BoardID);
            if (board == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error board not found!");
            }

            var purpose = await _applicationDbContext.Purpose.SingleOrDefaultAsync(p => p.BoardID == purposeUpdateModel.BoardID);
            purpose.Text = HttpUtility.HtmlEncode(purposeUpdateModel.Text);

            await _applicationDbContext.SaveChangesAsync();
            return Json("Updated.");
        }

        public async Task<object> GetViewModel(BoardTableModel boardTableViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var search = boardTableViewModel.Search?.ToUpper().Trim();

            if (boardTableViewModel.TableType == TableType.CELEBRATION)
            {
                var table = _applicationDbContext.Celebrations.Where(c => c.BoardID == boardTableViewModel.BoardID);
                var total = await table.CountAsync();
                //var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

                table = table.Sort(typeof(Celebration), boardTableViewModel.Sort, boardTableViewModel.Order);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    table = table.Where(c => c.Who.ToUpper().Contains(search)
                    || c.What.ToUpper().Contains(search)
                    || c.Why.ToUpper().Contains(search));
                }

                return new CelebrationViewModel
                {
                    Total = total,
                    Celebrations = await table.ToListAsync()
                };
            }

            if (boardTableViewModel.TableType == TableType.WIP)
            {
                var table = _applicationDbContext.WIPs.Where(c => c.BoardID == boardTableViewModel.BoardID);
                var total = await table.CountAsync();
                //var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

                table = table.Sort(typeof(WIP), boardTableViewModel.Sort, boardTableViewModel.Order);

                if (!string.IsNullOrWhiteSpace(search?.ToUpper().Trim()))
                {
                    table = table.Where(w => w.Why.ToUpper().Contains(search)
                    || w.Updates.ToUpper().Contains(search)
                    || w.StrategicGoals.ToUpper().Contains(search)
                    || w.StaffWorkingOnOpportunity.ToUpper().Contains(search)
                    || w.Saftey.ToUpper().Contains(search)
                    || w.Problem.ToUpper().Contains(search)
                    || w.Name.ToUpper().Contains(search)
                    || w.JustDoIt.ToUpper().Contains(search)
                    || w.EightWs.ToUpper().Contains(search));
                }

                return new WIPViewModel
                {
                    Total = total,
                    WIPs = await table.ToListAsync()
                };
            }

            if (boardTableViewModel.TableType == TableType.NEWIMPOP)
            {
                var table = _applicationDbContext.NewImpOps.Where(c => c.BoardID == boardTableViewModel.BoardID);
                var total = await table.CountAsync();
                //var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

                table = table.Sort(typeof(NewImpOp), boardTableViewModel.Sort, boardTableViewModel.Order);

                if (!string.IsNullOrWhiteSpace(search?.ToUpper().Trim()))
                {
                    table = table.Where(w => w.Legend.ToUpper().Contains(search)
                    || w.PersonIdentifyingOpportunity.ToUpper().Contains(search)
                    || w.Problem.ToUpper().Contains(search)
                    || w.StaffWorkingOnOpportunity.ToUpper().Contains(search)
                    || w.StrategicGoals.ToUpper().Contains(search)
                    || w.EightWs.ToUpper().Contains(search)
                    || w.JustDoIt.ToUpper().Contains(search));
                }

                return new NewImpOpViewModel
                {
                    Total = total,
                    NewImpOps = await table.ToListAsync()
                };
            }

            if (boardTableViewModel.TableType == TableType.IMPIDEAS)
            {
                var table = _applicationDbContext.ImpIdeasImplemented.Where(c => c.BoardID == boardTableViewModel.BoardID);
                var total = await table.CountAsync();
                //var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

                table = table.Sort(typeof(ImpIdeasImplemented), boardTableViewModel.Sort, boardTableViewModel.Order);

                if (!string.IsNullOrWhiteSpace(search?.ToUpper().Trim()))
                {
                    table = table.Where(w => w.Name.ToUpper().Contains(search)
                    || w.Problem.ToUpper().Contains(search)
                    || w.Owner.ToUpper().Contains(search)
                    || w.Pillar.ToUpper().Contains(search)
                    || w.EightWs.ToUpper().Contains(search)
                    || w.JustDoIt.ToUpper().Contains(search)
                    || w.Solution.ToUpper().Contains(search)
                    || w.DateEnterIntoDatabase.ToUpper().Contains(search));
                }

                return new ImpIdeasImplementedViewModel
                {
                    Total = total,
                    ImpIdeasImplementeds = await table.ToListAsync()
                };
            }

            return null;
        }

        public async Task<T> GetViewModel<T>(BoardTableModel boardTableViewModel)
        {
            return (T)await GetViewModel(boardTableViewModel);
        }

        public async Task<IActionResult> GetChangeRequests(GetChangeRequestModel getChangeRequestModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var changeRequests = await _applicationDbContext.ChangeRequests.Where(c => c.Username.Equals(user.UserName) && c.BoardID.Equals(getChangeRequestModel.BoardID) && c.TableName.Equals(getChangeRequestModel.TableType))
                .OrderBy(cr => cr.ID)
                .ToListAsync();

            return Json(changeRequests);
        }

        public PartialViewResult Drivers()
        {
            return PartialView();
        }

        public PartialViewResult Scorecards()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> FileUpload(FileUploadModel fileUploadModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));
            if (!bypassChangeRequest)
            {
                return RedirectToAction("Index", "Home");
            }

            var fileName = fileUploadModel.FormFile.FileName;
            var path = _hostEnvironment.WebRootPath + Path.DirectorySeparatorChar + "Uploads" + Path.DirectorySeparatorChar + fileUploadModel.BoardId + Path.DirectorySeparatorChar + fileUploadModel.Type;
            System.IO.Directory.CreateDirectory(path);

            var filePath = path + Path.DirectorySeparatorChar + fileName;
            if (fileUploadModel.FormFile.Length > 0)
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await fileUploadModel.FormFile.CopyToAsync(stream);
                }

                System.IO.File.SetCreationTime(filePath, DateTime.Now);
            }

            return RedirectToAction("Index", "Board", new { boardId = fileUploadModel.BoardId, TableType = fileUploadModel.TableType });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(FileUploadModel fileUploadModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));
            if (!bypassChangeRequest)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("You do not have permission!");
            }

            var fileName = fileUploadModel.FileName;
            var path = _hostEnvironment.WebRootPath + Path.DirectorySeparatorChar + "Uploads" + Path.DirectorySeparatorChar + fileUploadModel.BoardId + Path.DirectorySeparatorChar + fileUploadModel.Type;
            var filePath = path + Path.DirectorySeparatorChar + fileName;
            System.IO.File.Delete(filePath);

            return Json("Deleted!");
        }

        [HttpPost]
        public async Task<IActionResult> GetListOfFiles(FileUploadModel fileUploadModel)
        {
            var user = await _userManager.GetUserAsync(User);
            /*var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));
            if (!bypassChangeRequest)
            {
                return RedirectToAction("Index", "Home");
            }*/

            var path = _hostEnvironment.WebRootPath + Path.DirectorySeparatorChar + "Uploads" + Path.DirectorySeparatorChar + fileUploadModel.BoardId + Path.DirectorySeparatorChar + fileUploadModel.Type;
            System.IO.Directory.CreateDirectory(path);

            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
            var fileNames = files.Select(c => c.Name).ToList();

            return Json(fileNames);
        }

        [HttpPost]
        public async Task<IActionResult> UndoChangeRequests(List<int> ids)
        {
            var user = await _userManager.GetUserAsync(User);
            foreach (int id in ids)
            {
                var changeRequest = await _applicationDbContext.ChangeRequests.SingleOrDefaultAsync(c => c.ID == id);
                if (changeRequest == null || !changeRequest.Username.Equals(user.Email))
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("You are not able to undo requests of other users!");
                }

                _applicationDbContext.ChangeRequests.Remove(changeRequest);
            }
            await _applicationDbContext.SaveChangesAsync();
            return Json("Reverted change requests.");
        }

        [HttpPost]
        public async Task<IActionResult> TransferWIP(TransferModel transferModel)
        {
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var staffRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Staff"))).Id;
            var bypassChangeRequest = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && (r.RoleId.Equals(adminRoleID) || r.RoleId.Equals(staffRoleID)));
            if (!bypassChangeRequest)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("You do not have permission to transfer!");
            }

            if (transferModel.IsNewImp)
            {
                if (!_applicationDbContext.NewImpOps.Where(n => n.BoardID == transferModel.BoardID && n.ID == transferModel.RowId).Any())
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Row not found! Try again.");
                }

                var newImpOps = await _applicationDbContext.NewImpOps.FirstAsync(n => n.BoardID == transferModel.BoardID && n.ID == transferModel.RowId);

                _applicationDbContext.WIPs.Add(new WIP
                {
                    Saftey = "",
                    Name = "",
                    Date = DateTime.Now,
                    Problem = newImpOps.Problem,
                    EightWs = newImpOps.EightWs,
                    StrategicGoals = newImpOps.StrategicGoals,
                    IsPtFamilyInvovlmentOpportunity = newImpOps.IsPtFamilyInvovlmentOpportunity,
                    PickChart = newImpOps.PickChart,
                    DateAssigned = newImpOps.DateIdentified,
                    StaffWorkingOnOpportunity = newImpOps.StaffWorkingOnOpportunity,
                    Why = "",
                    JustDoIt = newImpOps.JustDoIt,
                    Updates = "",
                    BoardID = newImpOps.BoardID
                });

                _applicationDbContext.NewImpOps.Remove(newImpOps);
                await _applicationDbContext.SaveChangesAsync();
                return Json("Successfully transfered to WIP.");
            }
            else
            {
                if (!_applicationDbContext.WIPs.Where(n => n.BoardID == transferModel.BoardID && n.ID == transferModel.RowId).Any())
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Row not found! Try again.");
                }

                var wip = await _applicationDbContext.WIPs.FirstAsync(n => n.BoardID == transferModel.BoardID && n.ID == transferModel.RowId);

                _applicationDbContext.NewImpOps.Add(new NewImpOp
                {
                    DateIdentified = wip.Date,
                    Problem = wip.Problem,
                    EightWs = wip.EightWs,
                    StrategicGoals = wip.StrategicGoals,
                    IsPtFamilyInvovlmentOpportunity = wip.IsPtFamilyInvovlmentOpportunity,
                    PickChart = wip.PickChart,
                    StaffWorkingOnOpportunity = wip.StaffWorkingOnOpportunity,
                    JustDoIt = wip.JustDoIt,
                    BoardID = wip.BoardID
                });

                _applicationDbContext.WIPs.Remove(wip);
                await _applicationDbContext.SaveChangesAsync();
                return Json("Successfully transfered to back to New Improvement Opportunity.");
            }
        }
    }
}