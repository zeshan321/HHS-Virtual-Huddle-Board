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
                    if (changeRequest.TableName == TableType.WIP)
                    {
                        var toRemove = _applicationDbContext.WIPs.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        _applicationDbContext.WIPs.Remove(toRemove);
                    }
                    if (changeRequest.TableName == TableType.NEWIMPOP)
                    {
                        var toRemove = _applicationDbContext.NewImpOps.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        _applicationDbContext.NewImpOps.Remove(toRemove);
                    }
                    if (changeRequest.TableName == TableType.IMPIDEAS)
                    {
                        var toRemove = _applicationDbContext.NewImpOps.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        _applicationDbContext.NewImpOps.Remove(toRemove);
                    }
                    break;

                case ChangeRequestType.MODIFY:
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
                    if (changeRequest.TableName == TableType.WIP)
                    {
                        var wip = await _applicationDbContext.WIPs.Where(c => c.ID == changeRequest.AssociatedID).FirstOrDefaultAsync();
                        var proptery = wip.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var memberType = proptery.PropertyType;
                        var nonNullType = Nullable.GetUnderlyingType(memberType);
                        if (nonNullType != null)
                            memberType = nonNullType;
                        var converted = ConvertHelper.ConvertType(memberType, JObject.Parse(changeRequest.Values).GetValue("Value").ToString());

                        if (converted != null)
                        {
                            proptery.SetValue(wip, Convert.ChangeType(converted, memberType), null);
                        }
                    }
                    if (changeRequest.TableName == TableType.NEWIMPOP)
                    {
                        var newImp = await _applicationDbContext.NewImpOps.Where(c => c.ID == changeRequest.AssociatedID).FirstOrDefaultAsync();
                        var proptery = newImp.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var memberType = proptery.PropertyType;
                        var nonNullType = Nullable.GetUnderlyingType(memberType);
                        if (nonNullType != null)
                            memberType = nonNullType;
                        var converted = ConvertHelper.ConvertType(memberType, JObject.Parse(changeRequest.Values).GetValue("Value").ToString());

                        if (converted != null)
                        {
                            proptery.SetValue(newImp, Convert.ChangeType(converted, memberType), null);
                        }
                    }
                    if (changeRequest.TableName == TableType.IMPIDEAS)
                    {
                        var impIdeasImplemented = await _applicationDbContext.ImpIdeasImplemented.Where(c => c.ID == changeRequest.AssociatedID).FirstOrDefaultAsync();
                        var proptery = impIdeasImplemented.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        var memberType = proptery.PropertyType;
                        var nonNullType = Nullable.GetUnderlyingType(memberType);
                        if (nonNullType != null)
                            memberType = nonNullType;
                        var converted = ConvertHelper.ConvertType(memberType, JObject.Parse(changeRequest.Values).GetValue("Value").ToString());

                        if (converted != null)
                        {
                            proptery.SetValue(impIdeasImplemented, Convert.ChangeType(converted, memberType), null);
                        }
                    }
                    break;

                case ChangeRequestType.ADD:
                    var json = JObject.Parse(changeRequest.Values);

                    if (changeRequest.TableName == TableType.CELEBRATION)
                    {
                        _applicationDbContext.Celebrations.Add(new Celebration
                        {
                            Who = json.GetValue("who").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("who").ToString()),
                            What = json.GetValue("what").ToString() ??  HttpUtility.HtmlEncode(json.GetValue("what").ToString()),
                            Why = json.GetValue("why").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("why").ToString()),
                            Date = DateTime.Parse(json.GetValue("date").ToString()),
                            BoardID = changeRequest.BoardID
                        });
                    }
                    if (changeRequest.TableName == TableType.WIP)
                    {
                        var wip = new WIP
                        {
                            BoardID = changeRequest.BoardID,
                            Saftey = json.GetValue("saftey").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("saftey").ToString()),
                            Name = json.GetValue("name").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("name").ToString()),
                            Date = DateTime.Parse(json.GetValue("date").ToString()),
                            Problem = json.GetValue("problem").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("problem").ToString()),
                            EightWs = json.GetValue("eightWs").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("eightWs").ToString()),
                            StrategicGoals = json.GetValue("strategicGoals").ToString(),
                            StaffWorkingOnOpportunity = json.GetValue("staffWorkingOnOpportunity").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("staffWorkingOnOpportunity").ToString()),
                            Why = json.GetValue("why").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("why").ToString()),
                            JustDoIt = json.GetValue("justDoIt").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("justDoIt").ToString()),
                            Updates = json.GetValue("updates").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("updates").ToString())
                        };

                        if (json.GetValue("isPtFamilyInvovlmentOpportunity").HasValues)
                            wip.IsPtFamilyInvovlmentOpportunity = bool.Parse(json.GetValue("isPtFamilyInvovlmentOpportunity").ToString());
                        if (json.GetValue("pickChart").HasValues)
                            wip.PickChart = (PickChart)int.Parse(json.GetValue("pickChart").ToString());
                        if (json.GetValue("dateAssigned").HasValues)
                            wip.DateAssigned = DateTime.Parse(json.GetValue("dateAssigned").ToString());

                        _applicationDbContext.WIPs.Add(wip);
                    }
                    if (changeRequest.TableName == TableType.NEWIMPOP)
                    {
                        var newImp = new NewImpOp
                        {
                            BoardID = changeRequest.BoardID,
                            Legend = json.GetValue("legend").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("legend").ToString()),
                            PersonIdentifyingOpportunity = json.GetValue("personIdentifyingOpportunity").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("personIdentifyingOpportunity").ToString()),
                            DateIdentified = DateTime.Parse(json.GetValue("dateIdentified").ToString()),
                            Problem = json.GetValue("problem").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("problem").ToString()),
                            StaffWorkingOnOpportunity = json.GetValue("staffWorkingOnOpportunity").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("staffWorkingOnOpportunity").ToString()),
                            StrategicGoals = json.GetValue("strategicGoals").ToString(),
                            EightWs = json.GetValue("eightWs").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("eightWs").ToString()),
                            JustDoIt = json.GetValue("justDoIt").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("justDoIt").ToString()),

                        };

                        if (json.GetValue("isPtFamilyInvovlmentOpportunity").HasValues)
                            newImp.IsPtFamilyInvovlmentOpportunity = bool.Parse(json.GetValue("isPtFamilyInvovlmentOpportunity").ToString());
                        if (json.GetValue("pickChart").HasValues)
                            newImp.PickChart = (PickChart)int.Parse(json.GetValue("pickChart").ToString());

                        _applicationDbContext.NewImpOps.Add(newImp);
                    }

                    if (changeRequest.TableName == TableType.IMPIDEAS)
                    {
                        var impIdeas = new ImpIdeasImplemented
                        {
                            BoardID = changeRequest.BoardID,
                            Name = json.GetValue("name").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("name").ToString()),
                            Date = DateTime.Parse(json.GetValue("date").ToString()),
                            Owner = json.GetValue("owner").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("owner").ToString()),
                            Pillar = json.GetValue("pillar").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("pillar").ToString()),
                            EightWs = json.GetValue("eightWs").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("eightWs").ToString()),
                            JustDoIt = json.GetValue("justDoIt").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("justDoIt").ToString()),
                            Solution = json.GetValue("solution").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("solution").ToString()),
                            DateEnterIntoDatabase = json.GetValue("dateEnterIntoDatabase").ToString() ?? HttpUtility.HtmlEncode(json.GetValue("dateEnterIntoDatabase").ToString()),
                        };

                        if (json.GetValue("isPtFamilyInvovlmentOpportunity").HasValues)
                            impIdeas.IsPtFamilyInvovlmentOpportunity = bool.Parse(json.GetValue("isPtFamilyInvovlmentOpportunity").ToString());
                        if (json.GetValue("pickChart").HasValues)
                            impIdeas.PickChart = (PickChart)int.Parse(json.GetValue("pickChart").ToString());
                        if (json.GetValue("dateComplete").HasValues)
                            impIdeas.DateComplete = DateTime.Parse(json.GetValue("dateComplete").ToString());
                        if (json.GetValue("workCreated").HasValues)
                            impIdeas.WorkCreated = bool.Parse(json.GetValue("workCreated").ToString());
                        if (json.GetValue("processObservationCreated").HasValues)
                            impIdeas.ProcessObservationCreated = bool.Parse(json.GetValue("processObservationCreated").ToString());

                        _applicationDbContext.ImpIdeasImplemented.Add(impIdeas);
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

            if (!string.IsNullOrWhiteSpace(search?.ToUpper().Trim()))
            {
                data = data.Where(d => d.TableName.ToString().ToUpper().Contains(search)
                || d.Username.ToUpper().Contains(search)
                || d.Values.ToUpper().Contains(search)
                || d.ChangeRequestType.ToString().ToUpper().Contains(search)
                || d.AssociatedName.ToUpper().Contains(search)
                || d.Board.Name.ToUpper().Contains(search)
                || d.Board.Unit.Name.ToUpper().Contains(search));
            }

            data = data.Include(d => d.Board).Include(d => d.Board.Unit);

            List<ApproveViewModel> approveViewModels = new List<ApproveViewModel>();
            foreach (var changeRequest in data)
            {
                var previousValues = "";
                if (changeRequest.ChangeRequestType == ChangeRequestType.MODIFY)
                {
                    dynamic json = new JObject();
                    json.Name = changeRequest.AssociatedName;

                    if (changeRequest.TableName == TableType.CELEBRATION)
                    {
                        var celebration = _applicationDbContext.Celebrations.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        var property = celebration.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        
                        json.Value = property.GetValue(celebration, null) ?? "";
                    }

                    if (changeRequest.TableName == TableType.WIP)
                    {
                        var wip = _applicationDbContext.WIPs.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        var property = wip.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        json.Value = property.GetValue(wip, null) ?? "";
                    }

                    if (changeRequest.TableName == TableType.NEWIMPOP)
                    {
                        var newImpOp = _applicationDbContext.NewImpOps.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        var property = newImpOp.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        json.Value = property.GetValue(newImpOp, null) ?? "";
                    }

                    if (changeRequest.TableName == TableType.IMPIDEAS)
                    {
                        var impIdeasImplemented = _applicationDbContext.ImpIdeasImplemented.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        var property = impIdeasImplemented.GetType().GetProperty(changeRequest.AssociatedName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        json.Value = property.GetValue(impIdeasImplemented, null) ?? "";
                    }

                    previousValues = json.ToString();
                }
                if (changeRequest.ChangeRequestType == ChangeRequestType.DELETE)
                {
                    dynamic json = new JObject();

                    if (changeRequest.TableName == TableType.CELEBRATION)
                    {
                        var celebration = _applicationDbContext.Celebrations.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        json.id = celebration.ID;
                        json.who = celebration.Who ?? HttpUtility.HtmlEncode(celebration.Who);
                        json.what = celebration.What ?? HttpUtility.HtmlEncode(celebration.What);
                        json.why = celebration.What ?? HttpUtility.HtmlEncode(celebration.What);
                        json.date = celebration.Date;
                        json.BoardID = celebration.BoardID;
                    }

                    if (changeRequest.TableName == TableType.WIP)
                    {
                        var wip = _applicationDbContext.WIPs.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        json.id = wip.ID;
                        json.saftey = wip.Saftey ?? HttpUtility.HtmlEncode(wip.Saftey);
                        json.name = wip.Name ?? HttpUtility.HtmlEncode(wip.Name);
                        json.date = wip.Date;
                        json.problem = wip.Problem ?? HttpUtility.HtmlEncode(wip.Problem);
                        json.eightWs = wip.EightWs ?? HttpUtility.HtmlEncode(wip.EightWs);
                        json.strategicGoals = wip.StrategicGoals ?? HttpUtility.HtmlEncode(wip.StrategicGoals);
                        json.isPtFamilyInvovlmentOpportunity = wip.IsPtFamilyInvovlmentOpportunity;
                        json.pickChart = wip.PickChart;
                        json.dateAssigned = wip.DateAssigned;
                        json.staffWorkingOnOpportunity = wip.StaffWorkingOnOpportunity ?? HttpUtility.HtmlEncode(wip.StaffWorkingOnOpportunity);
                        json.why = wip.Why ?? HttpUtility.HtmlEncode(wip.Why);
                        json.justDoIt = wip.JustDoIt ?? HttpUtility.HtmlEncode(wip.JustDoIt);
                        json.updates = wip.Updates ?? HttpUtility.HtmlEncode(wip.Updates);
                        json.BoardID = wip.BoardID;
                    }

                    if (changeRequest.TableName == TableType.NEWIMPOP)
                    {
                        var newImpOp = _applicationDbContext.NewImpOps.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        json.id = newImpOp.ID;
                        json.legend = newImpOp.Legend ?? HttpUtility.HtmlEncode(newImpOp.Legend);
                        json.personIdentifyingOpportunity = newImpOp.PersonIdentifyingOpportunity ?? HttpUtility.HtmlEncode(newImpOp.PersonIdentifyingOpportunity);
                        json.dateIdentified = newImpOp.DateIdentified;
                        json.problem = newImpOp.Problem ?? HttpUtility.HtmlEncode(newImpOp.Problem);
                        json.staffWorkingOnOpportunity = newImpOp.StaffWorkingOnOpportunity ?? HttpUtility.HtmlEncode(newImpOp.StaffWorkingOnOpportunity);
                        json.strategicGoals = newImpOp.StrategicGoals ?? HttpUtility.HtmlEncode(newImpOp.StrategicGoals);
                        json.isPtFamilyInvovlmentOpportunity = newImpOp.IsPtFamilyInvovlmentOpportunity;
                        json.eightWs = newImpOp.EightWs ?? HttpUtility.HtmlEncode(newImpOp.EightWs);
                        json.pickChart = newImpOp.PickChart;
                        json.justDoIt = newImpOp.JustDoIt ?? HttpUtility.HtmlEncode(newImpOp.JustDoIt);
                        json.BoardID = newImpOp.BoardID;
                    }

                    if (changeRequest.TableName == TableType.IMPIDEAS)
                    {
                        var impIdeasImplemented = _applicationDbContext.ImpIdeasImplemented.SingleOrDefault(c => c.ID == changeRequest.AssociatedID);
                        json.id = impIdeasImplemented.ID;
                        json.name = impIdeasImplemented.Name ?? HttpUtility.HtmlEncode(impIdeasImplemented.Name);
                        json.date = impIdeasImplemented.Date;
                        json.problem = impIdeasImplemented.Problem ?? HttpUtility.HtmlEncode(impIdeasImplemented.Problem);
                        json.owner = impIdeasImplemented.Owner ?? HttpUtility.HtmlEncode(impIdeasImplemented.Owner);
                        json.pillar = impIdeasImplemented.Pillar ?? HttpUtility.HtmlEncode(impIdeasImplemented.Pillar);
                        json.isPtFamilyInvovlmentOpportunity = impIdeasImplemented.IsPtFamilyInvovlmentOpportunity;
                        json.eightWs = impIdeasImplemented.EightWs ?? HttpUtility.HtmlEncode(impIdeasImplemented.EightWs);
                        json.pickChart = impIdeasImplemented.PickChart;
                        json.justDoIt = impIdeasImplemented.JustDoIt ?? HttpUtility.HtmlEncode(impIdeasImplemented.JustDoIt);
                        json.solution = impIdeasImplemented.Solution ?? HttpUtility.HtmlEncode(impIdeasImplemented.Solution);
                        json.dateComplete = impIdeasImplemented.DateComplete;
                        json.workCreated = impIdeasImplemented.WorkCreated;
                        json.processObservationCreated = impIdeasImplemented.ProcessObservationCreated;
                        json.dateEnterIntoDatabase = impIdeasImplemented.DateEnterIntoDatabase ?? HttpUtility.HtmlEncode(impIdeasImplemented.DateEnterIntoDatabase);
                        json.BoardID = impIdeasImplemented.BoardID;
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
                    BoardName = changeRequest.Board.Name,
                    UnitName = changeRequest.Board.Unit.Name,
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