using AplicacaoMedicina.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace AplicacaoMedicina.DataContexts
{
 //   [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class DrMedContext : DbContext
    {
        public DrMedContext()
            : base("DefaultConnection_DatabasePublish")
        {
            Database.SetInitializer<DrMedContext>(new DrMedContextInitializer());
        }
       
        public DbSet<Area> Areas {get; set;}
        public DbSet<AreaConsultorio> AreaConsultorios { get; set; }
        public DbSet<Avaliacao> Avaliacaos { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<Consultorio> Consultorios { get; set; }        
        public DbSet<Convenio> Convenios { get; set; }
        public DbSet<ConsultorioConvenio> ConsultorioConvenios { get; set; }
        public DbSet<Exame> Exames { get; set; }
        public DbSet<Laboratorio> Laboratorios { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<MedicoArea> MedicoAreas { get; set; }
        public DbSet<MedicoConsultorio> MedicoConsultorios { get; set; }
        public DbSet<MedicoConvenio> MedicoConvenios { get; set; }
        public DbSet<Notificacao> Notificacao { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Tipo> Tipos { get; set; }
        public DbSet<TipoLaboratorio> TipoLaboratorios { get; set; }
        public DbSet<UsuarioPaciente> UsuarioPacientes { get; set; }
        public DbSet<UsuarioLaboratorio> UsuarioLaboratorios { get; set; }
        public DbSet<UsuarioConsultorio> UsuarioConsultorios { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<UsuarioLaboratorio>()
                .HasRequired(r => r.Laboratorio)
                .WithMany()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<UsuarioPaciente>()
                .HasRequired(r => r.Pacientes)
                .WithMany()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<UsuarioConsultorio>()
               .HasRequired(r => r.Consultorio)
               .WithMany()
               .WillCascadeOnDelete(true);


        }
        
    }


    public class DrMedContextInitializer : DropCreateDatabaseAlways<DrMedContext> { }
}