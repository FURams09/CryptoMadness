using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using March_Madness.Models;

namespace March_Madness.Helpers
{
    public static class Utility
    {
        private static ApplicationDbContext _context = new ApplicationDbContext();

        public static List<string> GetRegionNames()
        {
            return Enum.GetNames(typeof(Regions)).ToList();
        }

        public static IQueryable<TeamModels> GetAllTeams()
        {
            return _context.Teams.OrderBy(t => t.Name);
        }
    }
}