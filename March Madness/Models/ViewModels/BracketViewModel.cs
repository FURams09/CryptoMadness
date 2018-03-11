using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace March_Madness.Models.ViewModels
{
	public class BracketViewModel
	{

		public List<BracketEntry> UserBrackets { get; set; }

		public int TournamentId { get; set; }

		public string TournamentEntryName { get; set; }

		public List<List<List<int>>> BracketPicks { get; set; }

		public string TournamentTeams { get; set; }

		[MaxLength(42)]
		public string EntryAddress { get; set; }
		
	}
}