using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Area
    {
        [Key]
        public int ID_Area { get; set; }
     
        [Required]
        [MaxLength(45)]
        public string Nome_Area { get; set; }

    }
}