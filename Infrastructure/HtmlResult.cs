using System.Text;
using System.Web.Mvc;

namespace MvcSTSApplication.Infrastructure
{
    public class HtmlResult : ContentResult
    {
        public HtmlResult(string html)
        {
            Content = html;
            ContentEncoding = Encoding.UTF8;
            ContentType = "text/html";
        }
    }
}