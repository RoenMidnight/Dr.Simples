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
    public class AvaliacaosController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        // GET: Avaliacaos/Create
        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Avaliacaos(int? id)
        {
            int pacId = Int32.Parse(Request.Cookies["PacienteID"].Value);

            var aval = db.Avaliacaos.Where(x => x.ID_MediConsu == id    &&
                                                x.ID_Paci      == pacId &&
                                               (x.rate_Aval     > 0     &&
                                                x.review_Aval  != null
                                               )).FirstOrDefault();

            if (aval != default(Avaliacao)) {
                ViewBag.Aval = "true";

                return PartialView(aval);

            } else {
                ViewBag.ID_MediConsu = id;
                ViewBag.ID_Paci = pacId;
                ViewBag.Aval = "false";

                return PartialView(new Avaliacao());

                /*ViewBag.ID_MediConsu = id;
                ViewBag.ID_Paci = pacId;

                Avaliacao avalicao = new Avaliacao();

                avalicao.ID_Aval = aval.ID_Aval;
                avalicao.ID_MediConsu = aval.ID_MediConsu;
                avalicao.ID_Paci = aval.ID_Paci;
                avalicao.favo_Aval = aval.favo_Aval;
                avalicao.rate_Aval = aval.rate_Aval;
                avalicao.review_Aval = aval.review_Aval;*/
               
            }
           
        }

        // POST: Avaliacaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Avaliacaos(Avaliacao avaliacao)
        {
            var aval = db.Avaliacaos.Where(x => x.ID_MediConsu == avaliacao.ID_MediConsu &&
                                           x.ID_Paci == avaliacao.ID_Paci).FirstOrDefault();
            
            if (aval != default(Avaliacao)) {
                                
                if (ModelState.IsValid)
                {
                    db.Avaliacaos.Add(avaliacao);
                    db.SaveChanges();
                    return Redirect("/MedicoConsultorios/MedicosVisitados");
                }

                ViewBag.ID_MediConsu = avaliacao.ID_MediConsu;
                ViewBag.ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);
                return View(avaliacao);
            } else {

                if (ModelState.IsValid)
                {
                    avaliacao.ID_Aval = aval.ID_Aval;

                    db.Entry(aval).State = EntityState.Detached;
                    db.Entry(avaliacao).State = EntityState.Modified;
                    
                    db.SaveChanges();
                    return Redirect("/MedicoConsultorios/MedicosVisitados");
                }
                ViewBag.ID_MediConsu = avaliacao.ID_MediConsu;
                ViewBag.ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);
                return View(avaliacao);
            }
        }

        // GET: Avaliacaos/Edit/5
        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public JsonResult Favoritar(int id)
        {
            int ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);

            var aval = db.Avaliacaos.Where(x => x.ID_MediConsu == id ).FirstOrDefault();                        
           
            if (aval == null)
            {
                Avaliacao avaliacao = new Avaliacao();

                avaliacao.ID_MediConsu = id;
                avaliacao.ID_Paci = ID_Paci;
                avaliacao.favo_Aval = true;

                if (ModelState.IsValid)
                {
                    db.Avaliacaos.Add(avaliacao);
                    db.SaveChanges();
                    return Json(new{ success = true }, JsonRequestBehavior.AllowGet);
                   // return Redirect("/MedicoConsultorios/MedicosVisitados");
                }

                ViewBag.ID_MediConsu = id;
                ViewBag.ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                //  return View(avaliacao);
            } else {
                Avaliacao avaliacao = new Avaliacao();

                avaliacao.ID_Aval = aval.ID_Aval;
                avaliacao.ID_MediConsu = id;
                avaliacao.ID_Paci = ID_Paci;
                avaliacao.favo_Aval = true;
                avaliacao.rate_Aval = aval.rate_Aval;
                avaliacao.review_Aval = aval.review_Aval;

                ModelState.Clear();
                TryValidateModel(avaliacao);

                if (ModelState.IsValid)
                {
                    db.Entry(aval).State = EntityState.Detached;
                    db.Entry(avaliacao).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    //   return Redirect("/MedicoConsultorios/MedicosVisitados");
                }
                ViewBag.ID_MediConsu = id;
                ViewBag.ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                //   return View(avaliacao);
            }
        }

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public JsonResult RemoverFav(int id)
        {
            int ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);

            var aval = db.Avaliacaos.Where(x => x.ID_MediConsu == id).FirstOrDefault();

            if (aval == null)
            {
                Avaliacao avaliacao = new Avaliacao();

                avaliacao.ID_MediConsu = id;
                avaliacao.ID_Paci = ID_Paci;
                avaliacao.favo_Aval = false;

                if (ModelState.IsValid)
                {
                    db.Avaliacaos.Add(avaliacao);
                    db.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    // return Redirect("/MedicoConsultorios/MedicosVisitados");
                }

                ViewBag.ID_MediConsu = id;
                ViewBag.ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                //  return View(avaliacao);
            }
            else {
                Avaliacao avaliacao = new Avaliacao();

                avaliacao.ID_Aval = aval.ID_Aval;
                avaliacao.ID_MediConsu = id;
                avaliacao.ID_Paci = ID_Paci;
                avaliacao.favo_Aval = false;
                avaliacao.rate_Aval = aval.rate_Aval;
                avaliacao.review_Aval = aval.review_Aval;

                ModelState.Clear();
                TryValidateModel(avaliacao);

                if (ModelState.IsValid)
                {
                    db.Entry(aval).State = EntityState.Detached;
                    db.Entry(avaliacao).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    //   return Redirect("/MedicoConsultorios/MedicosVisitados");
                }
                ViewBag.ID_MediConsu = id;
                ViewBag.ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                //   return View(avaliacao);
            }
        }


    }
}
