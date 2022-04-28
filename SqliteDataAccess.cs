using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ReadingCompanion
{
    public class SqliteDataAccess
    {
        public static int PageAmount = 20;

        public static void CreateDB()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute(@"CREATE TABLE IF NOT EXISTS Item (Id INTEGER NOT NULL UNIQUE, 
                                                    Title TEXT NOT NULL, ReadProgress REAL NOT NULL DEFAULT 0.0,
                                                    ReadTotal REAL NOT NULL DEFAULT 0.0,
                                                    Rating REAL NOT NULL DEFAULT 0.0,
                                                    Genre TEXT NOT NULL,
                                                    SubGenre1 TEXT,
                                                    SubGenre2 TEXT,
                                                    SubGenre3 TEXT,
                                                    Status INTEGER NOT NULL DEFAULT 0,
                                                    ImageData BLOB,
                                                    PRIMARY KEY(Id AUTOINCREMENT)
                                                    );");
            }
        }

        #region SelectFunctions

        public static List<ItemModel> SelectItems(Status status, int page, string searchQuery, SortBy sort)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                IEnumerable<ItemModel> output = new List<ItemModel>();

                if((int) sort != 1) //when sorting by title or genre
                    output = cnn.Query<ItemModel>($"select * from Item where Status={(int)status} and Title COLLATE UTF8_GENERAL_CI LIKE '%{searchQuery}%' order by upper({sort.ToString()}) asc LIMIT {page*PageAmount},{PageAmount}", new DynamicParameters());
                else                //when sorting by rating
                    output = cnn.Query<ItemModel>($"select * from Item where Status={(int)status} and Title COLLATE UTF8_GENERAL_CI LIKE '%{searchQuery}%' order by {sort.ToString()} desc LIMIT {page*PageAmount},{PageAmount}", new DynamicParameters());
                return output.ToList();
            }
        }

        public static int SelectRowCount(Status status, string searchQuery)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                int output = new int();

                output = cnn.ExecuteScalar<int>($"select count(*) from Item where Status={(int)status} and Title COLLATE UTF8_GENERAL_CI LIKE '%{searchQuery}%'");

                return output;
            }
        }

        public static List<ItemModel> SelectAllItems()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                IEnumerable<ItemModel> output = new List<ItemModel>();
                output = cnn.Query<ItemModel>("select * from Item", new DynamicParameters());

                return output.ToList();
            }
        }
        #endregion

        #region UpdateFunctions
        public static void UpdateItem(ItemModel item)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"update Item set Title=@Title, ReadProgress=@ReadProgress, ReadTotal=@ReadTotal, Rating=@Rating, Genre=@Genre, SubGenre1=@SubGenre1, SubGenre2=@SubGenre2, SubGenre3=@SubGenre3, Status=@Status, ImageData=@ImageData where Id=@Id", item);
            }
        }

        public static void UpdateItemStatus(int id, int status)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"update Item set Status=@Status where Id=@Id", new { Status = status, Id = id });
            }
        }

        public static void UpdateItemRating(int id, double rating)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"update Item set Rating=@Rating where Id=@Id", new {Rating = rating, Id = id});
            }
        }

        public static void UpdateItemReadProgress(int id, float readprogress)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"update Item set ReadProgress=@ReadProgress where Id=@Id", new {ReadProgress = readprogress, Id = id});
            }
        }

        public static void UpdateItemReadTotal(int id, float readtotal)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"update Item set ReadTotal=@ReadTotal where Id=@Id", new {ReadTotal = readtotal, Id = id});
            }
        }
        #endregion

        #region InsertFunctions
        public static void InsertItem(ItemModel item)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Item (Title, ReadProgress, ReadTotal, Rating, Genre, SubGenre1, SubGenre2, SubGenre3, Status, ImageData) values (@Title, @ReadProgress, @ReadTotal, @Rating, @Genre, @SubGenre1, @SubGenre2, @SubGenre3, @Status, @ImageData)", item);
            }
        }
        #endregion

        #region DeleteFunctions
        public static void DeleteItem(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"delete from Item where Id={id}");
            }
        }
        #endregion

        public static void TotalProgress() // TODO 
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"SELECT SUM(ReadProgress) as Score FROM Item");
            }
        }
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
