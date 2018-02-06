using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Tipo
    {
        [Key]
        public int Id_Tipo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Nome_Tipo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Expertise_Tipo { get; set; }

        [Required]
        [ForeignKey("TipoLaboratorio")]
        public int Id_TipoLaboratorio { get; set; }

        public virtual TipoLaboratorio TipoLaboratorio { get; set; }

    }
}