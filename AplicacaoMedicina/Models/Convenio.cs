using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Convenio
    {
        [Key]
        public int ID_Conv { get; set; }
        [MaxLength(45)]
        public string Nome_Conv { get; set; }

    }
}