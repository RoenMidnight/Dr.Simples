using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AplicacaoMedicina.DataContexts;
using AplicacaoMedicina.Models;
using System.Web.Security;
using System.Web.Helpers;

namespace AplicacaoMedicina.Controllers
{
    public class UsuarioConsultoriosController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        // GET: UsuarioConsultorios
   /*     public ActionResult Index()
        {
            var usuarioConsultorios = db.UsuarioConsultorios.Include(u => u.Consultorio);
            return View(usuarioConsultorios.ToList());
        }

        // GET: UsuarioConsultorios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioConsultorio usuarioConsultorio = db.UsuarioConsultorios.Find(id);
            if (usuarioConsultorio == null)
            {
                return HttpNotFound();
            }
            return View(usuarioConsultorio);
        } */

        // GET: UsuarioConsultorios/Create
     /*   public ActionResult Create()
        {
            ViewBag.ID_Consu = new SelectList(db.Consultorios, "ID_Consu", "Nome_Consu");
            return View();
        }*/

        // POST: UsuarioConsultorios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
     /*   [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_UsuCons,Usuario_UsuCons,Senha_UsuCons,ID_Consu")] UsuarioConsultorio usuarioConsultorio)
        {
            if (ModelState.IsValid)
            {
                db.UsuarioConsultorios.Add(usuarioConsultorio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Consu = new SelectList(db.Consultorios, "ID_Consu", "Nome_Consu", usuarioConsultorio.ID_Consu);
            return View(usuarioConsultorio);
        }

        // GET: UsuarioConsultorios/Edit/5
        public ActionResult Edit()
        {
            if (Request.Cookies["UserID"].Value == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int id = Int32.Parse(Request.Cookies["UserID"].Value);

            UsuarioConsultorio usuarioConsultorio = db.UsuarioConsultorios.Find(id);
            if (usuarioConsultorio == null)
            {
                return HttpNotFound();
            }
        
            return PartialView(usuarioConsultorio);
        }
        
        // POST: UsuarioConsultorios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_UsuCons,Usuario_UsuCons,Senha_UsuCons,ID_Consu")] UsuarioConsultorio usuarioConsultorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuarioConsultorio).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("/Consultorio/Edit");
            }

            return PartialView(usuarioConsultorio);
        }

        // GET: UsuarioConsultorios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioConsultorio usuarioConsultorio = db.UsuarioConsultorios.Find(id);
            if (usuarioConsultorio == null)
            {
                return HttpNotFound();
            }
            return View(usuarioConsultorio);
        }

        // POST: UsuarioConsultorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioConsultorio usuarioConsultorio = db.UsuarioConsultorios.Find(id);
            db.UsuarioConsultorios.Remove(usuarioConsultorio);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/

        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                return Redirect("/Home/Dashboard");
            }

            return View();
        }

        [HttpPost]
     //   [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioConsultorio model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (DrMedContext entities = new DrMedContext())
                {
                    string user = model.Usuario_UsuCons;
                    string pass = model.Senha_UsuCons;
                    string hashPass = "";

                    var userValid = (from _UsuCons in db.UsuarioConsultorios
                                     where user == _UsuCons.Usuario_UsuCons &&
                                           _UsuCons.Consultorio.Ativo == true
                                     select new 
                                     {
                                         _UsuCons.ID_Consu,
                                         _UsuCons.Senha_UsuCons,
                                         _UsuCons.Consultorio,
                                         _UsuCons.Id_UsuCons
                                     }).FirstOrDefault();

                    if (userValid != null)  {
                        hashPass = userValid.Senha_UsuCons;
                    }
                    
                    if (userValid != null && Crypto.VerifyHashedPassword(hashPass, pass))
                    {

                        FormsAuthentication.SetAuthCookie("cons"+user, true);
                        Response.Cookies["ConsultorioID"].Value = userValid.Consultorio.ID_Consu.ToString();
                        Response.Cookies["ConsultorioID"].Expires = DateTime.Now.AddDays(1);

                        Response.Cookies["UserID"].Value = userValid.Id_UsuCons.ToString();
                        Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(1);

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {                            
                            return Redirect(returnUrl);
                        }
                        else {
                            GeraLogAuditoria("Nulo", "UsuarioMedico", "Login");
                            return RedirectToAction("Dashboard", "Home");
                        }

                    }
                    else {
                        ModelState.AddModelError("", "A senha esta incorreta");
                    }
                }
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            GeraLogAuditoria("Nulo", "UsuarioConsultorios", "LogOff");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Valida(string token) {

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


            Consultorio consultorio = db.Consultorios.Find(id);

            if (consultorio == null)
            {
                return RedirectToAction("Index", "Home");
            }

            consultorio.Ativo = true;

            db.Entry(consultorio).State = EntityState.Modified;
            db.SaveChanges();

            var usuarioConsult = db.UsuarioConsultorios.Where(x => x.ID_Consu == consultorio.ID_Consu).First();

            FormsAuthentication.SetAuthCookie("cons" + consultorio.Email_Consu, true);
            Response.Cookies["ConsultorioID"].Value = consultorio.ID_Consu.ToString();
            Response.Cookies["ConsultorioID"].Expires = DateTime.Now.AddDays(1);

            Response.Cookies["UserID"].Value = usuarioConsult.Id_UsuCons.ToString();
            Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(1);

            return RedirectToAction("New", "Medico");

        }

        public ActionResult RedefinirSenha()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RedefinirSenha(string Email)
        {
            var email = db.UsuarioConsultorios.Where(x => x.Usuario_UsuCons == Email).FirstOrDefault();

            if (email == default(UsuarioConsultorio)){
                View();
            }

            string token = GeraToken(email.Id_UsuCons);

            string body = "Você realizou o pedido de redefinição de sua senha, clique o link abaixo para que seja efetuada. <br/>"+
                "<a href=http://" + System.Web.HttpContext.Current.Request.Url.Host + "/UsuarioConsultorios/Redefine?token=" + token + ">Clique Aqui</a> <br/>"+
                "Atenciosamente, equipe Doutor Simples.";

            EnviaNotificacao(Email,"Redefinir Senha (Doutor Simples)", body);

            return RedirectToAction("Login","UsuarioConsultorios");
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

            UsuarioConsultorio usuarioConsult = db.UsuarioConsultorios.Find(id);

            if (usuarioConsult == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Id_UsuCons = usuarioConsult.Id_UsuCons;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Redefine(int Id_UsuCons, string Senha)
        {
            UsuarioConsultorio usuarioConsult = db.UsuarioConsultorios.Find(Id_UsuCons);

            if (usuarioConsult == null)
            {
                return RedirectToAction("Index", "Home");
            }

            usuarioConsult.Senha_UsuCons = Crypto.HashPassword(Senha);

            db.Entry(usuarioConsult).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Login", "UsuarioConsultorios");

        }


        }
}
