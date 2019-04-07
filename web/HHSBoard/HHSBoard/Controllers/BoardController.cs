using HHSBoard.Data;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        public BoardController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int boardID, TableType tableType)
        {
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var isAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));

            ViewBag.BoardID = boardID;
            ViewBag.TableType = tableType;
            ViewBag.IsAdmin = isAdmin;
            
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
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var isAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));

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

            if (isAdmin)
            {
                _applicationDbContext.Celebrations.Add(new Celebration
                {
                    Who = createCelebrationModel.Who ?? HttpUtility.HtmlEncode(createCelebrationModel.Who),
                    What = createCelebrationModel.What ?? HttpUtility.HtmlEncode(createCelebrationModel.What),
                    Why = createCelebrationModel.Why ?? HttpUtility.HtmlEncode(createCelebrationModel.Why),
                    Date = createCelebrationModel.Date.Value,
                    BoardID = createCelebrationModel.BoardID
                });
            }
            else
            {
                dynamic json = new JObject();
                json.id = Guid.NewGuid().ToString();
                json.who = createCelebrationModel.Who ?? HttpUtility.HtmlEncode(createCelebrationModel.Who);
                json.what = createCelebrationModel.What ?? HttpUtility.HtmlEncode(createCelebrationModel.What);
                json.why = createCelebrationModel.What ?? HttpUtility.HtmlEncode(createCelebrationModel.What);
                json.date = createCelebrationModel.Date.Value;
                json.BoardID = createCelebrationModel.BoardID;

                _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                {
                    Username = user.UserName,
                    ChangeRequestType = ChangeRequestType.CREATE,
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
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var isAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));
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

            if (isAdmin)
            {
                _applicationDbContext.WIPs.Add(new WIP
                {
                    BoardID = createWipModel.BoardID,
                    Saftey = createWipModel.Saftey ?? HttpUtility.HtmlEncode(createWipModel.Saftey),
                    Name = createWipModel.Name ?? HttpUtility.HtmlEncode(createWipModel.Name),
                    Date = createWipModel.Date.Value,
                    Problem = createWipModel.Problem ?? HttpUtility.HtmlEncode(createWipModel.Problem),
                    EightWs = createWipModel.EightWs ?? HttpUtility.HtmlEncode(createWipModel.EightWs),
                    StrategicGoals = createWipModel.StrategicGoals ?? HttpUtility.HtmlEncode(createWipModel.StrategicGoals),
                    IsPtFamilyInvovlmentOpportunity = createWipModel.IsPtFamilyInvovlmentOpportunity,
                    PickChart = createWipModel.PickChart,
                    DateAssigned = createWipModel.DateAssigned,
                    StaffWorkingOnOpportunity = createWipModel.StaffWorkingOnOpportunity ?? HttpUtility.HtmlEncode(createWipModel.StaffWorkingOnOpportunity),
                    Why = createWipModel.Why ?? HttpUtility.HtmlEncode(createWipModel.Why),
                    JustDoIt = createWipModel.JustDoIt ?? HttpUtility.HtmlEncode(createWipModel.JustDoIt),
                    Updates = createWipModel.Updates ?? HttpUtility.HtmlEncode(createWipModel.Updates)
                });
            }
            else
            {
                dynamic json = new JObject();
                json.id = Guid.NewGuid().ToString();
                json.saftey = createWipModel.Saftey ?? HttpUtility.HtmlEncode(createWipModel.Saftey);
                json.name = createWipModel.Name ?? HttpUtility.HtmlEncode(createWipModel.Name);
                json.date = createWipModel.Date.Value;
                json.problem = createWipModel.Problem ?? HttpUtility.HtmlEncode(createWipModel.Problem);
                json.eightWs = createWipModel.EightWs ?? HttpUtility.HtmlEncode(createWipModel.EightWs);
                json.strategicGoals = createWipModel.StrategicGoals ?? HttpUtility.HtmlEncode(createWipModel.StrategicGoals);
                json.isPtFamilyInvovlmentOpportunity = createWipModel.IsPtFamilyInvovlmentOpportunity;
                json.pickChart = createWipModel.PickChart;
                json.dateAssigned = createWipModel.DateAssigned;
                json.staffWorkingOnOpportunity = createWipModel.StaffWorkingOnOpportunity ?? HttpUtility.HtmlEncode(createWipModel.StaffWorkingOnOpportunity);
                json.why = createWipModel.Why ?? HttpUtility.HtmlEncode(createWipModel.Why);
                json.justDoIt = createWipModel.JustDoIt ?? HttpUtility.HtmlEncode(createWipModel.JustDoIt);
                json.updates = createWipModel.Updates ?? HttpUtility.HtmlEncode(createWipModel.Updates);

                _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                {
                    Username = user.UserName,
                    ChangeRequestType = ChangeRequestType.CREATE,
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
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var isAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));
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

            if (isAdmin)
            {
                _applicationDbContext.NewImpOps.Add(new NewImpOp
                {
                    BoardID = createNewImpOp.BoardID,
                    Legend = createNewImpOp.Legend ?? HttpUtility.HtmlEncode(createNewImpOp.Legend),
                    PersonIdentifyingOpportunity = createNewImpOp.PersonIdentifyingOpportunity ?? HttpUtility.HtmlEncode(createNewImpOp.PersonIdentifyingOpportunity),
                    DateIdentified = createNewImpOp.DateIdentified.Value,
                    Problem = createNewImpOp.Problem ?? HttpUtility.HtmlEncode(createNewImpOp.Problem),
                    StaffWorkingOnOpportunity = createNewImpOp.StaffWorkingOnOpportunity ?? HttpUtility.HtmlEncode(createNewImpOp.StaffWorkingOnOpportunity),
                    StrategicGoals = createNewImpOp.StrategicGoals ?? HttpUtility.HtmlEncode(createNewImpOp.StrategicGoals),
                    IsPtFamilyInvovlmentOpportunity = createNewImpOp.IsPtFamilyInvovlmentOpportunity,
                    EightWs = createNewImpOp.EightWs ?? HttpUtility.HtmlEncode(createNewImpOp.EightWs),
                    PickChart = createNewImpOp.PickChart,
                    JustDoIt = createNewImpOp.JustDoIt ?? HttpUtility.HtmlEncode(createNewImpOp.JustDoIt)
                });
            }
            else
            {
                dynamic json = new JObject();
                json.id = Guid.NewGuid().ToString();
                json.legend = createNewImpOp.Legend ?? HttpUtility.HtmlEncode(createNewImpOp.Legend);
                json.personIdentifyingOpportunity = createNewImpOp.PersonIdentifyingOpportunity ?? HttpUtility.HtmlEncode(createNewImpOp.PersonIdentifyingOpportunity);
                json.dateIdentified = createNewImpOp.DateIdentified.Value;
                json.problem = createNewImpOp.Problem ?? HttpUtility.HtmlEncode(createNewImpOp.Problem);
                json.staffWorkingOnOpportunity = createNewImpOp.StaffWorkingOnOpportunity ?? HttpUtility.HtmlEncode(createNewImpOp.StaffWorkingOnOpportunity);
                json.strategicGoals = createNewImpOp.StrategicGoals ?? HttpUtility.HtmlEncode(createNewImpOp.StrategicGoals);
                json.isPtFamilyInvovlmentOpportunity = createNewImpOp.IsPtFamilyInvovlmentOpportunity;
                json.eightWs = createNewImpOp.EightWs ?? HttpUtility.HtmlEncode(createNewImpOp.EightWs);
                json.pickChart = createNewImpOp.PickChart;
                json.justDoIt = createNewImpOp.JustDoIt ?? HttpUtility.HtmlEncode(createNewImpOp.JustDoIt);

                _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                {
                    Username = user.UserName,
                    ChangeRequestType = ChangeRequestType.CREATE,
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
            var user = await _userManager.GetUserAsync(User);
            var adminRoleID = (await _applicationDbContext.Roles.SingleOrDefaultAsync(r => r.Name.Equals("Admin"))).Id;
            var isAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));
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

            if (isAdmin)
            {
                _applicationDbContext.ImpIdeasImplemented.Add(new ImpIdeasImplemented
                {
                    BoardID = createImpIdeasImplemented.BoardID,
                    Name = createImpIdeasImplemented.Name ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Name),
                    Date = createImpIdeasImplemented.Date.Value,
                    Problem = createImpIdeasImplemented.Problem ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Problem),
                    Owner = createImpIdeasImplemented.Owner ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Owner),
                    Pillar = createImpIdeasImplemented.Pillar ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Pillar),
                    IsPtFamilyInvovlmentOpportunity = createImpIdeasImplemented.IsPtFamilyInvovlmentOpportunity,
                    EightWs = createImpIdeasImplemented.EightWs ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.EightWs),
                    PickChart = createImpIdeasImplemented.PickChart,
                    JustDoIt = createImpIdeasImplemented.JustDoIt ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.JustDoIt),
                    Solution = createImpIdeasImplemented.Solution ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Solution),
                    DateComplete = createImpIdeasImplemented.DateComplete.Value,
                    WorkCreated = createImpIdeasImplemented.WorkCreated,
                    ProcessObservationCreated = createImpIdeasImplemented.ProcessObservationCreated,
                    DateEnterIntoDatabase = createImpIdeasImplemented.DateEnterIntoDatabase ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.DateEnterIntoDatabase),
                });
            }
            else
            {
                dynamic json = new JObject();
                json.id = Guid.NewGuid().ToString();
                json.name = createImpIdeasImplemented.Name ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Name);
                json.date = createImpIdeasImplemented.Date.Value;
                json.problem = createImpIdeasImplemented.Problem ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Problem);
                json.owner = createImpIdeasImplemented.Owner ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Owner);
                json.pillar = createImpIdeasImplemented.Pillar ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Pillar);
                json.isPtFamilyInvovlmentOpportunity = createImpIdeasImplemented.IsPtFamilyInvovlmentOpportunity;
                json.eightWs = createImpIdeasImplemented.EightWs ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.EightWs);
                json.pickChart = createImpIdeasImplemented.PickChart;
                json.justDoIt = createImpIdeasImplemented.JustDoIt ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.JustDoIt);
                json.solution = createImpIdeasImplemented.Solution ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.Solution);
                json.dateComplete = createImpIdeasImplemented.DateComplete.Value;
                json.workCreated = createImpIdeasImplemented.WorkCreated;
                json.processObservationCreated = createImpIdeasImplemented.ProcessObservationCreated;
                json.dateEnterIntoDatabase = createImpIdeasImplemented.DateEnterIntoDatabase ?? HttpUtility.HtmlEncode(createImpIdeasImplemented.DateEnterIntoDatabase);

                _applicationDbContext.ChangeRequests.Add(new ChangeRequest
                {
                    Username = user.UserName,
                    ChangeRequestType = ChangeRequestType.CREATE,
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
            var isAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));

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

            if (isAdmin)
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
                        BoardID =fieldDeleteModel.BoardID,
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
            var isAdmin = await _applicationDbContext.UserRoles.AnyAsync(r => r.UserId.Equals(user.Id) && r.RoleId.Equals(adminRoleID));

            if (isAdmin)
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

                var changeRequestType = ChangeRequestType.UPDATE;
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
                var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    data = data.Where(c => c.Who.ToUpper().Contains(search)
                    || c.What.ToUpper().Contains(search)
                    || c.Why.ToUpper().Contains(search));
                }
                
                return new CelebrationViewModel
                {
                    Total = total,
                    Celebrations = await data.ToListAsync()
                };
            }

            if (boardTableViewModel.TableType == TableType.WIP)
            {
                var table = _applicationDbContext.WIPs.Where(c => c.BoardID == boardTableViewModel.BoardID);
                var total = await table.CountAsync();
                var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

                if (!string.IsNullOrWhiteSpace(search?.ToUpper().Trim()))
                {
                    data = data.Where(w => w.Why.ToUpper().Contains(search)
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
                    WIPs = await data.ToListAsync()
                };
            }

            if (boardTableViewModel.TableType == TableType.NEWIMPOP)
            {
                var table = _applicationDbContext.NewImpOps.Where(c => c.BoardID == boardTableViewModel.BoardID);
                var total = await table.CountAsync();
                var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

                if (!string.IsNullOrWhiteSpace(search?.ToUpper().Trim()))
                {
                    data = data.Where(w => w.Legend.ToUpper().Contains(search)
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
                    NewImpOps = await data.ToListAsync()
                };
            }

            if (boardTableViewModel.TableType == TableType.IMPIDEAS)
            {
                var table = _applicationDbContext.ImpIdeasImplemented.Where(c => c.BoardID == boardTableViewModel.BoardID);
                var total = await table.CountAsync();
                var data = table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit);

                if (!string.IsNullOrWhiteSpace(search?.ToUpper().Trim()))
                {
                    data = data.Where(w => w.Name.ToUpper().Contains(search)
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
                    ImpIdeasImplementeds = await data.ToListAsync()
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
    }
}