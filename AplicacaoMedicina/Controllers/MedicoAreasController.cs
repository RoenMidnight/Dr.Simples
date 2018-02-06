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
    public class MedicoAreasController : Controller
    {
        private DrMedContext db = new DrMedContext();

        // GET: MedicoAreas
        public ActionResult Index()
        {
            var medicoAreas = db.MedicoAreas.Include(m => m.Area).Include(m => m.Medico);
            return View(medicoAreas.ToList());
        }

        // GET: MedicoAreas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoArea medicoArea = db.MedicoAreas.Find(id);
            if (medicoArea == null)
            {
                return HttpNotFound();
            }
            return View(medicoArea);
        }

        // GET: MedicoAreas/Create
        public ActionResult Create()
        {
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
            ViewBag.ID_Medi = new SelectList(db.Medicos, "ID_Medi", "Nome_Medi");
            return View();
        }

        // POST: MedicoAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_MediArea,ID_Area,ID_Medi")] MedicoArea medicoArea)
        {
            if (ModelState.IsValid)
            {
                db.MedicoAreas.Add(medicoArea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area", medicoArea.ID_Area);
            ViewBag.ID_Medi = new SelectList(db.Medicos, "ID_Medi", "Nome_Medi", medicoArea.ID_Medi);
            return View(medicoArea);
        }

        // GET: MedicoAreas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoArea medicoArea = db.MedicoAreas.Find(id);
            if (medicoArea == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area", medicoArea.ID_Area);
            ViewBag.ID_Medi = new SelectList(db.Medicos, "ID_Medi", "Nome_Medi", medicoArea.ID_Medi);
            return View(medicoArea);
        }

        // POST: MedicoAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_MediArea,ID_Area,ID_Medi")] MedicoArea medicoArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicoArea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area", medicoArea.ID_Area);
            ViewBag.ID_Medi = new SelectList(db.Medicos, "ID_Medi", "Nome_Medi", medicoArea.ID_Medi);
            return View(medicoArea);
        }

        // GET: MedicoAreas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoArea medicoArea = db.MedicoAreas.Find(id);
            if (medicoArea == null)
            {
                return HttpNotFound();
            }
            return View(medicoArea);
        }

        // POST: MedicoAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MedicoArea medicoArea = db.MedicoAreas.Find(id);
            db.MedicoAreas.Remove(medicoArea);
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
