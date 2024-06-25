using PAUP_RAD___NIKOLA_NOVAK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.tool.xml;

namespace PAUP_RAD___NIKOLA_NOVAK.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;

        public HomeController()
        {
            _context = new MyDbContext();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Index()
        {
            List<string> marke = _context.Automobili.Select(a => a.Naziv).Distinct().ToList();
            ViewBag.Marke = marke;
            return View();
        }

        [HttpPost]
        public ActionResult ChooseModel(string selectedMarka)
        {
            var modeli = new List<Model>();
            if (!string.IsNullOrEmpty(selectedMarka))
            {
                var automobil = _context.Automobili.FirstOrDefault(a => a.Naziv == selectedMarka);
                if (automobil != null)
                {
                    modeli = _context.Modeli.Where(m => m.AutomobilID == automobil.AutomobilID).ToList();
                }
            }

            ViewBag.Modeli = modeli;
            Session["SelectedMarka"] = selectedMarka;
            return View();
        }

        [HttpPost]
        public ActionResult ChooseEngine(string selectedModel)
        {
            var cubicCapacities = new List<string> { "1.0", "1.2", "1.4", "1.6", "1.8", "2.0" };
            var fuels = new List<string> { "Benzin", "Dizel" };

            var engines = new List<Engine>();
            foreach (var cubicCapacity in cubicCapacities)
            {
                foreach (var fuel in fuels)
                {
                    engines.Add(new Engine { CubicCapacity = cubicCapacity, FuelType = fuel });
                }
            }

            var distinctEngines = engines.GroupBy(e => new { e.CubicCapacity, e.FuelType })
                                          .Select(g => g.First())
                                          .ToList();

            ViewBag.Engines = distinctEngines;
            Session["SelectedModel"] = selectedModel;
            return View();
        }

        [HttpPost]
        public ActionResult ChooseInsurance(string selectedEngine)
        {
            Session["SelectedEngine"] = selectedEngine;
            return RedirectToAction("ChooseInsurance");
        }

        [HttpGet]
        public ActionResult ChooseInsurance()
        {
            var selectedEngine = Session["SelectedEngine"] as string;
            var selectedModel = Session["SelectedModel"] as string;
            var selectedMarka = Session["SelectedMarka"] as string;

            if (string.IsNullOrEmpty(selectedEngine) || string.IsNullOrEmpty(selectedModel) || string.IsNullOrEmpty(selectedMarka))
            {
                TempData["ErrorMessage"] = "Nedostaju podaci, molimo popunite sve podatke.";
                return RedirectToAction("Index");
            }

            ViewBag.SelectedMarka = selectedMarka;
            ViewBag.SelectedModel = selectedModel;
            ViewBag.SelectedEngine = selectedEngine;

            var osiguranja = _context.Osiguranje.ToList();
            ViewBag.Osiguranja = osiguranja;

            return View();
        }

        [HttpPost]
        public ActionResult ChooseInsurancePost(string selectedInsurance)
        {
            Session["SelectedInsurance"] = selectedInsurance;
            return RedirectToAction("Ispis");
        }

        [HttpGet]
        public ActionResult Ispis()
        {
            var selectedMarka = Session["SelectedMarka"] as string;
            var selectedModel = Session["SelectedModel"] as string;
            var selectedEngine = Session["SelectedEngine"] as string;
            var selectedInsurance = Session["SelectedInsurance"] as string;

            if (string.IsNullOrEmpty(selectedMarka) || string.IsNullOrEmpty(selectedModel) || string.IsNullOrEmpty(selectedEngine) || string.IsNullOrEmpty(selectedInsurance))
            {
                TempData["ErrorMessage"] = "Molimo popunite sve podatke prije nastavka.";
                return RedirectToAction("ChooseInsurance");
            }

            var engineParts = selectedEngine.Split(' ');
            var cubicCapacity = engineParts[0];
            var fuelType = engineParts[1];

            var fuelConsumption = GetFuelConsumption(cubicCapacity, fuelType);
            var registrationCost = GetRegistrationCost(cubicCapacity, fuelType);
            var maintenanceCost = GetMaintenanceCost(cubicCapacity, fuelType);

            int insuranceId;
            if (!int.TryParse(selectedInsurance, out insuranceId))
            {
                TempData["ErrorMessage"] = "Neispravan ID osiguranja.";
                return RedirectToAction("ChooseInsurance");
            }

            var insurancePrice = GetInsurancePrice(insuranceId);

            var ispis = new Ispis
            {
                Marka = selectedMarka,
                Model = selectedModel,
                KubikazaIMotor = selectedEngine,
                Osiguranje = selectedInsurance
            };

            try
            {
                _context.Ispis.Add(ispis);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                throw;
            }

            var totalCost = registrationCost + insurancePrice + maintenanceCost;

            var povijest = new PovijestTroskova
            {
                Marka = selectedMarka,
                Model = selectedModel,
                KubikazaIMotor = selectedEngine,
                Osiguranje = selectedInsurance,
                FuelConsumption = fuelConsumption,
                RegistrationCost = registrationCost,
                InsurancePrice = insurancePrice,
                MaintenanceCost = maintenanceCost,
                TotalCost = totalCost,
                DateCreated = DateTime.Now
            };

            _context.PovijestTroskova.Add(povijest);
            _context.SaveChanges();

            ViewBag.FuelConsumption = fuelConsumption;
            ViewBag.RegistrationCost = registrationCost;
            ViewBag.InsurancePrice = insurancePrice;
            ViewBag.MaintenanceCost = maintenanceCost;
            ViewBag.TotalCost = totalCost;

            return View();
        }



        [HttpGet]
        public ActionResult Putovanje()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Putovanje(double distance)
        {
            var selectedEngine = Session["SelectedEngine"] as string;

            if (string.IsNullOrEmpty(selectedEngine))
            {
                TempData["ErrorMessage"] = "Nedostaju podaci o motoru, molimo popunite sve podatke.";
                return RedirectToAction("Index");
            }

            var engineParts = selectedEngine.Split(' ');
            var cubicCapacity = engineParts[0];
            var fuelType = engineParts[1];

            var fuelConsumption = GetFuelConsumption(cubicCapacity, fuelType);
            var fuelNeeded = (distance / 100) * fuelConsumption;

            var putovanje = new Putovanje
            {
                Distance = distance,
                FuelNeeded = fuelNeeded
            };

            ViewBag.Putovanje = putovanje;

            return View(putovanje);
        }

        private double GetFuelConsumption(string cubicCapacity, string fuelType)
        {
            var consumptionRates = new Dictionary<string, Dictionary<string, double>>
            {
                { "1.0", new Dictionary<string, double> { { "Benzin", 5.0 }, { "Dizel", 4.0 } } },
                { "1.2", new Dictionary<string, double> { { "Benzin", 5.5 }, { "Dizel", 4.5 } } },
                { "1.4", new Dictionary<string, double> { { "Benzin", 6.0 }, { "Dizel", 5.0 } } },
                { "1.6", new Dictionary<string, double> { { "Benzin", 7.5 }, { "Dizel", 5.5 } } },
                { "1.8", new Dictionary<string, double> { { "Benzin", 8.5 }, { "Dizel", 6.0 } } },
                { "2.0", new Dictionary<string, double> { { "Benzin", 9.0 }, { "Dizel", 7.5 } } },
            };

            if (consumptionRates.ContainsKey(cubicCapacity) && consumptionRates[cubicCapacity].ContainsKey(fuelType))
            {
                return consumptionRates[cubicCapacity][fuelType];
            }

            throw new Exception("Nepoznata kombinacija kubikaže i tipa goriva.");
        }

        private Dictionary<int, double> GetInsurancePrices()
        {
            return new Dictionary<int, double>
            {
                { 1, 100.0 }, // OsiguranjeID 1
                { 2, 120.0 }, // OsiguranjeID 2
                { 3, 140.0 }, // OsiguranjeID 3
                { 4, 160.0 }, // OsiguranjeID 4
                { 5, 180.0 }, // OsiguranjeID 5
                { 6, 200.0 }, // OsiguranjeID 6
                { 7, 220.0 }, // OsiguranjeID 7
                { 8, 240.0 }, // OsiguranjeID 8
                { 9, 260.0 }, // OsiguranjeID 9
                { 10, 280.0 }, // OsiguranjeID 10
                { 11, 300.0 }, // OsiguranjeID 11
                { 12, 320.0 }, // OsiguranjeID 12
                { 13, 340.0 }, // OsiguranjeID 13
                { 14, 360.0 }, // OsiguranjeID 14
                { 15, 380.0 }  // OsiguranjeID 15
            };
        }

        private double GetInsurancePrice(int insuranceId)
        {
            var prices = GetInsurancePrices();
            if (prices.ContainsKey(insuranceId))
            {
                return prices[insuranceId];
            }

            throw new Exception("Nepoznat ID osiguranja.");
        }

        private double GetRegistrationCost(string cubicCapacity, string fuelType)
        {
            var registrationCosts = new Dictionary<string, Dictionary<string, double>>
            {
                { "1.0", new Dictionary<string, double> { { "Benzin", 250.0 }, { "Dizel", 280.0 } } },
                { "1.2", new Dictionary<string, double> { { "Benzin", 300.0 }, { "Dizel", 320.0 } } },
                { "1.4", new Dictionary<string, double> { { "Benzin", 350.0 }, { "Dizel", 370.0 } } },
                { "1.6", new Dictionary<string, double> { { "Benzin", 400.0 }, { "Dizel", 420.0 } } },
                { "1.8", new Dictionary<string, double> { { "Benzin", 450.0 }, { "Dizel", 470.0 } } },
                { "2.0", new Dictionary<string, double> { { "Benzin", 500.0 }, { "Dizel", 520.0 } } },
            };

            if (registrationCosts.ContainsKey(cubicCapacity) && registrationCosts[cubicCapacity].ContainsKey(fuelType))
            {
                return registrationCosts[cubicCapacity][fuelType];
            }

            throw new Exception("Nepoznata kombinacija kubikaže i tipa goriva.");
        }

        private double GetMaintenanceCost(string cubicCapacity, string fuelType)
        {
            var maintenanceCosts = new Dictionary<string, Dictionary<string, double>>
            {
                { "1.0", new Dictionary<string, double> { { "Benzin", 200.0 }, { "Dizel", 220.0 } } },
                { "1.2", new Dictionary<string, double> { { "Benzin", 250.0 }, { "Dizel", 270.0 } } },
                { "1.4", new Dictionary<string, double> { { "Benzin", 300.0 }, { "Dizel", 320.0 } } },
                { "1.6", new Dictionary<string, double> { { "Benzin", 350.0 }, { "Dizel", 370.0 } } },
                { "1.8", new Dictionary<string, double> { { "Benzin", 400.0 }, { "Dizel", 420.0 } } },
                { "2.0", new Dictionary<string, double> { { "Benzin", 450.0 }, { "Dizel", 470.0 } } },
            };

            if (maintenanceCosts.ContainsKey(cubicCapacity) && maintenanceCosts[cubicCapacity].ContainsKey(fuelType))
            {
                return maintenanceCosts[cubicCapacity][fuelType];
            }

            throw new Exception("Nepoznata kombinacija kubikaže i tipa goriva.");
        }

        public ActionResult GeneratePDF()
        {
            var selectedMarka = Session["SelectedMarka"] as string;
            var selectedModel = Session["SelectedModel"] as string;
            var selectedEngine = Session["SelectedEngine"] as string;
            var selectedInsurance = Session["SelectedInsurance"] as string;

            if (string.IsNullOrEmpty(selectedMarka) || string.IsNullOrEmpty(selectedModel) || string.IsNullOrEmpty(selectedEngine) || string.IsNullOrEmpty(selectedInsurance))
            {
                TempData["ErrorMessage"] = "Molimo popunite sve podatke prije nastavka.";
                return RedirectToAction("ChooseInsurance");
            }

            var engineParts = selectedEngine.Split(' ');
            var cubicCapacity = engineParts[0];
            var fuelType = engineParts[1];

            var fuelConsumption = GetFuelConsumption(cubicCapacity, fuelType);
            var registrationCost = GetRegistrationCost(cubicCapacity, fuelType);
            var maintenanceCost = GetMaintenanceCost(cubicCapacity, fuelType);

            int insuranceId;
            if (!int.TryParse(selectedInsurance, out insuranceId))
            {
                TempData["ErrorMessage"] = "Neispravan ID osiguranja.";
                return RedirectToAction("ChooseInsurance");
            }

            var insurancePrice = GetInsurancePrice(insuranceId);

            var totalCost = registrationCost + insurancePrice + maintenanceCost;

            var htmlContent = $@"
                <h2>Troškovi</h2>
                <p>Vaš odabir je uspješno spremljen.</p>
                <p>Vaš odabir automobila ima sljedeće troškove:</p>
                <p>Potrošnja goriva na 100km: {fuelConsumption} litara</p>
                <p>Potrošnja goriva na 1000km: {fuelConsumption * 10} litara</p>
                <p>Cijena registracije: {registrationCost} EUR</p>
                <p>Cijena osiguranja: {insurancePrice} EUR</p>
                <p>Godišnji trošak servisa i održavanja: {maintenanceCost} EUR</p>
                <hr />
                <p><strong>UKUPNI TROŠAK:</strong> {totalCost} EUR</p>
            ";

            using (MemoryStream stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                using (StringReader sr = new StringReader(htmlContent))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                }
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Troskovi.pdf");
            }

        }


        [HttpGet]
        public ActionResult Povijest()
        {
            var povijest = _context.PovijestTroskova.ToList();

            var povijestViewModel = povijest.Select(p => new PovijestTroskova
            {
                Id = p.Id,
                Marka = p.Marka,
                Model = _context.Modeli.FirstOrDefault(m => m.ModelID.ToString() == p.Model)?.Naziv,
                KubikazaIMotor = p.KubikazaIMotor,
                Osiguranje = _context.Osiguranje.FirstOrDefault(o => o.OsiguranjeID.ToString() == p.Osiguranje)?.Naziv,
                FuelConsumption = p.FuelConsumption,
                RegistrationCost = p.RegistrationCost,
                InsurancePrice = p.InsurancePrice,
                MaintenanceCost = p.MaintenanceCost,
                TotalCost = p.TotalCost,
                DateCreated = p.DateCreated
            }).ToList();

            return View(povijestViewModel);
        }


    }
}
