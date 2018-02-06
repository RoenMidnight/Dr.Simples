using AplicacaoMedicina.DataContexts;
using AplicacaoMedicina.Models;
using AplicacaoMedicina.Helper;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Data.Entity;

namespace AplicacaoMedicina
{
    public class MvcApplication : System.Web.HttpApplication
    {

      //  string con = ConfigurationManager.ConnectionStrings["DefaultConnection_DatabasePublish"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

    //        SqlDependency.Start(con);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true) {
                if(Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        string username = 
                            FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

                        string roles = string.Empty;

                        string logUser = username.Substring(0, 4);

                        using (DrMedContext entities = new DrMedContext())
                        {

                            switch (logUser)
                            {
                                case "cons":
                                    {
                                        UsuarioConsultorio userC =
                                            entities.UsuarioConsultorios.SingleOrDefault(u => u.Usuario_UsuCons == username);
                                        roles = "Consultorio";
                                        break;
                                    }
                                case "paci":
                                    {
                                        UsuarioPaciente userP = entities.UsuarioPacientes.SingleOrDefault(u => u.Usuario_UsuPaci == username);
                                        roles = "Paciente";
                                        break;
                                    }
                                case "labo":
                                    {
                                        UsuarioLaboratorio userL = entities.UsuarioLaboratorios.SingleOrDefault(u => u.Usuario_UsuLab == username);
                                        roles = "Laboratorio";
                                        break;
                                    }

                                default:
                                    {
                                        roles = string.Empty;
                                        break;
                                    }
                            }
                                                                
                        }

                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                            new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));


                    } catch (Exception) {
                        
                    }

                }
            }

         /*   NotificationComponents NC = new NotificationComponents();
            var currentTime = DateTime.Now;
            HttpContext.Current.Session["LastUpdated"] = currentTime;
            NC.RegisterNotification(currentTime); */

        }

        protected void Application_End()
        {
    //        SqlDependency.Stop(con);
        }

    }
}
