using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PAUP_RAD___NIKOLA_NOVAK.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MyDbContext : DbContext
    {
        public DbSet<Automobili> Automobili { get; set; }
        public DbSet<Model> Modeli {  get; set; }
        public DbSet<Osiguranje> Osiguranje { get; set; }
        public DbSet<Ispis> Ispis { get; set; }
        public DbSet<Engine> Engines { get; set; }
        public DbSet<PovijestTroskova> PovijestTroskova { get; set; }

    }
}
