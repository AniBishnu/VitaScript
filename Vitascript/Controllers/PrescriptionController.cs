using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vitascript.DTOs;

namespace Vitascript.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly HttpClient client;

        public PrescriptionController()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44347/api/prescriptions/")
            };
        }

        // GET: /Prescription/All
        public async Task<JsonResult> All()
        {
            var response = await client.GetAsync("all");

            if (response.IsSuccessStatusCode)
            {
                var prescriptions = await response.Content.ReadAsAsync<IEnumerable<PrescriptionViewDTO>>();
                return Json(prescriptions, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "Failed to fetch prescriptions" }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Prescription/Prescription/5
        public async Task<JsonResult> Details(int id)
        {
            var response = await client.GetAsync($"Prescription/{id}");

            if (response.IsSuccessStatusCode)
            {
                var prescription = await response.Content.ReadAsAsync<PrescriptionViewDTO>();
                return Json(prescription, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "Prescription not found" }, JsonRequestBehavior.AllowGet);
        }

        // POST: /Prescription/Create
        [HttpPost]
        public async Task<JsonResult> Create(CreatePrescriptionDTO dto)
        {
            var response = await client.PostAsJsonAsync("create", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<object>();
                return Json(new { success = true, result });
            }

            return Json(new { success = false, message = "Failed to create prescription" });
        }

        // GET: /Prescription/ByDoctor/5
        public async Task<JsonResult> ByDoctor(int doctorId)
        {
            var response = await client.GetAsync($"doctor/{doctorId}");

            if (response.IsSuccessStatusCode)
            {
                var prescriptions = await response.Content.ReadAsAsync<IEnumerable<PrescriptionViewDTO>>();
                return Json(prescriptions, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "Doctor not found or has no prescriptions" }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Prescription/ByPatient/5
        public async Task<JsonResult> ByPatient(int patientId)
        {
            var response = await client.GetAsync($"patient/{patientId}");

            if (response.IsSuccessStatusCode)
            {
                var prescriptions = await response.Content.ReadAsAsync<IEnumerable<PrescriptionViewDTO>>();
                return Json(prescriptions, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "Patient not found or has no prescriptions" }, JsonRequestBehavior.AllowGet);
        }
    }
}