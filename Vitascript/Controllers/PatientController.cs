using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vitascript.Context;
using Vitascript.CustomAuthorization;
using Vitascript.DTOs;
using Vitascript.Models;
using Vitascript.ViewModel;

namespace Vitascript.Controllers
{
    [PatientAuthorize]
    public class PatientController : Controller
    {
        private ModelVitascript db;

        public PatientController()
        {
            db = new ModelVitascript();
        }
        public ActionResult Index()
        {
            int patientUserId = Convert.ToInt32(Session["UserId"]);

            var doctorsVisited = db.Prescriptions
                .Where(p => p.PatientId == patientUserId)
                .Select(p => p.DoctorId)
                .Distinct()
                .Count();

            var totalPrescriptions = db.Prescriptions
                .Count(p => p.PatientId == patientUserId);

            var upcomingAppointments = db.Patients
                .Where(p => p.PatientId == patientUserId && !p.Prescribed)
                .Select(p => new PatientDoctorViewModel
                {
                    PatientId = p.Id,
                    Prescribed = p.Prescribed,
                    DoctorName = p.AssignedDoctor.Name,
                    DoctorPhone = p.AssignedDoctor.Phone,
                    DoctorEmail = p.AssignedDoctor.Email
                })
                .ToList();

            ViewBag.DoctorsVisited = doctorsVisited;
            ViewBag.TotalPrescriptions = totalPrescriptions;
            ViewBag.UpcomingAppointments = upcomingAppointments;

            var user = db.Users.Find(patientUserId);
            return View(user);
        }

        public ActionResult BookAppointment()
        {
            var doctorTypes = db.DoctorTypes.ToList();
            ViewBag.DoctorTypes = new SelectList(doctorTypes, "Id", "Name");

            var doctors = db.Users
                            .Where(u => u.UserTypeId == 2) 
                            .ToList();
            return View(doctors);
        }

        [HttpPost]
        public ActionResult BookAppointment(int doctorId)
        {
            int patientUserId = Convert.ToInt32(Session["UserId"]);

            var existingAppointment = db.Patients
                .FirstOrDefault(p => p.PatientId == patientUserId && p.AssignedDoctorId == doctorId && !p.Prescribed);

            if (existingAppointment != null)
            {
                return Json(new { success = false, message = "You already have a pending appointment with this doctor." });
            }

            var newAppointment = new Patient
            {
                PatientId = patientUserId,
                AssignedDoctorId = doctorId,
                Prescribed = false
            };

            db.Patients.Add(newAppointment);
            db.SaveChanges();

            return Json(new { success = true });
        }

        public async Task<ActionResult> PrescriptionListPatient()
        {
            int patientId = (int)Session["UserId"];

            var prescriptionController = new PrescriptionController();
            var result = await prescriptionController.ByPatient(patientId) as JsonResult;

            var prescriptions = new List<PrescriptionViewDTO>();
            if (result?.Data is IEnumerable<PrescriptionViewDTO> data)
            {
                prescriptions = data.ToList();
            }

            return View(prescriptions);
        }

        public async Task<ActionResult> PrescriptionDetailsPatient(int id)
        {
            var prController = new PrescriptionController();
            var prResult = await prController.Details(id) as JsonResult;

            var prescription = prResult?.Data as PrescriptionViewDTO;
            if (prescription == null)
                return HttpNotFound("Prescription not found");

            var viewModel = new PatientDetailsViewModel
            {
                PatientUser = null, 
                MedicalHistories = new List<MedicalHistoryDTO>(), 
                Prescriptions = new List<PrescriptionViewDTO> { prescription }
            };

            return View(viewModel);
        }

        public ActionResult PProfile()
        {
            int patientId = (int)Session["UserId"];
            var patient = db.Users
                            .Where(u => u.Id == patientId && u.UserTypeId == 1)
                            .FirstOrDefault();

            if (patient == null)
                return HttpNotFound();

            return View(patient);
        }

        

    }
}