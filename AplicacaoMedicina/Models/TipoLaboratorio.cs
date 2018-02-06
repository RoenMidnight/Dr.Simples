using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class TipoLaboratorio
    {
        [Key]
        public int Id_TipoLaboratorio { get; set; }
        
        [Required]
        [ForeignKey("Laboratorio")]
        public int ID_Labo { get; set; }

        public virtual Laboratorio Laboratorio { get; set; }
    }
}