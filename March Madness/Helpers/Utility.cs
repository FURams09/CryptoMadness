using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using March_Madness.Models;

namespace March_Madness.Helpers
{
	public class Utility
	{
		private ApplicationDbContext _context;

		public Utility()
		{
			_context = new ApplicationDbContext();
		}

		public static List<int> Round1PairingOrder = new List<int>() { 1, 16, 8, 9, 5, 12, 4, 13, 6, 11, 3, 14, 7, 10, 2, 15 };

		public static List<string> GetRegionNames()
		{
			return Enum.GetNames(typeof(Regions)).ToList();
		}

		public IQueryable<Teams> GetAllTeams()
		{
			return _context.Teams.OrderBy(t => t.Name);
		}

		
		public Dictionary<Regions, List<TournamentTeams>> GetTournamentBracket() { 

			List<TournamentTeams> regionTeams = new List<TournamentTeams>() {null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };

			Dictionary<Regions, List<TournamentTeams>> bracket = new Dictionary<Regions, List<TournamentTeams>>
			{
				{ Regions.East, new List<TournamentTeams>(regionTeams) },
				{ Regions.Midwest, new List<TournamentTeams>(regionTeams) },
				{ Regions.West, new List<TournamentTeams>(regionTeams) },
				{ Regions.South, new List<TournamentTeams>(regionTeams) }
			};

			var tournamentTeams = _context.TournamentTeam.Include(t => t.Team)
				.ToList();

			foreach (TournamentTeams team in tournamentTeams)
			{
				bracket[team.Region][team.Seed - 1] = team;
			}

			return bracket;	
		}

		public List<List<List<int>>> GetIndividualBracket(int bracketId)
		{

			var bracket = new List<List<List<int>>>();

			for (int i = 0; i < 6; i++)
			{
				bracket.Add(new List<List<int>>());
			}

			var tournamentTeams = _context.TournamentGame
				.Where(t => t.TournamentEntryID == bracketId)
				.OrderBy(t => new { t.RoundNo, t.GameNo})
				.Include(t => t.PickedTeam)
				.ToList();

			foreach (TournamentGamePick game in tournamentTeams)
			{
				var gameList = new List<int>() { 0, 0 };
				switch (game.HomeOrAway)
				{
					case HomeOrAway.Home:
						bracket[game.RoundNo].Add(new List<int>() { 1, 0 });
						break;
					case HomeOrAway.Away:
						bracket[game.RoundNo].Add(new List<int>() { 0, 1 });
						break;

				}
				
			}

			return bracket;
		}

	}
}