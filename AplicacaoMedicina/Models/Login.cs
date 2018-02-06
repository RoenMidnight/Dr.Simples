using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Login
    {
        [Required]
        [Index(IsUnique = true)]
        public string Usuario_Login { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Senha_Login { get; set; }
    }
}