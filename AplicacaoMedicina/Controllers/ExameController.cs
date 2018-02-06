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
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using System.Configuration;

namespace AplicacaoMedicina.Controllers
{
    public class ExameController : MasterController
    {
        private DrMedContext db = new DrMedContext();

        // GET: Exame
        public ActionResult Index()
        {
            var exames = db.Exames.Include(e => e.Paciente).Include(e => e.Laboratorio);
            return View(exames.ToList());
        }

        // GET: Exame/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exame exame = db.Exames.Find(id);
            if (exame == null)
            {
                return HttpNotFound();
            }
            return View(exame);
        }

        // GET: Exame/Create
        public ActionResult Create()
        {
            ViewBag.Id_PaciConv = new SelectList(db.Pacientes, "ID_Paci", "ID_Paci");
            ViewBag.ID_ConvLabo = new SelectList(db.Laboratorios, "ID_Labo", "ID_Labo");
            return View();
        }

        // POST: Exame/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Exame,Data,Realizado,Entrega,Protocolo,SenhaProtocolo,Id_PaciConv,ID_ConvLabo")] Exame exame)
        {
            if (ModelState.IsValid)
            {
                db.Exames.Add(exame);
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Exame", "Create");
                return RedirectToAction("Index");
            }

            ViewBag.Id_PaciConv = new SelectList(db.Pacientes, "ID_Paci", "ID_Paci");
            ViewBag.ID_ConvLabo = new SelectList(db.Laboratorios, "ID_Labo", "ID_Labo");
            return View(exame);
        }

        // GET: Exame/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exame exame = db.Exames.Find(id);
            if (exame == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_PaciConv = new SelectList(db.Pacientes, "ID_Paci", "ID_Paci");
            ViewBag.ID_ConvLabo = new SelectList(db.Laboratorios, "ID_Labo", "ID_Labo");
            return View(exame);
        }

        // POST: Exame/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Exame,Data,Realizado,Entrega,Protocolo,SenhaProtocolo,Id_PaciConv,ID_Labo,ID_ConvLabo")] Exame exame)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exame).State = EntityState.Modified;
                db.SaveChanges();

                GeraLogAuditoria("Nulo", "Exame", "Edit");
                return RedirectToAction("Index");
            }
            ViewBag.Id_PaciConv = new SelectList(db.Pacientes, "ID_Paci", "ID_Paci");
            ViewBag.ID_ConvLabo = new SelectList(db.Laboratorios, "ID_Labo", "ID_Labo");
            return View(exame);
        }

        // GET: Exame/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exame exame = db.Exames.Find(id);
            if (exame == null)
            {
                return HttpNotFound();
            }
            return View(exame);
        }

        // POST: Exame/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Exame exame = db.Exames.Find(id);
            db.Exames.Remove(exame);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "Exame", "Delete");
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

        //======> Actions JSON
        //==================Gera uma lista de exames a partir do Paciente============================
        // Unico   =     Retorna apenas um exame específico.
        // NumPed  =     Retorna os ultimos X Pedidos.
        // DataAnt =     Retorna todos anteriores a Data Atual.
        // DataPos =     Retorna todos após a Data Atual.
        // NRealiz =     Retorna exames não realizados.
        // Default =     Retorna exames consultas.
        //==============================================================================================
        public JsonResult GetExame(int? ID_Labo,int? Id_Exame, int? ID_Paci, string tipoConsulta, int? NumPed)
        {           
            
            switch (tipoConsulta)
            {

                case "NRealiz":
                    {
                        var exames = (from _Exame in db.Exames
                                      join _Labor in db.Laboratorios on _Exame.Laboratorio.ID_Labo equals _Labor.ID_Labo
                                      where (_Exame.Id_Exame == Id_Exame ||
                                            _Exame.Paciente.ID_Paci == ID_Paci ||
                                            _Exame.Laboratorio.ID_Labo == ID_Labo) &&
                                            _Exame.Realizado == false
                                      select new
                                      {
                                          _Exame.Id_Exame,
                                          _Exame.Paciente.Nome_Paci,
                                          _Exame.Protocolo,
                                          _Exame.SenhaProtocolo,
                                          _Exame.Laboratorio.Nome_Labo,
                                          _Exame.Data,
                                          _Exame.Realizado,
                                          _Exame.Entrega,
                                          _Exame.NomeArquivo
                                      }).OrderBy(x => x.Data).ToList();
                     
                        var examina = from item in exames
                                      select new
                                      {
                                          Id_Exame = item.Id_Exame,
                                          Nome_Paci = item.Nome_Paci,
                                          Protocolo = item.Protocolo,
                                          SenhaProtocolo = item.SenhaProtocolo,
                                          Laboratorio = item.Nome_Labo,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          Realizado = item.Realizado,
                                          Entrega = item.Entrega.ToString("yyyy-MM-dd HH:mm"),
                                          NomeArquivo = item.NomeArquivo
                                      };

                        GeraLogAuditoria("Nulo", "Exame", "GetExame");
                        return Json(new { success = true, exames = examina.ToList() }, JsonRequestBehavior.AllowGet);

                    }

                case "Unico":
                    {
                        var exames = (from _Exame in db.Exames
                                      join _Labor in db.Laboratorios on _Exame.Laboratorio.ID_Labo equals _Labor.ID_Labo
                                      where _Exame.Id_Exame                    == Id_Exame ||
                                            _Exame.Paciente.ID_Paci            == ID_Paci  ||
                                            _Exame.Laboratorio.ID_Labo         == ID_Labo
                                      select new
                                      {
                                          _Exame.Id_Exame,
                                          _Exame.Paciente.Nome_Paci,
                                          _Exame.Protocolo,
                                          _Exame.SenhaProtocolo,
                                          _Exame.Laboratorio.Nome_Labo,
                                          _Exame.Data,
                                          _Exame.Realizado,
                                          _Exame.Entrega,
                                          _Exame.NomeArquivo
                                      }).ToList();

                        var examina = from item in exames
                                      select new {
                                          Id_Exame    = item.Id_Exame,
                                          Nome_Paci   = item.Nome_Paci,
                                          Protocolo   = item.Protocolo,
                                          SenhaProtocolo = item.SenhaProtocolo,
                                          Laboratorio = item.Nome_Labo,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          Realizado = item.Realizado,
                                          Entrega = item.Entrega.ToString("yyyy-MM-dd HH:mm"),
                                          NomeArquivo = item.NomeArquivo
                                      };

                        GeraLogAuditoria("Nulo", "Exame", "GetExame");
                        return Json(new {success = true, exames = examina.ToList()}, JsonRequestBehavior.AllowGet);
                    }
                case "NumPed":
                    {
                        var exames = (from _Exame in db.Exames
                                     join _Labor in db.Laboratorios on _Exame.Laboratorio.ID_Labo equals _Labor.ID_Labo
                                     where _Exame.Id_Exame                    == Id_Exame ||
                                           _Exame.Paciente.ID_Paci            == ID_Paci  ||
                                           _Exame.Laboratorio.ID_Labo         == ID_Labo
                                      select new
                                     {
                                         _Exame.Id_Exame,
                                         _Exame.Paciente.Nome_Paci,
                                         _Exame.Protocolo,
                                         _Exame.SenhaProtocolo,
                                         _Exame.Laboratorio.Nome_Labo,
                                         _Exame.Data,
                                         _Exame.Realizado,
                                         _Exame.Entrega,
                                         _Exame.NomeArquivo
                                     }).ToList();

                        var examina = from item in exames
                                      select new
                                      {
                                          Id_Exame = item.Id_Exame,
                                          Nome_Paci = item.Nome_Paci,
                                          Protocolo = item.Protocolo,
                                          SenhaProtocolo = item.SenhaProtocolo,
                                          Laboratorio = item.Nome_Labo,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          Realizado = item.Realizado,
                                          Entrega = item.Entrega.ToString("yyyy-MM-dd HH:mm"),
                                          NomeArquivo = item.NomeArquivo
                                      };

                        GeraLogAuditoria("Nulo", "Exame", "GetExame");
                        return Json(new { success = true, exames = examina.ToList().Take(NumPed.Value) }, JsonRequestBehavior.AllowGet);
                    }
                case "DataAnt":
                    {
                        DateTime DataHoje = DateTime.Today;

                        var exames = (from _Exame in db.Exames
                                     join _Labor in db.Laboratorios on _Exame.Laboratorio.ID_Labo equals _Labor.ID_Labo
                                     where (_Exame.Id_Exame                    == Id_Exame  ||
                                            _Exame.Paciente.ID_Paci            == ID_Paci   ||
                                            _Exame.Laboratorio.ID_Labo         == ID_Labo ) &&
                                           DataHoje >= _Exame.Data
                                     select new
                                     {
                                         _Exame.Id_Exame,
                                         _Exame.Paciente.Nome_Paci,
                                         _Exame.Protocolo,
                                         _Exame.SenhaProtocolo,
                                         _Exame.Laboratorio.Nome_Labo,
                                         _Exame.Data,
                                         _Exame.Realizado,
                                         _Exame.Entrega,
                                         _Exame.NomeArquivo
                                     }).ToList();

                        var examina = from item in exames
                                      select new
                                      {
                                          Id_Exame = item.Id_Exame,
                                          Nome_Paci = item.Nome_Paci,
                                          Protocolo = item.Protocolo,
                                          SenhaProtocolo = item.SenhaProtocolo,
                                          Laboratorio = item.Nome_Labo,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          Realizado = item.Realizado,
                                          Entrega = item.Entrega.ToString("yyyy-MM-dd HH:mm"),
                                          NomeArquivo = item.NomeArquivo
                                      };

                        GeraLogAuditoria("Nulo", "Exame", "GetExame");
                        return Json(new { success = true, exames = examina.ToList() }, JsonRequestBehavior.AllowGet);
                    }
                case "DataPos":
                    {
                        DateTime DataHoje = DateTime.Today;

                        var exames = (from _Exame in db.Exames
                                     join _Labor in db.Laboratorios on _Exame.Laboratorio.ID_Labo equals _Labor.ID_Labo
                                     where (_Exame.Id_Exame                    == Id_Exame ||
                                            _Exame.Paciente.ID_Paci            == ID_Paci  ||
                                            _Exame.Laboratorio.ID_Labo         == ID_Labo) &&
                                           DataHoje <= _Exame.Data
                                     select new
                                     {
                                         _Exame.Id_Exame,
                                         _Exame.Paciente.Nome_Paci,
                                         _Exame.Protocolo,
                                         _Exame.SenhaProtocolo,
                                         _Exame.Laboratorio.Nome_Labo,
                                         _Exame.Data,
                                         _Exame.Realizado,
                                         _Exame.Entrega,
                                         _Exame.NomeArquivo
                                     }).OrderBy(x => x.Data).ToList();

                        var examina = from item in exames
                                      select new
                                      {
                                          Id_Exame = item.Id_Exame,
                                          Nome_Paci = item.Nome_Paci,
                                          Protocolo = item.Protocolo,
                                          SenhaProtocolo = item.SenhaProtocolo,
                                          Laboratorio = item.Nome_Labo,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          Realizado = item.Realizado,
                                          Entrega = item.Entrega.ToString("yyyy-MM-dd HH:mm"),
                                          NomeArquivo = item.NomeArquivo
                                      };

                        GeraLogAuditoria("Nulo", "Exame", "GetExame");
                        return Json(new { success = true, exames = examina.ToList() }, JsonRequestBehavior.AllowGet);
                    }
                default:
                    {
                        var exames = (from _Exame in db.Exames
                                     join _Labor in db.Laboratorios on _Exame.Laboratorio.ID_Labo equals _Labor.ID_Labo
                                     where _Exame.Paciente.ID_Paci    == ID_Paci ||
                                           _Exame.Laboratorio.ID_Labo == ID_Labo
                                      select new
                                     {
                                         _Exame.Id_Exame,
                                         _Exame.Paciente.Nome_Paci,
                                         _Exame.Protocolo,
                                         _Exame.SenhaProtocolo,
                                         _Exame.Laboratorio.Nome_Labo,
                                         _Exame.Data,
                                         _Exame.Realizado,
                                         _Exame.Entrega,
                                         _Exame.NomeArquivo
                                     }).ToList();

                        var examina = from item in exames
                                      select new
                                      {
                                          Id_Exame = item.Id_Exame,
                                          Nome_Paci = item.Nome_Paci,
                                          Protocolo = item.Protocolo,
                                          SenhaProtocolo = item.SenhaProtocolo,
                                          Laboratorio = item.Nome_Labo,
                                          Data = item.Data.ToString("yyyy-MM-dd HH:mm"),
                                          Realizado = item.Realizado,
                                          Entrega = item.Entrega.ToString("yyyy-MM-dd HH:mm"),
                                          NomeArquivo = item.NomeArquivo
                                      };

                        GeraLogAuditoria("Nulo", "Exame", "GetExame");
                        return Json(new { success = true, exames = examina.ToList() }, JsonRequestBehavior.AllowGet);
                    }
            }
        }

        //========= Cria Exame por JSON ============//
        // Cria um novo Exame
        //
        // Entrega     = Data
        // ID_PaciConv = Convenio do Paciente
        // ID_Labo     = Id do laboratorio
        // Protoclo    = Protocolo
        // SenhaProtocolo = Senha do Protoclo
        //===============================================

        public JsonResult CadastrarExame(DateTime Entrega, int ID_Paci, int ID_Labo, string Protocolo, string SenhaProtocolo)
        {            
            Exame exame = new Exame();

            exame.ID_Paci        = ID_Paci;
            exame.ID_Labo        = ID_Labo;
            exame.Data           = DateTime.Now;
            exame.Entrega        = Entrega;
            exame.Protocolo      = Protocolo;
            exame.SenhaProtocolo = SenhaProtocolo;
            exame.Realizado      = false;

            ModelState.Clear();
            TryValidateModel(exame);

            if (ModelState.IsValid)
            {
                db.Exames.Add(exame);
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    Id_Exame = exame.Id_Exame
                }, JsonRequestBehavior.AllowGet);
            } else {
                var message = string.Join("|", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                GeraLogAuditoria("Nulo", "Exame", "CadastrarExame");
                return Json(new { success = false, message }, JsonRequestBehavior.AllowGet);
            }           
        }

        //Deleta Exame
        public JsonResult DeleteExame(int Id_Exame)
        {
            Exame exame = db.Exames.Where(x => x.Id_Exame == Id_Exame).FirstOrDefault();

            if (exame == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }


            db.Exames.Remove(exame);
            db.SaveChanges();

            GeraLogAuditoria("Nulo", "Exame", "DeleteExame");
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //JSON - Verifica o Arquivo
        public JsonResult SubirExame( string Protocolo, int? Id_Exame, string Nome_Arquivo, string hashArquivo)
        {

            Exame exame = db.Exames.Where(x => x.Protocolo == Protocolo &&
                                               x.Id_Exame == Id_Exame).FirstOrDefault();

            if (exame == null)  {
                return Json(new { success = false, message = "Exame Inválido" }, JsonRequestBehavior.AllowGet);
            } 

            //Pega String de Conexão
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare share = fileClient.GetShareReference("exames");

            //Verifica conexão.
            if (share.Exists())
            {
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                //Se criarmos algum diretório Descomentar
                //CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("<Diretório>");

                //Verifica se o diretório existe
                if (rootDir.Exists()) {
                    CloudFile file = rootDir.GetFileReference(Nome_Arquivo);
                    //Verifica se existe
                    if (file.Exists())  {

                        //byte[] hash;

                        //Calcula Hash do Arquivo                      
                        /*  using (var md5 = System.Security.Cryptography.MD5.Create()) {
                              using (var stream = file.OpenRead()) {
                                  hash = md5.ComputeHash(stream);
                              }
                          }

                          var hashString = Convert.ToBase64String(hash).Replace("-", "").ToLower();*/

                        //  if (string.Compare(hashArquivo, hashString, StringComparison.InvariantCulture) == 0) {
                      
                        var size = file.Properties.Length.ToString();
              
                        if (size == hashArquivo) {

                            exame.NomeArquivo = Nome_Arquivo;
                            exame.Realizado = true;
                            exame.Hash = hashArquivo;

                            ModelState.Clear();
                            TryValidateModel(exame);

                            if (ModelState.IsValid)  {
                                db.Entry(exame).State = EntityState.Modified;
                                db.SaveChanges();
                            } else {
                                file.DeleteIfExists();
                                return Json(new { success = false, message = "Arquivo não armazenado", teste = size });
                            }

                            GeraLogAuditoria("Nulo", "Exame", "SubirExame");
                            return Json(new { success = true, message = "O Arquivo foi armazenado" }, JsonRequestBehavior.AllowGet);
                        }

                        //Apaga o arquivo
                        file.DeleteIfExists();
                        
                        return Json(new { success = false, message = "Arquivo Corrompido", teste = size });
                    }
                    return Json(new { success = false, message = "Arquivo Inexistente" });
                }
                return Json(new { success = false, message = "RootDir Inexistente" });
            }            
            return Json(new { success = false, message = "Conexão Inválida" }, JsonRequestBehavior.AllowGet);
        }


    }
}
