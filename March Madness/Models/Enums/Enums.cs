using System.ComponentModel.DataAnnotations;
namespace March_Madness.Models
{
	public enum Regions
	{
		[Display(Name ="East")]
		East = 1,

		[Display(Name = "MidWest")]
		MidWest = 2,

		[Display(Name = "West")]
		West = 3,

		[Display(Name = "South")]
		South = 4
	}
}