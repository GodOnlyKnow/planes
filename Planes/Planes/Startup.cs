using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Planes.Startup))]
namespace Planes
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
