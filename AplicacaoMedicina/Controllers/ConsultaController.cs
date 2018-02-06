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
using System.Data.Entity.Validation;
using System.Globalization;

namespace AplicacaoMedicina.Controllers
{
    public class ConsultaController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        // GET: Consulta
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult Index(string filtro)
        {
            IQueryable<Consulta> constultas;

            if (!String.IsNullOrEmpty(filtro)) {

                int consultID = Int32.Parse(Request.Cookies["ConsultorioID"].Value);

                ViewBag.Filtro = filtro;

                constultas = db.Consultas
                    .Include(c => c.Paciente)
                    .Where(x => x.Situacao_Consa == filtro && 
                                x.MedicoConsultorio.Consultorio.ID_Consu == consultID);
            } else {

                ViewBag.Filtro = "Ambos";

                int consultID = Int32.Parse(Request.Cookies["ConsultorioID"].Value);

                constultas = db.Consultas
                    .Include(c => c.Paciente)
                    .Where(x => x.MedicoConsultorio.Consultorio.ID_Consu == consultID);
            }

            return View(constultas.OrderByDescending(x => x.Situacao_Consa == "Em Espera").ThenBy(x => x).ToList());
        }
                        

        [HttpGet]
        public ActionResult GetConsultaInfo(int? id, int? consulta)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Paciente paciente = db.Pacientes.Find(id);
            ViewBag.Consulta = consulta;

            if (paciente == null) {
                return HttpNotFound();
            }

            return PartialView(paciente);
        }

        public ActionResult Agenda(string pesquisa)
        {
            IQueryable<Consulta> consultas;

            if (!String.IsNullOrEmpty(pesquisa)) {
                 consultas = db.Consultas.Include(c => c.Paciente)
                    .Where(x => x.Situacao_Consa == "Confirmado" && x.Paciente.Nome_Paci.Contains(pesquisa))
                    .OrderBy(x => x.Data_Consa);
            } else  {
                consultas = db.Consultas.Include(c => c.Paciente)
                    .Where(x => x.Situacao_Consa == "Confirmado").OrderBy(x => x.Data_Consa);
            }

            return View(consultas.ToList());
        }

        // GET: Consulta/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var consulta = (from _Consult in db.Consultas
                           join _MediAre in db.MedicoAreas on _Consult.MedicoConsultorio.ID_Medi equals _MediAre.ID_Medi
                           where _Consult.ID_Consa == id
                           select new DetalheConsulta { Consulta = _Consult, MedicoArea = _MediAre }).First();
                        
            //Consulta consulta = db.Consultas.Find(id);



            if (consulta == null)
            {
                return HttpNotFound();
            }
            return PartialView(consulta);
        }

        // GET: Consulta/Create
        public ActionResult Create()
        {
            ViewBag.ID_MediConsu = new SelectList(db.MedicoConsultorios, "ID_MediConsu", "ID_MediConsu");
            ViewBag.ID_Paci = new SelectList(db.Pacientes, "ID_Paci", "ID_Paci");
            return View();
        }

        // POST: Consulta/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Consa,Notas_Consa,Data_Cons,Hora_Cons,Motivo_Consa,ID_MediConsu")] Consulta consulta, string Data_Cons, string Hora_Cons)
        {
            DateTime Data = DateTime.Parse(Data_Cons + " " + Hora_Cons);
            DateTime Teste = DateTime.Now;

            Teste = Teste.AddMinutes(10);

            if (Teste >= Data) {
                return RedirectToAction("Confirma", "Paciente", new { id = consulta.ID_MediConsu, redirect = "true" });
            }

            consulta.ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);
            consulta.Data_Consa = Data;
            consulta.Situacao_Consa = "Em Espera";
            consulta.DtMarc_Consa = DateTime.Now;

            ModelState.Clear();
            TryValidateModel(consulta);

            Paciente paciente = db.Pacientes.Find(Int32.Parse(Request.Cookies["PacienteID"].Value));

            if (ModelState.IsValid) {                
                db.Consultas.Add(consulta);
                db.SaveChanges();

                var infos = db.MedicoConsultorios.Find(consulta.ID_MediConsu);

                string corpoEmail = "Seu pedido de consulta foi feito realizado. <br/>" +
                                    "Pedido feito as: " + consulta.DtMarc_Consa.ToString("dd/MM/yyyy HH:mm") + ".<br/>" +
                                    "Medico: " + infos.Medico.Nome_Medi + ".<br/>" +
                                    "Consultorio: " + infos.Consultorio.Nome_Consu + ".<br/>" +
                                    "Data: " + Data.ToString("dd/MM/yyyy HH:mm") + ".<br/>" +
                                    "Por favor, espere a confirmação do Consultório. <br/>" +
                                    "Atenciosamente, <br/> " +
                                    "Doutor Simples.";

                EnviaNotificacao(paciente.Email_Paci, "Pedido de Consulta - " + Data.ToString("dd/MM/yyyy HH:mm"), corpoEmail);

                corpoEmail = "Foi realizado um pedido de consulta. <br/>" +
                             "Pedido feito as: " + consulta.DtMarc_Consa.ToString("dd/MM/yyyy HH:mm") + " por "+consulta.Paciente.Nome_Paci+ ".<br/>" +
                             "Medico: " + infos.Medico.Nome_Medi + ".<br/>" + 
                             "Consultorio: " + infos.Consultorio.Nome_Consu + ".<br/>" +
                             "Data: " + Data.ToString("dd/MM/yyyy HH:mm") + ".<br/>" +
                             "Por favor, confirme a situação da consulta. <br/>" +
                             "Atenciosamente, <br/>" +
                             "Doutor Simples.";

                EnviaNotificacao(infos.Consultorio.Email_Consu, "Pedido de Consulta - " + Data.ToString("dd/MM/yyyy HH:mm"), corpoEmail);

                GeraLogAuditoria("Nulo", "Consulta", "Create");
                return Redirect("/Paciente/DashBoard");
            }

            return View(consulta);
        }

        public ActionResult Prontuario(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulta consulta = db.Consultas.Find(id);
            if (consulta == null)
            {
                return HttpNotFound();
            }
           /* ViewBag.Id_ConsuConv = new SelectList(db.ConsultorioConvenios, "Id_ConsuConv", "Id_ConsuConv", consulta.Id_ConsuConv);
            ViewBag.Id_PaciConv = new SelectList(db.PacienteConvenios, "Id_PaciConv", "Codigo", consulta.Id_PaciConv); */
            return View(consulta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Prontuario(int ID_Consa, string Notas_Consa, string confirma)
        {

            Consulta consulta = db.Consultas.Find(ID_Consa);
            consulta.Notas_Consa = Notas_Consa;

            if (confirma == "end") {
                consulta.Situacao_Consa = "Realizada";
            }

            ModelState.Clear();
            TryValidateModel(consulta);

            if (ModelState.IsValid)
            {                
                db.Entry(consulta).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Consulta", "Prontuario");
                if (confirma == "end") {
                    return RedirectToAction("Index");
                } else {
                    return View(consulta);
                }
                
            }
            //  ViewBag.Id_ConsuConv = new SelectList(db.ConsultorioConvenios, "Id_ConsuConv", "Id_ConsuConv", consulta.Id_ConsuConv);
            //  ViewBag.Id_PaciConv = new SelectList(db.PacienteConvenios, "Id_PaciConv", "Codigo", consulta.Id_PaciConv);
            return View(consulta);
        }


        // GET: Consulta/Edit/5
        public ActionResult Edit(int? id, String acao)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulta consulta = db.Consultas.Find(id);
            if (consulta == null)  {
                return HttpNotFound();
            }

            ViewBag.Acao = acao;
            ViewBag.ID_MediConsu = new SelectList(db.MedicoConsultorios, "ID_MediConsu", "ID_MediConsu");
            ViewBag.ID_Paci = new SelectList(db.Pacientes, "ID_Paci", "ID_Paci");
            return PartialView(consulta);
        }

        // POST: Consulta/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Consa,Notas_Consa,Data_Consa,DtMarc_Consa, Situacao_Consa,"+
                                                 "Motivo_Consa,ID_MediConsu,ID_Paci,Situacao_Consa")] Consulta consulta)
        {
            
            ModelState.Clear();
            TryValidateModel(consulta);
            
            if (ModelState.IsValid)
            {                
                db.Entry(consulta).State = EntityState.Modified;
                db.SaveChanges();

                var info = db.MedicoConsultorios.Find(consulta.ID_MediConsu);             

                string corpoEmail = "";
                string assunto = "";

                if (consulta.Situacao_Consa == "Recusado") {

                    var paci = db.Pacientes.Find(consulta.ID_Paci);

                    if (User.IsInRole("Consultorio")) {
                        
                        assunto = "Cancelamento de Consulta - " + consulta.Data_Consa.ToString("dd/MM/yyyy HH:mm");

                        corpoEmail = "Sentimos muito mas sua consulta foi cancelada. <br/>" +
                                     "Cancelamento feito as: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "<br/>" +
                                     "Motivo: " + consulta.Motivo_Consa + "<br/><br/>" +
                                     "Médico: " + info.Medico.Nome_Medi + "<br/>" +
                                     "Consultório: " + info.Consultorio.Nome_Consu + "<br/>" +
                                     "Data:" + consulta.Data_Consa.ToString("dd/MM/yyyy HH:mm") + "<br/>" +
                                     "Atenciosamente, <br/>" +
                                     "Doutor Simples";

                        EnviaNotificacao(paci.Email_Paci, assunto, corpoEmail);
                    } else  {                       
                        assunto = "Cancelamento de Consulta - " + consulta.Data_Consa.ToString("dd/MM/yyyy HH:mm");

                        corpoEmail = "Sentimos muito o paciente "+paci.Nome_Paci+" cancelou a consulta. <br/>" +
                                     "Cancelamento feito as: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "<br/>" +                                     
                                     "Médico: " + info.Medico.Nome_Medi + "<br/>" +
                                     "Consultório: " + info.Consultorio.Nome_Consu + "<br/>" +
                                     "Data:" + consulta.Data_Consa.ToString("dd/MM/yyyy HH:mm") + "<br/>" +
                                     "Atenciosamente, <br/>" +
                                     "Doutor Simples";

                        EnviaNotificacao(info.Consultorio.Email_Consu, assunto, corpoEmail);
                    }

                } else {
                    var paci = db.Pacientes.Find(consulta.ID_Paci);

                    assunto = "Cofirmação de Consulta - " + consulta.Data_Consa.ToString("dd/MM/yyyy HH:mm");

                    corpoEmail = "Sua consulta foi confirmada. <br/>" + 
                                 "Médico: " + info.Medico.Nome_Medi+"<br/>" +
                                 "Consultório: " + info.Medico.Nome_Medi +"<br/>"+
                                 "Data: " + consulta.Data_Consa.ToString("dd/MM/yyyy HH:mm") + "<br/>"
                                 +"Atenciosamente, <br/>" +
                                 "Doutor Simples";

                    EnviaNotificacao(paci.Email_Paci, assunto, corpoEmail);
                }
            
                

                GeraLogAuditoria("Nulo", "Consulta", "Edit");
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
          //  ViewBag.Id_ConsuConv = new SelectList(db.ConsultorioConvenios, "Id_ConsuConv", "Id_ConsuConv", consulta.Id_ConsuConv);
          //  ViewBag.Id_PaciConv = new SelectList(db.PacienteConvenios, "Id_PaciConv", "Codigo", consulta.Id_PaciConv);
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Consulta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulta consulta = db.Consultas.Find(id);
            if (consulta == null)
            {
                return HttpNotFound();
            }
            return View(consulta);
        }

        // POST: Consulta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {            
            Consulta consulta = db.Consultas.Find(id);
            db.Consultas.Remove(consulta);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "Consulta", "DeleteConfirmed");
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


        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult New()
        {
            int id = Int32.Parse(Request.Cookies["ConsultorioID"].Value);

            var medicos = from item in db.MedicoConsultorios
                          where item.ID_Consu == id
            select new { Id = item.ID_MediConsu, Display = item.Medico.Nome_Medi };

            ViewBag.ID_MediConsu = medicos;

            return View();
        }

        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public JsonResult jSonConsult(string Data_Cons, int? id)  {

            int Ano = Convert.ToInt32(Data_Cons.Substring(6, 4));
            int Mes = Convert.ToInt32(Data_Cons.Substring(3, 2));
            int Dia = Convert.ToInt32(Data_Cons.Substring(0, 2));

            DateTime Data = new DateTime(Ano, Mes, Dia);

            String diaDaSemana;
            CultureInfo provider = new CultureInfo("pt-br");

            int dateToday = (int)Data.DayOfWeek;

            switch (dateToday) {
                case 1:
                    diaDaSemana = "Segunda";
                    break;
                case 2:
                    diaDaSemana = "Terca";
                    break;
                case 3:
                    diaDaSemana = "Quarta";
                    break;
                case 4:
                    diaDaSemana = "Quinta";
                    break;
                case 5:
                    diaDaSemana = "Sexta";
                    break;
                case 6:
                    diaDaSemana = "Sabado";
                    break;
                default:
                    diaDaSemana = "Domingo";
                    break;
            }


            var medConsul = from _MedCons in db.MedicoConsultorios
                            join _Agenda in db.Agendas on _MedCons.ID_MediConsu equals _Agenda.ID_MediConsul
                            where _MedCons.ID_MediConsu == id
                            select new Confirma { MedicoConsultorio = _MedCons, Agenda = _Agenda };

            var medConsula = (from x in medConsul
                              let consult = from _Consult in db.Consultas
                                            select new { Hora = _Consult.Data_Consa.Hour, Minut = _Consult.Data_Consa.Minute }
                              where !consult.Contains(new { Hora = x.Agenda.Data_Agenda.Hour, Minut = x.Agenda.Data_Agenda.Minute })
                              select x).Where(x => x.Agenda.DSeman_Agenda == diaDaSemana)
                            .OrderBy(x => x.Agenda.Data_Agenda)
                            .ToList();

            if (medConsula == null)  {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


            DateTime DataNow = DateTime.Now;

            if (DataNow.Date == Data)  {
                medConsula = (from item in medConsula
                              select new Confirma
                              {
                                  MedicoConsultorio = item.MedicoConsultorio,
                                  Agenda = item.Agenda
                              }).Where(x => x.Agenda.Data_Agenda.TimeOfDay > DataNow.TimeOfDay).ToList();
            }

            var medReturn = (from item in medConsula
                             select new
                             {
                                 AgendaId = item.Agenda.ID_Agenda,
                                 DoW = item.Agenda.DSeman_Agenda,
                                 DAgen = item.Agenda.Data_Agenda.ToString("yyyy-MM-dd HH:mm:ss"),
                                 Ativo    = item.Agenda.ativo,
                                 MedicoConsultorio = item.MedicoConsultorio,
                                 DtCons = item.Agenda.Data_Agenda.ToString("HH:mm")
                             }).ToList();

            return Json(new { success = true, consult = medReturn, teste = medConsula }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public JsonResult jSonConsulNew(string Data_Cons, int? id)
        {

            int Ano = Convert.ToInt32(Data_Cons.Substring(6, 4));
            int Mes = Convert.ToInt32(Data_Cons.Substring(3, 2));
            int Dia = Convert.ToInt32(Data_Cons.Substring(0, 2));

            DateTime Data = new DateTime(Ano, Mes, Dia);

            String diaDaSemana;
            CultureInfo provider = new CultureInfo("pt-br");

            int dateToday = (int)Data.DayOfWeek;

            switch (dateToday) {
                case 1:
                    diaDaSemana = "Segunda";
                    break;
                case 2:
                    diaDaSemana = "Terca";
                    break;
                case 3:
                    diaDaSemana = "Quarta";
                    break;
                case 4:
                    diaDaSemana = "Quinta";
                    break;
                case 5:
                    diaDaSemana = "Sexta";
                    break;
                case 6:
                    diaDaSemana = "Sabado";
                    break;
                default:
                    diaDaSemana = "Domingo";
                    break;
            }


            var medConsul = from _MedCons in db.MedicoConsultorios
                            join _Agenda in db.Agendas on _MedCons.ID_MediConsu equals _Agenda.ID_MediConsul
                            where _MedCons.ID_MediConsu == id
                            select new Confirma { MedicoConsultorio = _MedCons, Agenda = _Agenda };

            var medConsula = (from x in medConsul
                              let consult = from _Consult in db.Consultas
                                            select new { Hora = _Consult.Data_Consa.Hour, Minut = _Consult.Data_Consa.Minute }
                              where !consult.Contains(new { Hora = x.Agenda.Data_Agenda.Hour, Minut = x.Agenda.Data_Agenda.Minute })
                              select x).Where(x => x.Agenda.DSeman_Agenda == diaDaSemana)
                            .OrderBy(x => x.Agenda.Data_Agenda)
                            .ToList();

            if (medConsul == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


            DateTime DataNow = DateTime.Now;

           /* if (DataNow.Date == Data)
            {
                medConsul = (from item in medConsul
                              select new Confirma
                              {
                                  MedicoConsultorio = item.MedicoConsultorio,
                                  Agenda = item.Agenda
                              }).ToList();
            }*/


            var medReturn = (from item in medConsula
                             select new
                             {
                                 AgendaId = item.Agenda.ID_Agenda,
                                 DoW = item.Agenda.DSeman_Agenda,
                                 DAgen = item.Agenda.Data_Agenda.ToString("yyyy-MM-dd HH:mm:ss"),
                                 Ativo = item.Agenda.ativo,
                                 MedicoConsultorio = item.MedicoConsultorio,
                                 DtCons = item.Agenda.Data_Agenda.ToString("HH:mm")
                             }).ToList();

            return Json(new { success = true, consult = medReturn, teste = medConsula }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public JsonResult jsonConsultAtt(Agenda[] arrAgenda)
        {


            foreach (var item in arrAgenda)
            {
                ModelState.Clear();
                TryValidateModel(item);

                if (ModelState.IsValid)
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                } else {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(true, JsonRequestBehavior.AllowGet);
            
        }

        //ACTIONS JSON
        public JsonResult GetConsulta()
        {
            var consulta = from _Consulta in db.Consultas
                           select new
                            {
                                _Consulta.ID_Consa,
                                _Consulta.Notas_Consa,
                                _Consulta.Motivo_Consa,
                                _Consulta.Situacao_Consa,
                                _Consulta.Data_Consa,                            
                                _Consulta.MedicoConsultorio.Medico.Nome_Medi                               
                            };

            GeraLogAuditoria("Nulo", "Consulta", "GetConsulta");
            return Json(new { success = true, Consulta = consulta.ToList() }, JsonRequestBehavior.AllowGet);
        }
        //========= Cria uma Consutla por JSON ============//
        public JsonResult CreateJSON([Bind(Exclude = "Nota_Consa, Situacao_Consa")] Consulta consulta, int Id_ConsuConv)
        {           

            if (ModelState.IsValid)
            {

                db.Consultas.Add(consulta);
                try {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    return Json(new { message = e }, JsonRequestBehavior.AllowGet);
                }

                GeraLogAuditoria("Nulo", "Consulta", "CreateJSON");
                return Json(new { success = true, ID_Consa = consulta.ID_Consa }, JsonRequestBehavior.AllowGet);
            } else {
                var message = string.Join("|", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        public ActionResult ConfirmaConsulta(string confirma, int? id)
        {
            
            Consulta consulta = db.Consultas.Find(id);

            if (confirma != "Indisponível" &&
                confirma != "Confirmado"   &&
                confirma != "Recusado")
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            if (confirma != "Indisponível")  {
                consulta.Situacao_Consa = confirma;
            } else {
                consulta.Situacao_Consa = "Recusado";
                consulta.Motivo_Consa = "Médico Indisponível";
            }

            ModelState.Clear();
            TryValidateModel(consulta);           

            if (ModelState.IsValid)
            {
                db.Entry(consulta).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Consulta", "ConfirmaConsulta");
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelaConsulta(int? ID_Consa)
        {

            Consulta consulta = db.Consultas.Find(ID_Consa);

            consulta.Situacao_Consa = "Cancelada";
            consulta.Motivo_Consa   = "Cancelado pelo paciente as " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            ModelState.Clear();
            TryValidateModel(consulta);

            if (ModelState.IsValid)
            {
                db.Entry(consulta).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Consulta", "CancelaConsulta");
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

    }
}
