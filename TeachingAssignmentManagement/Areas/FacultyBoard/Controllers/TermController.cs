using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class TermController : Controller
    {
        private readonly ITermRepository termRepository;

        public TermController()
        {
            this.termRepository = new TermRepository(new CP25Team03Entities());
        }

        public TermController(ITermRepository termRepository)
        {
            this.termRepository = termRepository;
        }

        // GET: FacultyBoard/Term
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetData()
        {
            // Get terms data from datatabse
            return Json(termRepository.GetTerms(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            // Populate year dropdown for create view
            int currentYear = DateTime.Now.Year;
            List<SelectListItem> startYear = PopulateYears(currentYear);
            List<SelectListItem> endYear = PopulateYears(currentYear + 1);

            ViewBag.start_year = startYear;
            ViewBag.end_year = endYear;
            return View(new term());
        }

        [HttpPost]
        public ActionResult Create(term term)
        {
            try
            {
                // Create new term
                termRepository.InsertTerm(term);
                termRepository.Save();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var term = termRepository.GetTermByID(id);

            // Set selected year on edit view
            List<SelectListItem> startYear = PopulateYears(term.start_year - 10);
            List<SelectListItem> endYear = PopulateYears(term.end_year - 10);
            startYear.Find(s => s.Value == term.start_year.ToString()).Selected = true;
            endYear.Find(s => s.Value == term.end_year.ToString()).Selected = true;

            ViewBag.start_year = startYear;
            ViewBag.end_year = endYear;
            return View(term);
        }

        [HttpPost]
        public ActionResult Edit(term term)
        {
            // Update term
            termRepository.UpdateTerm(term);
            termRepository.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                // Delete major
                termRepository.DeleteTerm(id);
                termRepository.Save();
            }
            catch
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> PopulateYears(int startYear)
        {
            // Create year select list
            List<SelectListItem> years = new List<SelectListItem>();
            for (int year = startYear; year <= startYear + 20; year++)
            {
                string sYear = year.ToString();
                years.Add(new SelectListItem() { Text = sYear, Value = sYear });
            }
            return years;
        }

        protected override void Dispose(bool disposing)
        {
            termRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}