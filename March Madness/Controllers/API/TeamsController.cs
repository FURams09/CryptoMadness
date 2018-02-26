using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using March_Madness.Models;
using March_Madness.Helpers;
using RestSharp;
using HtmlAgilityPack;
using System.Web;

namespace March_Madness.Controllers.API
{
    public class TeamsController : ApiController
    {
		private ApplicationDbContext _context;

		public TeamsController()
		{
			_context = new ApplicationDbContext();
		}

		[HttpDelete]
		public void Delete(int id)
		{
			try
			{
				// TODO: Add delete logic here
				var teamToDelete = _context.Teams.SingleOrDefault(t => t.Id == id);

				if (teamToDelete != null)
				{
					_context.Teams.Remove(teamToDelete);
					_context.SaveChanges();
				} else
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);
				}

			}
			catch
			{
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			}
		}

		[Route("api/teams/refresh")]
		[HttpPost]
		//[Authorize]
		public IHttpActionResult RefreshTeamList()
		{
			try
			{
				var teamClient = new RestClient("https://www.sports-reference.com");

				var request = new RestRequest("/cbb/pi/shareit/R1Syn", Method.GET);

				IRestResponse response = teamClient.Execute(request);

				var content = response.Content;

				var teamPage = new HtmlDocument();
				teamPage.LoadHtml(content);

				var teams = teamPage.DocumentNode.SelectNodes("//table[@id='schools']/tbody/tr");

				foreach (var item in teams)
				{
					if ((item.Attributes["class"] == null) && item.ChildNodes[3].InnerText == "2018")
					{
						var name = HttpUtility.HtmlDecode( item.ChildNodes[1].InnerText);
						if (_context.Teams.SingleOrDefault(t => t.Name == name) == null)
						{
						_context.Teams.Add(new Teams()
							{
								Name = name
							});
						}
						
					}
				}

				if (ModelState.IsValid)
				{
					_context.SaveChanges();
					var _utility = new Utility();
					return Ok(_utility.GetAllTeams().ToList());
				}
				else
				{
					return BadRequest();
				}
				
			}
			catch (Exception)
			{

				return BadRequest();
			}
			
		}

		[Route("api/teams")]
		public IHttpActionResult GetTeams()
		{
			try
			{
				var allTeams = _context.Teams.OrderBy(t => t.Name);
				return Ok(allTeams.ToList());
			}
			catch
			{
				return NotFound();
			}
			
		}
    }
}
