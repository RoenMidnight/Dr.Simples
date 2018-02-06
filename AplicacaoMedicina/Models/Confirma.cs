using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class Confirma
    {
        public virtual MedicoConsultorio MedicoConsultorio { get; set; }
        public virtual Consultorio Consultorio { get; set; }
        public virtual Medico Medico { get; set; }
        public virtual Consulta Consulta { get; set; }
        public virtual MedicoArea MedicoArea { get; set; }
        public virtual Agenda Agenda { get; set; }
    }
}