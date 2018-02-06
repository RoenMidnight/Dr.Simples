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
    public class AreaConsultoriosController : Controller
    {
        private DrMedContext db = new DrMedContext();

        // GET: AreaConsultorios
       /* public ActionResult Index()
        {
            var areaConsultorios = db.AreaConsultorios.Include(a => a.Consultorio);
            return View(areaConsultorios.ToList());
        }*/

        // GET: AreaConsultorios/Details/5
    /*    public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaConsultorio areaConsultorio = db.AreaConsultorios.Find(id);
            if (areaConsultorio == null)
            {
                return HttpNotFound();
            }
            return View(areaConsultorio);
        }*/

        // GET: AreaConsultorios/Create
        public ActionResult Create()
        {           
            return View();
        }

        // POST: AreaConsultorios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "ID_Area,Nome_Area,ID_Consu")] AreaConsultorio areaConsultorio)
        {
            areaConsultorio.ID_Consu = Int32.Parse(Request.Cookies["ConsultorioID"].Value);

            ModelState.Clear();
            TryUpdateModel(areaConsultorio);

            if (ModelState.IsValid)
            {
                db.AreaConsultorios.Add(areaConsultorio);
                db.SaveChanges();
                return Json(new { success = true, message = areaConsultorio }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: AreaConsultorios/Edit/5
     /*   public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaConsultorio areaConsultorio = db.AreaConsultorios.Find(id);
            if (areaConsultorio == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Consu = new SelectList(db.Consultorios, "ID_Consu", "Nome_Consu", areaConsultorio.ID_Consu);
            return View(areaConsultorio);
        }*/

        // POST: AreaConsultorios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
 /*       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Area,Nome_Area,ID_Consu")] AreaConsultorio areaConsultorio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(areaConsultorio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Consu = new SelectList(db.Consultorios, "ID_Consu", "Nome_Consu", areaConsultorio.ID_Consu);
            return View(areaConsultorio);
        }
        */
        // GET: AreaConsultorios/Delete/5
      /*  public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AreaConsultorio areaConsultorio = db.AreaConsultorios.Find(id);
            if (areaConsultorio == null)
            {
                return HttpNotFound();
            }
            return View(areaConsultorio);
        }*/

        // POST: AreaConsultorios/Delete/5
     /*   [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AreaConsultorio areaConsultorio = db.AreaConsultorios.Find(id);
            db.AreaConsultorios.Remove(areaConsultorio);
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
    }
}
