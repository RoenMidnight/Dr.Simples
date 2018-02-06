using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AplicacaoMedicina.Models
{
    public class Agenda
    {
        [Key]
        public int ID_Agenda { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Data_Agenda { get; set; }

        public string DSeman_Agenda { get; set; }

        [Required]
        [ForeignKey("MedicoConsultorio")]
        public int ID_MediConsul { get; set; }

        [Required]
        public bool ativo { get; set; }

        public virtual MedicoConsultorio MedicoConsultorio { get; set; }
    }
}