using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PAUP_RAD___NIKOLA_NOVAK.Models
{
    [Table("osiguranje")]
    public class Osiguranje
    {
        [Key]
        public int OsiguranjeID { get; set; }

        [Required]
        public string Naziv { get; set; }
    }
}