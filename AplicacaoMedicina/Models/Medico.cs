using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Medico
    {

        [Key]
        public int ID_Medi { get; set; }

        [Required(ErrorMessage = "Nome_Medi")]
        [MaxLength(45)]
        public string Nome_Medi { get; set; }

        [Required(ErrorMessage = "CRM_Medi")]
        [MaxLength(10)]
        [Index(IsUnique = true)]
        public string CRM_Medi  { get; set; }

        [Required(ErrorMessage = "Email_Medi")]
        [MaxLength(45)]
        public string Email_Medi { get; set; }

        [Required(ErrorMessage = "Telefone_Medi")]
        [MaxLength(15)]
        public string Telefone_Medi { get; set; }
        
        public int  TipoInscri_Medi { get; set; }

        [MaxLength(15)]
        public string Valor_Medi { get; set; }
        
              
    }
}