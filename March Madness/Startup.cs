using March_Madness.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;


[assembly: OwinStartupAttribute(typeof(March_Madness.Startup))]
namespace March_Madness
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
			CreateRolesAndUsers();
        }

		
		private void CreateRolesAndUsers()
		{
			ApplicationDbContext context = new ApplicationDbContext();

			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
			var UserManager = new UserManager<User>(new UserStore<User>(context));


			// In Startup iam creating first Admin Role and creating a default Admin User    
			if (!roleManager.RoleExists(Roles.Admin))
			{

				// first we create Admin rool   
				var role = new IdentityRole
				{
					Name = Roles.Admin
				};
				roleManager.Create(role);

				//Here we create a Admin super user who will maintain the website                  

				var user = new User
				{
					UserName = "GWP",
					Email = "furams09@yahoo.com"
				};

				string userPWD = "A@ng8Z6pna4wAtZwre3ye";

				var chkUser = UserManager.Create(user, userPWD);

				//Add default User to Role Admin   
				if (chkUser.Succeeded)
				{
					var result1 = UserManager.AddToRole(user.Id, "Admin");

				}
			}

		}

	}
}
