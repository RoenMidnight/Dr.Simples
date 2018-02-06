using AplicacaoMedicina.DataContexts;
using AplicacaoMedicina.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using AplicacaoMedicina.Helper;
using AplicacaoMedicina.Controllers;
using Facebook;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web.Helpers;

namespace AplicacaoMedicina.Controllers
{
    public class UsuarioPacienteController : MasterController
    {
        private DrMedContext db = new DrMedContext();
 //       private const string facebookToke = "193acb1e7a6e12b568c152081da5abcd";

        public JsonResult GetJSON()
        {
            var usuarioPaciente = db.UsuarioPacientes.Include(p => p.Pacientes);
            return Json(usuarioPaciente.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: UsuarioPaciente
      /*  public ActionResult Index()
        {
            var usuarioPaciente = db.UsuarioPacientes.Include(p => p.Pacientes);
            return View(usuarioPaciente.ToList());
        }*/

        // GET: UsuarioPaciente/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioPaciente usuarioPaciente = db.UsuarioPacientes.Find(id);
            if (usuarioPaciente == null)
            {
                return HttpNotFound();
            }
            return View(usuarioPaciente);
        }

        // GET: UsuarioPaciente/Create
        public ActionResult Create()
        {
            ViewBag.Id_Paci = new SelectList(db.Pacientes, "Id_Paci", "Nome_Paci");
            return View();
        }

        // POST: UsuarioPaciente/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_UsuPaci, Usuario_UsuPaci, Senha_UsuPaci, ID_Paci")] UsuarioPaciente usuarioPaciente)
        {
            if (ModelState.IsValid)
            {
                db.UsuarioPacientes.Add(usuarioPaciente);
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "UsuarioPaciente", "Create");
                return RedirectToAction("Login");
            }

            ViewBag.Id_Paci = new SelectList(db.Pacientes, "Id_Paci", "Nome_Paci", usuarioPaciente.ID_Paci);
            return View(usuarioPaciente);
        }

        // GET: UsuarioPacientes/Edit/5
        public ActionResult Edit()
        {           
            if (Request.Cookies["UserID"].Value == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int id = Int32.Parse(Request.Cookies["UserID"].Value);

            UsuarioPaciente usuarioPaciente = db.UsuarioPacientes.Find(id);
            if (usuarioPaciente == null)
            {
                return HttpNotFound();
            }

            return PartialView(usuarioPaciente);
        }

        // POST: UsuarioPacientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_UsuPaci, Usuario_UsuPaci, Senha_UsuPaci, ID_Paci")] UsuarioPaciente usuarioPaciente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuarioPaciente).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "UsuarioPaciente", "Edit");
                return Redirect("/Paciente/Edit");
            }


            return PartialView(usuarioPaciente);
        }

        // GET: UsuarioPaciente/Delete/5
      /*  public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioPaciente usuarioPaciente = db.UsuarioPacientes.Find(id);
            if (usuarioPaciente == null)
            {
                return HttpNotFound();
            }
            return View(usuarioPaciente);
        } */

        // POST: UsuarioPacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioPaciente usuarioPaciente = db.UsuarioPacientes.Find(id);
            db.UsuarioPacientes.Remove(usuarioPaciente);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "UsuarioPaciente", "DeleteConfirmed");
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
                
        public ActionResult Login()
        {
            /*   if (Request.Cookies["PacienteID"].Value != null)  {
                   return Redirect("/Paciente/Dashboard");
               } */

            if (Request.IsAuthenticated) {
                return Redirect("/Paciente/Dashboard");
            }

            return View();
        }

        public JsonResult AcaoFacebook(string facebookToken)
        {

            //Faz a busca pelas informações do usuário
            string result = FacebookHelper.FacebookSecretProof(facebookToken);
          
            //Array com as informações do usuário
            string[] resArray = result.Split(',');            

            //Id da Conta Facebook
            string resId   = resArray[0].Substring(7, resArray[0].Length - 8);

            //Nome da Conta Facebook - Conserta a acentuação.
            StringBuilder sb = new StringBuilder();
            string resName = resArray[1].Substring(8, resArray[1].Length - 9);

            foreach (char c in resName) {
                if (c > 127) {
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                } else {
                    sb.Append(c);
                }
            }
           
            resName = Regex.Replace(sb.ToString(), @"\\u(?<Value>[a-zA-Z0-9]{4})", m => {
                return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
            });
            

            //E-mail da Conta Facebook
            string resUser = resArray[2].Substring(9, resArray[2].Length - 10);
            
            //Verificar se é um e-mail
            try {
                var addr = new System.Net.Mail.MailAddress(resUser);
            } catch {
                return Json(new { message = "Email" }, JsonRequestBehavior.AllowGet);
            }
            
            //Sexo da Conta Facebook
            string resGend = resArray[3].Substring(10, resArray[3].Length - 11);

            //Verifica se o usuário já esta cadastrado com este e-mail. Se sim Loga.   
            
            var pacExist = db.Pacientes.Where(x => x.Email_Paci == resUser).FirstOrDefault();
            
            if (pacExist != default(Paciente))  {
                                
                UsuarioPaciente usuPaciente = new UsuarioPaciente();
                usuPaciente.Usuario_UsuPaci = resUser;
                usuPaciente.Senha_UsuPaci = resId;

                Login(usuPaciente, "");
                return Json(new { message = "login"}, JsonRequestBehavior.AllowGet);
            }

            //Cmonta objeto Paciente para realizar o cadastro

            if (resGend == "male")
            {
                resGend = "Masculino";
            }
            else {
                resGend = "Feminino";
            }

            Paciente usuPac   = new Paciente();
            usuPac.Email_Paci = resUser;
            usuPac.Nome_Paci  = resName;
            usuPac.Sexo_Paci  = resGend;

            ModelState.Clear();
            TryValidateModel(usuPac);

            if (ModelState.IsValid)
            {
                db.Pacientes.Add(usuPac);

                UsuarioPaciente usuarioPaciente = new UsuarioPaciente();

                usuarioPaciente.Usuario_UsuPaci = usuPac.Email_Paci;
                usuarioPaciente.Senha_UsuPaci = Crypto.HashPassword(resId);
                usuarioPaciente.ID_Paci = usuPac.ID_Paci;

                db.UsuarioPacientes.Add(usuarioPaciente);

                db.SaveChanges();               

                usuarioPaciente.Senha_UsuPaci = resId;               

                GeraLogAuditoria("Nulo", "Paciente", "Create");
                //Loga Usuário Recem Cadastrado.
                Login(usuarioPaciente, "");

                string Subject = "Cadastro (Doutor Simples)";

                string Body = "Olá, <br/>Você se cadastrou no Doutor Simples. <br/> Caso deseje acessar nossos serviços utilize a seguinte senha.<br/>" +
                    "<br/> Sua Senha: " + resId +
                    "<br/> Obrigado, <br/> Doutor Simples.";

                EnviaNotificacao(usuPac.Email_Paci, Subject, Body);

            } else  {
                var error = string.Join("|", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

                return Json(new { message = error}, JsonRequestBehavior.AllowGet);
            }                          

            return Json(new { message = "create" }, JsonRequestBehavior.AllowGet);
        }

      

        [HttpPost]
        public ActionResult Login(UsuarioPaciente model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (DrMedContext entities = new DrMedContext())
                {
                    string user = model.Usuario_UsuPaci;
                    string pass = model.Senha_UsuPaci;
                    string hashPass = "";

                    var userValid = (from _UsuPaci in db.UsuarioPacientes
                                     where user == _UsuPaci.Usuario_UsuPaci
                                     select new {
                                         _UsuPaci.Id_UsuPaci,
                                         _UsuPaci.Usuario_UsuPaci,
                                         _UsuPaci.Senha_UsuPaci,
                                         _UsuPaci.Pacientes
                                     }).FirstOrDefault();

                    if (userValid != null)
                    {
                        hashPass = userValid.Senha_UsuPaci;
                    }


                    if (userValid != null && Crypto.VerifyHashedPassword(hashPass, pass)) {
                        
                        FormsAuthentication.SetAuthCookie("paci"+user, false);
                        Response.Cookies["PacienteID"].Value = userValid.Pacientes.ID_Paci.ToString();
                        Response.Cookies["PacienteID"].Expires = DateTime.Now.AddDays(1);

                        Response.Cookies["UserID"].Value = userValid.Id_UsuPaci.ToString();
                        Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(1);


                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            &&  !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) {
                            return Redirect(returnUrl);
                        } else {

                            GeraLogAuditoria("Nulo", "UsuarioPaciente", "Login");
                            return RedirectToAction("Dashboard", "Paciente");
                        }
                        
                    } else {
                        ModelState.AddModelError("", "A senha esta incorreta");
                    }
                }
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            GeraLogAuditoria("Nulo", "UsuarioPaciente", "LogOff");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // Ações JSON 
        
        [HttpPost]
        public JsonResult LoginJson(UsuarioPaciente model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (DrMedContext entities = new DrMedContext())
                {
                    string user = model.Usuario_UsuPaci;
                    string pass = model.Senha_UsuPaci;

                    var userValid = (from _UsuPaci in db.UsuarioPacientes
                                     where user == _UsuPaci.Usuario_UsuPaci &&
                                           pass == _UsuPaci.Senha_UsuPaci
                                     select new
                                     {
                                         _UsuPaci.Id_UsuPaci,
                                         _UsuPaci.Usuario_UsuPaci,
                                         _UsuPaci.Senha_UsuPaci,
                                         _UsuPaci.Pacientes
                                     }).FirstOrDefault();

                    if (userValid != null)
                    {


                        FormsAuthentication.SetAuthCookie("paci" + user, false);
                        Response.Cookies["PacienteID"].Value = userValid.Pacientes.ID_Paci.ToString();
                        Response.Cookies["PacienteID"].Expires = DateTime.Now.AddDays(1);

                        Response.Cookies["UserID"].Value = userValid.Id_UsuPaci.ToString();
                        Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(1);


                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Json(new { success = true}, JsonRequestBehavior.AllowGet) ;
                        }
                        else {

                            GeraLogAuditoria("Nulo", "UsuarioPaciente", "Login");
                            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else {
                        ModelState.AddModelError("", "A senha esta incorreta");
                        return Json(new { success = false, message = "Senha incorreta" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(new { success = false, message = "Model Inválido"}, JsonRequestBehavior.AllowGet);

        }

        public JsonResult LogOffJson()
        {
            GeraLogAuditoria("Nulo", "UsuarioPaciente", "LogOffJson");
            FormsAuthentication.SignOut();
            return Json(true);
        }

        public ActionResult RedefinirSenha()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RedefinirSenha(string Email)
        {
            var email = db.UsuarioPacientes.Where(x => x.Usuario_UsuPaci == Email).FirstOrDefault();

            if (email == default(UsuarioPaciente))
            {
                View();
            }

            string token = GeraToken(email.Id_UsuPaci);

            string body = "Você realizou o pedido de redefinição de sua senha, clique o link abaixo para que seja efetuada. <br/>" +
                 "<a href=http://" + System.Web.HttpContext.Current.Request.Url.Host + "/UsuarioPaciente/Redefine?token=" + token + ">Clique Aqui</a> <br/>" +
                "Atenciosamente, equipe Doutor Simples.";

            EnviaNotificacao(Email, "Redefinir Senha (Doutor Simples)", body);

            return RedirectToAction("Login", "UsuarioPaciente");
        }

        public ActionResult Redefine(string token)
        {

            token = token.Trim().Replace(" ", "+");

            if (token.Length % 4 > 0)
                token = token.PadRight(token.Length + 4 - token.Length % 4, '=');


            byte[] data = Convert.FromBase64String(token);

            int id = BitConverter.ToInt32(data, 0);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 4));

            if (when < DateTime.UtcNow.AddHours(-24))
            {
                return RedirectToAction("Index", "Home");
            }

            UsuarioPaciente usuarioPaciente = db.UsuarioPacientes.Find(id);

            if (usuarioPaciente == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Id_UsuPaci = usuarioPaciente.Id_UsuPaci;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Redefine(int Id_UsuPaci, string Senha)
        {
            UsuarioPaciente usuarioPaciente = db.UsuarioPacientes.Find(Id_UsuPaci);

            if (usuarioPaciente == null)
            {
                return RedirectToAction("Index", "Home");
            }
           
            usuarioPaciente.Senha_UsuPaci = Crypto.HashPassword(Senha);

            db.Entry(usuarioPaciente).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Login", "UsuarioPaciente");

        }

        /*  public JsonResult CreateJSON(UsuarioPaciente usuarioPaciente)
          {
              if (ModelState.IsValid)
              {
                  db.UsuarioPacientes.Add(usuarioPaciente);
                  db.SaveChanges();
                  return Json(new { success = true, ID_UsuPaci = usuarioPaciente.Id_UsuPaci }, JsonRequestBehavior.AllowGet);
              }
              return Json(new { success = false, usuarioPaciente }, JsonRequestBehavior.AllowGet);
          } */

        //=========== Altera Senha Paciente JSON =====================//
        // Usuario_UsuPaci     = Usuario Paciente
        // Senha_UsuPaci       = Nova Senha Paciente
        //=============================================================
        public ActionResult EditJSON(string Usuario_UsuPaci, string Senha_UsuPaci)
        {
           
            var usuarioPaciente = db.UsuarioPacientes.Where(e => e.Usuario_UsuPaci == Usuario_UsuPaci).SingleOrDefault();

            usuarioPaciente.Senha_UsuPaci = Senha_UsuPaci;

            ModelState.Clear();
            TryValidateModel(usuarioPaciente);

            if (ModelState.IsValid)
            {
                db.Entry(usuarioPaciente).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "UsuarioPaciente", "EditJSON");
                return Json(new { success = true, Id_PaciConv = usuarioPaciente.Id_UsuPaci }, JsonRequestBehavior.AllowGet);
            }
           
            return Json(new { success = false, usuarioPaciente }, JsonRequestBehavior.AllowGet);
        }


    }
}
