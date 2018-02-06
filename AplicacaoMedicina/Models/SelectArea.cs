using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class SelectArea
    {
        public virtual MedicoConsultorio MedicoConsultorio { get; set; }    
        public virtual MedicoArea MedicoArea { get; set; }
    }
}