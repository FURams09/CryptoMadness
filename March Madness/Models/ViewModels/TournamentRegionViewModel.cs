using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace March_Madness.Models.ViewModels
{
	public class TournamentRegionViewModel
	{
		public List<string> Regions { get; set; }

		[Display(Name = "1")]
		public int OneSeedId { get; set; }
		[Display(Name = "16")]
		public int SixteenSeedId { get; set; }

		[Display(Name = "8")]
		public int EightSeedId { get; set; }
		[Display(Name = "9")]
		public int NineSeedId { get; set; }

		[Display(Name = "5")]
		public int FiveSeedId { get; set; }
		[Display(Name = "12")]
		public int TwelveSeedId { get; set; }

		[Display(Name = "4")]
		public int FourSeedId { get; set; }
		[Display(Name = "13")]
		public int ThirteeenSeedId { get; set; }

		[Display(Name = "6")]
		public int SixSeedId { get; set; }
		[Display(Name = "11")]
		public int ElevenSeedId { get; set; }

		[Display(Name = "3")]
		public int ThreeSeedId { get; set; }
		[Display(Name = "14")]
		public int FourteenSeedId { get; set; }

		[Display(Name = "7")]
		public int SevenSeedId { get; set; }
		[Display(Name = "10")]
		public int TenSeedId { get; set; }

		[Display(Name = "2")]
		public int TwoSeedId { get; set; }
		[Display(Name = "15")]
		public int FifteenSeedId { get; set; }


		public List<TeamModels> Teams { get; set; }
	
	}
}