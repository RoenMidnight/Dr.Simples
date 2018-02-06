using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Consultorio
    {
        public Consultorio()
        {
            Ativo = false;
        }

        [Key]
        public int ID_Consu { get; set; }

        [Required(ErrorMessage = "Preencha o Nome")]
        [MaxLength(45)]
        public string Nome_Consu { get; set; }

        [MaxLength(45)]
        public string Razao_Consu { get; set; }

        [Required(ErrorMessage = "CNPJ Inválido.")]
        [MaxLength(18)]
        [Index(IsUnique = true)]
        public string CNPJ_Consu { get; set; }

        [Required(ErrorMessage = "CEP Inválido")]
        [MaxLength(45)]
        public string CEP_Consu { get; set; }

        [Required(ErrorMessage = "Preencha o Endereço")]
        [MaxLength(45)]
        public string Endereco_Consu { get; set; }

        [Required(ErrorMessage = "Preencha o Número")]
        [MaxLength(5)]
        public string Numero_Consu { get; set; }

        [MaxLength(45)]
        public string Complemento_Consu { get; set; }

        [Required(ErrorMessage = "Preencha o Bairro")]
        [MaxLength(45)]
        public string Bairro_Consu { get; set; }

        [Required(ErrorMessage = "Preencha a Cidade")]
        [MaxLength(45)]
        public string Cidade_Consu { get; set; }

        [Required(ErrorMessage = "Preencha o Estado")]
        [MaxLength(20)]
        public string Estado_Consu { get; set; }
        
        [MaxLength(45)]
        public string Responsavel_Consu { get; set; }

        [MaxLength(15)]
        public string Telefone_Consu { get; set; }

        [Required(ErrorMessage = "E-Mail Inválido")]
        [MaxLength(45)]
        public string Email_Consu { get; set; }

        public string Descricao_Consu { get; set; }

        public bool Ativo { get; set; }

    }
}