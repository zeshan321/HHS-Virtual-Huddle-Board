using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HHSBoard.Data;
using HHSBoard.Models;
using HHSBoard.Models.ScorecardsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HHSBoard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ScoreCardsController : Controller
    {
        private ApplicationDbContext _applicationDbContext;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _hostEnvironment;

        public ScoreCardsController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHostingEnvironment hostEnvironment)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ChangeRequestAmount = await _applicationDbContext.ChangeRequests.CountAsync();

            var scoreCards = new List<BoardScorecardViewModel>();
            foreach (var board in _applicationDbContext.Boards)
            {
                var path = _hostEnvironment.WebRootPath + Path.DirectorySeparatorChar + "Uploads" + Path.DirectorySeparatorChar + board.ID + Path.DirectorySeparatorChar + "scorecard";
                System.IO.Directory.CreateDirectory(path);

                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
                var fileNames = files.Select(c => c.Name).ToList();

                scoreCards.Add(new BoardScorecardViewModel
                {
                    BoardId = board.ID,
                    BoardName = board.Name,
                    FileNames = fileNames
                });
            }

            return View(new AllScorecardsViewModel
            {
                Scorecards = scoreCards
            });
        }
    }
}