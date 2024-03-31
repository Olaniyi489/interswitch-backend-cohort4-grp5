using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using Microsoft.Graph.Reports.AuthenticationMethods.UsersRegisteredByFeatureWithIncludedUserTypesWithIncludedUserRoles;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
    public class ClaimsHelper
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _config;
        private readonly AzureAd _azureAd;
        public ClaimsHelper(IHttpContextAccessor httpContext, IConfiguration config, AzureAd azureAd)
        {
            _httpContext = httpContext;
            _config = config;
            _azureAd = azureAd;
        }

        public AzureUser GetUser()
        {
            var user = new AzureUser();
            string name = string.Empty;
            string token = string.Empty;
            var claimsIdentity = (ClaimsIdentity)_httpContext.HttpContext.User.Identity;
            if (claimsIdentity != null)
            {
                IEnumerable<Claim> claims = claimsIdentity.Claims;
                if (claims != null)
                {
                    user.Name = claimsIdentity.FindFirst("name")?.Value;
                    user.Code = claimsIdentity.FindFirst("aio")?.Value;
                    user.Role = claimsIdentity.FindFirst("role")?.Value;
                    user.Email = _httpContext.HttpContext.User.Identity.Name;

                }
            }
            return user;
        }
        public async Task<string> UserAzure()
        {
            //var azureAppServicePrincipalIdHeader = _httpContext.HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"][0];
            //var azureAppServicePrincipalNameHeader = _httpContext.HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"][0];
            var cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };
            string uriString = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";
            foreach (var c in _httpContext.HttpContext.Request.Cookies)
            {
                cookieContainer.Add(new Uri(uriString), new Cookie(c.Key, c.Value));
            }
            string jsonResult = string.Empty;
            using (HttpClient client = new HttpClient(handler))
            {
                var res = await client.GetAsync($"{uriString}/.auth/me");
                jsonResult = await res.Content.ReadAsStringAsync();
            }
            return jsonResult;
        }

        public async Task<GraphServiceClient> GetUserDetail()
        {

            var az = GetUser();
            //var scopes = new[] { "User.Read" };
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = _azureAd.TenantID;

            // Values from app registration
            var clientId = _azureAd.ClientID;
            var clientSecret = _azureAd.ClientSecret;

            // For authorization code flow, the user signs into the Microsoft
            // identity platform, and the browser is redirected back to your app
            // with an authorization code in the query parameters
            // var authorizationCode = "AUTH_CODE_FROM_REDIRECT";
            var authorizationCode = az.Token;

            // using Azure.Identity;
            var options = new AuthorizationCodeCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            // https://learn.microsoft.com/dotnet/api/azure.identity.authorizationcodecredential
            var authCodeCredential = new AuthorizationCodeCredential(
                tenantId, clientId, clientSecret, authorizationCode, options);

            var graphClient = new GraphServiceClient(authCodeCredential, scopes);
            var user = await graphClient.Me
    .GetAsync();
            return graphClient;
            #region Check again
            //        var messages = await graphClient.Me.Messages
            //.GetAsync(requestConfig =>
            //{
            //    requestConfig.QueryParameters.Select =
            //        new string[] { "subject", "sender" };
            //    requestConfig.QueryParameters.Filter =
            //        "subject eq 'Hello world'";
            //});

            //        GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            //        var users = await graphClient.Users
            //            .Request()
            //            .GetAsync();

            #endregion
        }
        public async Task<GraphServiceClient> CreateClient()
        {
            // Multi-tenant apps can use "common",
            // single-tenant apps must use the tenant ID from the Azure portal
            var tenantId = _azureAd.TenantID;

            // Values from app registration
            var clientId = _azureAd.ClientID;

            var authenticationProvider = new BaseBearerTokenAuthenticationProvider(
                new IntegratedWindowsTokenProvider(_azureAd));

            var graphClient = new GraphServiceClient(authenticationProvider);

            return graphClient;
        }

        public async Task<string> GetDynamic()
        {
            // string url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            string url = $"https://login.microsoftonline.com/{_azureAd.TenantID}/oauth2/v2.0/token";
            string @params = $"client_id={_azureAd.ClientID}&" +
                        "scope=User.Read &" +
                       // $"scope={_azureAd.ScopeName} &" +
                        $"client_secret={_azureAd.ClientSecret}&" +
                        "grant_type=authorization_code";

          //  var st = $"client_id={_azureAd.ClientID}&response_type=code&state=12345&grant_type=authorization_code&redirect_uri=/authentication/aurthcode&response_mode=query&scope=https://graph.microsoft.com/mail.read";
            var st = $"client_id={_azureAd.ClientID}&response_type=code&state=12345&grant_type=authorization_code&redirect_uri=/authentication/aurthcode&response_mode=query&scope=https://graph.microsoft.com/mail.read";

            using (var client = new HttpClient())
            {
                var content = new StringContent(@params, Encoding.UTF8, "application/json");


                var jsonStringResult = await client.PostAsync(url, content);
                return await jsonStringResult.Content.ReadAsStringAsync();
            }

        }

        public async Task<string> GetToken()
        {
            // Token Request End Point
            string tokenUrl = $"https://login.microsoftonline.com/{_azureAd.TenantID}/oauth2/v2.0/token";
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            //I am Using client_credentials as It is mostly recommended
            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _azureAd.ClientID,
                ["client_secret"] = _azureAd.ClientSecret,
                ["scope"] = "https://graph.microsoft.com/.default"
            });

          
            // AccessTokenClass results = new AccessTokenClass();
            HttpClient client = new HttpClient();

            var tokenResponse = await client.SendAsync(tokenRequest);

          var  json = await tokenResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AzureToken>(json);
            await GetUserDetailsAsync(result.access_token);
            return json;
            // 
        }
        public async Task<string> AuthorizationCode()
        {
            // Token Request End Point
            string tokenUrl = $"https://login.microsoftonline.com/{_azureAd.TenantID}/oauth2/v2.0/token";
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            var code = GetUser();
            //I am Using client_credentials as It is mostly recommended
            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                // ["grant_type"] = "client_credentials",
                ["grant_type"] = "authorization_code",
                ["client_id"] = _azureAd.ClientID,
                ["code"] = code.Code,
                ["client_secret"] = _azureAd.ClientSecret,
                ["scope"] = "https://graph.microsoft.com/.default"
            });


            dynamic json;
            // AccessTokenClass results = new AccessTokenClass();
            HttpClient client = new HttpClient();

            var tokenResponse = await client.SendAsync(tokenRequest);

            json = await tokenResponse.Content.ReadAsStringAsync();

      
            return json;
            // results = JsonConvert.DeserializeObject<AccessTokenClass>(json);
        }

        public   async Task<dynamic> GetUserDetailsAsync(string accessToken)
        {
            #region Old
            //    var graphClient = new GraphServiceClient(
            //        new DelegateAuthenticationProvider(
            //            async (requestMessage) =>
            //            {
            //                requestMessage.Headers.Authorization =
            //                    new AuthenticationHeaderValue("Bearer", accessToken);
            //            }));

            //    var user = await graphClient.Me.Request()
            //        .Select(u => new
            //        {
            //            u.DisplayName,
            //            u.Mail,
            //            u.UserPrincipalName
            //        })
            //        .GetAsync();

            #endregion
            //var scopes = new[] { "User.Read" };
            var scopes = new[] { _azureAd.ScopeName };
            var onBehalfOfCredential = new OnBehalfOfCredential(_azureAd.TenantID, _azureAd.ClientID, _azureAd.ClientSecret, accessToken);
            var tokenRequestContext = new TokenRequestContext(scopes);
             
            var token2 = onBehalfOfCredential.GetTokenAsync(tokenRequestContext, new CancellationToken()).Result.Token;
            var graphClient = new GraphServiceClient(onBehalfOfCredential, scopes);
            var user = await graphClient.Users.GetAsync();
            return user;
        }
    }
}
