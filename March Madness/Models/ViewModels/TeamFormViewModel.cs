using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using March_Madness.Models;


namespace March_Madness.Models.ViewModels
{

	
	public class TeamFormViewModel
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(60)]
		public string Name { get; set; }

		[Required]
		[MaxLength(60)]
		public string Mascot { get; set; }

		public List<TeamModels> AllTeams { get; set; }
	}
}