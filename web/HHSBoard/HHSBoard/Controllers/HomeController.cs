using HHSBoard.Data;
using HHSBoard.Models;
using HHSBoard.Models.CelebrationViewModels;
using HHSBoard.Models.HomeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HHSBoard.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        // Incorrect way. Should just include units in board.
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.name = user.Email;

            // TODO: Only show boards user has access to
            return View(new HomeIndexViewModel
            {
                Units = await _applicationDbContext.Units.ToListAsync(),
                Boards = await _applicationDbContext.Boards.ToListAsync()
            });
        }

        public async Task<IActionResult> SearchBoards(BoardTableModel boardTableModel)
        {
            var search = boardTableModel.Search?.ToUpper().Trim();
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                var units = _applicationDbContext.Units.Where(u => u.Name.ToUpper().Contains(search));
                var boards = _applicationDbContext.Boards.Where(b => b.Name.ToUpper().Contains(search)).Include(b => b.Unit);

                return Json(new HomeIndexViewModel
                {
                    Units = await units.ToListAsync(),
                    Boards = await boards.ToListAsync()
                });
            }

            return Json(new HomeIndexViewModel
            {
                Units = await _applicationDbContext.Units.ToListAsync(),
                Boards = await _applicationDbContext.Boards.ToListAsync()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard(CreateBoardModel createBoardModel)
        {
            var user = await _userManager.GetUserAsync(User);

            if (string.IsNullOrWhiteSpace(createBoardModel.Name))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Invalid board name.");
            }

            var unit = _applicationDbContext.Units.Where(u => u.ID == createBoardModel.UnitID).SingleOrDefault();
            if (unit == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Invalid unit selected.");
            }

            var board = _applicationDbContext.Boards.Where(b => b.UnitID == createBoardModel.UnitID && b.Name.ToUpperInvariant() == createBoardModel.Name.ToUpperInvariant());
            if (board.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json($"A board with that name exists in {unit.Name}");
            }

            var newBoard = (await _applicationDbContext.Boards.AddAsync(new Board
            {
                Name = createBoardModel.Name,
                UnitID = createBoardModel.UnitID
            })).Entity;

            // Create defaults for board
            var defaultPurpose = _applicationDbContext.Defaults.Where(d => d.Field == "BussinessRules").SingleOrDefault().Value;
            defaultPurpose = defaultPurpose.Replace("{{Name}}", createBoardModel.Name);

            _applicationDbContext.Purpose.Add(new Purpose
            {
                BoardID = newBoard.ID,
                Text = defaultPurpose
            });

            await _applicationDbContext.SaveChangesAsync();
            return Json(newBoard);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBoard(DeleteBoardModel deleteBoardModel)
        {
            var board = _applicationDbContext.Boards.Where(b => b.ID == deleteBoardModel.ID).SingleOrDefault();

            if(board == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Invalid board id.");
            }
            else
            {
                _applicationDbContext.Boards.Remove(board);
            }

            await _applicationDbContext.SaveChangesAsync();
            return Json("Deleted");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}