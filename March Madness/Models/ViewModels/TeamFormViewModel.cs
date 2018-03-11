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
		[MaxLength(150)]
		public string Name { get; set; }

		[MaxLength(30)]
		public string DisplayName { get; set; }

		public List<Team> AllTeams { get; set; }
	}
}