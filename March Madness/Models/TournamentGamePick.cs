using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace March_Madness.Models
{
	public class TournamentGamePick
	{
		public int Id { get; set; }
		public	int	GameId { get; set; }
		public int Winner { get; set; }
	}
}