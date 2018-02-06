using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class MedicoConvenio
    {
        [Key]
        public int ID_MediConv { get; set; }

        [Required]
        [ForeignKey("Medico")]
        public int ID_Medi { get; set; }

        [Required]
        [ForeignKey("Convenio")]
        public int ID_Conv { get; set; }

        public virtual Convenio Convenio { get; set; }
        public virtual Medico Medico { get; set; }


    }
}