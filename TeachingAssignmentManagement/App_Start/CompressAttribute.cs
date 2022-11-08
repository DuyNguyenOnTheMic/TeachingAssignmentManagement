using Brotli;
using System.IO.Compression;
using System.Web.Mvc;

public class CompressAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        string encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
        if (string.IsNullOrEmpty(encodingsAccepted)) return;

        encodingsAccepted = encodingsAccepted.ToLowerInvariant();
        System.Web.HttpResponseBase response = filterContext.HttpContext.Response;

        if (encodingsAccepted.Contains("br") || encodingsAccepted.Contains("brotli"))
        {
            response.AppendHeader("Content-Encoding", "br");
            response.Filter = new BrotliStream(response.Filter, CompressionMode.Compress);
        }
        else if (encodingsAccepted.Contains("gzip"))
        {
            response.AppendHeader("Content-encoding", "gzip");
            response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
        }
        else if (encodingsAccepted.Contains("deflate"))
        {
            response.AppendHeader("Content-encoding", "deflate");
            response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
        }
    }
}