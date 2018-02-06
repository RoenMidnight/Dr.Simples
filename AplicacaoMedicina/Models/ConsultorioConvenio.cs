using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class ConsultorioConvenio
    {
        [Key]
        public int ID_ConsConv { get; set; }

        [Required]
        [ForeignKey("Consultorio")]
        public int ID_Consu { get; set; }

        [Required]
        [ForeignKey("Convenio")]
        public int ID_Conv { get; set; }

        public virtual Consultorio Consultorio { get; set; }
        public virtual Convenio Convenio { get; set; }

    }
}