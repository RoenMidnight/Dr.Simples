using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Paciente
    {
        public Paciente()
        {
            DtNasc_Paci = DateTime.Today;
        }

        [Key]
        public int ID_Paci { get; set; }

        [Required(ErrorMessage = "Preencha o Nome")]
        [MaxLength(100)]
        public string Nome_Paci { get; set; }
        [MaxLength(100)]
        public string NomeSocial_Paci { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DtNasc_Paci { get; set; }

        [Required(ErrorMessage = "Preencha o Sexo")]
        [MaxLength(10)]
        public string Sexo_Paci { get; set; }
     
        [MaxLength(15)]
        public string CPF_Paci { get; set; }

        [MaxLength(20)]
        public string RG_Paci { get; set; }

        [MaxLength(20)]
        public string Orgao_Paci { get; set; }
        [Required(ErrorMessage = "E-Mail Inválido")]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Email_Paci { get; set; }
     
        [MaxLength(9)]
        public string CEP_Paci { get; set; }
        
        [MaxLength(50)]
        public string Endereco_Paci { get; set; }
       
        [MaxLength(10)]
        public string Numero_Paci { get; set; }
        [MaxLength(45)]
        public string Complemento_Paci { get; set; }

        [MaxLength(20)]
        public string Bairro_Paci { get; set; }
       
        [MaxLength(45)]
        public string Cidade_Paci { get; set; }
      
        [MaxLength(45)]
        public string Estado_Paci { get; set; }
        [MaxLength(50)]
        public string Mae_Paci { get; set; }
        [MaxLength(50)]
        public string Pai_Paci { get; set; }
        [MaxLength(15)]
        public string Telefone_Paci { get; set; }

        //ATIVIDADE FÍSICA
        [MaxLength(10)]
        public string AtivFisic_Paci { get; set; }

        public string Alergias_Paci  { get; set; }        

        public string DoenCron_Paci { get; set; }

        [MaxLength(15)]
        public string Diabetes_Paci { get; set; }

        [MaxLength(3)]
        public string Etilismo_Paci { get; set; }

        [MaxLength(3)]
        public string Fumanete_Paci { get; set; }

        [MaxLength(3)]
        public string Hipertens_Paci { get; set; }

        [MaxLength(3)]
        public string Neoplasia_Paci { get; set; }

        public string RemeConsta_Paci { get; set; }

        public string Peso_Paci { get; set; }

        public string Altura_Paci { get; set; }



    }
}