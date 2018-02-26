using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace March_Madness.Models
{
	public class TournamentEntry
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		public string Name { get; set; }

		public List<TournamentGamePick> Picks { get; set; }

	}
}