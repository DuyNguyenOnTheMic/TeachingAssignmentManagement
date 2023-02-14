using System.Collections.Generic;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    public class RemunerationController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public RemunerationController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public RemunerationController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Subject()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpGet]
        public ActionResult GetSubjectPartial(int termId, string majorId)
        {
            ViewData["termId"] = termId;
            ViewData["majorId"] = majorId;
            return PartialView("_Subject");
        }

        [HttpGet]
        public JsonResult GetSubjectData(int termId, string majorId)
        {
            IEnumerable<subject> query_subjects = majorId != "-1"
                ? unitOfWork.SubjectRepository.GetSubjects(termId, majorId)
                : unitOfWork.SubjectRepository.GetTermSubjects(termId);
            return Json(query_subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditSubject(string id)
        {
            return View(unitOfWork.SubjectRepository.GetSubjectByID(id));
        }

        [HttpPost]
        public ActionResult EditSubject([Bind(Include = "id,is_vietnamese,theoretical_coefficient,practice_coefficient")] subject subject)
        {
            // Update subject
            unitOfWork.SubjectRepository.UpdateSubject(subject);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }
    }
}