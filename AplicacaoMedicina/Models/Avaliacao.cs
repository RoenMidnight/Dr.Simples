using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Avaliacao
    {
        public Avaliacao()
        {
            favo_Aval = false;
            rate_Aval = 0;
        }

        [Key]
        public int ID_Aval { get; set; }

        public bool favo_Aval { get; set; }

        public int rate_Aval { get; set; }

        [DataType(DataType.MultilineText)]
        public string review_Aval { get; set; }

        [Required]
        [ForeignKey("Paciente")]
        public int ID_Paci { get; set; }

        [Required]
        [ForeignKey("MedicoConsultorio")]
        public int ID_MediConsu { get; set; }

        public virtual Paciente Paciente { get; set; }
        public virtual MedicoConsultorio MedicoConsultorio { get; set; }
    }
}