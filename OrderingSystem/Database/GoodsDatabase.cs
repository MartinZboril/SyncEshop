using OrderingSystem.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingSystem.Database
{
    public class GoodsDatabase
    {
        // SQLite connection
        private SQLiteAsyncConnection database;

        public GoodsDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Goods>().Wait();
        }

        // Query
        public Task<List<Goods>> GetItemsAsync()
        {
            return database.Table<Goods>().ToListAsync();
        }

        // Query using SQL query string
        public Task<List<Goods>> GetItemsNotDoneAsync()
        {
            return database.QueryAsync<Goods>("SELECT * FROM [Goods]");
        }

        public Task<List<Goods>> GetGoodsByName()
        {
            return database.QueryAsync<Goods>("SELECT * FROM [Goods] ORDER BY Name");
        }

        public Task<List<Goods>> GetGoodsByHighestPrice()
        {
            return database.QueryAsync<Goods>("SELECT * FROM [Goods] ORDER BY Price DESC");
        }

        public Task<List<Goods>> GetGoodsByLowestPrice()
        {
            return database.QueryAsync<Goods>("SELECT * FROM [Goods] ORDER BY Price");
        }

        public Task<List<Goods>> GetSearchWord(string word)
        {
            return database.QueryAsync<Goods>("SELECT * FROM [Goods] WHERE Name LIKE '" + word + '%' + "'");
        }

        // Query using LINQ
        public Task<Goods> GetItemByIDAsync(string id)
        {
            return database.Table<Goods>().Where(i => i.ID.Equals(id)).FirstOrDefaultAsync();
        }

        // Query using LINQ
        public Task<Goods> GetItemAsyncByID(int id)
        {
            return database.Table<Goods>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public void SaveItem(Goods item)
        {
            if (item.localID != 0)
            {
                database.UpdateAsync(item);
            }
            else
            {
                if (GetItemsNotDoneAsync().Result.Count > 0)
                {
                    List<Goods> Goods = new List<Goods>();
                    Goods = GetItemsNotDoneAsync().Result;

                    List<int> AllLocalID = new List<int>();

                    for (int i = 0; i < Goods.Count; i++)
                    {
                        AllLocalID.Add(Goods[i].localID);
                    }

                    int max = AllLocalID.Max();

                    item.localID = max + 1;
                }
                else
                {
                    item.localID = 1;
                }

                string sql = $"INSERT INTO Goods(ID, Name, Image, Category, Type, YearOfRealising, Content, Price, localID) VALUES ('{item.ID}', '{item.Name}', '{item.Image}', '{item.Category}', '{item.Type}', '{item.YearOfRealising}', '{item.Content}', '{item.Price}', '{item.localID}')";
                database.QueryAsync<User>(sql);
            }
        }

        public async Task DeleteItemAsync(Goods item)
        {
            if (item.ID != 0)
            {
                string sql = $"DELETE FROM Goods WHERE ID = '{item.ID}'";
                database.QueryAsync<Goods>(sql);
            } else
            {
                database.DeleteAsync(item);
            }
        }
    }
}
