using HHSBoard.Data;
using HHSBoard.Helpers;
using HHSBoard.Models;
using HHSBoard.Models.CelebrationViewModels;
using HHSBoard.Models.PurposeViewModels;
using HHSBoard.Models.WipViewModels;
using HHSBoard.Models.WIPViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public IActionResult Index(int boardID, TableType tableType)
        {
            ViewBag.BoardID = boardID;
            ViewBag.TableType = tableType;

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
            }

            return Json("No table found.");
        }

        [HttpPost]
        public async Task<IActionResult> AddCeleration(CreateCelebrationModel createCelebrationModel)
        {
            var board = _applicationDbContext.Boards.Where(b => b.ID == createCelebrationModel.BoardID);
            if (!board.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json($"No board found.");
            }

            if (!createCelebrationModel.Date.HasValue)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Date is required.");
            }

            _applicationDbContext.Celebrations.Add(new Celebration
            {
                Who = createCelebrationModel.Who ?? HttpUtility.HtmlEncode(createCelebrationModel.Who),
                What = createCelebrationModel.What ?? HttpUtility.HtmlEncode(createCelebrationModel.What),
                Why = createCelebrationModel.Why ?? HttpUtility.HtmlEncode(createCelebrationModel.Why),
                Date = createCelebrationModel.Date.Value,
                BoardID = createCelebrationModel.BoardID
            });

            await _applicationDbContext.SaveChangesAsync();
            return Json("Created");
        }

        [HttpPost]
        public async Task<IActionResult> AddWIP(CreateWipModel createWipModel)
        {
            var board = _applicationDbContext.Boards.Where(b => b.ID == createWipModel.BoardID);
            if (!board.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json($"No board found.");
            }

            if (!createWipModel.Date.HasValue)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Date is required.");
            }

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

            await _applicationDbContext.SaveChangesAsync();
            return Json("Created");
        }

        public async Task<IActionResult> DeleteFields(FieldDeleteModel fieldDeleteModel)
        {
            if (fieldDeleteModel.Delete == null || !fieldDeleteModel.Delete.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("No fields sent to be deleted.");
            }

            if (fieldDeleteModel.TableType == TableType.CELEBRATION)
            {
                var celebrations = _applicationDbContext.Celebrations.Where(c => fieldDeleteModel.Delete.Contains(c.ID));
                _applicationDbContext.Celebrations.RemoveRange(celebrations);

                await _applicationDbContext.SaveChangesAsync();
            }
            else if (fieldDeleteModel.TableType == TableType.WIP)
            {
                var wip = _applicationDbContext.WIPs.Where(c => fieldDeleteModel.Delete.Contains(c.ID));
                _applicationDbContext.WIPs.RemoveRange(wip);

                await _applicationDbContext.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("No table found.");
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Deleted.");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateField(FieldUpdateModel fieldUpdateModel)
        {
            if (fieldUpdateModel.TableType == TableType.CELEBRATION)
            {
                var celebration = await _applicationDbContext.Celebrations.Where(c => c.ID == fieldUpdateModel.Pk).FirstOrDefaultAsync();
                var proptery = celebration.GetType().GetProperty(fieldUpdateModel.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var memberType = proptery.PropertyType;
                var nonNullType = Nullable.GetUnderlyingType(memberType);
                if (nonNullType != null)
                    memberType = nonNullType;
                var converted = ConvertType(memberType, fieldUpdateModel.Value);

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
                var converted = ConvertType(memberType, fieldUpdateModel.Value);

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
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("No table found.");
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

            return null;
        }

        public async Task<T> GetViewModel<T>(BoardTableModel boardTableViewModel)
        {
            return (T)await GetViewModel(boardTableViewModel);
        }

        public object ConvertType(Type type, string value)
        {
            if (type == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out DateTime date))
                {
                    return date;
                }

                return null;
            }
            else if (type == typeof(PickChart))
            {
                return (PickChart)int.Parse(value);
            }
            else if (type == typeof(bool))
            {
                return int.Parse(value);
            }
            else
            {
                return HttpUtility.HtmlEncode(value);
            }
        }
    }
}