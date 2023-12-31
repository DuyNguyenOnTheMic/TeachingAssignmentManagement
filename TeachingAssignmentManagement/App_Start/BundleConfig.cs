﻿using System.Web.Optimization;

namespace TeachingAssignmentManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new MyScriptBundle("~/bundles/vendors").Include(
                        "~/app-assets/vendors/js/vendors.min.js",
                        "~/app-assets/vendors/js/screenfull/screenfull.min.js"));

            bundles.Add(new MyScriptBundle("~/bundles/app").Include(
                       "~/app-assets/js/core/app-menu.min.js",
                       "~/app-assets/js/core/app.min.js"));

            bundles.Add(new MyScriptBundle("~/bundles/extensions").Include(
                       "~/app-assets/vendors/js/extensions/toastr.min.js",
                       "~/app-assets/vendors/js/extensions/sweetalert2.min.js",
                       "~/app-assets/vendors/js/forms/select/select2.full.min.js"));

            bundles.Add(new MyScriptBundle("~/bundles/jqueryval").Include(
                       "~/app-assets/vendors/js/forms/validation/jquery.validate.min.js",
                       "~/app-assets/vendors/js/forms/validation/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new MyScriptBundle("~/bundles/datatables").Include(
                       "~/app-assets/vendors/js/tables/datatable/jquery.dataTables.min.js",
                       "~/app-assets/vendors/js/tables/datatable/dataTables.bootstrap5.min.js"));

            bundles.Add(new MyScriptBundle("~/bundles/export-dt").Include(
                       "~/app-assets/vendors/js/tables/datatable/jszip.min.js",
                       "~/app-assets/vendors/js/tables/datatable/pdfmake.min.js",
                       "~/app-assets/vendors/js/tables/datatable/vfs_fonts.js",
                       "~/app-assets/vendors/js/tables/datatable/buttons.html5.min.js",
                       "~/app-assets/vendors/js/tables/datatable/buttons.print.min.js"));

            bundles.Add(new MyStyleBundle("~/Content/login-theme").Include(
                        "~/app-assets/vendors/css/vendors.min.css",
                        "~/app-assets/css/bootstrap.min.css",
                        "~/app-assets/css/bootstrap-extended.min.css",
                        "~/app-assets/css/colors.min.css",
                        "~/app-assets/css/components.min.css",
                        "~/app-assets/css/pages/authentication.min.css"));

            bundles.Add(new MyStyleBundle("~/Content/theme").Include(
                        "~/app-assets/vendors/css/vendors.min.css",
                        "~/app-assets/vendors/css/tables/datatable/dataTables.bootstrap5.min.css",
                        "~/app-assets/vendors/css/forms/select/select2.min.css",
                        "~/app-assets/css/bootstrap.min.css",
                        "~/app-assets/css/bootstrap-extended.min.css",
                        "~/app-assets/css/colors.min.css",
                        "~/app-assets/css/components.min.css",
                        "~/app-assets/css/themes/dark-layout.min.css",
                        "~/app-assets/css/core/menu/menu-types/vertical-menu.min.css"));

            bundles.Add(new MyStyleBundle("~/Content/extensions").Include(
                        "~/app-assets/vendors/css/extensions/toastr.min.css",
                        "~/app-assets/vendors/css/extensions/sweetalert2.min.css",
                        "~/app-assets/css/plugins/extensions/ext-component-sweet-alerts.min.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}