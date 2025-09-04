using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vitascript.Models;
using Vitascript.DTOs;
using Vitascript.Context;
using System.Data.Entity;

namespace Vitascript.Controllers.API
{
    [RoutePrefix("api/prescriptions")]
    public class PrescriptionAPIController : ApiController
    {
        private ModelVitascript db;

        public PrescriptionAPIController()
        {
            db = new ModelVitascript();
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult CreatePrescription(CreatePrescriptionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(
                    x => x.Value.Errors.Select(error => $"Key: {x.Key}, Error: {error.ErrorMessage}")
                ).ToList();

                return BadRequest(string.Join("; ", errors));
            }
            var prescription = new Prescription
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                PrescribedMedicines = new List<PrescribedMedicine>()
            };

            // Create the medicine detail string for MedicalHistory
            var medicineDetailsList = new List<string>();

            foreach (var med in dto.PrescribedMedicines)
            {
                prescription.PrescribedMedicines.Add(new PrescribedMedicine
                {
                    GenericMedicineId = med.GenericMedicineId,
                    Dose = med.Dose,
                    TotalQuantity = med.TotalQuantity,
                    RemainingQuantity = med.TotalQuantity,
                    Duration = med.Duration,
                    Notes = med.Notes
                });

                var genericMedicineName = db.GenericMedicines
                    .Where(gm => gm.Id == med.GenericMedicineId)
                    .Select(gm => gm.Name)
                    .FirstOrDefault();

                string medDetail = $"Medicine Name: {genericMedicineName}\n," +
                                   $"Dose: {med.Dose}\n," +
                                   $"Total Quantity: {med.TotalQuantity}\n," +
                                   $"Duration: {med.Duration}\n days," +
                                   $"Notes: {(string.IsNullOrWhiteSpace(med.Notes) ? "N/A" : med.Notes)},";

                medicineDetailsList.Add(medDetail);
            }

            // Combine description
            string medicalHistoryDescription = $"The patient had been suffering from {dto.MedicalHistoryDescription}. " +
                                               "Therefore, the following medicine(s) were prescribed:\n\n" +
                                               string.Join("\n\n", medicineDetailsList);

            db.Prescriptions.Add(prescription);

            var patientRecord = db.Patients.FirstOrDefault(p => p.PatientId == dto.PatientId && p.Prescribed==false);
            if (patientRecord != null)
            {
                patientRecord.Prescribed = true;
            }

            var medicalHistory = new MedicalHistory
            {
                PatientId = dto.PatientId,
                Description = medicalHistoryDescription,
                DateRecorded = DateTime.Now
            };
            db.MedicalHistories.Add(medicalHistory);

            db.SaveChanges();

            return Ok(new
            {
                message = "Prescription and medical history created successfully",
                code = prescription.PrescriptionCode
            });
        }

        // GET: api/prescriptions/all
        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetPrescriptions()
        {
            var prescriptions = db.Prescriptions
                .Include(p => p.PrescribedMedicines.Select(pm => pm.GenericMedicine))
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .ToList();

            var dtoList = prescriptions.Select(p => new PrescriptionViewDTO
            {
                Id = p.Id,
                PrescriptionCode = p.PrescriptionCode,
                DoctorName = p.Doctor.Name, 
                DoctorID = p.Doctor.Id,
                PatientID = p.Patient.Id,
                PatientName = p.Patient.Name,
                IssuedDate = p.IssuedDate,
                PrescribedMedicines = p.PrescribedMedicines.Select(pm => new PrescribedMedicineViewDTO
                {
                    GenericMedicineName = pm.GenericMedicine.Name,
                    Dose = pm.Dose,
                    TotalQuantity = pm.TotalQuantity,
                    RemainingQuantity = pm.RemainingQuantity,
                    Duration = pm.Duration,
                    Notes = pm.Notes
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }

        // GET: api/prescriptions/doctor/{PrescriptionId}
        [HttpGet]
        [Route("Prescription/{PrescriptionId}")]
        public IHttpActionResult GetPrescriptionsByID(int PrescriptionId)
        {
            var prescription = db.Prescriptions
                .Include(p => p.PrescribedMedicines.Select(pm => pm.GenericMedicine))
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .FirstOrDefault(p => p.Id == PrescriptionId);

            if (prescription == null)
                return NotFound();

            var dto = new PrescriptionViewDTO
            {
                PrescriptionCode = prescription.PrescriptionCode,
                DoctorName = prescription.Doctor.Name,
                DoctorID = prescription.Doctor.Id,
                PatientID = prescription.Patient.Id,
                PatientName = prescription.Patient.Name,
                IssuedDate = prescription.IssuedDate,
                PrescribedMedicines = prescription.PrescribedMedicines.Select(pm => new PrescribedMedicineViewDTO
                {
                    GenericMedicineName = pm.GenericMedicine.Name,
                    Dose = pm.Dose,
                    TotalQuantity = pm.TotalQuantity,
                    RemainingQuantity = pm.RemainingQuantity,
                    Duration = pm.Duration,
                    Notes = pm.Notes
                }).ToList()
            };

            return Ok(dto);
        }

        // GET: api/prescriptions/doctor/{doctorId}
        [HttpGet]
        [Route("doctor/{doctorId}")]
        public IHttpActionResult GetPrescriptionsByDoctor(int doctorId)
        {
            var prescriptions = db.Prescriptions
                .Include(p => p.PrescribedMedicines.Select(pm => pm.GenericMedicine))
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .Where(p => p.DoctorId == doctorId)
                .ToList();

            if (!prescriptions.Any())
                return NotFound();

            var dtoList = prescriptions.Select(p => new PrescriptionViewDTO
            {
                Id = p.Id,
                PrescriptionCode = p.PrescriptionCode,
                DoctorName = p.Doctor.Name,
                DoctorID = p.Doctor.Id,
                PatientID = p.Patient.Id,
                PatientName = p.Patient.Name,
                IssuedDate = p.IssuedDate,
                PrescribedMedicines = p.PrescribedMedicines.Select(pm => new PrescribedMedicineViewDTO
                {
                    GenericMedicineName = pm.GenericMedicine.Name,
                    Dose = pm.Dose,
                    TotalQuantity = pm.TotalQuantity,
                    RemainingQuantity = pm.RemainingQuantity,
                    Duration = pm.Duration,
                    Notes = pm.Notes
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }

        // GET: api/prescriptions/patient/5
        [HttpGet]
        [Route("patient/{patientId}")]
        public IHttpActionResult GetPrescriptionsByPatient(int patientId)
        {
            var prescriptions = db.Prescriptions
                .Include(p => p.PrescribedMedicines.Select(pm => pm.GenericMedicine))
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .Where(p => p.PatientId == patientId)
                .ToList();

            var dtoList = prescriptions.Select(p => new PrescriptionViewDTO
            {
                Id = p.Id,
                PrescriptionCode = p.PrescriptionCode,
                DoctorName = p.Doctor.Name,
                DoctorID = p.Doctor.Id,
                PatientID = p.Patient.Id,
                PatientName = p.Patient.Name,
                IssuedDate = p.IssuedDate,
                PrescribedMedicines = p.PrescribedMedicines.Select(pm => new PrescribedMedicineViewDTO
                {
                    GenericMedicineName = pm.GenericMedicine.Name,
                    Dose = pm.Dose,
                    TotalQuantity = pm.TotalQuantity,
                    RemainingQuantity = pm.RemainingQuantity,
                    Duration = pm.Duration,
                    Notes = pm.Notes
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }

    }
}
