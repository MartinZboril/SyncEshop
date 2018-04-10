using OrderingSystem.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderingSystem.Database
{
    public class UserDatabase
    {
        // SQLite connection
        private SQLiteAsyncConnection database;

        public UserDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<User>().Wait();
        }

        // Query
        public Task<List<User>> GetItemsAsync()
        {
            return database.Table<User>().ToListAsync();
        }

        // Query using SQL query string
        public Task<List<User>> GetItemsNotDoneAsync()
        {
            return database.QueryAsync<User>("SELECT * FROM [User]");
        }

        // Query using LINQ
        public Task<User> GetItemAsync(string name)
        {
            return database.Table<User>().Where(i => i.Name == name).FirstOrDefaultAsync();
        }

        // Query using LINQ
        public Task<User> GetItemAsyncByID(int id)
        {
            return database.Table<User>().Where(i => i.localID == id).FirstOrDefaultAsync();
        }

        // Query using LINQ
        public Task<List<User>> GeNotSaveItem()
        {
            return database.Table<User>().Where(i => i.ID == 0).ToListAsync();
        }

        public void UpdateItem(User item)
        {
            string sql = $"UPDATE User SET Password = '{item.Password}' Where localID = '{item.localID}'";
            database.QueryAsync<User>(sql);
        }

        public void SaveItem(User item)
        {
            if (item.localID != 0)
            {
                string sql = $"UPDATE User SET Name = '{item.Name}', Surname = '{item.Surname}', Email = '{item.Email}', Phone = '{item.Phone}' Password = '{item.Password}', Where localID = '{item.localID}'"; 
                database.QueryAsync<User>(sql);
            }
            else
            {
                if (GetItemsNotDoneAsync().Result.Count > 0)
                {
                    List<User> Users = new List<User>();
                    Users = GetItemsNotDoneAsync().Result;

                    List<int> AllLocalID = new List<int>();

                    for (int i = 0; i < Users.Count; i++)
                    {
                        AllLocalID.Add(Users[i].localID);
                    }

                    int max = AllLocalID.Max();

                    item.localID = max + 1;
                }
                else
                {
                    item.localID = 1;
                }

                string sql = $"INSERT INTO User(ID, Name, Surname, Email, Phone, Password, localID) VALUES ('{item.ID}', '{item.Name}', '{item.Surname}', '{item.Email}', '{item.Phone}', '{item.Password}', '{item.localID}')";
                database.QueryAsync<User>(sql);
            }
        }

        public async Task DeleteItemAsync(User item)
        {
            if (item.ID != 0)
            {
                string sql = $"DELETE FROM User WHERE ID = '{item.ID}'";
                database.QueryAsync<User>(sql);
            }
            else
            {
                database.DeleteAsync(item);
            }
        }
    }
}