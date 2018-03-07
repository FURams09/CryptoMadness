using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace March_Madness.Models.ViewModels
{
	public class UserBracketViewModel
	{

		public List<TournamentEntry> UserBrackets { get; set; }

		public int TournamentId { get; set; }

		public string TournamentEntryName { get; set; }

		public List<List<List<int>>> BracketPicks { get; set; }
		
		public List<List<string>> TournamentTeams { get; set; }
		
	}
}