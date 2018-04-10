using OrderingSystem.Database;
using OrderingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrderingSystem
{
    /// <summary>
    /// Interakční logika pro OrderViewPage.xaml
    /// </summary>
    public partial class OrderViewPage : Page
    {
        Order Order = new Order();
        User User = new User();
        ObservableCollection<Goods> cart = new ObservableCollection<Goods>();
        public OrderViewPage(User user, ObservableCollection<Goods> CartGoodsList)
        {
            InitializeComponent();
            User = user;
            cart = CartGoodsList;
        }

        public OrderViewPage(ObservableCollection<Goods> CartGoodsList)
        {
            InitializeComponent();
            cart = CartGoodsList;
        }

        private async void SearchOrderButton_Click(object sender, RoutedEventArgs e)
        {
            int searchingOrderNumber = 0;
            if (SearchWordText.Text.Length != 0 && int.TryParse(SearchWordText.Text, out searchingOrderNumber))
            {
                List<Order> orders = new List<Order>();
                orders = OrderDatabase.GetItemsNotDoneAsync().Result;

                for (int i = 0; i < orders.Count; i++)
                {
                    if (orders[i].Number == searchingOrderNumber && orders[i].Deleted == 0)
                    {
                        Order = orders[i];
                    }
                }

                List<User> users = new List<User>();
                users = UserDatabase.GetItemsNotDoneAsync().Result;
                User selectedUser = new User();

                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i].localID == Order.UserID)
                    {
                        selectedUser = users[i];
                    }
                }
                
                if (Order.localID != 0 && selectedUser.localID != 0)
                {

                    Name.Text = selectedUser.Name;
                    Surname.Text = selectedUser.Surname;
                    Email.Text = selectedUser.Email;
                    Phone.Text = selectedUser.Phone.ToString();

                    Number.Text = Order.Number.ToString();
                    Price.Text = Order.Price.ToString();

                    Town.Text = Order.Town;
                    Street.Text = Order.Street;
                    PostNumber.Text = Order.PostNumber.ToString();
                    FailDisplay.Text = "";
                }
                else
                {
                    Name.Text = "";
                    Surname.Text = "";
                    Email.Text = "";
                    Phone.Text = "";

                    Number.Text = "";
                    Price.Text = "";

                    Town.Text = "";
                    Street.Text = "";
                    PostNumber.Text = "";
                    FailDisplay.Text = "Objednávka neexistuje!";
                }
            }
            else
            {
                Name.Text = "";
                Surname.Text = "";
                Email.Text = "";
                Phone.Text = "";

                Number.Text = "";
                Price.Text = "";

                Town.Text = "";
                Street.Text = "";
                PostNumber.Text = "";
                FailDisplay.Text = "Není vyplněno číslo objednávky nebo objednávka neexistuje!";
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.localID != 0)
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new CatalogPage(User, cart));
            }
            else
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new CatalogPage(cart));
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new LoginPage());
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new RegistrationPage());
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new ProfilePage(User, cart));
        }

        private static OrderDatabase _orderDatabase;
        public static OrderDatabase OrderDatabase
        {
            get
            {
                if (_orderDatabase == null)
                {
                    var fileHelper = new FileHelper();
                    _orderDatabase = new OrderDatabase(fileHelper.GetLocalFilePath("TodoSQLite.db3"));
                }
                return _orderDatabase;
            }
        }

        private static UserDatabase _userdatabase;
        public static UserDatabase UserDatabase
        {
            get
            {
                if (_userdatabase == null)
                {
                    var fileHelper = new FileHelper();
                    _userdatabase = new UserDatabase(fileHelper.GetLocalFilePath("TodoSQLite.db3"));
                }
                return _userdatabase;
            }
        }
    }
}
