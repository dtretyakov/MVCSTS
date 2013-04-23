using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using MvcSTSApplication.IdentityProviders;
using MvcSTSApplication.Infrastructure;

namespace MvcSTSApplication.Controllers
{
    public class WsFederationController : Controller
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IMetadataProvider _metadataProvider;

        public WsFederationController(IMetadataProvider metadataProvider, IIdentityProvider identityProvider)
        {
            _metadataProvider = metadataProvider;
            _identityProvider = identityProvider;
        }

        //
        // GET: /wsfederation/twittermetadata
        public ActionResult TwitterMetadata()
        {
            if (Request.Url == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            var twitterEndpoint = new UriBuilder(Request.Url)
                {
                    Path = (Url.Action("twitter") ?? string.Empty).ToLowerInvariant(),
                    Query = string.Empty
                };

            string metadata = _metadataProvider.GetFederationMetadata(twitterEndpoint.Uri);

            return new XmlResult(metadata);
        }

        //
        // GET: /wsfederation/twitter
        [HttpGet]
        public ActionResult Twitter()
        {
            Dictionary<string, string> parameters = Request.Params.ToDictionary();

            if (Request.Url == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            var callback = new UriBuilder(Request.Url)
                {
                    Path = (Url.Action("twittercallback") ?? string.Empty).ToLowerInvariant()
                };

            Uri authenticationUri = _identityProvider.GetAutheticationUri(parameters, callback.Uri);

            return Redirect(authenticationUri.AbsoluteUri);
        }


        //
        // GET: /wsfederation/twittercallback
        [HttpGet]
        public ActionResult TwitterCallback()
        {
            Dictionary<string, string> parameters = Request.Params.ToDictionary();

            // Create request message
            if (Request.Url == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            var signInUri = new UriBuilder(Request.Url)
                {
                    Path = (Url.Action("twitter") ?? string.Empty).ToLowerInvariant()
                };

            string result = _identityProvider.GetResponseHtml(parameters, signInUri.Uri);

            return new HtmlResult(result);
        }
    }
}