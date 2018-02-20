using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using March_Madness.Models;
using March_Madness.Models.ViewModels;

namespace March_Madness.Controllers
{
    public class TournamentController : Controller
    {
		private ApplicationDbContext _context;

		public TournamentController ()
		{
			_context = new ApplicationDbContext();
		}
        // GET: Tournament
        public ActionResult Index()
        {
			var teamList = _context.Teams.OrderBy(t => t.Name);

			var regions = Enum.GetNames(typeof(Regions));

			TournamentRegionViewModel tournamentRegionViewModel = new TournamentRegionViewModel()
			{
				Regions = regions.ToList(),
				Teams = teamList.ToList()
			};

            return View("TournamentForm", tournamentRegionViewModel);
        }
    }
}