
using OrderingSystem.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingSystem.Database
{
    public class GoodsOrderDatabase
    {
        // SQLite connection
        private SQLiteAsyncConnection database;

        public GoodsOrderDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<GoodsOrder>().Wait();
        }

        // Query
        public Task<List<GoodsOrder>> GetItemsAsync()
        {
            return database.Table<GoodsOrder>().ToListAsync();
        }

        // Query using SQL query string
        public Task<List<GoodsOrder>> GetItemsNotDoneAsync()
        {
            return database.QueryAsync<GoodsOrder>("SELECT * FROM [GoodsOrder]");
        }

        // Query using LINQ
        public Task<GoodsOrder> GetOneItemAsyncByLocalID(int id)
        {
            return database.Table<GoodsOrder>().Where(i => i.localID == id).FirstOrDefaultAsync();
        }

        // Query using LINQ
        public Task<GoodsOrder> GetOneItemAsyncByID(int id)
        {
            return database.Table<GoodsOrder>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<GoodsOrder> GetOneItemAsyncByIDString(string id)
        {
            return database.Table<GoodsOrder>().Where(i => i.localID.Equals(id)).FirstOrDefaultAsync();
        }

        // Query using LINQ
        public Task<List<GoodsOrder>> GetItemAsyncByID(int id)
        {
            return database.Table<GoodsOrder>().Where(i => i.OrderID == id).ToListAsync();
        }

        // Query using LINQ
        public Task<List<GoodsOrder>> GetGoodsOrderData(int id)
        {
            return database.Table<GoodsOrder>().Where(i => i.OrderID == id).ToListAsync();
        }

        // Query using LINQ
        public Task<List<GoodsOrder>> GetGoodsOrdersByUserAndOrder(int userID, int orderID)
        {
            return database.Table<GoodsOrder>().Where(i => i.UserID == userID && i.OrderID == orderID).ToListAsync();
        }

        public void SaveItem(GoodsOrder item)
        {
            if (item.localID != 0)
            {
                database.UpdateAsync(item);
            }
            else
            {
                if (GetItemsNotDoneAsync().Result.Count > 0)
                {
                    List<GoodsOrder> GoodsOrders = new List<GoodsOrder>();
                    GoodsOrders = GetItemsNotDoneAsync().Result;

                    List<int> AllLocalID = new List<int>();

                    for (int i = 0; i < GoodsOrders.Count; i++)
                    {
                        AllLocalID.Add(GoodsOrders[i].localID);
                    }

                    int max = AllLocalID.Max();

                    item.localID = max + 1;
                }
                else
                {
                    item.localID = 1;
                }

                database.InsertAsync(item);
            }
        }

        public async Task DeleteItemAsync(GoodsOrder item)
        {
            if (item.ID != 0)
            {
                string sql = $"DELETE FROM GoodsOrder WHERE GoodsID = '{item.GoodsID}'";
                database.QueryAsync<GoodsOrder>(sql);
            }
            else
            {
                database.DeleteAsync(item);
            }
        }
    }
}