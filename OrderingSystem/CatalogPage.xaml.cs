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
    /// Interakční logika pro CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        List<Goods> goods = new List<Goods>();
        ObservableCollection<Goods> cart = new ObservableCollection<Goods>();
        User User = new User();
        int tempSelectedIndex;

        public CatalogPage()
        {
            InitializeComponent();
            GetGoods();
        }

        public CatalogPage(Goods category, ObservableCollection<Goods> CartGoodsList)
        {
            InitializeComponent();
            GetGoodsByCategory(category.Category);
            cart = CartGoodsList;
            GetValueForShopCartInfo(cart);
        }

        public CatalogPage(User user)
        {
            InitializeComponent();
            User = user;
            Registration.Visibility = Visibility.Hidden;
            Login.Visibility = Visibility.Hidden;
            Profile.Visibility = Visibility.Visible;
            GetGoods();
        }

        public CatalogPage(User user, ObservableCollection<Goods> CartGoodsList)
        {
            InitializeComponent();
            User = user;
            Registration.Visibility = Visibility.Hidden;
            Login.Visibility = Visibility.Hidden;
            Profile.Visibility = Visibility.Visible;
            cart = CartGoodsList;
            GetValueForShopCartInfo(cart);
            GetGoods();
        }

        public CatalogPage(ObservableCollection<Goods> CartGoodsList)
        {
            InitializeComponent();

            cart = CartGoodsList;
            GetValueForShopCartInfo(cart);
            GetGoods();
        }

        public async Task GetGoods()
        {
            goods = GoodsDatabase.GetItemsNotDoneAsync().Result;
            GoodsListview.ItemsSource = goods;

            GetCategories(goods);
        }

        private async void SearchByLetter_Click(object sender, RoutedEventArgs e)
        {
            var keyword = (e.Source as Button).Content.ToString();
            string SearchWord = keyword;

            ObservableCollection<Goods> selectedGoodsByLetter = new ObservableCollection<Goods>();
            goods = GoodsDatabase.GetItemsNotDoneAsync().Result;

            for (int i = 0; i < goods.Count; i++)
            {
                if (goods[i].Name.Substring(0, 1).Equals(SearchWord))
                {
                    selectedGoodsByLetter.Add(goods[i]);
                }
            }

            GoodsListview.ItemsSource = selectedGoodsByLetter;
        }

        private async void OrderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            goods = GoodsDatabase.GetItemsNotDoneAsync().Result;

            ComboBoxItem cbi = (ComboBoxItem)OrderBox.SelectedItem;
            string selectedText = cbi.Content.ToString();

            if (selectedText.Equals("od nejlevnejsiho"))
            {
                var goodsByLowestPrice = goods.OrderBy(a => a.Price);
                GoodsListview.ItemsSource = goodsByLowestPrice;
            }
            else if (selectedText.Equals("od nejdrazsiho"))
            {
                var goodsByLowestPrice = goods.OrderByDescending(a => a.Price);
                GoodsListview.ItemsSource = goodsByLowestPrice;
            }
            else if (selectedText.Equals("od nejnovejsiho"))
            {
                var goodsByLowestPrice = goods.OrderByDescending(a => a.YearOfRealising);
                GoodsListview.ItemsSource = goodsByLowestPrice;
            }
            else if (selectedText.Equals("od nejstarsiho"))
            {
                var goodsByLowestPrice = goods.OrderBy(a => a.YearOfRealising);
                GoodsListview.ItemsSource = goodsByLowestPrice;
            }
            else if (selectedText.Equals("dle nazvu"))
            {
                var goodsByLowestPrice = goods.OrderBy(a => a.Name);
                GoodsListview.ItemsSource = goodsByLowestPrice;
            }
            else
            {
                GoodsListview.ItemsSource = goods;
            }

        }

        private void GoodsListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GoodsListview.SelectedItem != null)
            {
                var selectedGoods = GoodsListview.SelectedItem as Goods;
                tempSelectedIndex = GoodsListview.SelectedIndex;

                if (User.localID == 0)
                {               
                    NavigationService ns = NavigationService.GetNavigationService(this);
                    ns.Navigate(new DetailPage(selectedGoods, cart));
                }
                else
                {
                    NavigationService ns = NavigationService.GetNavigationService(this);
                    ns.Navigate(new DetailPage(selectedGoods, User, cart));
                }
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.localID != 0)
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new CartPage(cart, User));
            }
            else
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new CartPage(cart));
            }
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

        public async void Buy_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string item = (e.Source as Button).Tag.ToString();
            Goods selectedGoods = new Goods();
            selectedGoods = GoodsDatabase.GetItemByIDAsync(item).Result;

            if (cart.Count > 0)
            {
                var maxIDcart = cart.Max(m => m.IDcart);
                selectedGoods.IDcart = maxIDcart + 1;
            }
            else
            {
                selectedGoods.IDcart = 1;
            }

            cart.Add(selectedGoods);
            PieceOFSelectedGoods.Text = cart.Count.ToString();

            if (User.localID != 0)
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new CartPage(cart, User));
            }
            else
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new CartPage(cart));
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

        private async void ListViewOfCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewOfCategories.SelectedItem != null)
            {
                var category = ListViewOfCategories.SelectedItem as Goods;
                GetGoodsByCategory(category.Category);
            }
        }

        public async void GetGoodsByCategory(string category)
        {
            goods = GoodsDatabase.GetItemsNotDoneAsync().Result;
            ObservableCollection<Goods> goodsByCategory = new ObservableCollection<Goods>();
            Login.Content = category;

            for (int i = 0; i < goods.Count; i++)
            {
                if (goods[i].Category.Equals(category))
                {
                    goodsByCategory.Add(goods[i]);
                }
            }

            GoodsListview.ItemsSource = goodsByCategory;

            GetCategories(goods);
        }

        public void GetCategories(List<Goods> goods)
        {
            List<string> categories = new List<string>();

            for (int i = 0; i < goods.Count; i++)
            {
                if (categories.Contains(goods[i].Category))
                {

                }
                else
                {
                    categories.Add(goods[i].Category);
                }
            }

            ObservableCollection<Goods> goodsCategories = new ObservableCollection<Goods>();

            for (int x = 0; x < categories.Count; x++)
            {
                Goods goodsToList = new Goods();
                goodsToList.Category = categories[x];
                goodsCategories.Add(goodsToList);
            }

            ListViewOfCategories.ItemsSource = goodsCategories;
        }

        private async void AllCategories_Click(object sender, RoutedEventArgs e)
        {
            GetGoods();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            goods = GoodsDatabase.GetItemsNotDoneAsync().Result;

            string searchWord = Search.Text;
            var searchingGoods = goods.Where(x => x.Name.Contains(searchWord));

            GoodsListview.ItemsSource = searchingGoods;
        }

        private void SearchOrder_Click(object sender, RoutedEventArgs e)
        {
            if (User.localID != 0)
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new OrderViewPage(User, cart));
            }
            else
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new OrderViewPage(cart));
            }
        }

        private static GoodsDatabase _goodsDatabase;
        public static GoodsDatabase GoodsDatabase
        {
            get
            {
                if (_goodsDatabase == null)
                {
                    var fileHelper = new FileHelper();
                    _goodsDatabase = new GoodsDatabase(fileHelper.GetLocalFilePath("TodoSQLite.db3"));
                }
                return _goodsDatabase;
            }
        }
    }
}
