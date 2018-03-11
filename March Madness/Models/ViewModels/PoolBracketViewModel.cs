using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace March_Madness.Models.ViewModels
{
	public class PoolBracketViewModel

	{
		public string PoolNickname { get; set; }

		public int PoolId { get; set; }

		public List<BracketEntry> EntriesInPool { get; set; }
	}
}