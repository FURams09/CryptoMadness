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
		[HttpGet]
		public IHttpActionResult GetTournament()
		{
			var _utility = new Utility();
			var teamList = _utility.GetAllTeams();
			var regions = Utility.GetRegionNames();


			var seededOrder = _context.TournamentTeams
				.OrderBy(t => t.Seed);
			var east = seededOrder
			.Where(t => t.Region == Regions.East)
			.Select(t => new { t.Seed, t.TeamId });

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
				Bracket= bracket,
				Round1PairingOrder = Utility.Round1PairingOrder,
				Teams = teamList.ToList()
			};

			return Ok(tournamentRegionViewModel);
		}

        [Route("api/tournament")]
        [HttpPost]
        public IHttpActionResult SaveTournament(Dictionary<string, Dictionary<int, int>> bracket)
        {
			try
			{
				UpdateBracketRegion(bracket["east"], Regions.East);
                      
                UpdateBracketRegion(bracket["midwest"], Regions.Midwest);
                           
                UpdateBracketRegion(bracket["west"], Regions.West);
                           
                UpdateBracketRegion(bracket["south"], Regions.South);
                           
				if (ModelState.IsValid)
				{
					_context.SaveChanges();
				}
				else
				{
					return BadRequest();
				}
                   
            }

			catch (Exception ex)
			{
				var nds = ex;
				return NotFound();
			}
			return Ok();
		}

        private void UpdateBracketRegion(Dictionary<int, int> bracket, Regions region)
        {
            foreach (var teamAndseed in bracket)
            {
                var teamId = teamAndseed.Value;
                
				if (teamId != 0)
				{
					var oldTeam = _context.TournamentTeams.SingleOrDefault(t => (t.Seed == teamAndseed.Key) && t.Region == region);
					if (oldTeam == null  )
					{
						//
						List<BracketGamePick> oldPicks = _context.BracketGamePicks.Where(t => t.PickedTeamId == oldTeam.Id).ToList();
						oldPicks.ForEach(p => p.PickedTeamId = teamId);

						var newTeam = new TournamentTeams()
						{
							TeamId = teamId,
							Region = region,
							Seed = teamAndseed.Key
						};

						_context.TournamentTeams.Add(newTeam);
					}
					else
					{
						oldTeam.TeamId = teamId;
					}
				}
                
            }
        }

		//[Route("api/bracket")]
		//public IHttpActionResult  GetTournament()
		//{
		//	try
		//	{
		//		var utl = new Utility();

		//		return Ok(utl.GetBracket());




		//	}
		//	catch
		//	{
		//		return NotFound();
		//	}
		//}
	}

    
}
