using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListViewXamarin
{
    public partial class MainPage : ContentPage
    {
        #region Constructor
        public MainPage()
        {
            InitializeComponent();
        }
        #endregion

        protected async override void OnAppearing()
        {   
            base.OnAppearing();
            listView.ItemsSource = await viewModel.Database.GetContactsAsync();
        }
    }
}
