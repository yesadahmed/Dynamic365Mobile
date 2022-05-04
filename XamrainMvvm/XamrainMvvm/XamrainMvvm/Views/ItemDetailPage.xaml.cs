using System.ComponentModel;
using Xamarin.Forms;
using XamrainMvvm.ViewModels;

namespace XamrainMvvm.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}