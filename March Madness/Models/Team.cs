using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace March_Madness.Models
{
	public class Team
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(150)]
		[Index(IsUnique = true)]
		public string Name { get; set; }

		[MaxLength(30)]
		[Display (Name="Display Name")]
		public string DisplayName { get; set; }
	}

}