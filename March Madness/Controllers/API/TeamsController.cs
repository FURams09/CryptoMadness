using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using March_Madness.Models;

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
