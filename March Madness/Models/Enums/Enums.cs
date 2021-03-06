﻿using System.ComponentModel.DataAnnotations;
namespace March_Madness.Models
{
	public enum Regions
	{
		[Display(Name ="East")]
		East = 1,

		[Display(Name = "Midwest")]
		Midwest = 2,

		[Display(Name = "West")]
		West = 3,

		[Display(Name = "South")]
		South = 4
	}

	public enum HomeOrAway
	{
		[Display(Name = "Home")]
		Home = 0,
		[Display(Name = "Away")]
		Away = 1
	}
}