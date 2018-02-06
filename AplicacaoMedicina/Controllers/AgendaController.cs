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
    public class AgendaController : Controller
    {
        private DrMedContext db = new DrMedContext();

        // GET: Agenda
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult Index(int id, string redirect)
        {
            var medico = db.MedicoConsultorios.Find(id);

            if (medico == null) {
                return HttpNotFound();
            }

            ViewBag.ID_MediConsul = id;
            ViewBag.NomeMedico = medico.Medico.Nome_Medi;

            ViewBag.Redirect = redirect;
            //ViewBag.ConsultorioID = Int32.Parse(Request.Cookies["ConsultorioID"].Value);
            return View();
        }

        public JsonResult GetEvents(int? ID_MediConsul)
        {
            var agendas = db.Agendas.Where(x => x.ID_MediConsul == ID_MediConsul);

            var events = new List<object>();
            DateTime Data;
            string startDate;
            string endDate;

            foreach (var x in agendas) {

                switch(x.DSeman_Agenda){

                case "Segunda":
                        Data = DateTime.Parse("2017-07-10"+ " " + x.Data_Agenda.ToString("HH:mm"));
                        startDate = Data.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                        endDate = Data.AddMinutes(30).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

                        events.Add(new { allday = "", title = "Consulta", /*id = x.ID_Agenda,*/ start = startDate, end = endDate,  overlap = false, durationEditable = false });
                        break;
                case "Terca":
                        Data = DateTime.Parse("2017-07-11" + " " + x.Data_Agenda.ToString("HH:mm"));
                        startDate = Data.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                        endDate = Data.AddMinutes(30).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

                        events.Add(new { allday = "", title = "Consulta", /*id = x.ID_Agenda,*/ start = startDate, end = endDate,  overlap = false, durationEditable = false });

                        break;
                case "Quarta":
                        Data = DateTime.Parse("2017-07-12" + " " + x.Data_Agenda.ToString("HH:mm"));
                        startDate = Data.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                        endDate = Data.AddMinutes(30).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

                        events.Add(new { allday = "", title = "Consulta", /*id = x.ID_Agenda,*/ start = startDate, end = endDate,  overlap = false, durationEditable = false });

                        break;
                case "Quinta":
                        Data = DateTime.Parse("2017-07-13" + " " + x.Data_Agenda.ToString("HH:mm"));
                        startDate = Data.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                        endDate = Data.AddMinutes(30).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

                        events.Add(new { allday = "", title = "Consulta", /*id = x.ID_Agenda,*/ start = startDate, end = endDate,  overlap = false, durationEditable = false });

                        break;
                case "Sexta":
                        Data = DateTime.Parse("2017-07-14" + " " + x.Data_Agenda.ToString("HH:mm"));
                        startDate = Data.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                        endDate = Data.AddMinutes(30).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

                        events.Add(new { allday = "", title = "Consulta", /*id = x.ID_Agenda,*/ start = startDate, end = endDate,  overlap = false, durationEditable = false });

                        break;
                case "Sabado":
                        Data = DateTime.Parse("2017-07-15" + " " + x.Data_Agenda.ToString("HH:mm"));
                        startDate = Data.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                        endDate = Data.AddMinutes(30).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

                        events.Add(new { allday = "", title = "Consulta", /*id = x.ID_Agenda,*/ start = startDate, end = endDate,  overlap = false, durationEditable = false });

                        break;
                default:
                        Data = DateTime.Parse("2017-07-09" + " " + x.Data_Agenda.ToString("HH:mm"));
                        startDate = Data.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
                        endDate = Data.AddMinutes(30).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");

                        events.Add(new { allday = "", title = "Consulta", /*id = x.ID_Agenda,*/ start = startDate, end = endDate,  overlap = false, durationEditable = false });

                        break;
                }
                                                           
            }

            return Json(events, JsonRequestBehavior.AllowGet);

        }

        // POST: Agenda/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public JsonResult Create(List<FCalendar> events, int ID_MediConsul, string Redirect)
        {

            db.Agendas.RemoveRange(db.Agendas.Where(x => x.ID_MediConsul == ID_MediConsul));
                       
            string dayOfWeek;
                       
            foreach (var item in events)
            { 
                Agenda agenda = new Agenda();
                agenda.ID_MediConsul = ID_MediConsul;
                agenda.Data_Agenda = DateTime.Parse(item.start.Substring(0,19));

                dayOfWeek = item.start.Substring(0,10);

                switch (dayOfWeek)
                {
                    case "2017-07-10":
                        agenda.DSeman_Agenda = "Segunda";
                        break;
                    case "2017-07-11":
                        agenda.DSeman_Agenda = "Terca";
                        break;
                    case "2017-07-12":
                        agenda.DSeman_Agenda = "Quarta";
                        break;
                    case "2017-07-13":
                        agenda.DSeman_Agenda = "Quinta";
                        break;
                    case "2017-07-14":
                        agenda.DSeman_Agenda = "Sexta";
                        break;
                    case "2017-07-15":
                        agenda.DSeman_Agenda = "Sabado";
                        break;
                    default:
                        agenda.DSeman_Agenda = "Domingo";
                        break;
                }

                agenda.ativo = true;

                db.Agendas.Add(agenda);
                db.SaveChanges();
            }

            if (Redirect == "Save")  {
                return Json(new { success = true, result = "Save" }, JsonRequestBehavior.AllowGet);
            }

            if (Redirect == "MultipleSave") {
                return Json(new { success = true, result = "MultipleSave" }, JsonRequestBehavior.AllowGet);
            }


            var teste = db.Agendas.Where(x => x.ID_MediConsul == ID_MediConsul);
        
            return Json(new { success = true, result = "Conclusion" }, JsonRequestBehavior.AllowGet);

           // return Json(new { teste, events }, JsonRequestBehavior.AllowGet);


        }

       
    }
}
