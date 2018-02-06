using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AplicacaoMedicina.Startup))]
namespace AplicacaoMedicina
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);           
        }
    }
}
