using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Vitascript.Startup))]
namespace Vitascript
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
