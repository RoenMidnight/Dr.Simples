using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AplicacaoMedicina.Models
{
    public class UsuarioLaboratorio
    {
        [Key]
        public int Id_UsuLab{ get; set; }
        [Required]
        [MaxLength(30)]
        [Index(IsUnique = true)]
        public string Usuario_UsuLab { get; set; }
        [Required]
        [MaxLength(30)]
        public string Senha_UsuLab { get; set; }
        [Required]
        [ForeignKey("Laboratorio")]
        public int ID_Labo { get; set; }

        public virtual Laboratorio Laboratorio { get; set; }
    }
}