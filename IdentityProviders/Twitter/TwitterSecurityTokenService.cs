using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.Security.Claims;
using MvcSTSApplication.Infrastructure;
using TweetSharp;

namespace MvcSTSApplication.IdentityProviders.Twitter
{
    public sealed class TwitterSecurityTokenService : SecurityTokenService, IIdentityProvider
    {
        private readonly IConfigurationProvider _configurationProvider;

        public TwitterSecurityTokenService(SecurityTokenServiceConfiguration configuration, IConfigurationProvider configurationProvider)
            : base(configuration)
        {
            _configurationProvider = configurationProvider;
        }

        public Uri GetAutheticationUri(IDictionary<string, string> parameters, Uri callback)
        {
            var callbackUri = new UriBuilder(callback)
                {
                    Query = string.Format("context={0}", parameters["wctx"])
                };

            // Pass your credentials to the service
            string consumerKey = _configurationProvider.Get(Settings.TwitterConsumerKey);
            string consumerSecret = _configurationProvider.Get(Settings.TwitterConsumerSecret);

            var service = new TwitterService(consumerKey, consumerSecret);

            // Retrieve an OAuth Request Token
            OAuthRequestToken requestToken = service.GetRequestToken(callbackUri.ToString());

            // Redirect to the OAuth Authorization URL
            return service.GetAuthorizationUri(requestToken);
        }

        public string GetResponseHtml(IDictionary<string, string> parameters, Uri signinUri)
        {
            var requestToken = new OAuthRequestToken {Token = parameters["oauth_token"]};

            // Exchange the Request Token for an Access Token
            var service = new TwitterService(Settings.TwitterConsumerKey, Settings.TwitterConsumerSecret);
            OAuthAccessToken accessToken = service.GetAccessToken(requestToken, parameters["oauth_verifier"]);

            // Claim values
            string name = accessToken.ScreenName;
            string nameIdentifier = string.Format("https://twitter.com/account/redirect_by_id?id={0}", accessToken.UserId);
            string token = accessToken.Token;
            string tokenSecret = accessToken.TokenSecret;

            string wtRealm = _configurationProvider.Get(Settings.TwitterWtRealm);
            string wReply = _configurationProvider.Get(Settings.TwitterWReply);

            var requestMessage = new SignInRequestMessage(signinUri, wtRealm, wReply);

            // Add claims
            var identity = new ClaimsIdentity(AuthenticationTypes.Federation);
            identity.AddClaim(new Claim(ClaimTypes.Name, name));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
            identity.AddClaim(new Claim(TwitterClaims.TwitterToken, token));
            identity.AddClaim(new Claim(TwitterClaims.TwitterTokenSecret, tokenSecret));

            var principal = new ClaimsPrincipal(identity);

            // Serialize response message
            SignInResponseMessage responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, principal, this);
            responseMessage.Context = parameters["context"];

            return responseMessage.WriteFormPost();
        }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            if (request.AppliesTo == null)
            {
                throw new InvalidRequestException("The AppliesTo is null.");
            }

            var scope = new Scope(request.AppliesTo.Uri.OriginalString, SecurityTokenServiceConfiguration.SigningCredentials)
                {
                    TokenEncryptionRequired = false,
                    SymmetricKeyEncryptionRequired = false
                };

            scope.ReplyToAddress = string.IsNullOrEmpty(request.ReplyTo) ? scope.AppliesToAddress : request.ReplyTo;

            return scope;
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (principal == null)
            {
                throw new InvalidRequestException("The caller's principal is null.");
            }

            var claimsIdentity = principal.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                throw new InvalidRequestException("The caller's identity is invalid.");
            }

            return claimsIdentity;
        }
    }
}