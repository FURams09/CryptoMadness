using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using March_Madness.Models;
using March_Madness.Models.ViewModels;
using March_Madness.Helpers;

namespace March_Madness.Controllers
{
    public class TeamsController : Controller
    {

		private ApplicationDbContext _context;
		private Utility _utility;

		public TeamsController()
		{
			_context = new ApplicationDbContext();
			_utility = new Utility();
		}

        // GET: Team
        public ActionResult Index()
        {
			var teams = _utility.GetAllTeams();
            return View(teams);
        }

        // GET: Team/Create
        public ActionResult Create()
        {
			ViewBag.ActionText = "Add Team";


			var teamFormViewModel = new TeamFormViewModel() {
				AllTeams = _utility.GetAllTeams().ToList()
			};
            return View("TeamForm", teamFormViewModel);
        }

        // GET: Team/Edit/5
        public ActionResult Edit(int id)
        {
			ViewBag.ActionText = "Update Team";
			var team = _context.Teams.SingleOrDefault(t => t.Id == id);
			var teamFormViewModel = new TeamFormViewModel()
			{
				Id = team.Id,
				Name = team.Name,
				Mascot = team.Mascot
			};
			return View("TeamForm", teamFormViewModel);
        }

        // POST: Team/Edit/5
		[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(TeamModels teamModel)
        {
			var allTeams = _utility.GetAllTeams();
			TeamFormViewModel tfViewModel;
			try
            {

				bool isNewTeam = false;
				if (_context.Teams.Any(t => t.Name == teamModel.Name && t.Id != teamModel.Id))
				{
					ModelState.AddModelError("Name", "School Already added");
				}
				else
				{
					if (teamModel.Id == 0)
					{

						_context.Teams.Add(teamModel);

						isNewTeam = true;
					}
					else
					{
						var team = _context.Teams.SingleOrDefault(t => t.Id == teamModel.Id);

						team.Name = teamModel.Name;
						team.Mascot = teamModel.Mascot;
					}

				}

				if (!ModelState.IsValid)
				{
					tfViewModel = new TeamFormViewModel()
					{
						Id = teamModel.Id,
						Name = teamModel.Name,
						Mascot = teamModel.Mascot,
						AllTeams = allTeams.ToList()
					};
					return View("TeamForm", tfViewModel);
				}

				_context.SaveChanges();

				if (isNewTeam)
				{
					tfViewModel = new TeamFormViewModel()
					{
						Id = 0,
						Name = "",
						Mascot = "",
						AllTeams = allTeams.ToList()
					};
					return View("TeamForm", tfViewModel);
				}
				else
				{
					return RedirectToAction("Index");
				}
                
            }
            catch
            {
                return View("TeamForm", tfViewModel = new TeamFormViewModel()
				{
					Id = teamModel.Id,
					Name = teamModel.Name,
					Mascot = teamModel.Mascot,
					AllTeams = allTeams.ToList()
				});
            }
        }

    }
}
