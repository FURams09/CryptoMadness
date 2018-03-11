using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace March_Madness.Models
{
	public class BracketGamePick
	{
		public int Id { get; set; }

		public int BracketEntryId { get; set; }

		public int RoundNo { get; set; }

		public	int	GameNo { get; set; }

		public HomeOrAway HomeOrAway { get; set; }

		[Display (Name = "Winner")]
		public int PickedTeamId { get; set; }

		public TournamentTeams PickedTeam { get; set; }


	}
}