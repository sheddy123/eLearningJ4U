using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eLearningLMS.Startup))]
namespace eLearningLMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
