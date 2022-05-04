using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace XamrainMvvm.Services
{

    public class MicrosoftAuthenticationService
    {
        string RedirectUri
        {
            get
            {
                return $"msauth://com.mobile.app/ni%2B7c%2FQkiX8WjmGONe%2BaQkI9oFM%3D";
                ////if (DeviceInfo.Platform == DevicePlatform.Android)
                ////    return $"msauth://com.powertech.powermobilecrm/ni%2B7c%2FQkiX8WjmGONe%2BaQkI9oFM%3D";
                ////else if (DeviceInfo.Platform == DevicePlatform.iOS)
                ////    return $"msauth.{AppId}://auth";

                return string.Empty;
            }
        }

        readonly string AppId = "com.mobile.app";
        readonly string ClientID = "9ec175e7-*********-*********";
        readonly string[] Scopes = { "User.Read"};
        readonly IPublicClientApplication _pca;

        // Android uses this to determine which activity to use to show
        // the login screen dialog from.
        public static object ParentWindow { get; set; }

        public MicrosoftAuthenticationService()
        {
            _pca = PublicClientApplicationBuilder.Create(ClientID)
                .WithIosKeychainSecurityGroup(AppId)
                .WithRedirectUri(RedirectUri)
                .WithAuthority("https://login.microsoftonline.com/common")                
                .Build();


         
        }

        public async Task<bool> SignInAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();
                var authResult = await _pca.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();

                // Store the access token securely for later use.
                await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                return true;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This means we need to login again through the MSAL window.
                    var authResult = await _pca.AcquireTokenInteractive(Scopes)
                                                .WithParentActivityOrWindow(ParentWindow)
                                                .WithUseEmbeddedWebView(true)
                                                .ExecuteAsync();

                    // Store the access token securely for later use.
                    await SecureStorage.SetAsync("AccessToken", authResult?.AccessToken);

                    return  true;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine(ex2.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();

                // Go through all accounts and remove them.
                while (accounts.Any())
                {
                    await _pca.RemoveAsync(accounts.FirstOrDefault());
                    accounts = await _pca.GetAccountsAsync();
                }

                // Clear our access token from secure storage.
                SecureStorage.Remove("AccessToken");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }


       

    }

}
