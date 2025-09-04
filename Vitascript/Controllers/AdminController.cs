using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vitascript.Context;
using Vitascript.Models;
using System.Data.Entity;
using Vitascript.DTOs;
using Vitascript.CustomAuthorization;
using System.Threading.Tasks;
using Vitascript.ViewModel;

namespace Vitascript.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        // GET: Admin
        private ModelVitascript db;
        public AdminController()
        {
            db = new ModelVitascript();
        }

        // ---------------- Dashboard ----------------
        public ActionResult Index()
        {
            try
            {
                int adminId = (int)Session["UserId"];

                var admin = db.Users
                              .Include("UserType")
                              .FirstOrDefault(u => u.Id == adminId && u.UserType.UserTypeName == "Admin");

                if (admin == null)
                    return HttpNotFound();

                ViewBag.TotalDoctors = db.Users.Count(u => u.UserType.UserTypeName == "Doctor");
                ViewBag.TotalPatients = db.Users.Count(u => u.UserType.UserTypeName == "Patient");
                ViewBag.TotalPharmacies = db.Users.Count(u => u.UserType.UserTypeName == "Pharmacy");
                ViewBag.TotalGenericMedicines = db.GenericMedicines.Count();
                ViewBag.TotalBrandMedicines = db.BrandedMedicines.Count();
                ViewBag.TotalBrands = db.Brands.Count();
                ViewBag.TotalPrescriptions = db.Prescriptions.Count();
                ViewBag.TotalTransactions = db.MedicineTransactions.Count();

                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                var dailySales = db.MedicineTransactions
                    .Where(mt => mt.DatePurchased.Month == currentMonth && mt.DatePurchased.Year == currentYear)
                    .GroupBy(mt => DbFunctions.TruncateTime(mt.DatePurchased))
                    .Select(g => new DailySalesVM
                    {
                        Date = g.Key.Value,
                        TotalSales = (int)g.Sum(mt => mt.Price)
                    })
                    .OrderBy(g => g.Date)
                    .ToList();

                ViewBag.DailySales = dailySales;

                var mostSold = db.MedicineTransactions
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
                ViewBag.MostSoldMedicines = mostSold;

                var mostPrescribedMedicines = db.PrescribedMedicines
                        .GroupBy(pm => pm.GenericMedicine.Name)
                        .Select(g => new MostSoldMedicineVM
                        {
                            Name = g.Key,
                            TotalSold = g.Count()
                        })
                        .OrderByDescending(g => g.TotalSold)
                        .Take(7) // Optional: limit top 7
                        .ToList();

                ViewBag.MostPrescribedMedicines = mostPrescribedMedicines;

                return View(admin);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading dashboard: " + ex.Message);
                return View();
            }
        }



        // ---------------- Doctor Type ---------------- Dome all
        public ActionResult DoctorTypes()
        {
            return View(db.DoctorTypes.ToList());
        }

        [HttpGet]
        public ActionResult AddDoctorType(DoctorType dr)
        {
            try
            {
                if (dr.Id > 0)
                {
                    return View(dr);
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.NoData = 0;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading doctor type: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateDoctorType(DoctorType doctorType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (doctorType.Id <= 0)
                    {
                        db.DoctorTypes.Add(doctorType);
                        db.SaveChanges();
                        TempData["MsgAdd"] = "Doctor Type added successfully";
                        return RedirectToAction("DoctorTypes");
                    }
                    else
                    {
                        db.Entry(doctorType).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        TempData["MsgEdit"] = "Doctor Type updated successfully";
                        return RedirectToAction("DoctorTypes");
                    }
                }

                return View("AddDoctorType");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving doctor type: " + ex.Message);
                return View("AddDoctorType", doctorType);
            }
        }


        // ---------------- Brand ---------------- Dome all
        public ActionResult Brands()
        {
            return View(db.Brands.ToList());
        }

        // GET: Add or Edit Brand
        [HttpGet]
        public ActionResult AddBrand(Brand br)
        {
            try
            {
                if (br.Id > 0)
                {


                    return View(br);
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.NoData = 0;
                    return View(); // Add mode: empty form
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading brand: " + ex.Message);
                return View();
            }
        }

        // POST: Create or Update Brand
        [HttpPost]
        public ActionResult CreateBrand(Brand brand)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (brand.Id <= 0)
                    {
                        db.Brands.Add(brand);
                        db.SaveChanges();
                        TempData["MsgAdd"] = "Brand added successfully";
                        return RedirectToAction("Brands");
                    }
                    else
                    {
                        db.Entry(brand).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["MsgEdit"] = "Brand updated successfully";
                        return RedirectToAction("Brands");
                    }
                }

                return View(brand);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving brand: " + ex.Message);
                return View("AddBrand", brand);
            }
        }


        // ---------------- Generic Medicine ---------------- Dome all
        public ActionResult GenericMedicines()
        {
            return View(db.GenericMedicines.ToList());
        }

        [HttpGet]
        public ActionResult AddGenericMedicine(GenericMedicine med)
        {
            try
            {
                if (med.Id > 0)
                {
                    return View(med);
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.NoData = 0;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading generic medicine: " + ex.Message);
                return View();
            }
        }


        [HttpPost]
        public ActionResult CreateGenericMedicine(GenericMedicine med)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (med.Id <= 0)
                    {
                        db.GenericMedicines.Add(med);
                        db.SaveChanges();
                        TempData["MsgAdd"] = "Generic Medicine added successfully";
                        return RedirectToAction("GenericMedicines");
                    }
                    else
                    {
                        db.Entry(med).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["MsgEdit"] = "Generic Medicine updated successfully";
                        return RedirectToAction("GenericMedicines");
                    }
                }

                return View("AddGenericMedicine");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving generic medicine: " + ex.Message);
                return View("AddGenericMedicine", med);
            }
        }



        // ---------------- Branded Medicine ----------------  Done all
        public ActionResult BrandedMedicines()
        {
            var meds = db.BrandedMedicines.Include(b => b.Brand).Include(b => b.GenericMedicine).ToList();
            return View(meds);
        }

        [HttpGet]
        public ActionResult AddBrandedMedicine(BrandedMedicine med)
        {
            try
            {
                ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name", med?.BrandId);
                ViewBag.GenericMedicineId = new SelectList(db.GenericMedicines, "Id", "Name", med?.GenericMedicineId);

                if (med != null && med.Id > 0)
                {
                    var existingMed = db.BrandedMedicines.Find(med.Id);
                    if (existingMed != null)
                        return View(existingMed);
                    else
                    {
                        TempData["Error"] = "Branded Medicine not found.";
                        return RedirectToAction("BrandedMedicines");
                    }
                }
                else
                {
                    ModelState.Clear();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading branded medicine: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateBrandedMedicine(BrandedMedicine med)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (med.Id <= 0)
                    {
                        db.BrandedMedicines.Add(med);
                        db.SaveChanges();
                        TempData["MsgAdd"] = "Branded Medicine added successfully.";
                    }
                    else
                    {
                        db.Entry(med).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["MsgEdit"] = "Branded Medicine updated successfully.";
                    }

                    return RedirectToAction("BrandedMedicines");
                }

                ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name", med.BrandId);
                ViewBag.GenericMedicineId = new SelectList(db.GenericMedicines, "Id", "Name", med.GenericMedicineId);
                return View("AddBrandedMedicine", med);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving branded medicine: " + ex.Message);
                ViewBag.BrandId = new SelectList(db.Brands, "Id", "Name", med.BrandId);
                ViewBag.GenericMedicineId = new SelectList(db.GenericMedicines, "Id", "Name", med.GenericMedicineId);
                return View("AddBrandedMedicine", med);
            }
        }

        // ---------------- Manage Users ----------------Done all
        public ActionResult Users()
        {
            var users = db.Users.Include(u => u.UserType).Include(u => u.DoctorType).Include(u => u.Pharmacy).ToList();
            return View(users);
        }

        [HttpGet]
        public ActionResult AddUser(UserDTO model)
        {
            ViewBag.UserTypes = new SelectList(db.UserTypes, "Id", "UserTypeName");
            ViewBag.DoctorTypes = new SelectList(db.DoctorTypes, "Id", "Name");
            ViewBag.Cities = Enum.GetNames(typeof(Location)).ToList();

            if (model != null && model.Id > 0)
            {
                var user = db.Users.Find(model.Id);
                if (user == null)
                    return HttpNotFound();

                model.Name = user.Name;
                model.Password = user.Password;
                model.Phone = user.Phone;
                model.DOB = user.DOB;
                model.Email = user.Email;
                model.Address = user.Address;
                model.City = user.City;
                model.UserTypeId = user.UserTypeId;
                model.DoctorTypeId = user.DoctorTypeId;
                model.PharmacyId = user.PharmacyId;

                return View(model); // Editing
            }

            ModelState.Clear();
            return View(); // New user add
        }

        public ActionResult UsersDtls(int userID)
        {
            try
            {
                var users = db.Users.Include(u => u.UserType)
                .Include(u => u.DoctorType)
                .Include(u => u.Pharmacy)
                .FirstOrDefault(u => u.Id == userID);

                return View(users);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading user details: " + ex.Message);
                return View(new User());
            }
        }

        [HttpPost]
        public ActionResult CreateUser(UserDTO model, string PharmacyName)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id <= 0) // ADD
                    {
                        var user = new User
                        {
                            Name = model.Name,
                            Password = model.Password,
                            Phone = model.Phone,
                            DOB = model.DOB,
                            Email = model.Email,
                            Address = model.Address,
                            City = model.City,
                            UserTypeId = model.UserTypeId,
                            DoctorTypeId = model.DoctorTypeId
                        };

                        if (model.UserTypeId == 4 && !string.IsNullOrEmpty(PharmacyName))
                        {
                            var pharmacy = new Pharmacy { PharmacyName = PharmacyName };
                            db.Pharmacies.Add(pharmacy);
                            db.SaveChanges();
                            user.PharmacyId = pharmacy.Id;
                        }

                        db.Users.Add(user);
                        db.SaveChanges();
                        TempData["MsgAdd"] = "User added successfully";
                    }
                    else // EDIT
                    {
                        var user = db.Users.Find(model.Id);
                        if (user == null)
                            return HttpNotFound();

                        user.Name = model.Name;
                        user.Password = model.Password;
                        user.Phone = model.Phone;
                        user.DOB = model.DOB;
                        user.Email = model.Email;
                        user.Address = model.Address;
                        user.City = model.City;
                        user.UserTypeId = model.UserTypeId;
                        user.DoctorTypeId = model.DoctorTypeId;

                        if (model.UserTypeId == 4)
                        {
                            if (user.PharmacyId == null && !string.IsNullOrEmpty(PharmacyName))
                            {
                                var pharmacy = new Pharmacy { PharmacyName = PharmacyName };
                                db.Pharmacies.Add(pharmacy);
                                db.SaveChanges();
                                user.PharmacyId = pharmacy.Id;
                            }
                            else if (user.PharmacyId != null && !string.IsNullOrEmpty(PharmacyName))
                            {
                                var existingPharmacy = db.Pharmacies.Find(user.PharmacyId);
                                if (existingPharmacy != null)
                                {
                                    existingPharmacy.PharmacyName = PharmacyName;
                                }
                            }
                        }

                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        TempData["MsgEdit"] = "User updated successfully";
                    }

                    return RedirectToAction("Users");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Operation failed: " + ex.Message);
            }

            ViewBag.UserTypes = new SelectList(db.UserTypes, "Id", "UserTypeName", model.UserTypeId);
            ViewBag.DoctorTypes = new SelectList(db.DoctorTypes, "Id", "Name", model.DoctorTypeId);
            ViewBag.Cities = Enum.GetNames(typeof(Location)).ToList();

            return View("AddUser", model);
        }

        // ---------------- Prescriptions ---------------- Done all
        public async Task<ActionResult> Prescriptions()
        {
            try
            {
                int doctorId = (int)Session["UserId"];

                var prescriptionController = new PrescriptionController();
                var result = await prescriptionController.All() as JsonResult;

                var prescriptions = new List<PrescriptionViewDTO>();
                if (result?.Data is IEnumerable<PrescriptionViewDTO> data)
                {
                    prescriptions = data.ToList();
                }

                return View(prescriptions);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading prescriptions: " + ex.Message);
                return View(new List<PrescriptionViewDTO>());
            }
        }

        public async Task<ActionResult> PrescriptionDetails(int Id)
        {
            try
            {
                var doctorId = (int)Session["UserId"];

                // Get the prescription details from the API
                var prController = new PrescriptionController();
                var prResult = await prController.Details(Id) as JsonResult;

                var prescription = prResult?.Data as PrescriptionViewDTO;
                if (prescription == null)
                    return HttpNotFound("Prescription not found");

                // Get the medical history of the patient
                var mhController = new MedicalHistoryController();
                var mhResult = await mhController.ByPatient(prescription.PatientID) as JsonResult;

                var medicalHistories = new List<MedicalHistoryDTO>();
                if (mhResult?.Data is IEnumerable<MedicalHistoryDTO> histories)
                    medicalHistories = histories.ToList();

                // Get patient info from DB
                using (var db = new Context.ModelVitascript())
                {
                    var patient = db.Patients
                        .Include(p => p.PatientUser)
                        .FirstOrDefault(p => p.PatientId == prescription.PatientID);

                    if (patient == null)
                        return HttpNotFound("Patient not found or not assigned to this doctor");

                    var viewModel = new PatientDetailsViewModel
                    {
                        PatientUser = patient.PatientUser,
                        MedicalHistories = medicalHistories,
                        Prescriptions = new List<PrescriptionViewDTO> { prescription }
                    };

                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading prescription details: " + ex.Message);
                return View(new PatientDetailsViewModel());
            }
        }

        public ActionResult Transactions()
        {
            try
            {

                var sales = db.Payments
                              .Include("Patient")
                              .Include("Prescription")
                              .Include("PaymentType")
                              .ToList();

                return View(sales);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading transactions: " + ex.Message);
                return View(new List<Payment>());
            }
        }

        public ActionResult TransactionDTLs(int paymentId)
        {
            try
            {
                var transactions = db.MedicineTransactions
                    .Include(mt => mt.PrescribedMedicine.Prescription)
                    .Include(mt => mt.PrescribedMedicine.GenericMedicine)
                    .Include(mt => mt.Patient)
                    .Include(mt => mt.Payment)
                    .Where(mt => mt.PaymentId == paymentId)
                    .ToList();

                foreach (var transaction in transactions)
                {
                    var genericId = transaction.PrescribedMedicine.GenericMedicineId;

                    var brandedMedicine = db.PharmacyInventories
                        .Include(pi => pi.BrandedMedicine.Brand)
                        .Where(pi =>
                            pi.BrandedMedicine.GenericMedicineId == genericId)
                        .Select(pi => pi.BrandedMedicine)
                        .FirstOrDefault();

                    ViewData[$"branded_{transaction.Id}"] = brandedMedicine;
                }

                return View(transactions);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error loading transaction details: " + ex.Message);
                return View(new List<MedicineTransaction>());
            }
        }

        public ActionResult AdminProfile()
        {
            int adminUserId = (int)Session["UserId"];
            var adminUser = db.Users
                              .FirstOrDefault(u => u.Id == adminUserId && u.UserTypeId == 3);

            if (adminUser == null)
                return HttpNotFound();

            return View(adminUser);
        }

    }
}