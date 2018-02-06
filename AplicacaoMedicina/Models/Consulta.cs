using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Consulta
    {
        public Consulta()
        {
            Situacao_Consa = "Em Espera";
        }

        [Key]
        public int ID_Consa { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notas_Consa { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Data_Consa { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime DtMarc_Consa { get; set; }

        [Required]
        [MaxLength(30)]
        public string Situacao_Consa { get; set; }

        [MaxLength(50)]
        public string Motivo_Consa { get; set; }


        [ForeignKey("MedicoConsultorio")]
        public int ID_MediConsu { get; set; }
        [ForeignKey("Paciente")]
        public int ID_Paci { get; set; }


        public virtual MedicoConsultorio MedicoConsultorio { get; set; }
        public virtual Paciente Paciente { get; set; }
        
    }
}