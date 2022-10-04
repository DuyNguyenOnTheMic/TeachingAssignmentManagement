using NUglify;
using NUglify.Css;

namespace System.Web.Optimization
{
    public class MyStyleBundle : Bundle
    {
        public MyStyleBundle(string virtualPath) : base(virtualPath, new MyCssMinify())
        {
        }

        public MyStyleBundle(string virtualPath, string cdnPath) : base(virtualPath, cdnPath, new MyCssMinify())
        {
        }
    }

    public class MyCssMinify : IBundleTransform
    {
        internal static readonly MyCssMinify Instance = new MyCssMinify();

        internal static string CssContentType = "text/css";


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
                response.Content = Uglify.Css(response.Content, new CssSettings
                {
                    CommentMode = CssComment.None,
                    OutputMode = OutputMode.SingleLine
                }).ToString();
            }
            response.ContentType = CssContentType;
        }
    }
}