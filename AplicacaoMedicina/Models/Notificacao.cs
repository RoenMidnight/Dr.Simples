using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Notificacao
    {
        [Key]
        public int ID_NotifPaci { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime Data_NotifPaci { get; set; }

        [ForeignKey("Paciente")]
        public int ID_Paci { get; set; }
        
        public virtual Paciente Paciente { get; set; }

    }
}