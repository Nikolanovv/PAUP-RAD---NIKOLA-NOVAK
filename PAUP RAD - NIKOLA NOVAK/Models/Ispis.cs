using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAUP_RAD___NIKOLA_NOVAK.Models
{
    [Table("ispis")]
    public class Ispis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Marka { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string KubikazaIMotor { get; set; }

        [Required]
        public string Osiguranje { get; set; }
    }
}
