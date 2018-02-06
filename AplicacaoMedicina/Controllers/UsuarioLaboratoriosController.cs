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

namespace AplicacaoMedicina.Controllers
{
    public class UsuarioLaboratoriosController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        // GET: UsuarioLaboratorios
        public ActionResult Index()
        {
            var usuarioLaboratorios = db.UsuarioLaboratorios.Include(u => u.Laboratorio);
            return View(usuarioLaboratorios.ToList());
        }

        // GET: UsuarioLaboratorios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioLaboratorio usuarioLaboratorio = db.UsuarioLaboratorios.Find(id);
            if (usuarioLaboratorio == null)
            {
                return HttpNotFound();
            }
            return View(usuarioLaboratorio);
        }

        // GET: UsuarioLaboratorios/Create
        public ActionResult Create()
        {
            ViewBag.ID_Labo = new SelectList(db.Laboratorios, "ID_Labo", "Nome_Labo");
            return View();
        }

        // POST: UsuarioLaboratorios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_UsuLab,Usuario_UsuLab,Senha_UsuLab,ID_Labo")] UsuarioLaboratorio usuarioLaboratorio)
        {
            if (ModelState.IsValid)
            {
                db.UsuarioLaboratorios.Add(usuarioLaboratorio);
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "UsuarioLaboratorio", "Create");
                return RedirectToAction("Index");
            }

            ViewBag.ID_Labo = new SelectList(db.Laboratorios, "ID_Labo", "Nome_Labo", usuarioLaboratorio.ID_Labo);
            return View(usuarioLaboratorio);
        }

        // GET: UsuarioLaboratorios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioLaboratorio usuarioLaboratorio = db.UsuarioLaboratorios.Find(id);
            if (usuarioLaboratorio == null)
            {
                return HttpNotFound();
            }
            return PartialView(usuarioLaboratorio);
        }

        // POST: UsuarioLaboratorios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_UsuLab,Usuario_UsuLab,Senha_UsuLab,ID_Labo")] UsuarioLaboratorio usuarioLaboratorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuarioLaboratorio).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "UsuarioLaboratorio", "Edit");
                return RedirectToAction("Index");
            }
            ViewBag.ID_Labo = new SelectList(db.Laboratorios, "ID_Labo", "Nome_Labo", usuarioLaboratorio.ID_Labo);
            return View(usuarioLaboratorio);
        }

        // GET: UsuarioLaboratorios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioLaboratorio usuarioLaboratorio = db.UsuarioLaboratorios.Find(id);
            if (usuarioLaboratorio == null)
            {
                return HttpNotFound();
            }
            return View(usuarioLaboratorio);
        }

        // POST: UsuarioLaboratorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioLaboratorio usuarioLaboratorio = db.UsuarioLaboratorios.Find(id);
            db.UsuarioLaboratorios.Remove(usuarioLaboratorio);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "UsuarioLaboratorio", "DeleteConfirmed");
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

        //=================>ACTIONS JSON

        [HttpPost]
        public JsonResult LoginJSon(string Usuario_UsuLab, string Senha_UsuLab, string returnUrl)
        {

            using (DrMedContext entities = new DrMedContext())
            {
                string user = Usuario_UsuLab;
                string pass = Senha_UsuLab;


                var userValid = from _UsuLab in db.UsuarioLaboratorios
                                where user == _UsuLab.Usuario_UsuLab &&
                                      pass == _UsuLab.Senha_UsuLab
                                select new
                                {
                                    _UsuLab.Id_UsuLab,
                                    _UsuLab.Usuario_UsuLab,
                                    _UsuLab.Senha_UsuLab,
                                    _UsuLab.ID_Labo
                                };

                if (userValid != null)
                {
                    FormsAuthentication.SetAuthCookie("labo"+user, false);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Json(new { success = true, Usuario = userValid.ToList() }, JsonRequestBehavior.AllowGet);
                    }
                    else {

                        GeraLogAuditoria(Usuario_UsuLab, "UsuarioLaboratorio", "LoginJSON");
                        return Json(new { success = true, Usuario = userValid.ToList() }, JsonRequestBehavior.AllowGet);
                    }
                }
                else {
                    return Json(false);
                }
            }

        }

        public ActionResult LogOff()
        {
            GeraLogAuditoria("Nulo", "UsuarioMedico", "LogOff");
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public JsonResult LogOffJson()
        {
            GeraLogAuditoria("Nulo", "UsuarioLaboratorio", "LogOffJson");
            FormsAuthentication.SignOut();
            return Json(true);
        }

        //=========== Altera Senha Medico JSON =====================//
        // Usuario_UsuLab    = Usuario Laboratorio
        // Senha_UsuLab      = Nova Senha Laboratorio
        //=============================================================
        public ActionResult EditJSON(string Usuario_UsuLab, string Senha_UsuLab)
        {

            var usuarioLaboratorio = db.UsuarioLaboratorios.Where(e => e.Usuario_UsuLab == Usuario_UsuLab).SingleOrDefault();

            usuarioLaboratorio.Senha_UsuLab = Senha_UsuLab;

            ModelState.Clear();
            TryValidateModel(usuarioLaboratorio);

            if (ModelState.IsValid)
            {
                db.Entry(usuarioLaboratorio).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "UsuarioLaboratorio", "EditJSON");
                return Json(new { success = true, Id_UsuMedi = usuarioLaboratorio.Id_UsuLab }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, usuarioLaboratorio }, JsonRequestBehavior.AllowGet);
        }

    }
}
