using OrderingSystem.Database;
using OrderingSystem.DataService;
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
    /// Interakční logika pro ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        User User = new User();
        ObservableCollection<Goods> cart = new ObservableCollection<Goods>();
        public ProfilePage(User user, ObservableCollection<Goods> CartGoodsList)
        {
            InitializeComponent();
            User = user;
            Name.Text = User.Name;
            Surname.Text = User.Surname;
            Email.Text = User.Email;
            Phone.Text = User.Phone.ToString();
            Password.Password = User.Password;
            cart = CartGoodsList;
            GetValueForShopCartInfo(cart);
        }

        public ProfilePage(User user)
        {
            InitializeComponent();
            User = user;
            Name.Text = User.Name;
            Surname.Text = User.Surname;
            Email.Text = User.Email;
            Phone.Text = User.Phone.ToString();
            Password.Password = User.Password;
        }

        public void GetValueForShopCartInfo(ObservableCollection<Goods> GoodsFromCart)
        {
            int TotalPrice = GetTotalPriceOfSelectedGoods(GoodsFromCart);
            PriceOFSelectedGoods.Text = TotalPrice.ToString() + " " + "Kč";
            int TotalPiece = GoodsFromCart.Count;
            PieceOFSelectedGoods.Text = TotalPiece.ToString() + " " + "Položek";
        }

        public int GetTotalPriceOfSelectedGoods(ObservableCollection<Goods> cartgoods)
        {
            int TotalPrice = 0;
            for (int i = 0; i < cartgoods.Count; i++)
            {
                TotalPrice += cartgoods[i].Price;
            }
            return TotalPrice;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new CatalogPage(User, cart));
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new OrderEditPage(User, cart));
        }

        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Password.Password.ToString()) && !String.IsNullOrEmpty(NewPassword.Password.ToString()) && !String.IsNullOrEmpty(ConfirmPassword.Password.ToString()))
            {
                List<User> users = new List<User>();
                users = UserDatabase.GetItemsNotDoneAsync().Result;

                var user = users.FirstOrDefault(p => p.localID == User.localID);


                if (user.Password == Password.Password.ToString())
                {
                    if (NewPassword.Password.ToString().Equals(ConfirmPassword.Password.ToString()))
                    {
                        user.Password = ConfirmPassword.Password.ToString();
                        UserDatabase.UpdateItem(user);

                        string Message = "Heslo změněno, přihlaš te se novým heslem." ;

                        NavigationService navigation = NavigationService.GetNavigationService(this);
                        navigation.Navigate(new LoginPage(Message));
                    }
                    else
                    {
                        FailsDisplay.Text = "Hesla se neshodují!";
                    }
                }
                else
                {
                    FailsDisplay.Text = "Chybně zadané současné heslo!";
                }
            }
            else
            {
                FailsDisplay.Text = "Není vše vyplněno!";
            }
        }

        private async void ChangeUserInformationButton_Click(object sender, RoutedEventArgs e)
        {
            List<User> users = new List<User>();
            users = UserDatabase.GetItemsNotDoneAsync().Result;

            FailsDisplay.Text = "";
            if (!String.IsNullOrEmpty(Name.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Phone.Text))
            {
                if (Email.Text.Contains("@") && Email.Text.Contains("."))
                {
                    int phone = 0;
                    if (int.TryParse(Phone.Text, out phone))
                    {
                        bool contains = users.Any(p => p.Email == Email.Text);

                        if (User.Email.Equals(Email.Text))
                        {
                            User user = new User();
                            user.localID = User.localID;
                            user.Name = Name.Text;
                            user.Surname = Surname.Text;
                            user.Password = User.Password;
                            user.Email = Email.Text;
                            user.Phone = int.Parse(Phone.Text);

                            UserDatabase.SaveItem(user);

                            NavigationService ns = NavigationService.GetNavigationService(this);
                            ns.Navigate(new CatalogPage(user, cart));
                        }
                        else if (contains)
                        {
                            FailsDisplay.Text = "Email už je zabraný!";
                        }
                        else
                        {
                            User user = new User();
                            user.localID = User.localID;
                            user.Name = Name.Text;
                            user.Surname = Surname.Text;
                            user.Password = User.Password;
                            user.Email = Email.Text;
                            user.Phone = int.Parse(Phone.Text);

                            UserDatabase.SaveItem(user);

                            NavigationService ns = NavigationService.GetNavigationService(this);
                            ns.Navigate(new CatalogPage(user, cart));
                        }
                    }
                    else
                    {
                        FailsDisplay.Text = "Telefon není z číslic!";
                    }
                }
                else
                {
                    FailsDisplay.Text = "Nesprávný tvar emailu!";
                }
            }

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new CatalogPage(cart));
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new ProfilePage(User, cart));
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new CartPage(cart, User));
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

        private static GoodsOrderDatabase _goodsOrderDatabase;
        public static GoodsOrderDatabase GoodsOrderDatabase
        {
            get
            {
                if (_goodsOrderDatabase == null)
                {
                    var fileHelper = new FileHelper();
                    _goodsOrderDatabase = new GoodsOrderDatabase(fileHelper.GetLocalFilePath("TodoSQLite.db3"));
                }
                return _goodsOrderDatabase;
            }
        }
    }
}
