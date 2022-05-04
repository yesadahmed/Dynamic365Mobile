using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamrainMvvm.Models;
using XamrainMvvm.Services;

namespace XamrainMvvm.ViewModels
{
    public class MainLoginViewModel : BaseViewModel
    {
        readonly HttpClient httpClient;
        private readonly MicrosoftAuthenticationService _authService;
        private readonly MsGraphGetSimpleInfo _simpleGraphService;


        private bool isSignedIn;
        private bool isSigningIn;
        private string name;
        private string logintext = "Please login into  CRM instance.";
        public ObservableCollection<EntityInfo> _entitiesCount;
        //public ObservableCollection<EntityInfo> entitiesCount { get; set; }


        public ObservableCollection<EntityInfo> entitiesCount
        {

            get => _entitiesCount;
            set => SetProperty(ref _entitiesCount, value);

        }

        public string Logintext
        {

            get => logintext;
            set => SetProperty(ref logintext, value);

        }

        public bool IsSignedIn
        {

            get => isSignedIn;
            set => SetProperty(ref isSignedIn, value);

        }

        public bool IsSigningIn
        {

            get => isSigningIn;
            set => SetProperty(ref isSigningIn, value);

        }
        public string Name
        {

            get => name;
            set => SetProperty(ref name, value);

        }

        public ICommand SignInCommand { get; }
        public ICommand SignOutCommand { get; }

        public ICommand LoadCrmEntitiescount { get; }


        public MainLoginViewModel()
        {
            _authService = new MicrosoftAuthenticationService();
            _simpleGraphService = new MsGraphGetSimpleInfo();

            SignInCommand = new Command(async () => await SignInAsync());

            SignOutCommand = new Command(async () => await SignOutAsync());

            LoadCrmEntitiescount = new Command(async () => await GetCrmInfo());


            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://*****.api.crm4.dynamics.com/api/data/v9.2/");
            httpClient.Timeout = TimeSpan.FromSeconds(120);
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            entitiesCount = new ObservableCollection<EntityInfo>();
        }

        async Task SignInAsync()
        {
            IsSigningIn = true;

            if (await _authService.SignInAsync())
            {
                Name = await _simpleGraphService.GetNameAsync();
                Logintext = "Count of entities.";
                IsSignedIn = true;
                if (!string.IsNullOrWhiteSpace(Name))
                    await GetCrmInfo();
            }

            IsSigningIn = false;
        }

        async Task SignOutAsync()
        {
            if (await _authService.SignOutAsync())
            {
                Name = await _simpleGraphService.GetNameAsync();
                Logintext = "Please login into your CRM instance";
                IsSignedIn = false;
                entitiesCount = new ObservableCollection<EntityInfo>();
            }
        }


        async Task GetCrmInfo()
        {
            Random rand = new Random();
            List<string> colorsLst = new List<string>()
            { "LightBlue","Brown","LightRed","Green","Purple","Pink",
                "Yellow","Blue","PeachPuff"};
            EntityInfo entityInfo = null;
            var oauthToken = await SecureStorage.GetAsync("AccessToken");
            if (!string.IsNullOrWhiteSpace(oauthToken))
            {
                var crmtoken = await _simpleGraphService.GetCrmDataAsync();
                httpClient.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", crmtoken);

                string query = "RetrieveTotalRecordCount(EntityNames=['account','contact','incident','systemuser','email','activitypointer','phonecall', 'task'])";
                var response = await httpClient.GetAsync(query);
                var contents = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(contents))
                {
                    var result = JsonConvert.DeserializeObject<EntityData>(contents);
                    if (result != null && result.EntityRecordCountCollection != null)
                    {
                        if (result.EntityRecordCountCollection.Keys != null
                            && result.EntityRecordCountCollection.Values != null)
                        {
                            int count = 0;
                            foreach (var ent in result.EntityRecordCountCollection.Keys)
                            {
                                entityInfo = new EntityInfo();
                                entityInfo.EntityName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ent.ToLower()) + "s";
                                entityInfo.Count = result.EntityRecordCountCollection.Values[count].ToString();
                                entityInfo.Color = colorsLst[count];
                                count++;
                                entitiesCount.Add(entityInfo);
                            }
                        }

                    }

                }

            }

        }


    }
}
