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
using System.Net.NetworkInformation;
using OrderingSystem.Database;

namespace OrderingSystem
{
    /// <summary>
    /// Interakční logika pro RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private async void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            List<User> users = new List<User>();
            users = UserDatabase.GetItemsNotDoneAsync().Result;

            FailsDisplay.Text = "";
            if (!String.IsNullOrEmpty(Name.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Phone.Text) && !String.IsNullOrEmpty(Password.Password.ToString()) && !String.IsNullOrEmpty(ConfirmPassword.Password.ToString()))
            {
                if (Password.Password.ToString().Length > 7)
                {
                    if (Password.Password.ToString().Equals(ConfirmPassword.Password.ToString()))
                    {
                        if (Email.Text.Contains("@") && Email.Text.Contains("."))
                        {
                            int phone = 0;
                            if (int.TryParse(Phone.Text, out phone))
                            {
                                bool contains = users.Any(p => p.Email == Email.Text);

                                if (contains)
                                {
                                    FailsDisplay.Text = "Email už je zabraný!";
                                }
                                else
                                {
                                    User user = new User();
                                    user.Name = Name.Text;
                                    user.Surname = Surname.Text;
                                    user.Password = Password.Password.ToString();
                                    user.Email = Email.Text;
                                    user.Phone = int.Parse(Phone.Text);
                                    UserDatabase.SaveItem(user);

                                    NavigationService ns = NavigationService.GetNavigationService(this);
                                    ns.Navigate(new CatalogPage(user));
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
                    else
                    {
                        FailsDisplay.Text = "Hesla se neshodují!";
                    }
                }
                else
                {
                    FailsDisplay.Text = "Heslo je příliš krátké!";
                }
            }
            else
            {
                FailsDisplay.Text = "Není vše vyplněno!";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new CatalogPage());
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new LoginPage());
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
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
