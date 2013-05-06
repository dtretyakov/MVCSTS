using System;
using System.Collections.Generic;
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
            var twitterEndpoint = new UriBuilder(GetRequestUri())
                {
                    Path = GetActionAddress("twitter")
                };

            string metadata = _metadataProvider.GetFederationMetadata(twitterEndpoint.Uri);

            return new XmlResult(metadata);
        }

        //
        // GET: /wsfederation/twitter
        [HttpGet]
        public ActionResult Twitter()
        {
            var callback = new UriBuilder(GetRequestUri())
            {
                Path = GetActionAddress("twittercallback")
            };

            Dictionary<string, string> parameters = Request.Params.ToDictionary();

            Uri authorizationUri = _identityProvider.GetAuthorizationUri(parameters, callback.Uri);

            return Redirect(authorizationUri.AbsoluteUri);
        }


        //
        // GET: /wsfederation/twittercallback
        [HttpGet]
        public ActionResult TwitterCallback()
        {
            var signInUri = new UriBuilder(GetRequestUri())
            {
                Path = GetActionAddress("twitter")
            };

            Dictionary<string, string> parameters = Request.Params.ToDictionary();

            string result = _identityProvider.GetResponseHtml(parameters, signInUri.Uri);

            return new HtmlResult(result);
        }

        /// <summary>
        ///     Returns a controller action path.
        /// </summary>
        /// <param name="action">Action name.</param>
        /// <returns>Action request path.</returns>
        private string GetActionAddress(string action)
        {
            return (Url.Action(action) ?? string.Empty).ToLowerInvariant();
        }

        /// <summary>
        ///     Returns a request URI.
        /// </summary>
        /// <returns>Request URI.</returns>
        private Uri GetRequestUri()
        {
            return Request.Url;
        }
    }
}