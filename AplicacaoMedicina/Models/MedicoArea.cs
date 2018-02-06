using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class MedicoArea
    {
        [Key]
        public int ID_MediArea { get; set; }

        [Required]
        [ForeignKey("Area")]
        public int ID_Area { get; set; }

        [Required]
        [ForeignKey("Medico")]
        public int ID_Medi { get; set; }

        public virtual Area Area { get; set; }
        public virtual Medico Medico { get; set; }
    }
}