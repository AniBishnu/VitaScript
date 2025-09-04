using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vitascript.Context;
using Vitascript.DTOs;
using Vitascript.Models;
using Vitascript.ViewModel;

namespace Vitascript.Controllers
{
    public class HomeController : Controller
    {
        private ModelVitascript db;

        public HomeController()
        {
            db = new ModelVitascript();
        }

        public ActionResult Index()
        {
            return View();
        }


        // List all doctors
        public ActionResult Doctors()
        {
            var doctorUsers = db.Users
                                .Where(u => u.UserTypeId == 2)
                                .Include(u => u.DoctorType)
                                .Include(u => u.UserType)
                                .ToList();

            var doctors = doctorUsers.Select(u => new DrDrType
            {
                Doctor = u,
                DoctorType = u.DoctorType,
                UserType = u.UserType
            }).ToList();

            ViewBag.DoctorTypes = db.DoctorTypes.ToList();


            return View(doctors);
        }


        // Doctor details by ID
        public ActionResult DoctorDetails(int id)
        {
            var doctor = db.Users.FirstOrDefault(u => u.Id == id && u.UserTypeId == 2);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }


        // List all doctors
        public ActionResult Pharmacies()
        {
            var PharmacyUsers = db.Users
                                .Where(u => u.UserTypeId == 4)
                                .Include(u => u.Pharmacy)
                                .Include(u => u.UserType)
                                .ToList();

            var Pharmacies = PharmacyUsers.Select(u => new UserPharmacy
            {
                Pharmacy = u,
                pharmacyName = u.Pharmacy,
                UserType = u.UserType
            }).ToList();

            return View(Pharmacies);
        }


        // Doctor details by ID
        public ActionResult PharmacyDetails(int id)
        {
            var Phar = db.Users.FirstOrDefault(u => u.Id == id && u.UserTypeId == 4);
            if (Phar == null)
            {
                return HttpNotFound();
            }
            return View(Phar);
        }


        public ActionResult Register()
        {
            ViewBag.Cities = Enum.GetNames(typeof(Location)).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserDTO model, string PharmacyName)
        {
            try
            {
                model.UserTypeId = 1;
                if (ModelState.IsValid)
                {
                    var gmail=db.Users
                        .Where(u=>u.Email==model.Email);

                    if (gmail.Any())
                    { 
                        ViewBag.Error = "Email already exists. Please use a different email.";
                        ViewBag.Cities = Enum.GetNames(typeof(Location)).ToList();
                        return View(model);
                    }
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

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var state = ModelState[key];
                        foreach (var error in state.Errors)
                        {
                            ModelState.AddModelError(key, error.ErrorMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Registration failed: " + ex.Message);
            }

            // Repopulate ViewBags
            
            ViewBag.Cities = Enum.GetNames(typeof(Location)).ToList();
            return View(model);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and Password are required.";
                return View();
            }

            var user = db.Users.Include("UserType").FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["UserName"] = user.Name;
                Session["UserType"] = user.UserType.UserTypeName;

                if (user.UserType.UserTypeName == "Doctor")
                    return RedirectToAction("Index", "Doctor");
                else if (user.UserType.UserTypeName == "Pharmacy")
                    return RedirectToAction("Index", "Pharmacy");
                else if (user.UserType.UserTypeName == "Patient")
                    return RedirectToAction("Index", "Patient");
                else if (user.UserType.UserTypeName == "Admin")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials. Try again.";
            return View();
        }

        public ActionResult medicines()
        {
            var meds = db.BrandedMedicines.Include(b => b.Brand).Include(b => b.GenericMedicine).ToList();
            return View(meds);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }


}
