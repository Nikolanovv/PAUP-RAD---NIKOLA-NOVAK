using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAUP_RAD___NIKOLA_NOVAK.Models
{
    [Table("automobili")]
    public class Automobili
    {
        [Key]
        [Display (Name =" ID Automobila")]
        public int AutomobilID { get; set; }





        public string Naziv { get; set; }

        
    }
}