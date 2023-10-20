using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;

public static class DatabaseUtility
{
   private static string databasePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "MyDatabase.db");

    public static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection($"Data Source={databasePath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Create tables when we know what we need.
                //command.CommandText = "CREATE TABLE IF NOT EXISTS Items (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);";
                command.ExecuteNonQuery();
            }
        }
    }
}
