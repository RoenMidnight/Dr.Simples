using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Exame
    {
        [Key]
        public int Id_Exame { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Data { get; set; }

        [Required]
        public Boolean Realizado { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Entrega { get; set; }

        [Required]
        public string Protocolo { get; set; }

        [Required]
        public string SenhaProtocolo { get; set; }

        public string Hash { get; set; }

        public string NomeArquivo { get; set; }

        [ForeignKey("Paciente")]
        public int ID_Paci { get; set; }
        [ForeignKey("Laboratorio")]
        public int ID_Labo { get; set; }

        public virtual Paciente Paciente { get; set; }
        public virtual Laboratorio Laboratorio { get; set; }

    }
}