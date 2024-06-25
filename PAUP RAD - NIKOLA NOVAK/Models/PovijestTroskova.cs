using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAUP_RAD___NIKOLA_NOVAK.Models
{

    [Table("povijesttroskova")]
    public class PovijestTroskova
    {
        [Key]
        public int Id { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string KubikazaIMotor { get; set; }
        public string Osiguranje { get; set; }
        public double FuelConsumption { get; set; }
        public double RegistrationCost { get; set; }
        public double InsurancePrice { get; set; }
        public double MaintenanceCost { get; set; }
        public double TotalCost { get; set; }
        public DateTime DateCreated { get; set; }
    }
}