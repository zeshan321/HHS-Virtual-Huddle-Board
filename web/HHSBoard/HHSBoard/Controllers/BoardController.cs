using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using HHSBoard.Data;
using HHSBoard.Models;
using HHSBoard.Models.BoardViewModels;
using HHSBoard.Models.CelebrationViewModels;
using HHSBoard.Models.PurposeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            switch(boardTableViewModel.TableType)
            {
                case TableType.CELEBRATION:
                    var data = await GetViewModel<CelebrationViewModel>(boardTableViewModel);

                    return Json(data);
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
                Who = createCelebrationModel.Who??HttpUtility.HtmlEncode(createCelebrationModel.Who),
                What = createCelebrationModel.What??HttpUtility.HtmlEncode(createCelebrationModel.What),
                Why = createCelebrationModel.Why??HttpUtility.HtmlEncode(createCelebrationModel.Why),
                Date = createCelebrationModel.Date.Value,
                BoardID = createCelebrationModel.BoardID
            });

            await _applicationDbContext.SaveChangesAsync();
            return Json("Created");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateField(FieldUpdateModel fieldUpdateModel)
        {
            switch (fieldUpdateModel.TableType)
            {
                case TableType.CELEBRATION:
                    var celebration = await _applicationDbContext.Celebrations.Where(c => c.ID == fieldUpdateModel.Pk).FirstOrDefaultAsync();
                    var proptery = celebration.GetType().GetProperty(fieldUpdateModel.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    var converted = ConvertType(proptery, fieldUpdateModel.Value);

                    if (converted != null)
                    {
                        proptery.SetValue(celebration, Convert.ChangeType(converted, proptery.PropertyType), null);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json($"Invalid value format for {TableType.CELEBRATION.ToString()}");
                    }
                    
                    break;
                default:
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

        public enum TableType { PURPOSE = 0, CELEBRATION = 1, WIP = 2 }

        public async Task<object> GetViewModel(BoardTableModel boardTableViewModel)
        {
            if (boardTableViewModel.TableType == TableType.CELEBRATION)
            {
                var table = _applicationDbContext.Celebrations.Where(c => c.BoardID == boardTableViewModel.BoardID);
                var total = await table.CountAsync();
                var data = await table.Skip(boardTableViewModel.Offset).Take(boardTableViewModel.Limit).ToListAsync();

                return new CelebrationViewModel
                {
                    Total = total,
                    Celebrations = data
                };
            }

            return null;
        }

        public async Task<T> GetViewModel<T>(BoardTableModel boardTableViewModel)
        {
            return (T)await GetViewModel(boardTableViewModel);
        }

        public object ConvertType(PropertyInfo propertyInfo, string value)
        {
            if (propertyInfo.PropertyType == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out DateTime date))
                {
                    return date;
                }

                return null;
            }
            else
            {
                return HttpUtility.HtmlEncode(value);
            }
        }
    }
}