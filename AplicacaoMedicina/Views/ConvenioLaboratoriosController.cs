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

namespace AplicacaoMedicina.Views
{
    public class ConvenioLaboratoriosController : Controller
    {
        private DrMedContext db = new DrMedContext();

        // GET: ConvenioLaboratorios
        public ActionResult Index()
        {
            var convenioLaboratorios = db.ConvenioLaboratorios.Include(c => c.Convenio).Include(c => c.Laboratorio);
            return View(convenioLaboratorios.ToList());
        }

        // GET: ConvenioLaboratorios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConvenioLaboratorio convenioLaboratorio = db.ConvenioLaboratorios.Find(id);
            if (convenioLaboratorio == null)
            {
                return HttpNotFound();
            }
            return View(convenioLaboratorio);
        }

        // GET: ConvenioLaboratorios/Create
        public ActionResult Create()
        {
            ViewBag.ID_Conv = new SelectList(db.Convenios, "ID_Conv", "Nome_Conv");
            ViewBag.ID_Labo = new SelectList(db.Laboratorios, "ID_Labo", "Nome_Labo");
            return View();
        }

        // POST: ConvenioLaboratorios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_ConvLabo,ID_Labo,ID_Conv,ativo")] ConvenioLaboratorio convenioLaboratorio)
        {
            if (ModelState.IsValid)
            {
                db.ConvenioLaboratorios.Add(convenioLaboratorio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Conv = new SelectList(db.Convenios, "ID_Conv", "Nome_Conv", convenioLaboratorio.ID_Conv);
            ViewBag.ID_Labo = new SelectList(db.Laboratorios, "ID_Labo", "Nome_Labo", convenioLaboratorio.ID_Labo);
            return View(convenioLaboratorio);
        }

        // GET: ConvenioLaboratorios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConvenioLaboratorio convenioLaboratorio = db.ConvenioLaboratorios.Find(id);
            if (convenioLaboratorio == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Conv = new SelectList(db.Convenios, "ID_Conv", "Nome_Conv", convenioLaboratorio.ID_Conv);
            ViewBag.ID_Labo = new SelectList(db.Laboratorios, "ID_Labo", "Nome_Labo", convenioLaboratorio.ID_Labo);
            return View(convenioLaboratorio);
        }

        // POST: ConvenioLaboratorios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_ConvLabo,ID_Labo,ID_Conv,ativo")] ConvenioLaboratorio convenioLaboratorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(convenioLaboratorio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Conv = new SelectList(db.Convenios, "ID_Conv", "Nome_Conv", convenioLaboratorio.ID_Conv);
            ViewBag.ID_Labo = new SelectList(db.Laboratorios, "ID_Labo", "Nome_Labo", convenioLaboratorio.ID_Labo);
            return View(convenioLaboratorio);
        }

        // GET: ConvenioLaboratorios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConvenioLaboratorio convenioLaboratorio = db.ConvenioLaboratorios.Find(id);
            if (convenioLaboratorio == null)
            {
                return HttpNotFound();
            }
            return View(convenioLaboratorio);
        }

        // POST: ConvenioLaboratorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConvenioLaboratorio convenioLaboratorio = db.ConvenioLaboratorios.Find(id);
            db.ConvenioLaboratorios.Remove(convenioLaboratorio);
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
