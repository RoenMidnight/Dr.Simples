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
using Microsoft.Ajax.Utilities;

namespace AplicacaoMedicina.Controllers
{
    public class MedicoConsultoriosController : Controller
    {
        private DrMedContext db = new DrMedContext();

        // GET: MedicoConsultorios
        public ActionResult Index()
        {
            var medicoConsultorios = db.MedicoConsultorios.Include(m => m.Consultorio).Include(m => m.Medico);
            return View(medicoConsultorios.ToList());
        }

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult MedicosVisitados(string filtro)
        {

            int id = Int32.Parse(Request.Cookies["PacienteID"].Value);

            var consult = db.Consultas.Include(m => m.MedicoConsultorio)
                .Include(y => y.MedicoConsultorio.Consultorio)
                .Include(z => z.MedicoConsultorio.Medico)
                .Where(x => x.ID_Paci == id && 
                            x.Data_Consa < DateTime.Now && 
                            x.Situacao_Consa == "Confirmado")
                .DistinctBy(x => x.MedicoConsultorio);
                
            
            return View(consult.ToList());
        }

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult MedicosFavoritos(string filtro)
        {
            int ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);

            var avalicao = db.Avaliacaos.Include(m => m.MedicoConsultorio)
                .Include(m => m.Paciente)
                .Where(x => x.ID_Paci == ID_Paci && x.favo_Aval);
                    
            return View(avalicao.ToList());
        }

        // GET: MedicoConsultorios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoConsultorio medicoConsultorio = db.MedicoConsultorios.Find(id);
            if (medicoConsultorio == null)
            {
                return HttpNotFound();
            }
            return View(medicoConsultorio);
        }

        // GET: MedicoConsultorios/Create
        public ActionResult Create()
        {
            ViewBag.ID_Consu = new SelectList(db.Consultorios, "ID_Consu", "Nome_Consu");
            ViewBag.ID_Medi = new SelectList(db.Medicos, "ID_Medi", "Nome_Medi");
            return View();
        }

        // POST: MedicoConsultorios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_MediConsu,ID_Medi,ID_Consu,ativo")] MedicoConsultorio medicoConsultorio)
        {
            if (ModelState.IsValid)
            {
                db.MedicoConsultorios.Add(medicoConsultorio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Consu = new SelectList(db.Consultorios, "ID_Consu", "Nome_Consu", medicoConsultorio.ID_Consu);
            ViewBag.ID_Medi = new SelectList(db.Medicos, "ID_Medi", "Nome_Medi", medicoConsultorio.ID_Medi);
            return View(medicoConsultorio);
        }

        // GET: MedicoConsultorios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoConsultorio medicoConsultorio = db.MedicoConsultorios.Find(id);
            if (medicoConsultorio == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Consu = new SelectList(db.Consultorios, "ID_Consu", "Nome_Consu", medicoConsultorio.ID_Consu);
            ViewBag.ID_Medi = new SelectList(db.Medicos, "ID_Medi", "Nome_Medi", medicoConsultorio.ID_Medi);
            return View(medicoConsultorio);
        }

        // POST: MedicoConsultorios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_MediConsu,ID_Medi,ID_Consu,ativo")] MedicoConsultorio medicoConsultorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicoConsultorio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Consu = new SelectList(db.Consultorios, "ID_Consu", "Nome_Consu", medicoConsultorio.ID_Consu);
            ViewBag.ID_Medi = new SelectList(db.Medicos, "ID_Medi", "Nome_Medi", medicoConsultorio.ID_Medi);
            return View(medicoConsultorio);
        }

        // GET: MedicoConsultorios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoConsultorio medicoConsultorio = db.MedicoConsultorios.Find(id);
            if (medicoConsultorio == null)
            {
                return HttpNotFound();
            }
            return View(medicoConsultorio);
        }

        // POST: MedicoConsultorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MedicoConsultorio medicoConsultorio = db.MedicoConsultorios.Find(id);
            db.MedicoConsultorios.Remove(medicoConsultorio);
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
        }
    }
}
