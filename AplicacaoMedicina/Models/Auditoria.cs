using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Auditoria
    {
        [Key]
        public int Id_Audi { get; set; }
        [Required]
        public string Usua_Audi { get; set; }
        [Required]
        public string Cont_Audi { get; set; }
        [Required]
        public string Acti_Audi { get; set; }
    }
}