using System.Collections.Generic;
using System;
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
            List<SelectListItem> startYear = PopulateYears(term.start_year);
            List<SelectListItem> endYear = PopulateYears(term.end_year);
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

        public List<SelectListItem> PopulateYears(int startYear)
        {
            // Create year select list
            List<SelectListItem> years = new List<SelectListItem>();
            for (int year = startYear - 10; year <= startYear + 10; year++)
            {
                string sYear = year.ToString();
                years.Add(new SelectListItem() { Text = sYear, Value = sYear });
            }
            return years;
        }
    }
}