using OrderingSystem.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingSystem.Database
{
    public class OrderDatabase
    {
        // SQLite connection
        private SQLiteAsyncConnection database;

        public OrderDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Order>().Wait();
        }

        // Query
        public Task<List<Order>> GetItemsAsync()
        {
            return database.Table<Order>().ToListAsync();
        }

        // Query using SQL query string
        public Task<List<Order>> GetItemsNotDoneAsync()
        {
            return database.QueryAsync<Order>("SELECT * FROM [Order]");
        }

        // Query using LINQ
        public Task<Order> GetItemAsync(int number)
        {
            return database.Table<Order>().Where(i => i.Number == number).FirstOrDefaultAsync();
        }

        // Query using LINQ
        public Task<Order> GetItemAsyncByID(int id)
        {
            return database.Table<Order>().Where(i => i.localID == id).FirstOrDefaultAsync();
        }

        // Query using LINQ
        public Task<List<Order>> GetOrderByUser(int id)
        {
            return database.Table<Order>().Where(i => i.UserID == id).ToListAsync();
        }

        public void UpdateItemAsync(Order item)
        {

        }

        public void SaveItemAsync(Order item)
        { 
            if (item.localID != 0)
            {
                database.UpdateAsync(item);
            }
            else
            {
                if (GetItemsNotDoneAsync().Result.Count > 0)
                {
                    List<Order> Orders = new List<Order>();
                    Orders = GetItemsNotDoneAsync().Result;

                    List<int> AllLocalID = new List<int>();

                    for (int i = 0; i < Orders.Count; i++)
                    {
                        AllLocalID.Add(Orders[i].localID);
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

        public async Task DeleteItemAsync(Order item)
        {
            if (item.ID != 0)
            {
                string sql = $"DELETE FROM Order WHERE ID = '{item.ID}'";
                database.QueryAsync<Order>(sql);
            }
            else
            {
                database.DeleteAsync(item);
            }
        }
    }
}