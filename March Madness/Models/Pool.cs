using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace March_Madness.Models
{
	public class Pool
	{
		public int Id { get; set; }

		public string OwnerId { get; set; }
		public User Owner { get; set; }

		public string Nickname { get; set; }

		public decimal EntryFee { get; set; }

		[MaxLength(42)]
		[Required]
		public string OwnerAddress { get; set; }

	}
}