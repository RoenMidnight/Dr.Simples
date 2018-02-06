using System;
using System.Linq;
using System.Web.Mvc;
using AplicacaoMedicina.DataContexts;
using AplicacaoMedicina.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Net.Mail;
using System.Web.UI;

namespace AplicacaoMedicina.Controllers
{
    public class MasterController : Controller
    {
        private DrMedContext db = new DrMedContext();

        protected void GeraLogAuditoria(string Usua_Audi, string Cont_Audi, string Acti_Audi)
        {
            Auditoria auditoria = new Auditoria();

            auditoria.Usua_Audi = Usua_Audi;
            auditoria.Cont_Audi = Cont_Audi;
            auditoria.Acti_Audi = Acti_Audi;

            ModelState.Clear();
            TryValidateModel(auditoria);

            if (ModelState.IsValid) { 
                db.Auditorias.Add(auditoria);
                db.SaveChanges();
            }
            
        }

        public void EnviaEMail(string Email, int idClient, string username, string password )
        {

            SmtpClient client = new SmtpClient();

            string token = GeraToken(idClient);

            client.Host = "smtpi.doutorsimples.com.br";
           // client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("doutorsimples@doutorsimples.com.br", "RoenOwona1");
            client.Port = 587;
         //   client.Port = 465;

            MailMessage mail = new MailMessage();
            mail.Sender = new MailAddress("doutorsimples@doutorsimples.com.br", "Doutor Simples");
            mail.From = new MailAddress("doutorsimples@doutorsimples.com.br", "Doutor Simples");
            mail.To.Add(Email);
            mail.Subject = "Ativação de Cadastro (Doutor Simples)";

            mail.Body = "Olá, <br/>Você se cadastrou no Doutor Simples, para ter acesso ,clique no link de ativação e ative seu cadastro<br/>"+
                "<a href=http://" + System.Web.HttpContext.Current.Request.Url.Host + "/UsuarioConsultorios/Valida?token=" + token + ">Clique Aqui</a>" +
                "<br/> Sua Senha: " + password +
                "<br/> Obrigado, <br/> Doutor Simples.";
     

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            try
            {
                client.Send(mail);
            }
            catch (Exception ex) {
                Exception ex2 = ex;
                string errorMessage = string.Empty;
                while (ex2 != null)
                {
                    errorMessage += ex2.ToString();
                    ex2 = ex2.InnerException;
                }

                //return errorMessage;
            }
            finally {             
                mail = null; }

            //return "Deu bom";

        }

        public void EnviaNotificacao(string Email, string Subject, string Body) {

            SmtpClient client = new SmtpClient();
            
            client.Host = "smtpi.doutorsimples.com.br";
            // client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("doutorsimples@doutorsimples.com.br", "RoenOwona1");
            client.Port = 587;
            //   client.Port = 465;

            MailMessage mail = new MailMessage();
            mail.Sender = new MailAddress("doutorsimples@doutorsimples.com.br", "Doutor Simples");
            mail.From = new MailAddress("doutorsimples@doutorsimples.com.br", "Doutor Simples");
            mail.To.Add(Email);
            mail.Subject = Subject;

            mail.Body = Body;

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            try {
                client.Send(mail);
            } catch (Exception ex) {
                Exception ex2 = ex;
                string errorMessage = string.Empty;
                while (ex2 != null) {
                    errorMessage += ex2.ToString();
                    ex2 = ex2.InnerException;
                }
            } finally { mail = null; }
            
        }

        protected String GeraToken(int idClient)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key  = Guid.NewGuid().ToByteArray();
            byte[] id   = BitConverter.GetBytes(idClient);

            String token = Convert.ToBase64String(id.Concat(time.Concat(key)).ToArray());

            return token;
        }

        protected bool ValidaToken(string token)
        {
            token = token.Trim().Replace(" ", "+");
                
            if (token.Length % 4 > 0)
                token = token.PadRight(token.Length + 4 - token.Length % 4, '=');

            byte[] data = Convert.FromBase64String(token);

            int id = BitConverter.ToInt32(data, 0);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 4));

            if (when < DateTime.UtcNow.AddHours(-1))
            {
                return false;
            }

            return true;
        }


    }
}