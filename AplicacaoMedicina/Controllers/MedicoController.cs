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
using AplicacaoMedicina.Helper;

namespace AplicacaoMedicina.Controllers
{
    public class MedicoController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        public JsonResult GetJSON()
        {
            var medicos = db.Medicos;
            return Json(medicos.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Medico
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult Index(string filtro)
        {
            IQueryable<MedicoConsultorio> medicos;

            if (!String.IsNullOrEmpty(filtro)) {

                int consultID = Int32.Parse(Request.Cookies["ConsultorioID"].Value);
                ViewBag.Filtro = filtro;
                bool ativo = false;

                if (filtro == "Ativo") { ativo = true; }

                medicos = db.MedicoConsultorios.Include(x => x.Medico)
                    .Include(x => x.Consultorio)
                    .Where(x => x.ativo == ativo &&
                           x.Consultorio.ID_Consu == consultID);             
            } else {

                ViewBag.Filtro = "Ambos";
                int consultID = Int32.Parse(Request.Cookies["ConsultorioID"].Value);

                medicos = db.MedicoConsultorios.Include(x => x.Medico)
                    .Include(x => x.Consultorio)
                    .Where(x => x.Consultorio.ID_Consu == consultID);
            }

            return View(medicos.ToList());
        }

        // GET: Medico/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medico medico = db.Medicos.Find(id);
            if (medico == null)
            {
                return HttpNotFound();
            }
            return View(medico);
        }

        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult New()
        {
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult New([Bind(Include = "ID_Medi,Nome_Medi,CRM_Medi,Email_Medi,Telefone_Medi,TipoInscri_Medi,ID_Area, ID_Conv, Valor_Medi")] Medico medico, int ID_Area, string ID_Conv)
        {

            int id = Int32.Parse(Request.Cookies["ConsultorioID"].Value);

            var testaCRMCPF = db.MedicoConsultorios.Where(x =>
                                            (x.Medico.CRM_Medi == medico.CRM_Medi) 
                                             ).FirstOrDefault();

            if (testaCRMCPF != default(MedicoConsultorio)) {
                medico.CRM_Medi = "";

                ModelState.Clear();
                TryValidateModel(medico);

                ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
                return View(medico);
            }

            if (ModelState.IsValid)
            {
                if (testaCRMCPF == default(MedicoConsultorio)) {
                    db.Medicos.Add(medico);
                }                

                MedicoConsultorio medCons = new MedicoConsultorio();
                medCons.ID_Medi = medico.ID_Medi;
                medCons.ID_Consu = Int32.Parse(Request.Cookies["ConsultorioID"].Value);
                medCons.ativo = false;

                db.MedicoConsultorios.Add(medCons);

                MedicoArea medArea = new MedicoArea();
                medArea.ID_Area = ID_Area;
                medArea.ID_Medi = medico.ID_Medi;

                db.MedicoAreas.Add(medArea);

                string[] convArray = ID_Conv.Split(',');

                foreach(var item in convArray) {

                    int convName = Int32.Parse(item);

                    var conven = db.Convenios.Where(x => x.ID_Conv == convName ).FirstOrDefault();

                    if (conven != default(Convenio)) {
                        MedicoConvenio medConv = new MedicoConvenio();
                        medConv.ID_Conv = conven.ID_Conv;
                        medConv.ID_Medi = medico.ID_Medi;

                        db.MedicoConvenios.Add(medConv);
                    }
                }
                
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Medico", "Create");

                if (Request.Form["Save"] != null)
                {
                    return Redirect("/Agenda/Index/" + medCons.ID_MediConsu + "?redirect=Save");
                }

                if (Request.Form["MultipleSave"] != null)
                {
                    return Redirect("/Agenda/Index/"+medCons.ID_MediConsu+"?redirect=MultipleSave");
                   // return View();
                }
                
            } else {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
                return View(medico);

            }
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
            return View(medico);
            // ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
        }

        // GET: Medico/Create
        public ActionResult Create()
        {                  
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");

            return PartialView();
        }

        // POST: Medico/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "ID_Medi,Nome_Medi,CRM_Medi,Email_Medi,Telefone_Medi,TipoInscri_Medi,ID_Area,ID_Conv,Valor_Medi")] Medico medico, int ID_Area, string ID_Conv)
        {

            int id = Int32.Parse(Request.Cookies["ConsultorioID"].Value);

            var testaCRMCPF = db.MedicoConsultorios.Where(x => 
                                            (x.Medico.CRM_Medi == medico.CRM_Medi ) 
                                             ).FirstOrDefault();
            
            if (testaCRMCPF != null) {


                List<string> errorList = new List<string>();

                if (testaCRMCPF.Medico.CRM_Medi != "") { errorList.Add("CRM_Medi"); }              
                //     ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
                return Json(new { success = false, error = errorList.ToList() }, JsonRequestBehavior.AllowGet);
            }


            if (ModelState.IsValid)
            {
                db.Medicos.Add(medico);                

                MedicoConsultorio medCons = new MedicoConsultorio();
                medCons.ID_Medi = medico.ID_Medi;
                medCons.ID_Consu = Int32.Parse(Request.Cookies["ConsultorioID"].Value);
                medCons.ativo = false;

                db.MedicoConsultorios.Add(medCons);

                MedicoArea medArea = new MedicoArea();
                medArea.ID_Area = ID_Area;
                medArea.ID_Medi = medico.ID_Medi;

                db.MedicoAreas.Add(medArea);

                string[] convArray = ID_Conv.Split(',');

                foreach (var item in convArray)
                {

                    int convName = Int32.Parse(item);

                    var conven = db.Convenios.Where(x => x.ID_Conv == convName).FirstOrDefault();

                    if (conven != default(Convenio))
                    {
                        MedicoConvenio medConv = new MedicoConvenio();
                        medConv.ID_Conv = conven.ID_Conv;
                        medConv.ID_Medi = medico.ID_Medi;

                        db.MedicoConvenios.Add(medConv);
                    }
                }                

                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Medico", "Create");

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            } else {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = true, error = errorList }, JsonRequestBehavior.AllowGet);

            }

           // ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
            
        }

        // GET: Medico/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoConsultorio medCons = db.MedicoConsultorios.Find(id);
            if (medCons == null)
            {
                return HttpNotFound();
            }

            ViewBag.Conveni = db.MedicoConvenios.Where(x => x.ID_Medi == medCons.ID_Medi).ToList();
                        
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
            return PartialView(medCons);
        }

        // POST: Medico/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicoConsultorio medCons, string ID_Conv)
        {
            
            ModelState.Clear();
            TryValidateModel(medCons);

            if (ModelState.IsValid)
            {
                db.Entry(medCons).State = EntityState.Modified;
               
                Medico medico = db.Medicos.Find(medCons.ID_Medi);
                medico.Nome_Medi = medCons.Medico.Nome_Medi;
                medico.Telefone_Medi = medCons.Medico.Telefone_Medi;
                medico.TipoInscri_Medi = medCons.Medico.TipoInscri_Medi;              
                medico.CRM_Medi = medCons.Medico.CRM_Medi;
                medico.Email_Medi = medCons.Medico.Email_Medi;
                medico.TipoInscri_Medi = medCons.Medico.TipoInscri_Medi;
                medico.Valor_Medi = medCons.Medico.Valor_Medi;

                db.SaveChanges();

                db.Entry(medico).State = EntityState.Modified;

                db.SaveChanges();

                db.MedicoConvenios.RemoveRange(db.MedicoConvenios.Where(x => x.ID_Medi == medico.ID_Medi));

                string[] convArray = ID_Conv.Split(',');
                     
                foreach (var item in convArray)
                {
                    int convName = Int32.Parse(item);

                    var conven = db.Convenios.Where(x => x.ID_Conv == convName).FirstOrDefault();

                    if (conven != default(Convenio))
                    {
                        MedicoConvenio medConv = new MedicoConvenio();
                        medConv.ID_Conv = conven.ID_Conv;
                        medConv.ID_Medi = medico.ID_Medi;

                        db.MedicoConvenios.Add(medConv);   
                    }                    
                }

                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Medico", "Edit");
                return RedirectToAction("Index");
            } 
            
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
            return PartialView(medCons);
        }

        // GET: Medico/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medico medico = db.Medicos.Find(id);
            if (medico == null)
            {
                return HttpNotFound();
            }
            return View(medico);
        }

        // POST: Medico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Medico medico = db.Medicos.Find(id);
            db.Medicos.Remove(medico);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "Medico", "Delete");
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

        public JsonResult GetConvenios()
        {
            var conven = db.Convenios.ToList();

            var jSonConv = (from item in conven
                            select new
                            {
                                value = item.ID_Conv,
                                text = item.Nome_Conv                                
                            }).ToList();


            return Json( jSonConv , JsonRequestBehavior.AllowGet);
        }

        //========= Edita um Medico por JSON ============//

        public JsonResult EditJSON(Medico medico)
        {
            if (ModelState.IsValid)
            {              

                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Medico", "EditJSON");
                return Json(new { success = true, ID_Medi = medico.ID_Medi }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, medico }, JsonRequestBehavior.AllowGet);
        }


        //================Pega informação de um Medico específico ====================================

        public JsonResult GetMedico(int? ID_Medi, string CPF_Medi)
        {

            var medico = from _Medico in db.Medicos
                           where ID_Medi == _Medico.ID_Medi
                           select new { _Medico };

            

            return Json(new { success = true, Medico = medico.ToList() }, JsonRequestBehavior.AllowGet);
        }

        //========= Cria Paciente por JSON ============//
        // Cria um Paciente e Usuario Paciente
        //
        // Paciente = Objeto Paciente
        // Senha_UsuMedi = Senha do Paciente
        //===============================================

        public JsonResult CreateJSON(Medico medico, string Senha_UsuMedi)
        {
            if (ModelState.IsValid)
            {               

                GeraLogAuditoria("Nulo", "Medico", "CreateJSON");
                return Json(new
                {
                    success = true,
                    ID_Medi = medico.ID_Medi
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, medico }, JsonRequestBehavior.AllowGet);
        }
    }
}
