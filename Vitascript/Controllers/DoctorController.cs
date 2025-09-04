using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vitascript.Context;
using Vitascript.CustomAuthorization;
using Vitascript.DTOs;
using Vitascript.ViewModel;

namespace Vitascript.Controllers
{
    [DoctorAuthorize]
    public class DoctorController : Controller
    {
        // GET: Doctor
        
        private ModelVitascript db;

        public DoctorController()
        {
            db = new ModelVitascript();
        }
        public ActionResult Index()
        {
            try
            {
                int doctorId = (int)Session["UserId"];

                var doctor = db.Users
                               .Include("DoctorType")
                               .Include("PrescriptionsGiven")
                               .FirstOrDefault(u => u.Id == doctorId && u.UserType.UserTypeName == "Doctor");

                if (doctor == null)
                    return HttpNotFound();

                var totalPatients = db.Patients.Count(p => p.AssignedDoctorId == doctorId);

                var patientsWaiting = db.Patients
                                        .Count(p => p.AssignedDoctorId == doctorId && p.Prescribed == false);

                var prescriptionsGiven = db.Prescriptions.Count(p => p.DoctorId == doctorId);

                ViewBag.TotalPatients = totalPatients;
                ViewBag.PatientsWaiting = patientsWaiting;
                ViewBag.PrescriptionsGiven = prescriptionsGiven;

                var unprescribedPatients = db.Patients
                    .Where(p => p.AssignedDoctorId == doctorId && p.Prescribed == false)
                    .Select(p => p.PatientUser)
                    .ToList();

                ViewBag.UnprescribedPatients = unprescribedPatients;

                return View(doctor);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                ModelState.AddModelError("", "Error loading dashboard: " + ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DProfile()
        {
            int doctorId = (int)Session["UserId"];
            var doctor = db.Users
                           .Include("DoctorType")
                           .Include("UserType")
                           .FirstOrDefault(u => u.Id == doctorId && u.UserType.UserTypeName == "Doctor");

            if (doctor == null)
                return HttpNotFound();

            return View(doctor);
        }

        public ActionResult MyPatients()
        {
            int doctorId = (int)Session["UserId"];

            var patients = db.Patients
                .Where(p => p.AssignedDoctorId == doctorId)
                .Include(p => p.PatientUser) 
                .ToList();

            return View(patients);
        }

        public async Task<ActionResult> PatientsDetails(int patientId)
        {
            var patient = db.Patients.Include(p => p.PatientUser).FirstOrDefault(p => p.PatientId == patientId);

            if (patient == null)
                return HttpNotFound();

            var viewModel = new PatientDetailsViewModel
            {
                PatientUser = patient.PatientUser,
                MedicalHistories = new List<MedicalHistoryDTO>(),
                Prescriptions = new List<PrescriptionViewDTO>()
            };

            var mhController = new MedicalHistoryController();
            var mhResult = await mhController.ByPatient(patientId) as JsonResult;
            if (mhResult?.Data is IEnumerable<MedicalHistoryDTO> histories)
            {
                viewModel.MedicalHistories = histories.ToList();
            }

            var prController = new PrescriptionController();
            var prResult = await prController.ByPatient(patientId) as JsonResult;
            if (prResult?.Data is IEnumerable<PrescriptionViewDTO> prescriptions)
            {
                viewModel.Prescriptions = prescriptions.ToList();
            }

            return View(viewModel);
        }

        public ActionResult addPrescription(int patientId)
        {
            var doctorId = (int)Session["UserId"];

            // Assuming you have a DbContext or service to fetch medicines
            var allGenericMedicines = db.GenericMedicines
                                         .Select(g => new SelectListItem
                                         {
                                             Value = g.Id.ToString(),
                                             Text = g.Name
                                         }).ToList();

            ViewBag.GenericMedicines = allGenericMedicines;

            var model = new CreatePrescriptionDTO
            {
                PatientId = patientId,
                DoctorId = doctorId,
                PrescribedMedicines = new List<PrescribedMedicineDTO>()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> CreatePrescription(CreatePrescriptionDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid prescription data";
                return RedirectToAction("addPrescription", new { patientId = model.PatientId });
            }
            var prController = new PrescriptionController();
            var result = await prController.Create(model) as JsonResult;

            dynamic response = result.Data;
            if (response.success == true)
            {
                return RedirectToAction("MyPatients");
            }

            TempData["Error"] = "Failed to create prescription";
            return RedirectToAction("addPrescription", new { patientId = model.PatientId });
        }

        public async Task<ActionResult> PrescriptionDetails(int Id)
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
                    .FirstOrDefault(p => p.PatientId == prescription.PatientID && p.AssignedDoctorId == doctorId);

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

        public async Task<ActionResult> PrescriptionList()
        {
            int doctorId = (int)Session["UserId"];

            var prescriptionController = new PrescriptionController();
            var result = await prescriptionController.ByDoctor(doctorId) as JsonResult;

            var prescriptions = new List<PrescriptionViewDTO>();
            if (result?.Data is IEnumerable<PrescriptionViewDTO> data)
            {
                prescriptions = data.ToList();
            }

            return View(prescriptions);
        }

    }
}