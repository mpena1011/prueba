using Microsoft.Owin;
using Owin;
using System;
using System.Net.Mail;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

[assembly: OwinStartupAttribute(typeof(WebConsultaContasis.Startup))]
namespace WebConsultaContasis
{
    public partial class Startup {
        /// <summary>
        /// Metodo configuration por defecto de la aplicacion web
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app) {
           
            ConfigureAuth(app);
            JobScheduler.Start();
        }      
    }
}
