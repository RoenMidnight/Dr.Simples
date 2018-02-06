namespace AplicacaoMedicina.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MySqlMigration : DbMigration
    {
        public override void Up()
        {
     /*       CreateTable(
                "dbo.Agenda",
                c => new
                    {
                        ID_Agenda = c.Int(nullable: false, identity: true),
                        Data_Agenda = c.DateTime(nullable: false, precision: 0),
                        DSeman_Agenda = c.String(unicode: false),
                        ID_MediConsul = c.Int(nullable: false),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Agenda)
                .ForeignKey("dbo.MedicoConsultorio", t => t.ID_MediConsul)
                .Index(t => t.ID_MediConsul);
            
            CreateTable(
                "dbo.MedicoConsultorio",
                c => new
                    {
                        ID_MediConsu = c.Int(nullable: false, identity: true),
                        ID_Medi = c.Int(nullable: false),
                        ID_Consu = c.Int(nullable: false),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID_MediConsu)
                .ForeignKey("dbo.Consultorio", t => t.ID_Consu)
                .ForeignKey("dbo.Medico", t => t.ID_Medi)
                .Index(t => t.ID_Medi)
                .Index(t => t.ID_Consu);
            
            CreateTable(
                "dbo.Consultorio",
                c => new
                    {
                        ID_Consu = c.Int(nullable: false, identity: true),
                        Nome_Consu = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Razao_Consu = c.String(maxLength: 45, storeType: "nvarchar"),
                        CNPJ_Consu = c.String(nullable: false, maxLength: 18, storeType: "nvarchar"),
                        CEP_Consu = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Endereco_Consu = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Numero_Consu = c.String(nullable: false, maxLength: 5, storeType: "nvarchar"),
                        Complemento_Consu = c.String(maxLength: 45, storeType: "nvarchar"),
                        Bairro_Consu = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Cidade_Consu = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Estado_Consu = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        Responsavel_Consu = c.String(maxLength: 45, storeType: "nvarchar"),
                        Telefone_Consu = c.String(maxLength: 15, storeType: "nvarchar"),
                        Email_Consu = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Descricao_Consu = c.String(unicode: false),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Consu)
                .Index(t => t.CNPJ_Consu, unique: true);
            
            CreateTable(
                "dbo.Medico",
                c => new
                    {
                        ID_Medi = c.Int(nullable: false, identity: true),
                        Nome_Medi = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        CRM_Medi = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        Email_Medi = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Telefone_Medi = c.String(nullable: false, maxLength: 15, storeType: "nvarchar"),
                        TipoInscri_Medi = c.Int(nullable: false),
                        Valor_Medi = c.String(maxLength: 15, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ID_Medi)
                .Index(t => t.CRM_Medi, unique: true);
            
            CreateTable(
                "dbo.AreaConsultorio",
                c => new
                    {
                        ID_Area = c.Int(nullable: false, identity: true),
                        Nome_Area = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        ID_Consu = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Area)
                .ForeignKey("dbo.Consultorio", t => t.ID_Consu)
                .Index(t => t.ID_Consu);
            
            CreateTable(
                "dbo.Area",
                c => new
                    {
                        ID_Area = c.Int(nullable: false, identity: true),
                        Nome_Area = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ID_Area);
            
            CreateTable(
                "dbo.Auditoria",
                c => new
                    {
                        Id_Audi = c.Int(nullable: false, identity: true),
                        Usua_Audi = c.String(nullable: false, unicode: false),
                        Cont_Audi = c.String(nullable: false, unicode: false),
                        Acti_Audi = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id_Audi);
            
            CreateTable(
                "dbo.Avaliacao",
                c => new
                    {
                        ID_Aval = c.Int(nullable: false, identity: true),
                        favo_Aval = c.Boolean(nullable: false),
                        rate_Aval = c.Int(nullable: false),
                        review_Aval = c.String(unicode: false),
                        ID_Paci = c.Int(nullable: false),
                        ID_MediConsu = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Aval)
                .ForeignKey("dbo.MedicoConsultorio", t => t.ID_MediConsu)
                .ForeignKey("dbo.Paciente", t => t.ID_Paci)
                .Index(t => t.ID_Paci)
                .Index(t => t.ID_MediConsu);
            
            CreateTable(
                "dbo.Paciente",
                c => new
                    {
                        ID_Paci = c.Int(nullable: false, identity: true),
                        Nome_Paci = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        NomeSocial_Paci = c.String(maxLength: 50, storeType: "nvarchar"),
                        DtNasc_Paci = c.DateTime(nullable: false, precision: 0),
                        Sexo_Paci = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        CPF_Paci = c.String(maxLength: 15, storeType: "nvarchar"),
                        RG_Paci = c.String(maxLength: 9, storeType: "nvarchar"),
                        Orgao_Paci = c.String(maxLength: 20, storeType: "nvarchar"),
                        Email_Paci = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        CEP_Paci = c.String(maxLength: 9, storeType: "nvarchar"),
                        Endereco_Paci = c.String(maxLength: 50, storeType: "nvarchar"),
                        Numero_Paci = c.String(maxLength: 10, storeType: "nvarchar"),
                        Complemento_Paci = c.String(maxLength: 45, storeType: "nvarchar"),
                        Bairro_Paci = c.String(maxLength: 20, storeType: "nvarchar"),
                        Cidade_Paci = c.String(maxLength: 45, storeType: "nvarchar"),
                        Estado_Paci = c.String(maxLength: 45, storeType: "nvarchar"),
                        Mae_Paci = c.String(maxLength: 50, storeType: "nvarchar"),
                        Pai_Paci = c.String(maxLength: 50, storeType: "nvarchar"),
                        Telefone_Paci = c.String(maxLength: 15, storeType: "nvarchar"),
                        AtivFisic_Paci = c.String(maxLength: 10, storeType: "nvarchar"),
                        Alergias_Paci = c.String(unicode: false),
                        DoenCron_Paci = c.String(unicode: false),
                        Diabetes_Paci = c.String(maxLength: 15, storeType: "nvarchar"),
                        Etilismo_Paci = c.String(maxLength: 3, storeType: "nvarchar"),
                        Fumanete_Paci = c.String(maxLength: 3, storeType: "nvarchar"),
                        Hipertens_Paci = c.String(maxLength: 3, storeType: "nvarchar"),
                        Neoplasia_Paci = c.String(maxLength: 3, storeType: "nvarchar"),
                        RemeConsta_Paci = c.String(unicode: false),
                        Peso_Paci = c.String(unicode: false),
                        Altura_Paci = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.ID_Paci)
                .Index(t => t.Email_Paci, unique: true);
            
            CreateTable(
                "dbo.Consulta",
                c => new
                    {
                        ID_Consa = c.Int(nullable: false, identity: true),
                        Notas_Consa = c.String(unicode: false),
                        Data_Consa = c.DateTime(nullable: false, precision: 0),
                        DtMarc_Consa = c.DateTime(nullable: false, precision: 0),
                        Situacao_Consa = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        Motivo_Consa = c.String(maxLength: 50, storeType: "nvarchar"),
                        ID_MediConsu = c.Int(nullable: false),
                        ID_Paci = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_Consa)
                .ForeignKey("dbo.MedicoConsultorio", t => t.ID_MediConsu)
                .ForeignKey("dbo.Paciente", t => t.ID_Paci)
                .Index(t => t.ID_MediConsu)
                .Index(t => t.ID_Paci);
            
            CreateTable(
                "dbo.ConsultorioConvenio",
                c => new
                    {
                        ID_ConsConv = c.Int(nullable: false, identity: true),
                        ID_Consu = c.Int(nullable: false),
                        ID_Conv = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_ConsConv)
                .ForeignKey("dbo.Consultorio", t => t.ID_Consu)
                .ForeignKey("dbo.Convenio", t => t.ID_Conv)
                .Index(t => t.ID_Consu)
                .Index(t => t.ID_Conv);
            
            CreateTable(
                "dbo.Convenio",
                c => new
                    {
                        ID_Conv = c.Int(nullable: false, identity: true),
                        Nome_Conv = c.String(maxLength: 45, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ID_Conv);
            
            CreateTable(
                "dbo.Exame",
                c => new
                    {
                        Id_Exame = c.Int(nullable: false, identity: true),
                        Data = c.DateTime(nullable: false, precision: 0),
                        Realizado = c.Boolean(nullable: false),
                        Entrega = c.DateTime(nullable: false, precision: 0),
                        Protocolo = c.String(nullable: false, unicode: false),
                        SenhaProtocolo = c.String(nullable: false, unicode: false),
                        Hash = c.String(unicode: false),
                        NomeArquivo = c.String(unicode: false),
                        ID_Paci = c.Int(nullable: false),
                        ID_Labo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Exame)
                .ForeignKey("dbo.Laboratorio", t => t.ID_Labo)
                .ForeignKey("dbo.Paciente", t => t.ID_Paci)
                .Index(t => t.ID_Paci)
                .Index(t => t.ID_Labo);
            
            CreateTable(
                "dbo.Laboratorio",
                c => new
                    {
                        ID_Labo = c.Int(nullable: false, identity: true),
                        Nome_Labo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        CNPJ_Labo = c.String(nullable: false, maxLength: 18, storeType: "nvarchar"),
                        CEP_Labo = c.String(nullable: false, maxLength: 9, storeType: "nvarchar"),
                        Endereco_Labo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Numero_Labo = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        Complemento_Labo = c.String(maxLength: 10, storeType: "nvarchar"),
                        Bairro_Labo = c.String(nullable: false, maxLength: 20, storeType: "nvarchar"),
                        Cidade_Labo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Estado_Labo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Responsavel_Labo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Telefone_Labo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Email_Labo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ID_Labo)
                .Index(t => t.CNPJ_Labo, unique: true);
            
            CreateTable(
                "dbo.MedicoArea",
                c => new
                    {
                        ID_MediArea = c.Int(nullable: false, identity: true),
                        ID_Area = c.Int(nullable: false),
                        ID_Medi = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_MediArea)
                .ForeignKey("dbo.Area", t => t.ID_Area)
                .ForeignKey("dbo.Medico", t => t.ID_Medi)
                .Index(t => t.ID_Area)
                .Index(t => t.ID_Medi);
            
            CreateTable(
                "dbo.MedicoConvenio",
                c => new
                    {
                        ID_MediConv = c.Int(nullable: false, identity: true),
                        ID_Medi = c.Int(nullable: false),
                        ID_Conv = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_MediConv)
                .ForeignKey("dbo.Convenio", t => t.ID_Conv)
                .ForeignKey("dbo.Medico", t => t.ID_Medi)
                .Index(t => t.ID_Medi)
                .Index(t => t.ID_Conv);
            
            CreateTable(
                "dbo.Notificacao",
                c => new
                    {
                        ID_NotifPaci = c.Int(nullable: false, identity: true),
                        Data_NotifPaci = c.DateTime(nullable: false, precision: 0),
                        ID_Paci = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID_NotifPaci)
                .ForeignKey("dbo.Paciente", t => t.ID_Paci)
                .Index(t => t.ID_Paci);
            
            CreateTable(
                "dbo.TipoLaboratorio",
                c => new
                    {
                        Id_TipoLaboratorio = c.Int(nullable: false, identity: true),
                        ID_Labo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_TipoLaboratorio)
                .ForeignKey("dbo.Laboratorio", t => t.ID_Labo)
                .Index(t => t.ID_Labo);
            
            CreateTable(
                "dbo.Tipo",
                c => new
                    {
                        Id_Tipo = c.Int(nullable: false, identity: true),
                        Nome_Tipo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Expertise_Tipo = c.String(nullable: false, maxLength: 45, storeType: "nvarchar"),
                        Id_TipoLaboratorio = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Tipo)
                .ForeignKey("dbo.TipoLaboratorio", t => t.Id_TipoLaboratorio)
                .Index(t => t.Id_TipoLaboratorio);
            
            CreateTable(
                "dbo.UsuarioConsultorio",
                c => new
                    {
                        Id_UsuCons = c.Int(nullable: false, identity: true),
                        Usuario_UsuCons = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        Senha_UsuCons = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        ID_Consu = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_UsuCons)
                .ForeignKey("dbo.Consultorio", t => t.ID_Consu, cascadeDelete: true)
                .Index(t => t.Usuario_UsuCons, unique: true)
                .Index(t => t.ID_Consu);
            
            CreateTable(
                "dbo.UsuarioLaboratorio",
                c => new
                    {
                        Id_UsuLab = c.Int(nullable: false, identity: true),
                        Usuario_UsuLab = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        Senha_UsuLab = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        ID_Labo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_UsuLab)
                .ForeignKey("dbo.Laboratorio", t => t.ID_Labo, cascadeDelete: true)
                .Index(t => t.Usuario_UsuLab, unique: true)
                .Index(t => t.ID_Labo);
            
            CreateTable(
                "dbo.UsuarioPaciente",
                c => new
                    {
                        Id_UsuPaci = c.Int(nullable: false, identity: true),
                        Usuario_UsuPaci = c.String(nullable: false, maxLength: 30, storeType: "nvarchar"),
                        Senha_UsuPaci = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        ID_Paci = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_UsuPaci)
                .ForeignKey("dbo.Paciente", t => t.ID_Paci, cascadeDelete: true)
                .Index(t => t.Usuario_UsuPaci, unique: true)
                .Index(t => t.ID_Paci); */
            
        }
        
        public override void Down()
        {
      /*      DropForeignKey("dbo.UsuarioPaciente", "ID_Paci", "dbo.Paciente");
            DropForeignKey("dbo.UsuarioLaboratorio", "ID_Labo", "dbo.Laboratorio");
            DropForeignKey("dbo.UsuarioConsultorio", "ID_Consu", "dbo.Consultorio");
            DropForeignKey("dbo.Tipo", "Id_TipoLaboratorio", "dbo.TipoLaboratorio");
            DropForeignKey("dbo.TipoLaboratorio", "ID_Labo", "dbo.Laboratorio");
            DropForeignKey("dbo.Notificacao", "ID_Paci", "dbo.Paciente");
            DropForeignKey("dbo.MedicoConvenio", "ID_Medi", "dbo.Medico");
            DropForeignKey("dbo.MedicoConvenio", "ID_Conv", "dbo.Convenio");
            DropForeignKey("dbo.MedicoArea", "ID_Medi", "dbo.Medico");
            DropForeignKey("dbo.MedicoArea", "ID_Area", "dbo.Area");
            DropForeignKey("dbo.Exame", "ID_Paci", "dbo.Paciente");
            DropForeignKey("dbo.Exame", "ID_Labo", "dbo.Laboratorio");
            DropForeignKey("dbo.ConsultorioConvenio", "ID_Conv", "dbo.Convenio");
            DropForeignKey("dbo.ConsultorioConvenio", "ID_Consu", "dbo.Consultorio");
            DropForeignKey("dbo.Consulta", "ID_Paci", "dbo.Paciente");
            DropForeignKey("dbo.Consulta", "ID_MediConsu", "dbo.MedicoConsultorio");
            DropForeignKey("dbo.Avaliacao", "ID_Paci", "dbo.Paciente");
            DropForeignKey("dbo.Avaliacao", "ID_MediConsu", "dbo.MedicoConsultorio");
            DropForeignKey("dbo.AreaConsultorio", "ID_Consu", "dbo.Consultorio");
            DropForeignKey("dbo.Agenda", "ID_MediConsul", "dbo.MedicoConsultorio");
            DropForeignKey("dbo.MedicoConsultorio", "ID_Medi", "dbo.Medico");
            DropForeignKey("dbo.MedicoConsultorio", "ID_Consu", "dbo.Consultorio");
            DropIndex("dbo.UsuarioPaciente", new[] { "ID_Paci" });
            DropIndex("dbo.UsuarioPaciente", new[] { "Usuario_UsuPaci" });
            DropIndex("dbo.UsuarioLaboratorio", new[] { "ID_Labo" });
            DropIndex("dbo.UsuarioLaboratorio", new[] { "Usuario_UsuLab" });
            DropIndex("dbo.UsuarioConsultorio", new[] { "ID_Consu" });
            DropIndex("dbo.UsuarioConsultorio", new[] { "Usuario_UsuCons" });
            DropIndex("dbo.Tipo", new[] { "Id_TipoLaboratorio" });
            DropIndex("dbo.TipoLaboratorio", new[] { "ID_Labo" });
            DropIndex("dbo.Notificacao", new[] { "ID_Paci" });
            DropIndex("dbo.MedicoConvenio", new[] { "ID_Conv" });
            DropIndex("dbo.MedicoConvenio", new[] { "ID_Medi" });
            DropIndex("dbo.MedicoArea", new[] { "ID_Medi" });
            DropIndex("dbo.MedicoArea", new[] { "ID_Area" });
            DropIndex("dbo.Laboratorio", new[] { "CNPJ_Labo" });
            DropIndex("dbo.Exame", new[] { "ID_Labo" });
            DropIndex("dbo.Exame", new[] { "ID_Paci" });
            DropIndex("dbo.ConsultorioConvenio", new[] { "ID_Conv" });
            DropIndex("dbo.ConsultorioConvenio", new[] { "ID_Consu" });
            DropIndex("dbo.Consulta", new[] { "ID_Paci" });
            DropIndex("dbo.Consulta", new[] { "ID_MediConsu" });
            DropIndex("dbo.Paciente", new[] { "Email_Paci" });
            DropIndex("dbo.Avaliacao", new[] { "ID_MediConsu" });
            DropIndex("dbo.Avaliacao", new[] { "ID_Paci" });
            DropIndex("dbo.AreaConsultorio", new[] { "ID_Consu" });
            DropIndex("dbo.Medico", new[] { "CRM_Medi" });
            DropIndex("dbo.Consultorio", new[] { "CNPJ_Consu" });
            DropIndex("dbo.MedicoConsultorio", new[] { "ID_Consu" });
            DropIndex("dbo.MedicoConsultorio", new[] { "ID_Medi" });
            DropIndex("dbo.Agenda", new[] { "ID_MediConsul" });
            DropTable("dbo.UsuarioPaciente");
            DropTable("dbo.UsuarioLaboratorio");
            DropTable("dbo.UsuarioConsultorio");
            DropTable("dbo.Tipo");
            DropTable("dbo.TipoLaboratorio");
            DropTable("dbo.Notificacao");
            DropTable("dbo.MedicoConvenio");
            DropTable("dbo.MedicoArea");
            DropTable("dbo.Laboratorio");
            DropTable("dbo.Exame");
            DropTable("dbo.Convenio");
            DropTable("dbo.ConsultorioConvenio");
            DropTable("dbo.Consulta");
            DropTable("dbo.Paciente");
            DropTable("dbo.Avaliacao");
            DropTable("dbo.Auditoria");
            DropTable("dbo.Area");
            DropTable("dbo.AreaConsultorio");
            DropTable("dbo.Medico");
            DropTable("dbo.Consultorio");
            DropTable("dbo.MedicoConsultorio");
            DropTable("dbo.Agenda");  */
        }
    }
}
