using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace March_Madness.Models
{
	public class TournamentEntryModel
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public List<TournamentGamePick> Picks { get; set; }

	}
}