using PAUP_RAD___NIKOLA_NOVAK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace PAUP_RAD___NIKOLA_NOVAK.Controllers
{
    public class AutomobiliController : Controller
    {
        // GET: Automobili
        public ActionResult Index()
        {
            ViewBag.Title = "Početna o automobilima";
            ViewBag.Marka = "Audi automobil";


            return View();
        }



        public ActionResult Popis()
        {
            AutomobiliDB automobilidb = new AutomobiliDB();
            return View(automobilidb);
        }






        public ActionResult Detalji(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Popis");
            }

            AutomobiliDB automobiliDB = new AutomobiliDB();
            Automobili automobili = automobiliDB.VratiListu().FirstOrDefault(x => x.AutomobilID == id);

            if (automobili == null)
            {
                return RedirectToAction("Popis");
            }

            return View(automobili);
        }


    }
}