using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using March_Madness.Models;
using March_Madness.Models.ViewModels;
using March_Madness.Helpers;

namespace March_Madness.Controllers.API
{
    public class BracketController : ApiController
    {
		private ApplicationDbContext _context;

		public BracketController()
		{
			_context = new ApplicationDbContext();
		}

		[Route("api/bracket/save")]
		[HttpPost]
		//[Authorize]
		public IHttpActionResult SaveBracket(UserBracketViewModel bracket)
		{
			try
			{
				TournamentEntry tournamentBracket;
				string currentUserId = User.Identity.GetUserId();
				bool isNewBracket = false;

				tournamentBracket = _context.TournamentEntry.Include(t => t.Picks).SingleOrDefault(t => t.Id == bracket.TournamentId);
				if (tournamentBracket == null )
				{
					var bracketName = bracket.TournamentEntryName is null ? User.Identity.Name + ' ' + DateTime.Now.ToShortDateString() : bracket.TournamentEntryName;
					tournamentBracket = new TournamentEntry()
					{
						UserId = currentUserId,
						Name = bracketName,
						Picks = new List<TournamentGamePick>()
					};
					isNewBracket = true;
				} 

				var utl = new Utility();
				var teamList = utl.GetTournamentBracket();

				List<TournamentTeams> currentPicks = new List<TournamentTeams>() ;

				List<TournamentGamePick> currentBracket = new List<TournamentGamePick>();

				var round1Seeds = Utility.Round1PairingOrder;
				List<int> roundsGame1 = new List<int>();
				for (int i = 0; i < bracket.BracketPicks.Count; i++)
				{

					roundsGame1.Add(currentPicks.Count);

					for (var j = 0; j < bracket.BracketPicks[i].Count; j++)
					{
						var game = bracket.BracketPicks[i][j];
						
						if (!(game[0] == 1 ^ game[1] == 1) || game.Count > 2)
						{
							currentPicks.Add(new TournamentTeams() { TeamId = 0 });
							continue; //for debugging. Remove once filling out is done
							return BadRequest(); //This game doesn't have only 1 winner
						}
						else
						{
							TournamentTeams team1, team2, winner;
							HomeOrAway winnerHomeAway;
							if (i == 0)
							{
								var region = j / 8;
								var team1Seed = round1Seeds[(j * 2) - (region * 16)];
								var team2Seed = round1Seeds[((j * 2) + 1) - (region * 16)];

								
								switch (region)
								{
									case 0:
										team1 = teamList[Regions.East].Single(t => t.Seed == team1Seed);
										team2 = teamList[Regions.East].Single(t => t.Seed == team2Seed);

										break;
									case 1:
										team1 = teamList[Regions.Midwest].Single(t => t.Seed == team1Seed);
										team2 = teamList[Regions.Midwest].Single(t => t.Seed == team2Seed);
										break;
									case 2:
										team1 = teamList[Regions.West].Single(t => t.Seed == team1Seed);
										team2 = teamList[Regions.West].Single(t => t.Seed == team2Seed);
										break;
									case 3:
										team1 = teamList[Regions.South].Single(t => t.Seed == team1Seed);
										team2 = teamList[Regions.South].Single(t => t.Seed == team2Seed);
										break;

									default:
										return BadRequest();
								}
							}
							else
							{
								team1 = currentPicks[(j * 2) + roundsGame1[i - 1]];
								team2 = currentPicks[(j * 2) + roundsGame1[i - 1 ] + 1];					
						
							}

							if (game[1] == 1)
							{
								winner = team2;
								winnerHomeAway = HomeOrAway.Away;
								
							}
							else
							{
								winner = team1;
								winnerHomeAway = HomeOrAway.Home;
							}
							
							currentPicks.Add(winner);

							TournamentGamePick pick;
							if (isNewBracket)
							{
								pick = new TournamentGamePick()
								{
									RoundNo = i,
									GameNo = j,
									PickedTeamId = winner.Id,
									HomeOrAway = winnerHomeAway 

								};
								tournamentBracket.Picks.Add(pick);
							}
							else
							{
								var currentPick = tournamentBracket.Picks.Single(t => t.RoundNo == i & t.GameNo == j);
								if (currentPick == null)
								{
									pick = new TournamentGamePick()
									{
										RoundNo = i,
										GameNo = j,
										PickedTeamId = winner.Id,
										HomeOrAway = winnerHomeAway

									};

									tournamentBracket.Picks.Add(pick);
								}
								else
								{
									if (currentPick.PickedTeamId != winner.Id) {
										currentPick.PickedTeamId = winner.Id;
										currentPick.HomeOrAway = winnerHomeAway;
									}
									
								}
							}
							
						}
					}
				}
				if (isNewBracket)
				{
					_context.TournamentEntry.Add(tournamentBracket);
				}
				
				if (ModelState.IsValid)
				{
					_context.SaveChanges();
				}



			}
			catch (Exception ex)
			{
				var newMsg = ex;
				return NotFound();
			}
			return Ok();
		}

		[Route("api/bracket/{bracketId}")]
		[HttpGet]
		//[Authorize]
		public IHttpActionResult GetBracket(int bracketId)
		{
			var utl = new Utility();

			return Ok(utl.GetIndividualBracket(bracketId).ToArray());
		}
	}


}
