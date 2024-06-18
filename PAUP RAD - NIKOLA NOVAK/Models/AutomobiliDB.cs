using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PAUP_RAD___NIKOLA_NOVAK.Models
{
    public class AutomobiliDB
    {
        private static List<Automobili> lista = null; // Promijenjeno

        public AutomobiliDB()
        {
            if (lista == null) // Dodano
            {
                lista = new List<Automobili>(); // Promijenjeno

                lista.Add(new Automobili()
                {
                    AutomobilID = 1,
                    Naziv = "Audi"
                });

                lista.Add(new Automobili()
                {
                    AutomobilID = 2,
                    Naziv = "Peugeot"
                });

                lista.Add(new Automobili()
                {
                    AutomobilID = 3,
                    Naziv = "Mercedes"
                });

                lista.Add(new Automobili()
                {
                    AutomobilID = 4,
                    Naziv = "Suzuki"
                });
            }
        }

        public List<Automobili> VratiListu()
        {
            return lista;
        }
    }

}