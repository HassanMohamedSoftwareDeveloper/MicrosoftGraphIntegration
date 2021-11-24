using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;

namespace TestGraph
{
    public class DeviceCodeAuthProvider : IAuthenticationProvider
    {
        private IPublicClientApplication _msalClient;
        private static string[] _scopes = new[] { "User.Read", "Calendars.ReadWrite", "MailboxSettings.Read" };
        private IAccount _userAccount;
        private static string ClientId = "b8b06078-09e2-4e22-8ded-df1bcebe4339";
        private static string Tenant = "1ffe946b-7229-4311-b27a-39001f297202";
        public DeviceCodeAuthProvider()
        {
            _msalClient = PublicClientApplicationBuilder.Create(ClientId)
                .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                .WithAuthority(AzureCloudInstance.AzurePublic, Tenant)
                .Build();
        }
        public async Task<string> GetATokenForGraph()
        {
            try
            {
                if (_userAccount == null)
                {
                    var securePassword = new SecureString();
                    foreach (char c in "Dodohm@1234")
                        securePassword.AppendChar(c);
                    var authResult =
                    await _msalClient.AcquireTokenByUsernamePassword(_scopes, "HassanMohamed_HM@HassanMohamedHMhotmail.onmicrosoft.com",
                    securePassword).ExecuteAsync();
                    _userAccount = authResult.Account;
                    return authResult.AccessToken;
                }
                else
                {
                    var result = await _msalClient
                    .AcquireTokenSilent(_scopes, _userAccount)
                    .ExecuteAsync();
                    return result.AccessToken;
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }
        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            var token = await GetATokenForGraph();
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", token);//GetAccessToken()
        }
    }
}
