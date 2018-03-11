using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace March_Madness.Models.ViewModels
{
	public class OwnedPoolsViewModel
	{
		public string OwnerName { get; set; }

		public string OwnerId { get; set; }

		public List<Pool> OwnedPools { get; set; }
				
	}
}