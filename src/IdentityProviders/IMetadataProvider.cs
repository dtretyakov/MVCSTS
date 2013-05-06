using System;

namespace MvcSTSApplication.IdentityProviders
{
    public interface IMetadataProvider
    {
        string GetFederationMetadata(Uri endpoint);
    }
}