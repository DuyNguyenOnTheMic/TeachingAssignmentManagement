﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Hubs;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = CustomRoles.FacultyBoard)]
    public class TermController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public TermController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public TermController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByCustom = "userName")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetData()
        {
            // Get terms data from database
            return Json(unitOfWork.TermRepository.GetTerms(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "none")]
        public ActionResult Create()
        {
            // Populate year dropdown for create view
            int currentYear = DateTime.Now.Year;
            List<SelectListItem> startYear = PopulateYears(currentYear);
            List<SelectListItem> endYear = PopulateYears(currentYear);
            startYear.Find(s => s.Value == currentYear.ToString()).Selected = true;
            endYear.Find(s => s.Value == (currentYear + 1).ToString()).Selected = true;

            ViewData["start_year"] = startYear;
            ViewData["end_year"] = endYear;
            return View(new term());
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "id,start_year,end_year,start_week,start_date,max_lesson,max_class,status")] term term)
        {
            try
            {
                // Create new term
                unitOfWork.TermRepository.InsertTerm(term);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true, message = "Học kỳ này đã được tạo trong hệ thống!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            term term = unitOfWork.TermRepository.GetTermByID(id);

            // Set selected year on edit view
            List<SelectListItem> startYear = PopulateYears(term.start_year);
            List<SelectListItem> endYear = PopulateYears(term.end_year);
            startYear.Find(s => s.Value == term.start_year.ToString()).Selected = true;
            endYear.Find(s => s.Value == term.end_year.ToString()).Selected = true;

            ViewData["start_year"] = startYear;
            ViewData["end_year"] = endYear;
            return View(term);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,start_year,end_year,start_week,start_date,max_lesson,max_class,status")] term term)
        {
            // Update term
            unitOfWork.TermRepository.UpdateTerm(term);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditStatus(int id, bool status)
        {
            term term = unitOfWork.TermRepository.GetTermByID(id);
            term.status = status;
            unitOfWork.Save();
            TimetableHub.RefreshData(id, null);
            return Json(new { success = true, message = "Cập nhật trạng thái thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                // Delete term
                unitOfWork.LecturerRankRepository.DeleteAllLecturerRanks(id);
                unitOfWork.TermRepository.DeleteTerm(id);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true, message = "Không thể xoá do học kỳ này đã có dữ liệu!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers
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
        #endregion
    }
}