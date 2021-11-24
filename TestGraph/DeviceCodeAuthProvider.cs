using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TestGraph
{
    public class DeviceCodeAuthProvider : IAuthenticationProvider
    {
        private IPublicClientApplication _msalClient;
        private string[] _scopes;
        private IAccount _userAccount;

        public DeviceCodeAuthProvider(string appId, string[] scopes)
        {
            _scopes = scopes;
            _msalClient = PublicClientApplicationBuilder
                .CreateWithApplicationOptions(new PublicClientApplicationOptions()
                {
                    ClientId= "b8b06078-09e2-4e22-8ded-df1bcebe4339",
                    TenantId= "1ffe946b-7229-4311-b27a-39001f297202",
                }).Build();
                //.Create(appId)
                //.WithAuthority(AzureCloudInstance.AzurePublic, "6517d7e1-0324-4347-a1c8-f2376a7e8b38", true)
                //.Build();
            //.WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
            //    .WithAuthority(AzureCloudInstance.AzurePublic, "8c7c7c28-320f-4385-aa6b-19348f852df0")
            //    .Build();
        }
        public async Task<string> GetATokenForGraph()
        {
            var accounts = await _msalClient.GetAccountsAsync();
            _userAccount = accounts.FirstOrDefault();
            AuthenticationResult result = null;
            if (accounts.Any())
            {
                result = await _msalClient.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            else
            {
                try
                {
                    var securePassword = new SecureString();
                    foreach (char c in "dodo1234")        // you should fetch the password
                        securePassword.AppendChar(c);  // keystroke by keystroke

                    result = await _msalClient
                        .AcquireTokenByUsernamePassword(_scopes,
                                                                     "hassanmohamed_hm@hotmail.com",
                                                                      securePassword)
                                       .ExecuteAsync();
                }
                catch (MsalException e)
                {
                    // See details below
                }
            }
            Console.WriteLine(result.Account.Username);
            return result.AccessToken;
        }
        public async Task<string> GetAccessToken()
        {
            var accounts = await _msalClient.GetAccountsAsync();
            _userAccount = accounts.FirstOrDefault();
            // If there is no saved user account, the user must sign-in
            if (_userAccount == null)
            {
                try
                {
                    // Invoke device code flow so user can sign-in with a browser
                    var result = await _msalClient.AcquireTokenWithDeviceCode(_scopes, callback => {
                        Console.WriteLine(callback.Message);
                        return Task.FromResult(0);
                    }).ExecuteAsync();

                    _userAccount = result.Account;
                    return result.AccessToken;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error getting access token: {exception.Message}");
                    return null;
                }
            }
            else
            {
                // If there is an account, call AcquireTokenSilent
                // By doing this, MSAL will refresh the token automatically if
                // it is expired. Otherwise it returns the cached token.

                var result = await _msalClient
                    .AcquireTokenSilent(_scopes, _userAccount)
                    .ExecuteAsync();

                return result.AccessToken;
            }
        }


        // This is the required function to implement IAuthenticationProvider
        // The Graph SDK will call this function each time it makes a Graph
        // call.
        public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("bearer", await GetATokenForGraph());//GetAccessToken()
        }
    }
}
