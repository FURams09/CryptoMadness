using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace March_Madness.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
		[Required]
		[MaxLength(42)]
		public string MainAddress { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }



    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
		public DbSet<Team> Teams { get; set; }
		public DbSet<TournamentTeams> TournamentTeams { get; set; }
		public DbSet<BracketEntry> BracketEntries { get; set; }
		public DbSet<BracketGamePick> BracketGamePicks { get; set; }
		public DbSet<Pool> Pools { get; set; }
		public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}