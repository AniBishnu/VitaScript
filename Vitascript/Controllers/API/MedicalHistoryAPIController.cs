using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vitascript.Context;
using Vitascript.DTOs;
using Vitascript.Models;
using System.Data.Entity;

namespace Vitascript.Controllers.API
{
    [RoutePrefix("api/MedicalHistory")]
    public class MedicalHistoryAPIController : ApiController
    {
        private ModelVitascript db;

        public MedicalHistoryAPIController()
        {
            db = new ModelVitascript();
        }

        // GET: api/medicalhistory/all
        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAllMedicalHistories()
        {
            var histories = db.MedicalHistories
                .Include(m => m.Patient)
                .Select(m => new MedicalHistoryDTO
                {
                    Id = m.Id,
                    Description = m.Description,
                    DateRecorded = m.DateRecorded,
                    PatientName = m.Patient.Name
                }).ToList();

            return Ok(histories);
        }


        // GET: api/medicalhistory/patient/5
        [HttpGet]
        [Route("patient/{patientId}")]
        public IHttpActionResult GetMedicalHistoryByPatient(int patientId)
        {
            var histories = db.MedicalHistories
                .Include(m => m.Patient)
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.DateRecorded)
                .Select(m => new MedicalHistoryDTO
                {
                    Id = m.Id,
                    Description = m.Description,
                    DateRecorded = m.DateRecorded,
                    PatientName = m.Patient.Name // Optional
                })
                .ToList();

            if (!histories.Any())
                return NotFound();

            return Ok(histories);
        }

        // GET: api/medicalhistory/{id}
        [HttpGet]
        [Route("details/{id}")]
        public IHttpActionResult GetMedicalHistoryById(int id)
        {
            var history = db.MedicalHistories
                .Include(m => m.Patient)
                .Where(m => m.Id == id)
                .Select(m => new MedicalHistoryDTO
                {
                    Id = m.Id,
                    Description = m.Description,
                    DateRecorded = m.DateRecorded,
                    PatientName = m.Patient.Name
                })
                .FirstOrDefault();

            if (history == null)
                return NotFound();

            return Ok(history);
        }

    }
}
