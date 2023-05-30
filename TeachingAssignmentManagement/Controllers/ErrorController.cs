﻿using System.Web.Mvc;

namespace TeachingAssignmentManagement.Controllers
{
    public class ErrorController : Controller
    {
        // GET: NotFound
        public ActionResult NotFound()
        {
            return Request.IsAjaxRequest() ? View("NotFoundAjax") : View("NotFound");
        }
    }
}