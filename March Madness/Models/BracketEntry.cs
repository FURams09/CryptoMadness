using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace March_Madness.Models
{
	public class BracketEntry
	{
		public int Id { get; set; }

		public string UserId { get; set; }
		public User BracketOwner { get; set; }

		public string Name { get; set; }

		public List<BracketGamePick> Picks { get; set; }

		[ForeignKey("Pool")]
		public int PoolId { get; set; }
		public virtual Pool Pool { get; set; }

		[MaxLength(42)]
		public string OwnerAddress { get; set; }
	}
}