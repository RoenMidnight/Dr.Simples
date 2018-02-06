using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Laboratorio
    {
        [Key]
        public int ID_Labo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Nome_Labo { get; set; }
        [Required]
        [MaxLength(18)]
        [Index(IsUnique = true)]
        public string CNPJ_Labo { get; set; }
        [Required]
        [MaxLength(9)]
        public string CEP_Labo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Endereco_Labo { get; set; }
        [Required]
        [MaxLength(10)]
        public string Numero_Labo { get; set; }
        [MaxLength(10)]
        public string Complemento_Labo { get; set; }
        [Required]
        [MaxLength(20)]
        public string Bairro_Labo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Cidade_Labo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Estado_Labo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Responsavel_Labo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Telefone_Labo { get; set; }
        [Required]
        [MaxLength(45)]
        public string Email_Labo { get; set; }
        
    }
}