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
    public class AreaController : MasterController
    {
        private DrMedContext db = new DrMedContext();
        // GET: Area
        /*       public ActionResult Index()
               {
                   return View(db.Areas.ToList());
               }*/

        // GET: Area/Details/5
        /*    public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Area area = db.Areas.Find(id);
                if (area == null)
                {
                    return HttpNotFound();
                }
                return View(area);
            }
            */
        // GET: Area/Create
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Area/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public JsonResult Create([Bind(Include = "ID_Area, Nome_Area")] Area area)
        {
            if (ModelState.IsValid)
            {
                GeraLogAuditoria("Nulo", "Area", "Create");
                db.Areas.Add(area);
                db.SaveChanges();
                return Json(new { success = true, message = area }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Area/Edit/5
       /* public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Area area = db.Areas.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }
            return View(area);
        }

        // POST: Area/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Area, Nome_Area")] Area area)
        {
            if (ModelState.IsValid)
            {
                GeraLogAuditoria("Nulo", "Area", "Edit");
                db.Entry(area).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(area);
        }

        // GET: Area/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Area area = db.Areas.Find(id);
            if (area == null)
            {
                return HttpNotFound();
            }
            return View(area);
        }

        // POST: Area/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GeraLogAuditoria("Nulo", "Area", "DeleteConfirmed");

            Area area = db.Areas.Find(id);
            db.Areas.Remove(area);
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

        //Ações JSON
        public JsonResult GetAreas()
        {
            var area = from _Area in db.Areas
                           select new
                           {
                              _Area.ID_Area,
                              _Area.Nome_Area
                           };

            return Json(new { success = true, Area = area.ToList() }, JsonRequestBehavior.AllowGet);
        } */
    }
}
