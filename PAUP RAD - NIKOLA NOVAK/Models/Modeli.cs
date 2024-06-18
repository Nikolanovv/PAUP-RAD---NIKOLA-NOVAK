using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PAUP_RAD___NIKOLA_NOVAK.Models
{
    [Table("automobili")]
    public class Automobil
    {
        [Key]
        public int AutomobilID { get; set; } // Promijenjeno ime svojstva na AutomobilId

        public int ModelID { get; set; } // Dodano svojstvo za povezivanje s Modelom

        public string Naziv { get; set; }
    }

    
    [Table ("modeli")]
    public class Model
    {
        [Key]
        public int ModelID { get; set; }

        [Required]
        public int AutomobilID { get; set; }

        [Required]
        public string Naziv { get; set; }
    }

    

    
}