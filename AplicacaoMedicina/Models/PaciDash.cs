using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplicacaoMedicina.Models
{
    public class PaciDash
    {
        public virtual ConsultorioConvenio ConsultorioConvenio { get; set; }
        public virtual Consulta Consulta { get; set; }
    }
}