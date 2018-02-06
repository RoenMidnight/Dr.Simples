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
using System.Web.Security;
using System.Web.Helpers;
using Microsoft.Ajax.Utilities;

namespace AplicacaoMedicina.Controllers
{
    public class ConsultorioController : MasterController
    {
        private DrMedContext db = new DrMedContext();


        public JsonResult GetJSON()
        {
            return Json(db.Consultorios.ToList(), JsonRequestBehavior.AllowGet);
        }

        // GET: Consultorio
        /*public ActionResult Index()
        {
            return View(db.Consultorios.ToList());
        }*/

        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultorio consultorio = db.Consultorios.Find(id);
            if (consultorio == null)
            {
                return HttpNotFound();
            }
            return View(consultorio);
        }

        // GET: Consultorio/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Consultorio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Consu,Nome_Consu,CNPJ_Consu,Razao_Consu,CEP_Consu,Endereco_Consu,Numero_Consu,Complemento_Consu,Bairro_Consu,Cidade_Consu,Estado_Consu,Responsavel_Consu,Telefone_Consu,Email_Consu, Senha_UsuCons")] Consultorio consultorio, string Senha_UsuCons)
        {

       //     return Json(enviaEMail(consultorio.Email_Consu, consultorio.ID_Consu, consultorio.Email_Consu, Senha_UsuCons), JsonRequestBehavior.AllowGet);

            var conCNPJ = db.Consultorios.Where(x => x.CNPJ_Consu == consultorio.CNPJ_Consu).FirstOrDefault();

            if ((Senha_UsuCons == "") || (conCNPJ != default(Consultorio))) {
                consultorio.CNPJ_Consu = "";

                ModelState.Clear();
                TryValidateModel(consultorio); 
                
                return View(consultorio);
            }

            var conEmail = db.Consultorios.Where(x => x.Email_Consu == consultorio.Email_Consu).FirstOrDefault();

            if (conCNPJ != default(Consultorio)) {
                consultorio.Email_Consu = "";

                ModelState.Clear();
                TryValidateModel(consultorio);

                return View(consultorio);
            }


            if (ModelState.IsValid)  {
                db.Consultorios.Add(consultorio);

                UsuarioConsultorio usuarioConsult = new UsuarioConsultorio();

                usuarioConsult.Usuario_UsuCons = consultorio.Email_Consu;
                usuarioConsult.Senha_UsuCons = Crypto.HashPassword(Senha_UsuCons);
                usuarioConsult.ID_Consu = consultorio.ID_Consu;

                db.UsuarioConsultorios.Add(usuarioConsult);

                db.SaveChanges();

                EnviaEMail(consultorio.Email_Consu, usuarioConsult.Id_UsuCons, consultorio.Email_Consu, Senha_UsuCons);               
                              
                GeraLogAuditoria("Nulo", "Consultorio", "Create");
                return Redirect("/Home/Sucesso");
            }

            return View(consultorio);
        }

        // GET: Consultorio/Edit/5
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult Edit()
        {
            if (Request.Cookies["ConsultorioID"].Value == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int id = Int32.Parse(Request.Cookies["ConsultorioID"].Value);

            Consultorio consultorio = db.Consultorios.Find(id);          
            if (consultorio == null)
            {
                return HttpNotFound();
            }
            return View(consultorio);
        }

        // POST: Consultorio/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        public ActionResult Edit([Bind(Include = "ID_Consu,Nome_Consu,CNPJ_Consu,Razao_Consu,CEP_Consu,Endereco_Consu,Numero_Consu,Complemento_Consu,Bairro_Consu,Cidade_Consu,Estado_Consu,Responsavel_Consu,Telefone_Consu,Email_Consu")] Consultorio consultorio) {
           if (ModelState.IsValid)
            {
                db.Entry(consultorio).State = EntityState.Modified;

                UsuarioConsultorio usuarioConsultorio = new UsuarioConsultorio();
                var usuCons = db.UsuarioConsultorios.First(e => e.ID_Consu == consultorio.ID_Consu);

                usuarioConsultorio.ID_Consu = usuCons.ID_Consu;
                usuarioConsultorio.Id_UsuCons = usuCons.Id_UsuCons;
                usuarioConsultorio.Senha_UsuCons = Crypto.HashPassword(usuCons.Senha_UsuCons);
                usuarioConsultorio.Usuario_UsuCons = consultorio.Email_Consu;

                db.SaveChanges();

                db.Entry(usuCons).CurrentValues.SetValues(usuarioConsultorio);

                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Consultorio", "Edit");
                ViewBag.Sucesso = "Atualizado com Sucesso";
                return View(consultorio);
            }
            return View(consultorio);
        }

        [CustomAuthorization(LoginPage = "~/UsuarioPaciente/Login", Roles = "Paciente")]
        public ActionResult Confirma(int id, string redirect)
        {
            var medConsul = from item in db.MedicoConsultorios
                            where item.ID_Consu == id
                          select new {
                              ID_Consu = item.ID_Consu,
                              ID_Medi  = item.ID_Medi,
                              ID_MediConsu = item.ID_MediConsu,
                              Medico = item.Medico,
                              Consultorio = item.Consultorio,
                              ativo = item.ativo
                          } ;     

            if (medConsul == null)  {
                return HttpNotFound();
            }            

            var aval = db.Avaliacaos.Where(x => x.MedicoConsultorio.Consultorio.ID_Consu == id &&
                                                x.rate_Aval != 0);
            int avaliacao;

            if (aval.FirstOrDefault() == default(Avaliacao)) {
                avaliacao = 0;
            } else {
                avaliacao = (aval.Sum(x => x.rate_Aval) / aval.Count());
            }

            ViewBag.Avaliacao = avaliacao;
            
            ViewBag.NomeConsul = medConsul.First().Consultorio.Nome_Consu;
            ViewBag.TeleConsul = medConsul.First().Consultorio.Telefone_Consu;
            ViewBag.EndeConsul = medConsul.First().Consultorio.Endereco_Consu;
            ViewBag.BairConsul = medConsul.First().Consultorio.Bairro_Consu;
            ViewBag.CidaConsul = medConsul.First().Consultorio.Cidade_Consu;
            ViewBag.EstaConsul = medConsul.First().Consultorio.Estado_Consu;

            ViewBag.MedConsul = medConsul.First().ID_MediConsu;

            ViewBag.Error = false;

            var medicos = from item in medConsul
                          select new { Id = item.ID_MediConsu, Display = item.Medico.Nome_Medi };

            ViewBag.ID_MediConsu = medicos;


            return View();
        }

        // GET: Consultorio/Delete/5
        /* public ActionResult Delete(int? id)
         {
             if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }
             Consultorio consultorio = db.Consultorios.Find(id);
             if (consultorio == null)
             {
                 return HttpNotFound();
             }
             return View(consultorio);
         }*/

            // POST: Consultorio/Delete/5
            /* [HttpPost, ActionName("Delete")]
             [ValidateAntiForgeryToken]
             public ActionResult DeleteConfirmed(int id)
             {
                 GeraLogAuditoria("Nulo", "Consultorio", "DeleteConfirmed");
                 Consultorio consultorio = db.Consultorios.Find(id);
                 db.Consultorios.Remove(consultorio);
                 db.SaveChanges();
                 return RedirectToAction("Index");
             }*/
        [CustomAuthorization(LoginPage = "~/UsuarioConsultorios/Login", Roles = "Consultorio")]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        //================Pega informação de um Consultorio específico ====================================

        public JsonResult GetConsultorio(int? ID_Consu)
        {

            var consultorio = from _Consultorio in db.Consultorios
                         where ID_Consu == _Consultorio.ID_Consu 
                         select new { _Consultorio };

            GeraLogAuditoria("Nulo", "Consultorio", "GetConsultorio");
            return Json(new { success = true, Consultorio = consultorio.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEndereco(string Cidade, string Especialidade, string Medico, string Filtro )
        {

            var consultorio = db.Medicos.Join(db.MedicoAreas, p => p.ID_Medi,
                                                              x => x.ID_Medi,
                     (p, x) => new { Medicos = p, MedicoArea = x })

            .Join(db.MedicoConsultorios, px => px.Medicos.ID_Medi, md => md.ID_Medi,
                (px, md) => new { Medicos = px, MedicoConsultorio = md });

            if (Cidade != null) {

                string[] cidArray = Cidade.Split('-');
                string cidVar     = cidArray[0].Trim();

                consultorio = consultorio.Where(x => x.MedicoConsultorio.Consultorio.Cidade_Consu.Contains(cidVar));
            }
            if (Medico != null) {
                consultorio = consultorio.Where(x => x.MedicoConsultorio.Medico.Nome_Medi.Contains(Medico));
            }
            if (Especialidade != null)
            {
                consultorio = consultorio.Where(x => x.Medicos.MedicoArea.Area.Nome_Area.Contains(Especialidade));
            }

            if (Filtro == "Medico")
            {
                var consul = consultorio.Select(x => new
                {
                    id = x.MedicoConsultorio.ID_MediConsu,
                    medic = x.MedicoConsultorio.Medico.Nome_Medi,
                    consult = x.MedicoConsultorio.Consultorio.Nome_Consu,
                    address = x.MedicoConsultorio.Consultorio.Estado_Consu + " " + x.MedicoConsultorio.Consultorio.Cidade_Consu + " " +
                          x.MedicoConsultorio.Consultorio.Bairro_Consu + " " + x.MedicoConsultorio.Consultorio.Endereco_Consu + " " +
                          x.MedicoConsultorio.Consultorio.Complemento_Consu + " " + x.MedicoConsultorio.Consultorio.Numero_Consu
                });

                GeraLogAuditoria("Nulo", "Consultorio", "GetConsultorio");
                return Json(new { success = true, address = consul.ToList() }, JsonRequestBehavior.AllowGet);

            } else {                
                var consul = consultorio.Select(x => new
                {
                    id      = x.MedicoConsultorio.Consultorio.ID_Consu,
                    medic   = x.MedicoConsultorio.Consultorio.Nome_Consu,
                    consult = x.MedicoConsultorio.Consultorio.Telefone_Consu,
                    address = x.MedicoConsultorio.Consultorio.Estado_Consu + " " + x.MedicoConsultorio.Consultorio.Cidade_Consu + " " +
                             x.MedicoConsultorio.Consultorio.Bairro_Consu + " " + x.MedicoConsultorio.Consultorio.Endereco_Consu + " " +
                             x.MedicoConsultorio.Consultorio.Complemento_Consu + " " + x.MedicoConsultorio.Consultorio.Numero_Consu
                }).DistinctBy(x => x.id);

                GeraLogAuditoria("Nulo", "Consultorio", "GetConsultorio");
                return Json(new { success = true, address = consul.ToList() }, JsonRequestBehavior.AllowGet);
            }
                       
            
            
        }
    }
}
