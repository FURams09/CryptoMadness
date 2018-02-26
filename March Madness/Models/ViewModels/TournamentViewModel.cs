using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace March_Madness.Models.ViewModels
{
	public class TournamentRegionViewModel
	{
        
		public Dictionary<string, Dictionary<int, int>> Bracket { get; set; }
       
		public List<int> Round1PairingOrder { get; set; }

        public List<Teams> Teams { get; set; }
	
	}
}