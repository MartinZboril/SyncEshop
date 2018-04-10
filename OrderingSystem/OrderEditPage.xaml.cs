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
    /// Interakční logika pro OrderEditPage.xaml
    /// </summary>
    public partial class OrderEditPage : Page
    {
        ObservableCollection<Goods> goods = new ObservableCollection<Goods>();
        List<GoodsOrder> goodsOrder = new List<GoodsOrder>();
        ObservableCollection<Goods> cart = new ObservableCollection<Goods>();
        Order Order = new Order();
        User User = new User();

        public OrderEditPage(User user, ObservableCollection<Goods> CartGoodsList)
        {
            InitializeComponent();
            User = user;
            cart = CartGoodsList;
            GetOrders(User);
            GetGoodsToBox();
            GetValueForShopCartInfo(cart);
        }

        public OrderEditPage(User user, ObservableCollection<Goods> CartGoodsList, string selectedtext)
        {
            InitializeComponent();
            User = user;
            cart = CartGoodsList;
            GetOrdersByBox(selectedtext, User);
            GetGoodsToBox();
            GetValueForShopCartInfo(cart);
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

        public async void GetGoodsToBox()
        {
            List<Goods> goods = new List<Goods>();
            goods = GoodsDatabase.GetItemsNotDoneAsync().Result;

            List<string> goodsName = new List<string>();

            for (int i = 0; i < goods.Count; i++)
            {
                goodsName.Add(goods[i].Name);
            }
            
        }

        public async Task GetOrders(User user)
        {
            List<Order> orders = new List<Order>();
            orders = OrderDatabase.GetItemsNotDoneAsync().Result;

            ObservableCollection<Order> NotDeletedOrders = new ObservableCollection<Order>();

            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].Deleted == 0 && (orders[i].UserID == user.ID || orders[i].UserID == user.localID))
                {
                    if (orders[i].Price == 0)
                    {
                        orders[i].Deleted = 1;
                        OrderDatabase.SaveItemAsync(orders[i]);
                        NavigationService ns = NavigationService.GetNavigationService(this);
                        ns.Navigate(new OrderEditPage(User, cart));
                    }
                    else
                    {
                        NotDeletedOrders.Add(orders[i]);
                    }
                }
            }

            listView2.ItemsSource = NotDeletedOrders;
            listView2.Visibility = Visibility.Visible;
            listViewDeletedOrders.Visibility = Visibility.Hidden;
        }

        public async Task GetDeletedOrders(User user)
        {
            List<Order> orders = new List<Order>();
            orders = OrderDatabase.GetItemsNotDoneAsync().Result;

            ObservableCollection<Order> DeletedOrders = new ObservableCollection<Order>();

            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].Deleted == 1 && (orders[i].UserID == user.ID || orders[i].UserID == user.localID))
                {
                    DeletedOrders.Add(orders[i]);
                }
            }

            listViewDeletedOrders.ItemsSource = DeletedOrders;
            listViewDeletedOrders.Visibility = Visibility.Visible;
            listView2.Visibility = Visibility.Hidden;
        }

        public async Task GetGoodsOrder(Order order)
        {
            if (order.ID != 0 || order.localID != 0)
            {
                goods.Clear();
                goodsOrder.Clear();

                if (order.ID == 0)
                {
                    goodsOrder = await GoodsOrderDatabase.GetGoodsOrderData(order.localID);
                }
                else
                {
                    goodsOrder = await GoodsOrderDatabase.GetGoodsOrderData(order.ID);
                }

                ObservableCollection<GoodsOrder> NotDeletedGoodsOrders = new ObservableCollection<GoodsOrder>();

                for (int i = 0; i < goodsOrder.Count; i++)
                {
                    NotDeletedGoodsOrders.Add(goodsOrder[i]);
                }

                for (int i = 0; i < NotDeletedGoodsOrders.Count; i++)
                {
                    Goods selectedGoods = new Goods();
                    selectedGoods = await GoodsDatabase.GetItemAsyncByID(NotDeletedGoodsOrders[i].GoodsID);
                    selectedGoods.ID = NotDeletedGoodsOrders[i].ID;
                    goods.Add(selectedGoods);
                }

                listView.ItemsSource = goods;
                listView.Visibility = Visibility.Visible;               
                listViewWithDeletedgoods.Visibility = Visibility.Hidden;
            }
        }

        public async Task GetDeletedGoodsOrder(Order order)
        {
            if (order.ID != 0 || order.localID != 0)
            {
                goods.Clear();
                goodsOrder.Clear();

                if (order.ID == 0)
                {
                    goodsOrder = await GoodsOrderDatabase.GetGoodsOrderData(order.localID);
                }
                else
                {
                    goodsOrder = await GoodsOrderDatabase.GetGoodsOrderData(order.ID);
                }


                ObservableCollection<GoodsOrder> NotDeletedGoodsOrders = new ObservableCollection<GoodsOrder>();

                for (int i = 0; i < goodsOrder.Count; i++)
                {
                    NotDeletedGoodsOrders.Add(goodsOrder[i]);
                }

                for (int i = 0; i < NotDeletedGoodsOrders.Count; i++)
                {
                    Goods selectedGoods = new Goods();
                    selectedGoods = await GoodsDatabase.GetItemAsyncByID(NotDeletedGoodsOrders[i].GoodsID);
                    selectedGoods.ID = NotDeletedGoodsOrders[i].ID;
                    goods.Add(selectedGoods);
                }

                listViewWithDeletedgoods.ItemsSource = goods;
                listViewWithDeletedgoods.Visibility = Visibility.Visible;
                listView.Visibility = Visibility.Hidden;
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new CatalogPage(User, cart));
        }

        private async void listView2_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            var order = listView2.SelectedItem as Order;
            if (order != null)
            {
                OrderNumber.Text = order.Number.ToString();
                OrderID.Text = order.localID.ToString();
                GetGoodsOrder(order);
            }
        }    

        private async void DeleteOrder_Button_Click(object sender, RoutedEventArgs e)
        {
            string item = (e.Source as Button).Tag.ToString();
            Order order = new Order();
            
            order = await OrderDatabase.GetItemAsyncByID(int.Parse(item));
            order.Deleted = 1;          
            OrderDatabase.SaveItemAsync(order);

            List<Order> orders = new List<Order>();
            orders = OrderDatabase.GetItemsNotDoneAsync().Result;

            ObservableCollection<Order> NotDeletedOrders = new ObservableCollection<Order>();

            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].Deleted == 0 && orders[i].localID == int.Parse(item))
                {
                    if (orders[i].Price == 0)
                    {
                        orders[i].Deleted = 1;
                        order = orders[i];
                    }
                }
            }

            OrderNumber.Text = order.Number.ToString();
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new OrderEditPage(User, cart));
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

        private async void OrderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)OrderBox.SelectedItem;
            string selectedText = cbi.Content.ToString();

            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new OrderEditPage(User, cart, selectedText));
        }

        public async void GetOrdersByBox(string selectedText, User user)
        {
            if (selectedText.Equals("skryté"))
            {
                GetDeletedOrders(user);
                GetGoodsToBox();
            }
            else
            {
                GetOrders(user);
                GetGoodsToBox();
            }
        }

        private void listViewDeletedOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var order = listViewDeletedOrders.SelectedItem as Order;
            if (order != null)
            {
                OrderNumber.Text = order.Number.ToString();
                OrderID.Text = order.localID.ToString();
                GetDeletedGoodsOrder(order);
            }
        }

        private async void RestoreOrder_Button_Click(object sender, RoutedEventArgs e)
        {
            string item = (e.Source as Button).Tag.ToString();
            Order order = new Order();
            order = await OrderDatabase.GetItemAsyncByID(int.Parse(item));
            order.Deleted = 0;
            OrderDatabase.SaveItemAsync(order);

            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new OrderEditPage(User, cart));
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