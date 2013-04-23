using System.Text;
using System.Web.Mvc;

namespace MvcSTSApplication.Infrastructure
{
    public class XmlResult : ContentResult
    {
        public XmlResult(string xml)
        {
            Content = xml;
            ContentEncoding = Encoding.UTF8;
            ContentType = "text/xml";
        }
    }
}