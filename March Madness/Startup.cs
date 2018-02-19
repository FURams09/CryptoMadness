using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(March_Madness.Startup))]
namespace March_Madness
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
