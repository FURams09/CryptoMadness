using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using March_Madness.Models;
using March_Madness.Models.ViewModels;
using March_Madness.Helpers;
using Newtonsoft.Json;

namespace March_Madness.Controllers
{
	[Authorize]
    public class BracketController : Controller
    {
		private ApplicationDbContext _context;

		public BracketController()
		{
			_context = new ApplicationDbContext();
		}
		// GET: Bracket
		[Route("bracket/bracketForm/{bracketID?}")]
		public ActionResult BracketForm(int bracketID = 0)
		{
			var utl = new Utility();
			var bracket = utl.GetTournamentBracket();
			var userID = User.Identity.GetUserId();

			var round1Pos = Utility.Round1PairingOrder;

			var jQueryBracket = new List<List<string>>();

			foreach (var region in bracket)
			{
				for (int i = 0; i < round1Pos.Count; i++)
				{
					Team teamName = region.Value.First(t => t.Seed == round1Pos[i]).Team;
					if (i % 2 == 0)
					{
						jQueryBracket.Add(new List<string>() { teamName.Name });
					}
					else
					{
						jQueryBracket[jQueryBracket.Count - 1].Add(teamName.Name);
					}
				}
			}

			var userBrackets = _context.BracketEntries.Where(t => t.UserId == userID);

			BracketViewModel userBracketView = new BracketViewModel()
			{
				UserBrackets = userBrackets.ToList(),
				TournamentTeams = JsonConvert.SerializeObject( jQueryBracket),
				TournamentId = bracketID
			
			};
            return View("BracketForm", userBracketView);
        }

		public ViewResult Index()
		{
			string user = User.Identity.GetUserId();
		    List<BracketEntry> userEntries =	_context.BracketEntries.Where(t => t.UserId == user).ToList();

			return View("Index", userEntries);
		}
    }
}