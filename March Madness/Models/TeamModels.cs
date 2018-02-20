using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace March_Madness.Models
{
	public class TeamModels
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(60)]
		[Index(IsUnique = true)]
		public string Name { get; set; }

		[Required]
		[MaxLength(60)]
		public string Mascot { get; set; }
	}
}