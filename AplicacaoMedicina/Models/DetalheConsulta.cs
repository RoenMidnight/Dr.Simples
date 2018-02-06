using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AplicacaoMedicina.Models
{
    public class DetalheConsulta
    {
        [Key]
        public int ID_DetaConsu { get; set; }     

        public virtual Consulta Consulta { get; set; }
        public virtual MedicoArea MedicoArea { get; set; }
    }
}