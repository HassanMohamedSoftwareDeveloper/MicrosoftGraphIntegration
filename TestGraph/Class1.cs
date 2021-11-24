//using System;
//using System.Diagnostics;
//using System.Net.Http.Headers;
//using System.Net.Http;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Graph;
//using Microsoft.Identity.Client;

//namespace MSGraphConsole
//{
//    public class AuthenticationHelper
//    {
//        // The Client ID is used by the application to uniquely identify itself to the v2.0 authentication endpoint.  
//        // Add appId of your registered App  
//        static readonly string clientId = "51509718-3125-438f-z043-z70052846534";
//        public static string[] Scopes = { "User.Read", "Mail.Send", "Files.ReadWrite" };

//        public static PublicClientApplication IdentityClientApp = new Microsoft.Graph.PublicClientApplication(clientId);

//        public static string TokenForUser = null;
//        public static DateTimeOffset Expiration;

//        private static GraphServiceClient graphClient = null;

//        // Get an access token for the given context and resourced. An attempt is first made to   
//        // acquire the token silently. If that fails, then we try to acquire the token by prompting the user.  
//        public static GraphServiceClient GetAuthenticatedClient()
//        {
//            if (graphClient == null)
//            {
//                // Create Microsoft Graph client.  
//                try
//                {
//                    graphClient = new GraphServiceClient(
//                        "https://graph.microsoft.com/v1.0",
//                        new DelegateAuthenticationProvider(
//                            async (requestMessage) =>
//                            {
//                                var token = await GetTokenForUserAsync();
//                                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
//                                // This header has been added to identify our sample in the Microsoft Graph service.  If extracting this code for your project please remove.  
//                                requestMessage.Headers.Add("SampleID", "MSGraphConsoleApp");

//                            }));
//                    return graphClient;
//                }

//                catch (Exception ex)
//                {
//                    Debug.WriteLine("Could not create a graph client: " + ex.Message);
//                }
//            }

//            return graphClient;
//        }


//        /// <summary>  
//        /// Get Token for User.  
//        /// </summary>  
//        /// <returns>Token for user.</returns>  
//        public static async Task<string> GetTokenForUserAsync()
//        {
//            AuthenticationResult authResult;
//            try
//            {
//                authResult = await IdentityClientApp.AcquireTokenSilentAsync(Scopes, IdentityClientApp.GetAccountsAsync().Result.First());
//                TokenForUser = authResult.AccessToken;
//            }

//            catch (Exception)
//            {
//                if (TokenForUser == null || Expiration <= DateTimeOffset.UtcNow.AddMinutes(5))
//                {
//                    authResult = await IdentityClientApp.AcquireTokenAsync(Scopes);

//                    TokenForUser = authResult.AccessToken;
//                    Expiration = authResult.ExpiresOn;
//                }
//            }

//            return TokenForUser;
//        }

//        /// <summary>  
//        /// Signs the user out of the service.  
//        /// </summary>  
//        public static void SignOut()
//        {
//            foreach (var user in IdentityClientApp.GetAccountsAsync().Result)
//            {
//                IdentityClientApp.RemoveAsync(user);
//            }
//            graphClient = null;
//            TokenForUser = null;

//        }

//    }
//}