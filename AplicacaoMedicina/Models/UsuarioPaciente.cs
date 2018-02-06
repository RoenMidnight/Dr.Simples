using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class UsuarioPaciente
    {
        [Key]
        public int Id_UsuPaci { get; set; }
        [Required]
        [MaxLength(50)]
        [Index(IsUnique =true)]
        public string Usuario_UsuPaci { get; set; }
        [Required]
        [MaxLength(200)]
        public string Senha_UsuPaci { get; set; }
        [Required]
        [ForeignKey("Pacientes")]
        public int ID_Paci { get; set; }

        public virtual Paciente Pacientes { get; set; }

    }
}