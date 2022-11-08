using NUglify;

namespace System.Web.Mvc
{
    public static class HtmlMinifierExtensions
    {
        public static MvcHtmlString HtmlMinify(
            this HtmlHelper helper, Func<object, object> markup)
        {
            string notMinifiedHtml =
             markup.Invoke(helper.ViewContext)?.ToString() ?? "";

            string minifiedHtml = Uglify.Html(notMinifiedHtml).ToString();
            return new MvcHtmlString(minifiedHtml);
        }
    }
}