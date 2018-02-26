using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using March_Madness.Models;
using March_Madness.Models.ViewModels;
using March_Madness.Helpers;

namespace March_Madness.Controllers
{
    public class BracketController : Controller
    {
		private ApplicationDbContext _context;

		public BracketController()
		{
			_context = new ApplicationDbContext();
		}
		// GET: Bracket
		public ActionResult Index()
		{
			var utl = new Utility();
			var bracket = utl.GetBracket();
			var userID = User.Identity.GetUserId();

			var round1Pos = Utility.Round1PairingOrder;

			var jQueryBracket = new List<List<string>>();

			foreach (var region in bracket)
			{
				for (int i = 0; i < round1Pos.Count; i++)
				{
					Teams teamName = region.Value.First(t => t.Seed == round1Pos[i]).Team;
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

			var userBracket = _context.TournamentEntry.Where(t => t.UserId == userID);

			UserBracketViewModel userBracketView = new UserBracketViewModel()
			{
				UserBrackets = userBracket.ToList(),
				TournamentTeams = jQueryBracket

			};
            return View("BracketForm", userBracketView);
        }
    }
}