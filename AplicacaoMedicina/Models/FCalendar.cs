using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AplicacaoMedicina.Models
{
    public class FCalendar
    {
        public string allDay { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public bool durationEditable { get; set; }
        public bool overlap { get; set; }
    }
}