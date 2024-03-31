using Azure.Identity;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using blacklist.Application.Common.Models;

namespace blacklist.Application.Helpers
{
	public class MicsrosoftAADHelper
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContext;
		public MicsrosoftAADHelper(IConfiguration configuration, IHttpContextAccessor httpContext)
		{
			_configuration = configuration;
			_httpContext = httpContext??throw new ArgumentNullException(nameof(httpContext));
		}

		//public string GetPicture()
		//{
		//	var httpClient = new HttpClient();
		//	httpClient.SetBearerToken(info.AuthenticationTokens.Where(t => t.Name.Equals("access_token")).First().Value);
		//	var pictureResult = httpClient.GetAsync("https://graph.microsoft.com/v1.0/me/photo/$value").Result;
		//}
		public async Task<ClaimsPrincipal> GetUser()
		{
			// Create a user on current thread from provided header
			if (_httpContext.HttpContext.Request.Headers.ContainsKey("X-MS-CLIENT-PRINCIPAL-ID"))
			{
				// Read headers from Azure
				var azureAppServicePrincipalIdHeader = _httpContext.HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"][0];
				var azureAppServicePrincipalNameHeader = _httpContext.HttpContext.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"][0];

				#region extract claims via call /.auth/me
				//invoke /.auth/me
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

				//parse json
				var obj = JArray.Parse(jsonResult);
				string user_id = obj[0]["user_id"].Value<string>(); //user_id

				// Create claims id
				List<Claim> claims = new List<Claim>();
				foreach (var claim in obj[0]["user_claims"])
				{
					claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
				}

				// Set user in current context as claims principal
				var identity = new GenericIdentity(azureAppServicePrincipalIdHeader);
				identity.AddClaims(claims);
				#endregion

				// Set current thread user to identity
				_httpContext.HttpContext.User = new GenericPrincipal(identity, null);
				return _httpContext.HttpContext.User;
			}
			return null;
		}
		public async Task<MicrosoftAADResponse> GetLoggedInProfile()
		{
		 

			var scopes = new[] { "User.Read" };

			// Multi-tenant apps can use "common",
			// single-tenant apps must use the tenant ID from the Azure portal
			//var tenantId = "common";
			var tenantId = _configuration["AzureAd:TenantID"];

			// Values from app registration
			var clientId = _configuration["AzureAd:ClientID"];
			var clientSecret = _configuration["AzureAd:ClientSecret"];

			// For authorization code flow, the user signs into the Microsoft
			// identity platform, and the browser is redirected back to your app
			// with an authorization code in the query parameters
			var authorizationCode = "AUTH_CODE_FROM_REDIRECT";

			// using Azure.Identity;
			var options = new AuthorizationCodeCredentialOptions
			{
				AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
			};

			// https://learn.microsoft.com/dotnet/api/azure.identity.authorizationcodecredential
			var authCodeCredential = new AuthorizationCodeCredential(
				tenantId, clientId, clientSecret, authorizationCode, options);

			var graphClient = new GraphServiceClient(authCodeCredential, scopes);
			var result = await graphClient.Users["{user-id}"].GetAsync();

			return result?.Adapt<MicrosoftAADResponse>();
		}

	 
			 public async Task<string> MakeRequest(ClaimsPrincipal user)
			{
				var client = new HttpClient();
				var queryString = HttpUtility.ParseQueryString(string.Empty);

				/* OAuth2 is required to access this API. For more information visit:
				   https://msdn.microsoft.com/en-us/office/office365/howto/common-app-authentication-tasks */



				// Specify values for the following required parameters
				queryString["api-version"] = "1.6";
				// Specify values for path parameters (shown as {...})
				var uri = "https://graph.windows.net/myorganization/users/{user_id}?" + queryString;
			//client.DefaultRequestHeaders.Add("",);

				var response = await client.GetAsync(uri);

				if (response.Content != null)
				{
					var responseString = await response.Content.ReadAsStringAsync();
				 return responseString;
				}
				return string.Empty;
			}
		 
	}
}
