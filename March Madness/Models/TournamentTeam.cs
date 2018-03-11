using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace March_Madness.Models
{
	public class TournamentTeams
	{
		public int Id { get; set; }

		public Regions Region { get; set; }

		public int Seed { get; set; }

		public int TeamId { get; set; }

		public Team Team { get; set; }

	}
}