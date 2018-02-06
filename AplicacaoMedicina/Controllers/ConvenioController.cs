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
    public class ConvenioController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        // GET: Convenio
        public ActionResult Index()
        {
            return View(db.Convenios.ToList());
        }

        // GET: Convenio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Convenio convenio = db.Convenios.Find(id);
            if (convenio == null)
            {
                return HttpNotFound();
            }
            return View(convenio);
        }

        // GET: Convenio/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Convenio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Conv,Nome_Conv")] Convenio convenio)
        {
            if (ModelState.IsValid)
            {
                db.Convenios.Add(convenio);
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Convenio", "Create");
                return RedirectToAction("Index");
            }

            return View(convenio);
        }

        // GET: Convenio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Convenio convenio = db.Convenios.Find(id);
            if (convenio == null)
            {
                return HttpNotFound();
            }
            return View(convenio);
        }

        // POST: Convenio/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Conv,Nome_Conv")] Convenio convenio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(convenio).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Convenio", "Edit");
                return RedirectToAction("Index");
            }
            return View(convenio);
        }

        // GET: Convenio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Convenio convenio = db.Convenios.Find(id);
            if (convenio == null)
            {
                return HttpNotFound();
            }
            return View(convenio);
        }

        // POST: Convenio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Convenio convenio = db.Convenios.Find(id);
            db.Convenios.Remove(convenio);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "Convenio", "DeleteConfirmed");
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

        //ACTIONS JSON

        public JsonResult GetConvenio()
        {
            var convenios = from _Convenio in db.Convenios
                            select new
                            {
                                _Convenio.ID_Conv,
                                _Convenio.Nome_Conv
                            };

            GeraLogAuditoria("Nulo", "Convenio", "GetConvenio");
            return Json(new { success = true, Convenio = convenios.ToList() }, JsonRequestBehavior.AllowGet);
        }
    }
}
