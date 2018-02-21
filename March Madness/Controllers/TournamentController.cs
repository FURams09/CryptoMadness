using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using March_Madness.Models;
using March_Madness.Models.ViewModels;
using March_Madness.Helpers;

namespace March_Madness.Controllers
{
    public class TournamentController : Controller
    {
        private ApplicationDbContext _context;

        public TournamentController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Tournament
        public ActionResult Index()
        {
            var teamList = Utility.GetAllTeams();
            var regions = Utility.GetRegionNames();


            var seedOrder = _context.TournamentTeam
                .OrderBy(t => t.Seed);
            var east = seedOrder
            .Where(t => t.Region == Regions.East)
            .Select(t => t.TeamId );

            var midwest = seedOrder
            .Where(t => t.Region == Regions.Midwest)
            .Select(t =>t.TeamId );

            var west = seedOrder
            .Where(t => t.Region == Regions.West)
            .Select(t => t.TeamId );

            var south = seedOrder
            .Where(t => t.Region == Regions.South)
            .Select(t =>  t.TeamId );

            TournamentRegionViewModel tournamentRegionViewModel = new TournamentRegionViewModel()
            {
                East = east.ToList(),
                Midwest = midwest.ToList(),
                West = west.ToList(),
                South = south.ToList(),
                Teams = teamList.ToList()
            };

            return View("TournamentForm", tournamentRegionViewModel);
        }
    }
}