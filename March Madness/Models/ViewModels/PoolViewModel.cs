using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace March_Madness.Models.ViewModels
{
	public class PoolViewModel
	{
		public int PoolId { get; set; }

		public string Nickname { get; set; }

		public string Address { get; set; }

		public string OwnerAddress { get; set; }

		public decimal EntryFee { get; set; }
	}
}