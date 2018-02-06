using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class UsuarioConsultorio
    {
        [Key]
        public int Id_UsuCons { get; set; }
        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Usuario_UsuCons { get; set; }
        [Required]
        [MaxLength(200)]
        public string Senha_UsuCons { get; set; }
        [Required]
        [ForeignKey("Consultorio")]
        public int ID_Consu { get; set; }

        public virtual Consultorio Consultorio { get; set; }
    }
}