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
using Microsoft.AspNet.SignalR;
using AplicacaoMedicina.Hubs;
using System.Globalization;
using System.Web.Helpers;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace AplicacaoMedicina.Controllers
{
    public class PacienteController : MasterController
    {
        private DrMedContext db = new DrMedContext();


        // GET: Paciente
    /*    public ActionResult Index(string pesquisa)
        {
            if (!String.IsNullOrEmpty(pesquisa)) {
                return View(db.Pacientes.Where(x => x.Nome_Paci.Contains(pesquisa)).ToList());
            } else {
                return View(db.Pacientes.ToList());
            }
        }*/

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Dashboard()
        {
            int ID_Paci = Int32.Parse(Request.Cookies["PacienteID"].Value);

            var consultas = (db.Consultas.Where(x => x.ID_Paci == ID_Paci)).ToList();

            ViewBag.ProConsul = consultas.Where(x => x.Situacao_Consa == "Confirmado").Count();
            ViewBag.ConPenden = consultas.Where(x => x.Situacao_Consa == "Em Espera").Count();

            //SendMessage("TESTE");
           
            return View(consultas);
        }

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult ProcurarMedico()
        {
            var area = db.Areas.Select(x => x.Nome_Area).ToList();
            int i = area.Count();
            string[] areaArray = new string[i];

            for(var x = 0; x < i; ++x)
            {
                areaArray[x] = area[x];
            }

            ViewBag.AreaAC = areaArray;

            return View();
        }

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult SelectDoc(int? id) {

            if (id == null)  {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var consultorio = from _MedCons in db.MedicoConsultorios
                              join _MediAre in db.MedicoAreas on _MedCons.ID_Medi equals _MediAre.ID_Medi
                              where _MedCons.ID_Consu == id
                              select new Confirma { MedicoConsultorio = _MedCons, MedicoArea = _MediAre };

            ViewBag.Consultorio = consultorio.FirstOrDefault().MedicoConsultorio.Consultorio.Nome_Consu;

            if (consultorio == null) {
                return HttpNotFound();
            }

            return PartialView(consultorio.ToList());

        }

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Confirma(int? id, string redirect)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            DateTime dateNow = DateTime.Now;          
            
            var medConsul = from _MedCons in db.MedicoConsultorios
                            join _Agenda in db.Agendas on _MedCons.ID_MediConsu equals _Agenda.ID_MediConsul into mca
                            from _Agenda in mca.DefaultIfEmpty()
                            join _MedArea in db.MedicoAreas on _MedCons.ID_Medi equals _MedArea.ID_Medi

                            where _MedCons.ID_MediConsu == id   
                                                                 
                            select new Confirma
                            {
                                MedicoConsultorio = _MedCons,
                                MedicoArea = _MedArea,
                                Agenda = _Agenda                       
                            };

            

             medConsul = from x in medConsul
                                let consult = from _Consult in db.Consultas
                                              select new { Hora = _Consult.Data_Consa.Hour, Minut = _Consult.Data_Consa.Minute }
                                where !consult.Contains(new { Hora = x.Agenda.Data_Agenda.Hour, Minut = x.Agenda.Data_Agenda.Minute })                                      
                            select x;


            if (medConsul == null) {
                return HttpNotFound();
            }
            

            var aval = db.Avaliacaos.Where(x => x.ID_MediConsu == id &&
                                                x.rate_Aval    != 0 );
            int avaliacao;

            if (aval.FirstOrDefault() == default(Avaliacao) )  {
                avaliacao = 0;
            } else  {
                avaliacao = (aval.Sum(x => x.rate_Aval) / aval.Count());
            }                       

            ViewBag.Avaliacao = avaliacao;

            ViewBag.NomeMedico = medConsul.First().MedicoConsultorio.Medico.Nome_Medi;

            ViewBag.NomeConsul = medConsul.First().MedicoConsultorio.Consultorio.Nome_Consu;
            ViewBag.TeleConsul = medConsul.First().MedicoConsultorio.Consultorio.Telefone_Consu;            
            ViewBag.EndeConsul = medConsul.First().MedicoConsultorio.Consultorio.Endereco_Consu;
            ViewBag.BairConsul = medConsul.First().MedicoConsultorio.Consultorio.Bairro_Consu;
            ViewBag.CidaConsul = medConsul.First().MedicoConsultorio.Consultorio.Cidade_Consu;
            ViewBag.EstaConsul = medConsul.First().MedicoConsultorio.Consultorio.Estado_Consu;

            ViewBag.Especializ = medConsul.First().MedicoArea.Area.Nome_Area;

            ViewBag.MedConsul = id;

            int dateToday =(int)DateTime.Now.DayOfWeek;
            String diaDaSemana;

            DateTime dataTeste;

            switch (dateToday)
            {
                case 1:
                    diaDaSemana = "Segunda";
                    dataTeste = new DateTime(2017, 07, 10, dateNow.Hour, dateNow.Minute, dateNow.Second);
                    break;
                case 2:
                    diaDaSemana = "Terca";
                    dataTeste = new DateTime(2017, 07, 11, dateNow.Hour, dateNow.Minute, dateNow.Second);
                    break;
                case 3:
                    diaDaSemana = "Quarta";
                    dataTeste = new DateTime(2017, 07, 12, dateNow.Hour, dateNow.Minute, dateNow.Second);
                    break;
                case 4:
                    diaDaSemana = "Quinta";
                    dataTeste = new DateTime(2017, 07, 13, dateNow.Hour, dateNow.Minute, dateNow.Second);
                    break;
                case 5:
                    diaDaSemana = "Sexta";
                    dataTeste = new DateTime(2017, 07, 14, dateNow.Hour, dateNow.Minute, dateNow.Second);
                    break;
                case 6:
                    diaDaSemana = "Sabado";
                    dataTeste = new DateTime(2017, 07, 15, dateNow.Hour, dateNow.Minute, dateNow.Second);
                    break;
                default:
                    diaDaSemana = "Domingo";
                    dataTeste = new DateTime(2017, 07, 09, dateNow.Hour, dateNow.Minute, dateNow.Second);
                    break;
            }            

            var medResult = (from item in medConsul
                             select new Confirma {
                                 MedicoConsultorio = item.MedicoConsultorio,
                                 MedicoArea = item.MedicoArea,
                                 Agenda = item.Agenda,
                                 
                             }).Where(x => x.Agenda.DSeman_Agenda == diaDaSemana &&
                                x.Agenda.Data_Agenda >= dataTeste &&
                                x.Agenda.ativo == true)
                             .OrderBy(x => x.Agenda.Data_Agenda).ToList();

            if (redirect == "true") {
                ViewBag.Error = true;
            } else {
                ViewBag.Error = false;
            }

            ViewBag.Dia        = dateNow.Day.ToString();
            ViewBag.Mes        = dateNow.Month.ToString();
            ViewBag.Ano        = dateNow.Year.ToString();

            //ViewBag.Teste = teste;

            ViewBag.Nulo = "";
            if (medResult.Count() == 0) {
                ViewBag.Nulo = "Sentimos muito, mas não existem horários vagos neste dia :(";
            }

            return View(medResult);

        }

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Paciente()
        {
            return View(db.Pacientes.ToList());            
        }


        public JsonResult GetNotification()
        {
            var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;

            NotificationComponents NC = new NotificationComponents();
            var list = NC.GetContacts(notificationRegisterTime);
            Session["LastUpdate"] = DateTime.Now;

            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult GetPacienteInfo(int? id, int? consulta)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Paciente paciente   = db.Pacientes.Find(id);

            if (consulta.HasValue) {
                ViewBag.Consulta = consulta;
            } else  {
                ViewBag.Consulta = 0; 
            }

            if (paciente == null)  {
                return HttpNotFound();
            }

            return PartialView(paciente);
        }

        // GET: Paciente/Details/5
        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Details(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paciente paciente = db.Pacientes.Find(id);
            if (paciente == null)
            {
                return HttpNotFound();
            }
            return View(paciente);
        }

        // GET: Paciente/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult FacebookCreate(Paciente paciente, string id)
        {
            var pacCPF = db.Pacientes.Where(x => x.Email_Paci == paciente.Email_Paci).FirstOrDefault();
            
            if (pacCPF != default(Paciente))
            {
                return Redirect("/Paciente/Create");
            }
            
            if (ModelState.IsValid)
            {
                db.Pacientes.Add(paciente);

                UsuarioPaciente usuarioPaciente = new UsuarioPaciente();

                usuarioPaciente.Usuario_UsuPaci = paciente.Email_Paci;
                usuarioPaciente.Senha_UsuPaci = Crypto.HashPassword(id);
                usuarioPaciente.ID_Paci = paciente.ID_Paci;

                db.UsuarioPacientes.Add(usuarioPaciente);

                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Paciente", "Create");
                return Redirect("/UsuarioPaciente/Login");
            }

            return Redirect("/Paciente/Create");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Paci,Nome_Paci,NomeSocial_Paci,DtNasc_Paci,Sexo_Paci,CPF_Paci,RG_Paci,Orgao_Paci,Email_Paci,CEP_Paci,Endereco_Paci,Numero_Paci,Complemento_Paci,Bairro_Paci,Cidade_Paci,Estado_Paci,Mae_Paci,Pai_Paci,Telefone_Paci, Senha_UsuPaci")] Paciente paciente, string Senha_UsuPaci)
        {

            var pacMail = db.Pacientes.Where(x => x.Email_Paci == paciente.Email_Paci).FirstOrDefault();

            if ((Senha_UsuPaci == "") || (pacMail != default(Paciente))){               
                paciente.Email_Paci = "";

                ModelState.Clear();
                TryValidateModel(paciente);

                return View(paciente);
            }

            var pacCPF = db.Pacientes.Where(x => x.CPF_Paci == paciente.CPF_Paci).FirstOrDefault();
            
            if (pacCPF != default(Paciente)) {
                paciente.CPF_Paci = "";

                ViewBag.CPF = "I'm Not Null";

                ModelState.Clear();
                TryValidateModel(paciente);

                return View(paciente);
            }


            if (ModelState.IsValid)  {

                db.Pacientes.Add(paciente);

                UsuarioPaciente usuarioPaciente = new UsuarioPaciente();

                usuarioPaciente.Usuario_UsuPaci = paciente.Email_Paci;
                usuarioPaciente.Senha_UsuPaci = Crypto.HashPassword(Senha_UsuPaci);
                usuarioPaciente.ID_Paci = paciente.ID_Paci;

                db.UsuarioPacientes.Add(usuarioPaciente);
                        
                db.SaveChanges();

                FormsAuthentication.SetAuthCookie("paci" + paciente.Email_Paci, false);
                Response.Cookies["PacienteID"].Value = paciente.ID_Paci.ToString();
                Response.Cookies["PacienteID"].Expires = DateTime.Now.AddDays(1);

                Response.Cookies["UserID"].Value = usuarioPaciente.Id_UsuPaci.ToString();
                Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(1);

                GeraLogAuditoria("Nulo", "Paciente", "Create");
                return Redirect("/Paciente/Edit");
            } else {
                            
            }

            return View(paciente);
        }

        // GET: Paciente/Edit/5
        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Edit()
        {
            if (Request.Cookies["PacienteID"].Value == "")  {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int id = Int32.Parse(Request.Cookies["PacienteID"].Value);

            Paciente paciente = db.Pacientes.Find(id);
            if (paciente == null)
            {
                return HttpNotFound();
            }
            return View(paciente);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Edit(Paciente paciente)  {
                       
            if (ModelState.IsValid)
            {
                db.Entry(paciente).State = EntityState.Modified;

                UsuarioPaciente usuarioPaciente = new UsuarioPaciente();
                var usuPaci = db.UsuarioPacientes.First(e => e.ID_Paci == paciente.ID_Paci);

                usuarioPaciente.ID_Paci = usuPaci.ID_Paci;
                usuarioPaciente.Id_UsuPaci = usuPaci.Id_UsuPaci;
                usuarioPaciente.Senha_UsuPaci = usuPaci.Senha_UsuPaci;
                usuarioPaciente.Usuario_UsuPaci = paciente.Email_Paci;

                db.SaveChanges();

                db.Entry(usuPaci).CurrentValues.SetValues(usuarioPaciente);

                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Paciente", "Edit");
                ViewBag.Sucesso = "Atualizado com Sucesso";
                return View(paciente);
            }
            return View(paciente);
        }

        // GET: Paciente/Delete/5
    /*    public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paciente paciente = db.Pacientes.Find(id);
            if (paciente == null)
            {
                return HttpNotFound();
            }
            return View(paciente);
        } */

        // POST: Paciente/Delete/5
     //   [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
     /*   public ActionResult DeleteConfirmed(int id)
        {
            Paciente paciente = db.Pacientes.Find(id);
            db.Pacientes.Remove(paciente);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "Paciente", "Delete");
            return RedirectToAction("Index");
        } */

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public JsonResult JsonClone(int Id_Paci, int Id_Medi, string RedirecURL)
        {


            return Json(new { response = true }, JsonRequestBehavior.AllowGet );
        }

        //Ações JSOn
        public JsonResult jSonConsult(string Data_Cons, int? id) {

            int Ano = Convert.ToInt32(Data_Cons.Substring(6,4));
            int Mes = Convert.ToInt32(Data_Cons.Substring(3,2));
            int Dia = Convert.ToInt32(Data_Cons.Substring(0,2));

            DateTime Data = new DateTime(Ano, Mes, Dia );            

            String diaDaSemana;
            CultureInfo provider = new CultureInfo("pt-br");
       
            int dateToday = (int)Data.DayOfWeek;
            
            switch (dateToday)  {
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
                            where _MedCons.ID_MediConsu == id && _Agenda.ativo == true
                            select new Confirma { MedicoConsultorio = _MedCons, Agenda = _Agenda };

            var medConsula = (from x in medConsul
                        let consult = from _Consult in db.Consultas
                                      select new { Hora = _Consult.Data_Consa.Hour, Minut = _Consult.Data_Consa.Minute }
                        where !consult.Contains(new { Hora = x.Agenda.Data_Agenda.Hour, Minut = x.Agenda.Data_Agenda.Minute })
                        select x).Where(x => x.Agenda.DSeman_Agenda == diaDaSemana)
                            .OrderBy(x => x.Agenda.Data_Agenda)
                            .ToList();
            
            if (medConsula == null) {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


            DateTime DataNow = DateTime.Now;

            if (DataNow.Date == Data)
            {
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
                                  MedicoConsultorio = item.MedicoConsultorio,                            
                                  DtCons = item.Agenda.Data_Agenda.ToString("HH:mm")
                                }).ToList();

           

            return Json(new { success = true, consult = medReturn, teste = medConsula }, JsonRequestBehavior.AllowGet);
        }


        //========= Busca CPF Paciente por JSON ============//

        public JsonResult GetCPF(string CPF)
        {
            var pacienteCPF = db.Pacientes.Where(x => x.CPF_Paci == CPF).FirstOrDefault();

            if (pacienteCPF == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new {success = true }, JsonRequestBehavior.AllowGet);
        }

        //========= Busca Email Paciente JSON ============//

        public JsonResult GetEmail(string Email)
        {
            var pacienteEmail = db.Pacientes.Where(x => x.Email_Paci == Email).FirstOrDefault();

            if (pacienteEmail == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //========= Cria Paciente por JSON ============//
        // Cria um Paciente e Usuario Paciente
        //
        // Paciente = Objeto Paciente
        // Senha_UsuPaci = Senha do Paciente
        //===============================================

        public JsonResult CreateJSON(Paciente paciente, string Senha_UsuPaci)
        {      
            if (Senha_UsuPaci == "")
            {
                return Json(new { success = false, Error = "Senha nula" }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                db.Pacientes.Add(paciente);

                UsuarioPaciente usuarioPaciente = new UsuarioPaciente();

                usuarioPaciente.Usuario_UsuPaci = paciente.Email_Paci;
                usuarioPaciente.Senha_UsuPaci   = Crypto.HashPassword(Senha_UsuPaci);
                usuarioPaciente.ID_Paci         = paciente.ID_Paci;

                db.UsuarioPacientes.Add(usuarioPaciente);
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Paciente", "CreateJSON");
                return Json(new { success = true, ID_Paci = paciente.ID_Paci,
                    Id_UsuPaci = usuarioPaciente.Id_UsuPaci},  JsonRequestBehavior.AllowGet);
            } else {
                var message = string.Join("|", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        //========= Edita um Paciente por JSON ============//

        public JsonResult EditJSON([Bind(Exclude = "Senha_UsuPaci")] Paciente paciente)
        {

            if (ModelState.IsValid)
            {               
                db.Entry(paciente).State        = EntityState.Modified;

                UsuarioPaciente usuarioPaciente = new UsuarioPaciente();
                var usuPaci = db.UsuarioPacientes.First(e => e.ID_Paci == paciente.ID_Paci);

                usuarioPaciente.ID_Paci = usuPaci.ID_Paci;
                usuarioPaciente.Id_UsuPaci = usuPaci.Id_UsuPaci;
                usuarioPaciente.Senha_UsuPaci = Crypto.HashPassword(usuPaci.Senha_UsuPaci);
                usuarioPaciente.Usuario_UsuPaci = paciente.Email_Paci;

                db.SaveChanges();

                db.Entry(usuPaci).CurrentValues.SetValues(usuarioPaciente);

                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Paciente", "EditJSON");
                return Json(new { success = true, ID_Paci = paciente.ID_Paci }, JsonRequestBehavior.AllowGet);
            } else {
                var message = string.Join("|", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }

            
        }

        //================Pega informação de um Paciente específico ====================================

        public JsonResult GetPaciente(int? ID_Paci, string CPF_Paci)
        {
            var paciente = (from _Paciente in db.Pacientes
                           where ID_Paci == _Paciente.ID_Paci || CPF_Paci == _Paciente.CPF_Paci
                           select new {
                               _Paciente.ID_Paci, _Paciente.Nome_Paci, _Paciente.NomeSocial_Paci,
                               _Paciente.DtNasc_Paci,
                               _Paciente.Sexo_Paci, _Paciente.CPF_Paci, _Paciente.RG_Paci, _Paciente.Orgao_Paci,
                               _Paciente.Email_Paci, _Paciente.CEP_Paci, _Paciente.Endereco_Paci, _Paciente.Numero_Paci,
                               _Paciente.Complemento_Paci, _Paciente.Bairro_Paci, _Paciente.Cidade_Paci,
                               _Paciente.Estado_Paci,
                               _Paciente.Mae_Paci, _Paciente.Pai_Paci, _Paciente.Telefone_Paci
                              }).ToList();

            var retPac = from item in paciente
                         select new
                         {
                             item.ID_Paci,
                             item.Nome_Paci,
                             item.NomeSocial_Paci,
                             DtNasc_Paci = item.DtNasc_Paci.ToString("yyyy-MM-dd"),
                             item.Sexo_Paci,
                             item.CPF_Paci,
                             item.RG_Paci,
                             item.Orgao_Paci,
                             item.Email_Paci,
                             item.CEP_Paci,
                             item.Endereco_Paci,
                             item.Numero_Paci,
                             item.Complemento_Paci,
                             item.Bairro_Paci,
                             item.Cidade_Paci,
                             item.Estado_Paci,
                             item.Mae_Paci,
                             item.Pai_Paci,
                             item.Telefone_Paci

                         };

            return Json(new { success = true, Pacientes = retPac.First() }, JsonRequestBehavior.AllowGet);
        }


        //==================Gera uma lista de consultas a partir do Paciente============================
        // Unico   =     Retorna apenas uma consulta específica.
        // NumPed  =     Retorna os ultimos X Pedidos.
        // DataAnt =     Retorna todos anteriores a Data Atual.
        // DataPos =     Retorna todos após a Data Atual.
        // Default =     Retorna todas consultas.
        //==============================================================================================

        public JsonResult GetConsulta(int? ID_Paci, int? ID_Consa, string tipoConsulta, int? NumPed)
        {
            
            switch (tipoConsulta) {

                case "Unico":
                    {
                        var pacienteConsulta = (from _Paciente in db.Pacientes 
                                               join _Consulta in db.Consultas on _Paciente.ID_Paci equals _Consulta.ID_Paci                                                                          
                                               where ID_Paci == _Paciente.ID_Paci && ID_Consa == _Consulta.ID_Consa
                                               select new
                                               {
                                                   ID_Consa   = _Consulta.ID_Consa,
                                                   Nome_Paciente = _Paciente.Nome_Paci,
                                                   Data = _Consulta.Data_Consa,
                                                   Motivo = _Consulta.Motivo_Consa,
                                                   Situacao = _Consulta.Situacao_Consa,
                                                   Nome_Medico = _Consulta.MedicoConsultorio.Medico.Nome_Medi,
                                                   Nome_Consul = _Consulta.MedicoConsultorio.Consultorio.Nome_Consu
                                               }).ToList();

                        var pacCons = from item in pacienteConsulta
                                      select new
                                      {
                                          item.ID_Consa,
                                          item.Nome_Paciente,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          item.Motivo,
                                          item.Situacao,
                                          item.Nome_Medico,
                                          item.Nome_Consul
                                      };
                        
                        return Json(new { success = true, Pacientes = pacCons.ToList() }, JsonRequestBehavior.AllowGet);
                    }

                case "NumPed":
                    {
                        var pacienteConsulta = (from _Paciente in db.Pacientes
                                                join _Consulta in db.Consultas on _Paciente.ID_Paci equals _Consulta.ID_Paci
                                                where ID_Paci == _Paciente.ID_Paci
                                               select new
                                               {
                                                   ID_Consa = _Consulta.ID_Consa,
                                                   Nome_Paciente = _Paciente.Nome_Paci,
                                                   Data = _Consulta.Data_Consa,
                                                   Motivo = _Consulta.Motivo_Consa,
                                                   Situacao = _Consulta.Situacao_Consa,
                                                   Nome_Medico = _Consulta.MedicoConsultorio.Medico.Nome_Medi,
                                                   Nome_Consul = _Consulta.MedicoConsultorio.Consultorio.Nome_Consu
                                               }).ToList();

                        var pacCons = from item in pacienteConsulta
                                      select new
                                      {
                                          item.ID_Consa,
                                          item.Nome_Paciente,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          item.Motivo,
                                          item.Situacao,
                                          item.Nome_Medico,
                                          item.Nome_Consul
                                      };


                        return Json(new { success = true, Pacientes = pacCons.ToList().Take(NumPed.Value) }, JsonRequestBehavior.AllowGet);
                    }

                case "DataAnt":
                    {
                        DateTime DataHoje = DateTime.Today;

                        var pacienteConsulta = (from _Paciente in db.Pacientes
                                                join _Consulta in db.Consultas on _Paciente.ID_Paci equals _Consulta.ID_Paci
                                                where ID_Paci == _Paciente.ID_Paci && _Consulta.Data_Consa < DataHoje
                                               select new
                                               {
                                                   ID_Consa = _Consulta.ID_Consa,
                                                   Nome_Paciente = _Paciente.Nome_Paci,
                                                   Data = _Consulta.Data_Consa,
                                                   Motivo = _Consulta.Motivo_Consa,
                                                   Situacao = _Consulta.Situacao_Consa,
                                                   Nome_Medico = _Consulta.MedicoConsultorio.Medico.Nome_Medi,
                                                   Nome_Consul = _Consulta.MedicoConsultorio.Consultorio.Nome_Consu
                                               }).ToList();

                        var pacCons = from item in pacienteConsulta
                                      select new
                                      {
                                          item.ID_Consa,
                                          item.Nome_Paciente,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          item.Motivo,
                                          item.Situacao,
                                          item.Nome_Medico,
                                          item.Nome_Consul
                                      };


                        return Json(new { success = true, Pacientes = pacCons.ToList() }, JsonRequestBehavior.AllowGet);
                    }

                case "DataPos":
                    {
                        DateTime DataHoje = DateTime.Today;

                        var pacienteConsulta = (from _Paciente in db.Pacientes
                                                join _Consulta in db.Consultas on _Paciente.ID_Paci equals _Consulta.ID_Paci
                                                where ID_Paci == _Paciente.ID_Paci && _Consulta.Data_Consa > DataHoje
                                            
                                               select new
                                               {
                                                   ID_Consa = _Consulta.ID_Consa,
                                                   Nome_Paciente = _Paciente.Nome_Paci,
                                                   Data = _Consulta.Data_Consa,
                                                   Motivo = _Consulta.Motivo_Consa,
                                                   Situacao = _Consulta.Situacao_Consa,
                                                   Nome_Medico = _Consulta.MedicoConsultorio.Medico.Nome_Medi,
                                                   Nome_Consul = _Consulta.MedicoConsultorio.Consultorio.Nome_Consu
                                               }).OrderBy(x => x.Data).ToList();

                        var pacCons = from item in pacienteConsulta
                                      select new
                                      {
                                          item.ID_Consa,
                                          item.Nome_Paciente,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          item.Motivo,
                                          item.Situacao,
                                          item.Nome_Medico,
                                          item.Nome_Consul
                                      };



                        return Json(new { success = true, Pacientes = pacCons.ToList() }, JsonRequestBehavior.AllowGet);
                    }

                default: { 
                        var pacienteConsulta = (from _Paciente in db.Pacientes
                                                join _Consulta in db.Consultas on _Paciente.ID_Paci equals _Consulta.ID_Paci
                                                where ID_Paci == _Paciente.ID_Paci
                                               select new
                                               {
                                                   ID_Consa = _Consulta.ID_Consa,
                                                   Nome_Paciente = _Paciente.Nome_Paci,
                                                   Data = _Consulta.Data_Consa,
                                                   Motivo = _Consulta.Motivo_Consa,
                                                   Situacao = _Consulta.Situacao_Consa,
                                                   Nome_Medico = _Consulta.MedicoConsultorio.Medico.Nome_Medi,
                                                   Nome_Consul = _Consulta.MedicoConsultorio.Consultorio.Nome_Consu
                                               }).ToList();

                        var pacCons = from item in pacienteConsulta
                                      select new
                                      {
                                          item.ID_Consa,
                                          item.Nome_Paciente, 
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          item.Motivo,  item.Situacao,
                                          item.Nome_Medico, item.Nome_Consul
                                      };


                        return Json(new { success = true, Pacientes = pacCons.ToList() }, JsonRequestBehavior.AllowGet);
                    }                
            }
        }


    }    
}
