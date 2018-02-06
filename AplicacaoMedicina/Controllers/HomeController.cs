using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AplicacaoMedicina.DataContexts;
using AplicacaoMedicina.Models;
using Microsoft.Ajax.Utilities;
using AplicacaoMedicina.Helper;
using System.Collections.Generic;

namespace AplicacaoMedicina.Controllers
{
    public class HomeController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        public ActionResult Index()
        {
            ViewBag.ID_Area = new SelectList(db.Areas, "ID_Area", "Nome_Area");
            ViewBag.ID_Conv = new SelectList(db.Convenios, "ID_Conv", "Nome_Conv");

            return View();
        }

        public ActionResult IndexMed()
        {
            return View();
        }

        public JsonResult GetAreas()
        {
            var areas = from item in db.Areas
                        select item.Nome_Area;

            List<string> jSonArea = areas.ToList();

        /*    var jSonArea = (from item in areas
                            select new
                            {
                                item.Nome_Area
                            }).ToList();*/


            return Json(jSonArea, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConv()
        {
            var conven = from item in db.Convenios
                         select item.Nome_Conv;

            List<string> jSonConv = conven.ToList();
           /* var jSonConv = (from item in conven
                            select new
                            {
                                item.Nome_Conv
                            }).ToList();*/


            return Json(jSonConv, JsonRequestBehavior.AllowGet);
        }


        public JsonResult EnviaPedido(string Nome, string Email, string Telefone, string Subject, string Body)
        {

            Body = Nome + " - " + Email + "("+ Telefone +")" +"<br/>" + Body;

            try {
                EnviaNotificacao("suporte@doutorsimples.com.br", Subject, Body);
            } catch {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult Dashboard() {
            int ID_Consu = Int32.Parse(Request.Cookies["ConsultorioID"].Value);
            
            var consultas = (db.Consultas
               //     .Join(db.ConsultorioConvenios, x => x.MedicoConsultorio.Consultorio.ID_Consu,
                 //                                  p => p.Consultorio.ID_Consu,
                   //       (x, p) => new PaciDash { Consulta = x, ConsultorioConvenio = p })                
                
                .Where(x => x.MedicoConsultorio.Consultorio.ID_Consu == ID_Consu)).OrderBy(x => x.Data_Consa).ToList();
            
            var medNames = consultas
              .Where(x => x.MedicoConsultorio.Consultorio.ID_Consu == ID_Consu &&
                          x.Situacao_Consa == "Confirmado")
                          .OrderBy(x => x.ID_MediConsu)
                          .DistinctBy(x => x.ID_MediConsu)
              .Select(x => x.MedicoConsultorio.Medico.Nome_Medi).ToList();

            ViewBag.NumMedi = medNames.Count();
            ViewBag.Medicos = medNames;

            var aval = db.Avaliacaos.Where(x => x.MedicoConsultorio.Consultorio.ID_Consu == ID_Consu &&
                                                x.rate_Aval != 0);
            int avaliacao;

            if (aval.FirstOrDefault() == default(Avaliacao))  {
                avaliacao = 0;
            } else {
                avaliacao = (aval.Sum(x => x.rate_Aval) / aval.Count());
            }

            ViewBag.Avaliacao = avaliacao;

            ViewBag.NovosAGen = consultas.Where(x => x.Situacao_Consa == "Em Espera").Count();

            ViewBag.PaciToday = consultas.Where(x => x.Situacao_Consa == "Confirmado" &&
                                                    (x.Data_Consa.Year  == DateTime.Today.Year  &&
                                                     x.Data_Consa.Month == DateTime.Today.Month &&
                                                     x.Data_Consa.Day   == DateTime.Today.Day) 
            ).Count();                       

            //SendMessage("TESTE");

            return View(consultas);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Sucesso()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}