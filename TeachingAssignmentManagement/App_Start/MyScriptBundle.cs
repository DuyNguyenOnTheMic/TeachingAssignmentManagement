using NUglify;
using NUglify.JavaScript;

namespace System.Web.Optimization
{
    public class MyScriptBundle : Bundle
    {
        public MyScriptBundle(string virtualPath) : base(virtualPath, new MyJsMinify())
        {
        }

        public MyScriptBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath, new MyJsMinify())
        {
        }
    }

    public class MyJsMinify : IBundleTransform
    {
        internal static readonly MyJsMinify Instance = new MyJsMinify();

        internal static string JsContentType = "text/javascript";


        public virtual void Process(BundleContext context, BundleResponse response)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            if (!context.EnableInstrumentation)
            {
                response.Content = Uglify.Js(response.Content, new CodeSettings
                {
                    EvalTreatment = EvalTreatment.MakeImmediateSafe,
                    PreserveImportantComments = false
                }).ToString();
            }
            response.ContentType = JsContentType;
        }
    }
}