using OrderingSystem.Database;
using OrderingSystem.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace OrderingSystem.DataService
{
    public class syncService
    {
        private ObservableCollection<User> _remoteUsers;
        private ObservableCollection<Goods> _remoteGoods;
        private ObservableCollection<GoodsOrder> _remoteGoodsOrder;
        private ObservableCollection<Order> _remoteOrder;

        private User localUser;
        private Goods localGoods;
        private Goods localOrders;
        private GoodsOrder localGoodsOrders;

        private dataService dataservice = new dataService();

        public async Task SyncAsync()
        {
            await PullAsync();
        }

        private async Task CompareUsers()
        {
            foreach (User remoteUser in _remoteUsers)
            {
                List<User> users = new List<User>();
                users = UserDatabase.GetItemsNotDoneAsync().Result;

                if (users.Count > 0)
                {
                    bool check = true;

                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].ID == remoteUser.ID)
                        {
                            check = false;
                        }
                    }

                    if (true)
                    {
                        UserDatabase.SaveItem(remoteUser);
                    }
                } 
                else
                {
                    UserDatabase.SaveItem(remoteUser);
                }
            }

            List<User> usersToCheck = new List<User>();
            usersToCheck = UserDatabase.GetItemsNotDoneAsync().Result;

            for (int i = 0; i < usersToCheck.Count; i++)
            {
                var isDeleted = _remoteUsers.FirstOrDefault(p => p.ID == usersToCheck[i].ID);

                if (isDeleted == null)
                {
                    await UserDatabase.DeleteItemAsync(usersToCheck[i]);
                }
            }
        }

        private async Task CompareGoods()
        {
            foreach (Goods remoteGoods in _remoteGoods)
            {
                List<Goods> goods = new List<Goods>();
                goods = GoodsDatabase.GetItemsNotDoneAsync().Result;

                if (goods.Count > 0)
                {
                    bool check = true;

                    for (int i = 0; i < goods.Count; i++)
                    {
                        if (goods[i].ID == remoteGoods.ID)
                        {
                            check = false;
                        }
                    }


                    if (check)
                    {
                        GoodsDatabase.SaveItem(remoteGoods);
                    } 
                }
                else
                {
                    GoodsDatabase.SaveItem(remoteGoods);
                }
            }

            List<Goods> goodsToCheck = new List<Goods>();
            goodsToCheck = GoodsDatabase.GetItemsNotDoneAsync().Result;

            for (int i = 0; i < goodsToCheck.Count; i++)
            {
                var isDeleted = _remoteGoods.FirstOrDefault(p => p.ID == goodsToCheck[i].ID);

                if (isDeleted == null)
                {
                    await GoodsDatabase.DeleteItemAsync(goodsToCheck[i]);
                }
            }
        }

        private async Task CompareOrders()
        {
            foreach (Order remoteOrder in _remoteOrder)
            {
                List<Order> order = new List<Order>();
                order = OrderDatabase.GetItemsNotDoneAsync().Result;

                if (order.Count > 0)
                {
                    bool check = true;

                    for (int i = 0; i < order.Count; i++)
                    {
                        if (order[i].ID == remoteOrder.ID)
                        {
                            check = false;
                        }
                    }

                    if (check)
                    {
                        OrderDatabase.SaveItemAsync(remoteOrder);
                    }
                }
                else
                {
                    OrderDatabase.SaveItemAsync(remoteOrder);
                }
            }

            List<Order> ordersToCheck = new List<Order>();
            ordersToCheck = OrderDatabase.GetItemsNotDoneAsync().Result;

            for (int i = 0; i < ordersToCheck.Count; i++)
            {
                var isDeleted = _remoteOrder.FirstOrDefault(p => p.ID == ordersToCheck[i].ID);

                if (isDeleted == null)
                {
                    await OrderDatabase.DeleteItemAsync(ordersToCheck[i]);
                }
            }
        }

        private async Task CompareGoodsOrders()
        {
            foreach (GoodsOrder remoteGoodsOrder in _remoteGoodsOrder)
            {
                List<GoodsOrder> goodsOrder = new List<GoodsOrder>();
                goodsOrder = GoodsOrderDatabase.GetItemsNotDoneAsync().Result;

                if (goodsOrder.Count > 0)
                {
                    bool check = true;

                    for (int i = 0; i < goodsOrder.Count; i++)
                    {
                        if (goodsOrder[i].ID == remoteGoodsOrder.ID)
                        {
                            check = false;
                        }
                    }

                    if (check)
                    {
                        GoodsOrderDatabase.SaveItem(remoteGoodsOrder);
                    }
                }
                else
                {
                    GoodsOrderDatabase.SaveItem(remoteGoodsOrder);
                }
            }

            List<Goods> goodsToCheck = new List<Goods>();
            goodsToCheck = GoodsDatabase.GetItemsNotDoneAsync().Result;


            List<GoodsOrder> goodsOrderToCheck = new List<GoodsOrder>();
            goodsOrderToCheck = GoodsOrderDatabase.GetItemsNotDoneAsync().Result;

            for (int i = 0; i < goodsOrderToCheck.Count; i++)
            {
                var isDeleted = _remoteGoods.FirstOrDefault(p => p.ID == goodsOrderToCheck[i].GoodsID);

                if (isDeleted == null)
                {
                    await GoodsOrderDatabase.DeleteItemAsync(goodsOrderToCheck[i]);
                }
            }
        }

        private async Task PullAsync()
        {
            dataService dataservice = new dataService();

            List<User> addedUsers = new List<User>();
            List<Order> addedOrders = new List<Order>();
            List<GoodsOrder> addedGoodsOrders = new List<GoodsOrder>();

            ObservableCollection<User> postUser = new ObservableCollection<User>();
            ObservableCollection<Order> postOrder= new ObservableCollection<Order>();
            ObservableCollection<GoodsOrder> postGoodsOrder = new ObservableCollection<GoodsOrder>();

            addedUsers = await UserDatabase.GetItemsNotDoneAsync();

            for (int i = 0; i < addedUsers.Count; i++)
            {
                int UserID = 0;

                if (addedUsers[i].ID == 0)
                {
                    UserID = addedUsers[i].localID;
                    postUser = await dataservice.PostUserDataAsync(addedUsers[i]);
                }
                else
                {
                    UserID = addedUsers[i].ID;
                    postUser = await dataservice.UpdateUserDataAsync(addedUsers[i]);                    
                }

                addedOrders = await OrderDatabase.GetOrderByUser(UserID);              
            
                for (int x = 0; x < addedOrders.Count; x++)
                {
                    addedGoodsOrders = await GoodsOrderDatabase.GetGoodsOrdersByUserAndOrder(UserID, addedOrders[x].localID);
                    addedOrders[x].UserID = postUser[0].ID;

                    if (addedOrders[x].ID == 0)
                    {
                        postOrder = await dataservice.PostOrdersAsync(addedOrders[x]);
                    }
                    else
                    {                        
                        postOrder = await dataservice.UpdateOrder(addedOrders[x]);
                    }


                    for (int y = 0; y < addedGoodsOrders.Count; y++)
                    {
                        addedGoodsOrders[y].UserID = postUser[0].ID;
                        addedGoodsOrders[y].OrderID = postOrder[0].ID;

                        if (addedGoodsOrders[y].ID == 0)
                        {
                            await dataservice.PostGoodsOrdersAsync(addedGoodsOrders[y]);
                        }

                        await GoodsOrderDatabase.DeleteItemAsync(addedGoodsOrders[y]);
                    }

                    await OrderDatabase.DeleteItemAsync(addedOrders[x]);
                }
                await UserDatabase.DeleteItemAsync(addedUsers[i]);
            }

            _remoteUsers = await dataservice.GetUserData();
            _remoteGoods = await dataservice.GetGoodsData();
            _remoteOrder = await dataservice.GetOrdersData();
            _remoteGoodsOrder = await dataservice.GetGoodsOrderData();

            await CompareGoodsOrders();
            await CompareUsers();
            await CompareGoods();
            await CompareOrders();

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

        private static GoodsOrderDatabase _goodsOrderDatabase;
        public static GoodsOrderDatabase GoodsOrderDatabase
        {
            get
            {
                if (_goodsDatabase == null)
                {
                    var fileHelper = new FileHelper();
                    _goodsOrderDatabase = new GoodsOrderDatabase(fileHelper.GetLocalFilePath("TodoSQLite.db3"));
                }
                return _goodsOrderDatabase;
            }
        }

        private static OrderDatabase _OrderDatabase;
        public static OrderDatabase OrderDatabase
        {
            get
            {
                if (_OrderDatabase == null)
                {
                    var fileHelper = new FileHelper();
                    _OrderDatabase = new OrderDatabase(fileHelper.GetLocalFilePath("TodoSQLite.db3"));
                }
                return _OrderDatabase;
            }
        }
    }
}
