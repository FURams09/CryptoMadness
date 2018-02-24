using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using March_Madness.Models;

namespace March_Madness.Helpers
{
	public class Utility
	{
		private static ApplicationDbContext _context = new ApplicationDbContext();

		public static List<int> Round1PairingOrder = new List<int>() { 1, 16, 8, 9, 5, 12, 4, 13, 6, 11, 3, 14, 7, 10, 2, 15 };

		public static List<string> GetRegionNames()
		{
			return Enum.GetNames(typeof(Regions)).ToList();
		}

		public static IQueryable<TeamModels> GetAllTeams()
		{
			return _context.Teams.OrderBy(t => t.Name);
		}

		
		public Dictionary<Regions, List<TournamentTeams>> GetBracket() { 

			List<TournamentTeams> regionTeams = new List<TournamentTeams>() {null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };

			Dictionary<Regions, List<TournamentTeams>> bracket = new Dictionary<Regions, List<TournamentTeams>>();
			bracket.Add(Regions.East, new List<TournamentTeams>(regionTeams));
			bracket.Add(Regions.Midwest, new List<TournamentTeams>(regionTeams));
			bracket.Add(Regions.West, new List<TournamentTeams>(regionTeams));
			bracket.Add(Regions.South, new List<TournamentTeams>(regionTeams));

			var tournamentTeams = _context.TournamentTeam.Include(t => t.Team)
				.ToList();

			foreach (TournamentTeams team in tournamentTeams)
			{
				bracket[team.Region][team.Seed - 1] = team;
				
			}

			return bracket;	
		}
    }
}