# How to load the data from SQLite offline database in SfListView?

You can create a local database using SQLite and populate data for SfListView from the database in Xamarin.Forms. You can also refer to the Xamarin.Forms document on the local database from here.

You can also refer the following article.

https://www.syncfusion.com/kb/7550/how-to-load-the-data-from-sqlite-offline-database-in-sflistview

Please follow the steps below to create a local database for ListView,

**Step 1:** Install [sqlite-net-pcl](https://www.nuget.org/packages/sqlite-net-pcl/) nuget package to the shared code project. You can refer to the SQLite documentation from [here](https://github.com/praeclarum/sqlite-net/wiki/GettingStarted).

**Step 2:** Create database

Initialize [SQLiteAsyncConnection](https://github.com/oysteinkrog/SQLite.Net-PCL/#sqliteasyncconnection) and create a database table using the **CreateTableAsync** method. The ToListAsync returns the table data in the List format. The InsertAsync, DeleteAsync and UpdateAsync methods used to perform CRUD operations.

``` c#
namespace ListViewXamarin
{
    public class SQLiteDatabase
    {
        readonly SQLiteAsyncConnection _database;
 
        public SQLiteDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Contacts>().Wait();
        }
 
        public Task<List<Contacts>> GetContactsAsync()
        {
            return _database.Table<Contacts>().ToListAsync();
        }
 
        public Task<int> AddContactAsync(Contacts item)
        {
            return _database.InsertAsync(item);
        }
 
        public Task<int> DeleteContactAsync(Contacts item)
        {
            return _database.DeleteAsync(item);
        }
 
        public Task<int> UpdateContactAsync(Contacts item)
        {
            return _database.UpdateAsync(item);
        }
    }
}
```
**Step 3:** Add SfListView in the [page](https://docs.microsoft.com/en-us/dotnet/api/xamarin.forms.contentpage)
``` xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:ListViewXamarin"
             xmlns:syncfusion="clr-namespace:Syncfusion.ListView.XForms;assembly=Syncfusion.SfListView.XForms"
             x:Class="ListViewXamarin.MainPage" Title="Contacts">
 
    <ContentPage.ToolbarItems>
        <ToolbarItem IconImageSource="Add.png" Text="Add contact" Command="{Binding CreateContactsCommand}"/>
    </ContentPage.ToolbarItems>
    
    <ContentPage.BindingContext>
        <local:ContactsViewModel x:Name="viewModel"/>
    </ContentPage.BindingContext>
    …
    <ContentPage.Content>
        <StackLayout>
            <syncfusion:SfListView x:Name="listView" ItemSize="60" ItemSpacing="5" TapCommand="{Binding EditContactsCommand}">
                <syncfusion:SfListView.ItemTemplate >
                    <DataTemplate>
                        …
                    </DataTemplate>
                </syncfusion:SfListView.ItemTemplate>
            </syncfusion:SfListView>
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```
**Step 4: Access data for ListView from database**

Create database instance in the **ViewModel** and initialize the database.
``` c#
namespace ListViewXamarin
{
    public class ContactsViewModel : INotifyPropertyChanged
    {
        private SQLiteDatabase database;
        private string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "contacts.db3");
 
        public SQLiteDatabase Database
        {
            get
            {
                if (database == null)
                    database = new SQLiteDatabase(path);
                return database;
            }
        }
    }
}
```
Get data from the database and assign it to the SfListView.ItemsSource property in the **OnAppearing** override method.
``` c#
namespace ListViewXamarin
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
 
        protected async override void OnAppearing()
        {   
            base.OnAppearing();
            listView.ItemsSource = await viewModel.Database.GetContactsAsync();
        }
    }
}
```
**Step 5:** Performing CRUD operations

In the command execution method, invoke the database asynchronous methods to perform operations.
``` c#
namespace ListViewXamarin
{
    public class ContactsViewModel : INotifyPropertyChanged
    {
        public Contacts Item { get; set; }
 
        public Command CreateContactsCommand { get; set; }
        public Command<object> EditContactsCommand { get; set; }
        public Command SaveItemCommand { get; set; }
        public Command DeleteItemCommand { get; set; }
        public Command AddItemCommand { get; set; }
 
        public ContactsViewModel()
        {
            CreateContactsCommand = new Command(OnCreateContacts);
            EditContactsCommand = new Command<object>(OnEditContacts);
            SaveItemCommand = new Command(OnSaveItem);
            DeleteItemCommand = new Command(OnDeleteItem);
            AddItemCommand = new Command(OnAddNewItem);
        }
 
        private async void OnAddNewItem()
        {
            await this.Database.AddContactAsync(Item);
            await App.Current.MainPage.Navigation.PopAsync();
        }
 
        private async void OnDeleteItem()
        {
            await this.Database.DeleteContactAsync(Item);
            await App.Current.MainPage.Navigation.PopAsync();
        }
 
        private async void OnSaveItem()
        {
            await this.Database.UpdateContactAsync(Item);
            await App.Current.MainPage.Navigation.PopAsync();
        }
 
        private void OnEditContacts(object obj)
        {
            Item = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Contacts;
            var editPage = new Views.EditPage();
            editPage.BindingContext = this;
            App.Current.MainPage.Navigation.PushAsync(editPage);
        }
 
        private void OnCreateContacts()
        {
            Item = new Contacts() { ContactName = "", ContactNumber="" } ;
            var editPage = new Views.EditPage();
            editPage.BindingContext = this;
            App.Current.MainPage.Navigation.PushAsync(editPage);
        }
    }
}
```
