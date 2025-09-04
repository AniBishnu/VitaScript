using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vitascript.Context;
using Vitascript.ViewModel;
using Vitascript.Models;
using System.Data.Entity; 
using Vitascript.CustomAuthorization;
using Vitascript.DTOs;
using System.Threading.Tasks;

namespace Vitascript.Controllers
{
    [PharmacyAuthorize]
    public class PharmacyController : Controller
    {
        private ModelVitascript db;
        public PharmacyController()
        {
            db = new ModelVitascript();
        }
        // GET: Pharmacy
        public ActionResult Index()
        {
            var userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);

            if (user == null || user.PharmacyId == null)
                return HttpNotFound();

            int pharmacyId = user.PharmacyId.Value;

            var totalSales = db.Payments
                .Where(p => p.PharmacyId == pharmacyId)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            var totalMedicines = db.PharmacyInventories
                .Count(pi => pi.PharmacyId == pharmacyId);

            // Total unique users interacted (patients)
            var totalUsersInteracted = db.MedicineTransactions
                .Where(t => t.PharmacyId == pharmacyId)
                .Select(t => t.PatientId)
                .Distinct()
                .Count();

            var mostSold = db.MedicineTransactions
                .Where(mt => mt.PharmacyId == pharmacyId)
                .GroupBy(mt => mt.PrescribedMedicine.GenericMedicine.Name)
                .Select(g => new MostSoldMedicineVM
                {
                    Name = g.Key,
                    TotalSold = g.Sum(m => m.QuantityPurchased)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(5)
                .ToList();
            var now = DateTime.Now;

            var totalMonthlySales = db.MedicineTransactions
                .Where(mt => mt.PharmacyId == pharmacyId &&
                             mt.DatePurchased.Month == now.Month &&
                             mt.DatePurchased.Year == now.Year)
                .Sum(mt => (int?)mt.Price) ?? 0;


            var dailySales = db.MedicineTransactions
                .Where(mt => mt.PharmacyId == pharmacyId)
                .GroupBy(mt => DbFunctions.TruncateTime(mt.DatePurchased))
                .Select(g => new DailySalesVM
                {
                    Date = g.Key.Value,
                    TotalSales = (int)g.Sum(m => m.Price)
                })
                .OrderBy(g => g.Date)
                .ToList();


            var viewModel = new PharmacyDashboardVM
            {
                User = user,
                TotalSales = totalSales,
                TotalMedicines = totalMedicines,
                TotalUsersInteracted = totalUsersInteracted,
                MostSoldMedicines = mostSold,
                DailySales = dailySales,
                TotalMonthlySales = totalMonthlySales
            };



            return View(viewModel);
        }

        public ActionResult Inventory()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            var pharmacist = db.Users.Include(u => u.Pharmacy).FirstOrDefault(u => u.Id == userId);

            if (pharmacist == null || pharmacist.PharmacyId == null)
            {
                return RedirectToAction("Index", "Home"); // Or show an error
            }

            int pharmacyId = pharmacist.PharmacyId.Value;

            var inventoryData = db.PharmacyInventories
                .Include(pi => pi.BrandedMedicine.Brand)
                .Include(pi => pi.BrandedMedicine.GenericMedicine)
                .Where(pi => pi.PharmacyId == pharmacyId)
                .Select(pi => new PharmacyInventoryViewModel
                {
                    Id = pi.BrandedMedicine.Id,
                    MedicineName = pi.BrandedMedicine.MedicineName,
                    BrandName = pi.BrandedMedicine.Brand.Name,
                    GenericName = pi.BrandedMedicine.GenericMedicine.Name,
                    BuyingPrice = pi.BrandedMedicine.BuyingPrice,
                    MRP = pi.BrandedMedicine.MRP,
                    QuantityAvailable = pi.AvailableQuantity
                }).ToList();

            return View(inventoryData);
        }

        public async Task<ActionResult> SellMedicine()
        {
            var prescriptionController = new PrescriptionController();
            var result = await prescriptionController.All() as JsonResult;

            var prescriptions = new List<PrescriptionViewDTO>();
            if (result?.Data is IEnumerable<PrescriptionViewDTO> data)
            {
                prescriptions = data.ToList();
            }

            return View(prescriptions);
        }

        public ActionResult ProvideMedicine(int id)
        {
            var prescription = db.Prescriptions
                .Include("Patient")
                .Include("PrescribedMedicines.GenericMedicine.BrandedMedicines.Brand")
                .FirstOrDefault(p => p.Id == id);

            if (prescription == null)
            {
                return HttpNotFound();
            }

            var prescribedMedicines = prescription.PrescribedMedicines.Select(pm => new ProvideMedicineRowVM
            {
                PrescribedMedicineId = pm.Id,
                GenericName = pm.GenericMedicine.Name,
                RemainingQuantity = pm.RemainingQuantity,
                BrandedOptions = pm.GenericMedicine.BrandedMedicines.Select(bm => new BrandedMedicineOption
                {
                    Id = bm.Id,
                    Name = bm.MedicineName,
                    BrandName = bm.Brand.Name,
                    Price = bm.MRP,
                    QuantityAvailable = bm.QuantityAvailable
                }).ToList()
            }).ToList();

            var viewModel = new ProvideMedicineVM
            {
                PrescriptionId = prescription.Id,
                PrescriptionCode = prescription.PrescriptionCode,
                PatientName = prescription.Patient.Name,
                patientID = prescription.Patient.Id,
                Medicines = prescribedMedicines
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult PayBill(PayBillVM model)
        {
            var patientExists = db.Users.Any(u => u.Id == model.PatientId);
            if (!patientExists)
            {
                ViewBag.ErrorMessage = "Patient does not exist. Please check the Patient ID.";
                ModelState.AddModelError(model.PatientId.ToString(), "Patient does not exist.");
                model.PaymentOptions = db.PaymentTypes.ToList();
                return View("PayBill", model);
            }

            model.PaymentOptions = db.PaymentTypes.ToList();

            return View(model);
        }


        [HttpPost]
        public ActionResult ConfirmPayment(PayBillVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill in all required fields.";
                model.PaymentOptions = db.PaymentTypes.ToList();
                return View("PayBill", model);
            }
            int userId = Convert.ToInt32(Session["UserId"]);
            int pharmacyId = db.Users.FirstOrDefault(u => u.Id == userId)?.PharmacyId ?? 0;
            var patientExists = db.Users.Any(u => u.Id == model.PatientId);
            if (!patientExists)
            {
                ViewBag.ErrorMessage = "Patient does not exist. Please check the Patient ID.";
                ModelState.AddModelError(model.PatientId.ToString(), "Patient does not exist.");
                model.PaymentOptions = db.PaymentTypes.ToList();  
                return View("PayBill", model);
            }
            var payment = new Payment
            {
                Amount = model.GrandTotal,
                Mobilenumber = Convert.ToInt32(model.MobileNumber),
                PatientId = model.PatientId,
                PaymentTypeId = model.SelectedPaymentTypeId,
                PharmacyId = pharmacyId,
                PrescriptionId = model.PrescriptionId
            };

            db.Payments.Add(payment);
            db.SaveChanges();

            foreach (var item in model.CartItems)
            {
                bool exists = db.PrescribedMedicines.Any(pm => pm.Id == item.PrescribedMedicineId);
                if (!exists)
                {
                    ModelState.AddModelError("", $"Prescribed medicine with ID {item.PrescribedMedicineId} does not exist.");
                    model.PaymentOptions = db.PaymentTypes.ToList();
                    ViewBag.ErrorMessage = $"Prescribed medicine with ID {item.PrescribedMedicineId} does not exist.";
                    return View("PayBill", model);
                }
                var transaction = new MedicineTransaction
                {
                    PrescribedMedicineId = item.PrescribedMedicineId,
                    QuantityPurchased = item.Quantity,
                    Price = item.TotalPrice,
                    PharmacyId = payment.PharmacyId,
                    PatientId = payment.PatientId,
                    PaymentId = payment.Id
                };
                db.MedicineTransactions.Add(transaction);

                //Update RemainingQuantity in PrescribedMedicine
                var prescribedMed = db.PrescribedMedicines.FirstOrDefault(pm => pm.Id == item.PrescribedMedicineId);
                if (prescribedMed != null)
                {
                    prescribedMed.RemainingQuantity -= item.Quantity;
                    if (prescribedMed.RemainingQuantity < 0)
                        prescribedMed.RemainingQuantity = 0;
                }

                // Update AvailableQuantity in PharmacyInventory for this branded medicine
                var prescribed = db.PrescribedMedicines.FirstOrDefault(p => p.Id == item.PrescribedMedicineId);
                if (prescribed != null)
                {
                    prescribed.RemainingQuantity -= item.Quantity;
                    if (prescribed.RemainingQuantity < 0)
                        prescribed.RemainingQuantity = 0;
                }
            }

            db.SaveChanges();
            return RedirectToAction("Sales");
        }


        public ActionResult PharmacyProfile()
        {
            int pharmacyUserId = (int)Session["UserId"];
            var pharmacyUser = db.Users
                                .Include("Pharmacy")   
                                .FirstOrDefault(u => u.Id == pharmacyUserId && u.UserTypeId == 4);

            if (pharmacyUser == null)
                return HttpNotFound();

            return View(pharmacyUser);
        }

        public ActionResult Sales()
        {
            int userId = (int)Session["UserId"];

            var user = db.Users.FirstOrDefault(u => u.Id == userId && u.UserTypeId == 4);

            if (user == null || user.PharmacyId == null)
                return HttpNotFound("Pharmacy user not found or not associated with a pharmacy.");

            int pharmacyId = user.PharmacyId.Value;

            var sales = db.Payments
                          .Include("Patient")
                          .Include("Prescription")
                          .Include("PaymentType")
                          .Where(p => p.PharmacyId == pharmacyId)
                          .ToList();

            return View(sales); 
        }


        public ActionResult TransactionDetails(int paymentId)
        {
            int userId = (int)Session["UserId"];
            var user = db.Users.FirstOrDefault(u => u.Id == userId && u.UserTypeId == 4);

            if (user?.PharmacyId == null)
                return HttpNotFound("Pharmacy not found.");

            int pharmacyId = user.PharmacyId.Value;

            // Get all transactions for the selected payment ID and pharmacy
            var transactions = db.MedicineTransactions
                .Include(mt => mt.PrescribedMedicine.Prescription)
                .Include(mt => mt.PrescribedMedicine.GenericMedicine)
                .Include(mt => mt.Patient)
                .Include(mt => mt.Payment)
                .Where(mt => mt.PaymentId == paymentId && mt.PharmacyId == pharmacyId)
                .ToList();

            foreach (var transaction in transactions)
            {
                var genericId = transaction.PrescribedMedicine.GenericMedicineId;

                var brandedMedicine = db.PharmacyInventories
                    .Include(pi => pi.BrandedMedicine.Brand)
                    .Where(pi =>
                        pi.PharmacyId == pharmacyId &&
                        pi.BrandedMedicine.GenericMedicineId == genericId)
                    .Select(pi => pi.BrandedMedicine)
                    .FirstOrDefault();

                ViewData[$"branded_{transaction.Id}"] = brandedMedicine;
            }

            return View(transactions);
        }

        [HttpGet]
        public ActionResult AddPharmacyMedicine()
        {
            var model = new AddMedicineVM
            {
                GenericMedicines = db.GenericMedicines.Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                }).ToList(),

                Brands = new List<SelectListItem>(),
                BrandedMedicines = new List<SelectListItem>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPharmacyMedicine(AddMedicineVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid input. Please try again.";
                return RedirectToAction("AddPharmacyMedicine");
            }

            var userId = Convert.ToInt32(Session["UserId"]);
            var pharmacist = db.Users.FirstOrDefault(u => u.Id == userId);

            if (pharmacist?.PharmacyId == null)
            {
                TempData["Error"] = "No pharmacy assigned.";
                return RedirectToAction("AddPharmacyMedicine");
            }

            int pharmacyId = pharmacist.PharmacyId.Value;

            var brandedMedicine = db.BrandedMedicines
                .FirstOrDefault(b => b.Id == model.BrandedMedicineId
                                  && b.GenericMedicineId == model.GenericMedicineId
                                  && b.BrandId == model.BrandId);

            if (brandedMedicine == null)
            {
                TempData["Error"] = "Branded medicine not found.";
                return RedirectToAction("AddPharmacyMedicine");
            }

            if (model.Quantity <= 0 || model.Quantity > brandedMedicine.QuantityAvailable)
            {
                TempData["Error"] = $"Cannot add more than available stock ({brandedMedicine.QuantityAvailable}).";
                return RedirectToAction("AddPharmacyMedicine");
            }

            // Add to or update PharmacyInventory
            var inventory = db.PharmacyInventories.FirstOrDefault(i =>
                i.BrandedMedicineId == brandedMedicine.Id && i.PharmacyId == pharmacyId);

            if (inventory != null)
                inventory.AvailableQuantity += model.Quantity;
            else
                db.PharmacyInventories.Add(new PharmacyInventory
                {
                    BrandedMedicineId = brandedMedicine.Id,
                    PharmacyId = pharmacyId,
                    AvailableQuantity = model.Quantity
                });

            // Update branded medicine stock
            brandedMedicine.QuantityAvailable -= model.Quantity;

            db.SaveChanges();
            TempData["Success"] = "Medicine successfully added to pharmacy inventory.";

            return RedirectToAction("AddPharmacyMedicine");
        }

        public JsonResult GetBrandsByGeneric(int genericId)
        {
            var brands = db.BrandedMedicines
                .Where(b => b.GenericMedicineId == genericId)
                .Select(b => b.Brand)
                .Distinct()
                .Select(b => new { b.Id, b.Name })
                .ToList();

            return Json(brands, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBrandedByGenericAndBrand(int genericId, int brandId)
        {
            var meds = db.BrandedMedicines
                .Where(b => b.GenericMedicineId == genericId && b.BrandId == brandId)
                .Select(b => new
                {
                    b.Id,
                    Name = b.MedicineName + " (Available: " + b.QuantityAvailable + ")"
                }).ToList();

            return Json(meds, JsonRequestBehavior.AllowGet);
        }



    }
}