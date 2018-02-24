using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using March_Madness.Models;
using March_Madness.Models.ViewModels;
using March_Madness.Helpers;


namespace March_Madness.Controllers.API
{
    public class TournamentController : ApiController
    {
        private ApplicationDbContext _context;

        public TournamentController()
        {
            _context = new ApplicationDbContext();
        }

        [Route("api/tournament")]
        [HttpPost]
        public IHttpActionResult SaveTournament(TournamentRegionViewModel bracket)
        {
			try
			{
                for (int i = 1; i <= 4; i++)
                {
                    switch (i)
                    {
                        case 1:
                            UpdateBracketRegion(bracket.East, Regions.East);
                            break;
                        case 2:
                            UpdateBracketRegion(bracket.Midwest, Regions.Midwest);
                            break;

                        case 3:
                            UpdateBracketRegion(bracket.West, Regions.West);
                            break;

                        case 4:
                            UpdateBracketRegion(bracket.South, Regions.South);
                            break;
                    }
					if (ModelState.IsValid)
					{
						_context.SaveChanges();
						
					}
					else
					{
						return BadRequest();
					}
                   
                }
               

                
                
            }
			catch
			{
				return NotFound();
			}
			return Ok();
		}

        private void UpdateBracketRegion(List<int> bracket, Regions region)
        {
            for (int i = 0; i < bracket.Count; i++)
            {
                var teamId = bracket[i];
                var oldTeam = _context.TournamentTeam.SingleOrDefault(t => (t.Seed == i + 1) && t.Region == region);

                if (oldTeam == null)
                {
                    var newTeam = new TournamentTeams()
                    {
                        TeamId = teamId,
                        Region = region,
                        Seed = i + 1
                    };

                    _context.TournamentTeam.Add(newTeam);
                }
                else
                {
                    oldTeam.TeamId = teamId;
                }
            }
        }

		[Route("api/bracket")]
		public IHttpActionResult  GetTournament()
		{
			try
			{
				var utl = new Utility();

				return Ok(utl.GetBracket());




			}
			catch
			{
				return NotFound();
			}
		}
	}

    
}
