using System;
using System.Collections.Generic;

namespace MvcSTSApplication.IdentityProviders
{
    public interface IIdentityProvider
    {
        Uri GetAuthorizationUri(IDictionary<string, string> parameters, Uri callback);

        string GetResponseHtml(IDictionary<string, string> parameters, Uri signinUri);
    }
}