using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP_RAD___NIKOLA_NOVAK.Models
{
    [Table("engine")] 
    public class Engine
    {
        [Key]
        public int EngineID { get; set; }

        [Required]
        public string CubicCapacity { get; set; }

        [Required]
        public string FuelType { get; set; }
    }
}
