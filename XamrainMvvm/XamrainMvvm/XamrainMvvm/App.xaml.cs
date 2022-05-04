using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamrainMvvm.Services;
using XamrainMvvm.Views;

namespace XamrainMvvm
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
//https://devlinduldulao.pro/how-to-create-a-barchart-linechart-radialchart-pointchart-or-donutchart-in-xamarin-forms/