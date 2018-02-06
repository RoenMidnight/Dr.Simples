using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class MedicoConsultorio
    {
        [Key]
        public int ID_MediConsu { get; set; }

        [Required]
        [ForeignKey("Medico")]
        public int ID_Medi { get; set; }

        [Required]
        [ForeignKey("Consultorio")]
        public int ID_Consu { get; set; }

        [Required]
        public bool ativo { get; set; }

        public virtual Medico Medico { get; set; }
        public virtual Consultorio Consultorio { get; set; }        
    }
}