using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace XamrainMvvm.Services
{
    public class MsGraphGetSimpleInfo
    {
        public async Task<string> GetNameAsync()
        {
            using (var client = new HttpClient())
            {
                var token = await SecureStorage.GetAsync("AccessToken");

                if (!string.IsNullOrEmpty(token))
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                    message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.SendAsync(message);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = (JObject)JsonConvert.DeserializeObject(json);

                        if ((data.ContainsKey("givenName")) || (data.ContainsKey("given_name")))
                            return data["givenName"].Value<string>();
                        else
                            return "Mr. No Name";
                        //currentUser = JsonConvert.DeserializeObject<User>(json);
                    }
                }
                else
                {
                    return "Token Invalid";
                }
            }

            return "Name unknown";
        }

        public async Task<string> GetCrmDataAsync()
        {
            string accesstoken = "";
            
            string redirectUrl = $"msauth://com.mobile.app/ni%2B7c%2FQkiX8WjmGONe%2BaQkI9oFM%3D";//replace with your azure app redirect 
            string clientId = "9ec175e7-********";

            
            var authBuilder = PublicClientApplicationBuilder.Create(clientId)
                            .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                            .WithRedirectUri(redirectUrl)
                            .Build();

            var scope =  "https://****.api.crm4.dynamics.com/.default";
            string[] scopes = { scope };

            string username = "*****@***.onmicrosoft.com";//replace with yours
            string password = "******";//replace with yours

            AuthenticationResult authBuilderResult= null;

            try
            {
                if (username != string.Empty && password != string.Empty)
                {
                    //Make silent Microsoft.Identity.Client (MSAL) OAuth Token Request
                    var securePassword = new SecureString();
                    foreach (char ch in password) securePassword.AppendChar(ch);
                    authBuilderResult = authBuilder.AcquireTokenByUsernamePassword(scopes, username, securePassword)
                                .ExecuteAsync().Result;
                    accesstoken = authBuilderResult.AccessToken;
                }
                else
                {
                    //Popup authentication dialog box to get token
                    authBuilderResult = authBuilder.AcquireTokenInteractive(scopes)
                                .ExecuteAsync().Result;
                    accesstoken = authBuilderResult.AccessToken;
                }

            }
            catch (Exception x)
            {

               
            }
          

           

            return authBuilderResult.AccessToken;
        }

    }
}
