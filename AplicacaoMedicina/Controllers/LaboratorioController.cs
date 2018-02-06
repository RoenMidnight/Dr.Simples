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

namespace AplicacaoMedicina.Controllers
{
    public class LaboratorioController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        public JsonResult GetJSON()
        {
            return Json(db.Laboratorios.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Laboratorio
        public ActionResult Index()
        {
            return View(db.Laboratorios.ToList());
        }

        // GET: Laboratorio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Laboratorio laboratorio = db.Laboratorios.Find(id);
            if (laboratorio == null)
            {
                return HttpNotFound();
            }
            return View(laboratorio);
        }

        // GET: Laboratorio/Create
        public ActionResult Create()
        {
            ViewBag.Id_Tipo = new SelectList(db.Tipos, "Id_Tipo", "Nome_Tipo");
            return View();
        }

        // POST: Laboratorio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Labo,Nome_Labo,CNPJ_Labo,CEP_Labo,Endereco_Labo,Numero_Labo,Complemento_Labo,Bairro_Labo,Cidade_Labo,Estado_Labo,Responsavel_Labo,Telefone_Labo,Email_Labo,Id_Tipo")] Laboratorio laboratorio)
        {
            if (ModelState.IsValid)
            {
                db.Laboratorios.Add(laboratorio);
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Laboratorio", "Create");
                return RedirectToAction("Index");
            }

            return View(laboratorio);
        }

        // GET: Laboratorio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Laboratorio laboratorio = db.Laboratorios.Find(id);
            if (laboratorio == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Tipo = new SelectList(db.Tipos, "Id_Tipo", "Nome_Tipo");
            return View(laboratorio);
        }

        // POST: Laboratorio/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Labo,Nome_Labo,CNPJ_Labo,CEP_Labo,Endereco_Labo,Numero_Labo,Complemento_Labo,Bairro_Labo,Cidade_Labo,Estado_Labo,Responsavel_Labo,Telefone_Labo,Email_Labo, Id_Tipo")] Laboratorio laboratorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(laboratorio).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Laboratorio", "Edit");
                return RedirectToAction("Index");
            }
            return View(laboratorio);
        }

        // GET: Laboratorio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Laboratorio laboratorio = db.Laboratorios.Find(id);
            if (laboratorio == null)
            {
                return HttpNotFound();
            }
            return View(laboratorio);
        }

        // POST: Laboratorio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Laboratorio laboratorio = db.Laboratorios.Find(id);
            db.Laboratorios.Remove(laboratorio);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "Laboratorio", "DeleteConfirmed");
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

        //JSON

        public JsonResult GetLaboratorio(string Cidade, string Nome)
        {
            var laboratorio = from _Laboratorio in db.Laboratorios                                      
                              where 
                                _Laboratorio.Cidade_Labo.Contains(Cidade) &&
                                _Laboratorio.Nome_Labo.Contains(Nome)     
                           select new
                           {
                               _Laboratorio.ID_Labo,     _Laboratorio.Nome_Labo,
                               _Laboratorio.CNPJ_Labo,   _Laboratorio.CEP_Labo, _Laboratorio.Endereco_Labo,
                               _Laboratorio.Numero_Labo, _Laboratorio.Complemento_Labo, _Laboratorio.Bairro_Labo,
                               _Laboratorio.Cidade_Labo, _Laboratorio.Estado_Labo, _Laboratorio.Responsavel_Labo,
                               _Laboratorio.Email_Labo,                              
                           };

            return Json(new { success = true, Laboratorio = laboratorio.ToList() }, JsonRequestBehavior.AllowGet);
        }

        //========= Cria um Laboratorio por JSON ============//
        // Cria um Laboratorio e Usuario Laboratorio
        //
        // Laboratorio = Objeto Paciente
        // Senha_UsuLab = Senha do Laboratorio
        //===============================================

        public JsonResult CreateJSON(Laboratorio laboratorio, string Senha_UsuLab)
        {
            if (Senha_UsuLab == "")
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                db.Laboratorios.Add(laboratorio);

                UsuarioLaboratorio usuarioLaboratorio = new UsuarioLaboratorio();

                usuarioLaboratorio.Usuario_UsuLab = laboratorio.Email_Labo;
                usuarioLaboratorio.Senha_UsuLab = Senha_UsuLab;
                usuarioLaboratorio.ID_Labo = laboratorio.ID_Labo;

                db.UsuarioLaboratorios.Add(usuarioLaboratorio);
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Laboratorio", "CreateJSON");
                return Json(new
                {
                    success = true,
                    ID_Labo = laboratorio.ID_Labo,
                    Id_UsuLab = usuarioLaboratorio.Id_UsuLab
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, laboratorio }, JsonRequestBehavior.AllowGet);
        }

    }
}
