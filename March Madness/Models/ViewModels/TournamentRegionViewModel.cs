using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace March_Madness.Models.ViewModels
{
	public class TournamentRegionViewModel
	{
        
		public List<int> East { get; set; }
        public List<int> Midwest { get; set; }
        public List<int> West { get; set; }
        public List<int> South { get; set; }

		public List<int> Round1PairingOrder { get; set; }

        public List<TeamModels> Teams { get; set; }
	
	}
}