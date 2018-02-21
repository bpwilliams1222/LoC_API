using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LoCWebApp.Startup))]
namespace LoCWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
