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
		private Utility _utility;

        public TournamentController()
        {
            _context = new ApplicationDbContext();
			_utility = new Utility();
        }
        // GET: Tournament
        public ActionResult Index()
        {
            var teamList = _utility.GetAllTeams();
            var regions = Utility.GetRegionNames();


            var seededOrder = _context.TournamentTeams
                .OrderBy(t => t.Seed);
            var east = seededOrder
            .Where(t => t.Region == Regions.East)
            .Select(t => new { t.Seed, t.TeamId } );

            var midwest = seededOrder
            .Where(t => t.Region == Regions.Midwest)
			.Select(t => new { t.Seed, t.TeamId });

			var west = seededOrder
            .Where(t => t.Region == Regions.West)
			.Select(t => new { t.Seed, t.TeamId });

			var south = seededOrder
            .Where(t => t.Region == Regions.South)
			.Select(t => new { t.Seed, t.TeamId });

			var bracket = new Dictionary<string, Dictionary<int, int>>
			{
				{ "east", east.ToDictionary(t => t.Seed, t => t.TeamId) },
				{ "midwest", midwest.ToDictionary(t => t.Seed, t => t.TeamId) },
				{ "west", west.ToDictionary(t => t.Seed, t => t.TeamId) },
				{ "south", south.ToDictionary(t => t.Seed, t => t.TeamId) }
			};

			TournamentRegionViewModel tournamentRegionViewModel = new TournamentRegionViewModel()
            {
				Bracket = bracket,
				Round1PairingOrder = Utility.Round1PairingOrder,
                Teams = teamList.ToList()
            };

            return View("TournamentForm", tournamentRegionViewModel);
        }
    }
}