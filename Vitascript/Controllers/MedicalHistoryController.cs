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
    public class MedicalHistoryController : Controller
    {
        private readonly HttpClient client;

        public MedicalHistoryController()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44347/api/MedicalHistory/")
            };
        }

        // GET: /MedicalHistory/All
        public async Task<JsonResult> All()
        {
            var response = await client.GetAsync("all");

            if (response.IsSuccessStatusCode)
            {
                var histories = await response.Content.ReadAsAsync<IEnumerable<MedicalHistoryDTO>>();
                return Json(histories, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "Failed to fetch medical histories" }, JsonRequestBehavior.AllowGet);
        }

        // GET: /MedicalHistory/ByPatient/5
        public async Task<JsonResult> ByPatient(int patientId)
        {
            var response = await client.GetAsync($"patient/{patientId}");

            if (response.IsSuccessStatusCode)
            {
                var histories = await response.Content.ReadAsAsync<IEnumerable<MedicalHistoryDTO>>();
                return Json(histories, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "No medical history found for this patient" }, JsonRequestBehavior.AllowGet);
        }
        // GET: /MedicalHistory/Details/5
        public async Task<JsonResult> Details(int id)
        {
            var response = await client.GetAsync($"details/{id}");

            if (response.IsSuccessStatusCode)
            {
                var history = await response.Content.ReadAsAsync<MedicalHistoryDTO>();
                return Json(history, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "Medical history not found" }, JsonRequestBehavior.AllowGet);
        }


    }
}