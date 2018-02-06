using System.Web;
using System.Web.Optimization;

namespace AplicacaoMedicina
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.1.0.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery1").Include(
                        "~/Scripts/jquery-1.10.2.min.js"));

            /*     bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                             "~/Scripts/jquery.validate*"));*/

            bundles.Add(new ScriptBundle("~/bundles/doctorStrap").Include(
                       "~/Scripts/doctorScripts/bootstrap.min.js")
                       );

           bundles.Add(new ScriptBundle("~/bundles/doctorScripts").Include(
                        "~/Scripts/doctorScripts/material.min.js",
                        "~/Scripts/doctorScripts/chartist.min.js",
                        "~/Scripts/doctorScripts/bootstrap-notify.js",
                        "~/Scripts/doctorScripts/material-dashboard.js",
                        "~/Scripts/doctorScripts/demo.js",
                        "~/Scripts/custom.min.js",
                        "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
                "~/Scripts/inputmask/inputmask.js",               
                "~/Scripts/inputmask/inputmask.extensions.js",
                "~/Scripts/inputmask/inputmask.date.extensions.js",
                "~/Scripts/inputmask/inputmask.numeric.extensions.js",
                 "~/Scripts/inputmask/jquery.inputmask.js"));

            bundles.Add(new ScriptBundle("~/bundles/consulConfirma").Include(               
               "~/Scripts/consulConfirma/beautifier.js",
               "~/Scripts/consulConfirma/angular-material-datetimepicker.js",
               "~/Scripts/consulConfirma/demo.js" ));

            bundles.Add(new ScriptBundle("~/bundles/flot").Include(
                    "~/Scripts/flot/jquery.flot.js",
                    "~/Scripts/flot/jquery.flot.pie.js",
                    "~/Scripts/flot/jquery.flot.resize.js",
                    "~/Scripts/flot/jquery.flot.stack.js",
                    "~/Scripts/flot/jquery.flot.time.js",
                    "~/Scripts/flotPlugins/jquery.flot.orderBars.js",
                    "~/Scripts/flotPlugins/jquery.flot.spline.min.js",
                    "~/Scripts/flotPlugins/curvedLines.js",
                    "~/Scripts/doctorScripts/date.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/JQVMAP").Include(
                    "~/Scripts/JQVMap/jquery.vmap.js",
                    "~/Scripts/JQVMap/jquery.vmap.world.js",
                    "~/Scripts/JQVMap/jquery.vmap.sampledata.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/daterangepicker").Include(
                    "~/Scripts/daterangepicker/moment.min.js",
                    "~/Scripts/daterangepicker/daterangepicker.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                    "~/Scripts/custom.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/date").Include(
                "~/Scripts/date/picker.js",
                "~/Scripts/date/picker.date.js",
                "~/Scripts/date/legacy.js"
                ));

           

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            /*     bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                             "~/Scripts/modernizr-*")); */

            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                      "~/Scripts/home/skel.min.js",
                      "~/Scripts/home/util.js",
                      "~/Scripts/home/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                    "~/Scrips/js/jquery-1.11.1.min.js",
                    "~/Scrips/js/owl.carousel.min.js",
                    "~/Scrips/js/bootstrap.min.js",
                    "~/Scrips/js/wow.min.js",
                    "~/Scrips/js/typewriter.js",
                    "~/Scrips/js/jquery.onepagenav.js",
                    "~/Scrips/js/main.js"
                ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"  ));

            bundles.Add(new StyleBundle("~/Content/doctorStyles/css").Include(
                      "~/Content/doctorStyles/bootstrap.min.css",
                      "~/Content/doctorStyles/material-dashboard.css",
                      "~/Content/doctorStyles/demo.css"));

            bundles.Add(new StyleBundle("~/Content/doctorIndex/css").Include(
                     "~/Content/doctorIndex/bootstrap-progressbar-3.3.4.min.css",
                     "~/Content/doctorIndex/daterangepicker.css",
                     "~/Content/doctorIndex/green.css",
                     "~/Content/doctorIndex/jqvmap.min.css"));

            

        }
    }
}
