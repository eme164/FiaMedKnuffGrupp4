using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;

public static class DataAccess
{

    /// <summary>
    /// Initializes the local SQLite database for storing game state data.
    /// This function ensures that the necessary database file and tables are created.
    /// </summary>
    public async static void InitializeDatabase()
    {
        // Create or open the SQLite database file in the local folder
        await ApplicationData.Current.LocalFolder.CreateFileAsync("database.db", CreationCollisionOption.OpenIfExists);
        string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "database.db");
        Debug.WriteLine("shit path: " + dbpath);

        // Open a connection to the SQLite database
        using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
        {
            db.Open();

            // Define the SQL command to create the 'GameState' table if it doesn't exist
            String tableCommand = "CREATE TABLE IF NOT EXISTS GameState (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE, GameData TEXT)";

            // Create a SQLite command for executing the table creation SQL
            SqliteCommand createTable = new SqliteCommand(tableCommand, db);

            // Execute the SQL command to create the 'GameState' table
            createTable.ExecuteReader();

            // Close the database connection
            db.Close();
        }
    }

    /// <summary>
    /// Stores the serialized game state as a JSON string in the local database.
    /// This function takes a serialized game state in JSON format, associates it with a name,
    /// and stores it as a new record in the database for later retrieval.
    /// </summary>
    /// <param name="name">A unique name to associate with the game state.</param>
    /// <param name="gameStateJson">A JSON string representing the serialized game state to be stored.</param>
    public static void SetGameState(string name, string gameStateJson)
    {
        // Get the path to the local database file
        string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "database.db");

        // Open a connection to the SQLite database
        using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
        {
            db.Open();

            // Create a SQL command for inserting the game state JSON and associated name into the database
            SqliteCommand insertCommand = new SqliteCommand();
            insertCommand.Connection = db;

            // Use parameterized queries to prevent SQL injection attacks
            insertCommand.CommandText = "INSERT INTO GameState (Name, GameData) VALUES (@name, @gameState);";
            insertCommand.Parameters.AddWithValue("@name", name);
            insertCommand.Parameters.AddWithValue("@gameState", gameStateJson);

            // Execute the SQL command to insert the game state JSON and name
            insertCommand.ExecuteNonQuery();

            // Close the database connection
            db.Close();
        }
    }

    /// <summary>
    /// Retrieves the serialized game state as a JSON string from the local database
    /// based on the provided name.
    /// </summary>
    /// <param name="name">The unique name associated with the desired game state.</param>
    /// <returns>A JSON string representing the serialized game state, or null if not found.</returns>
    public static string GetGameState(string name)
    {
        // Initialize the game state JSON string to null
        string gameStateJson = null;

        // Get the path to the local database file
        string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "database.db");

        // Open a connection to the SQLite database
        using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
        {

            db.Open();

            // Create a SQL command to select the saved game state based on the provided name
            SqliteCommand selectCommand = new SqliteCommand("SELECT GameData FROM GameState WHERE Name = @name;", db);
            selectCommand.Parameters.AddWithValue("@name", name);

            // Execute the SQL command and retrieve the result
            var result = selectCommand.ExecuteScalar();

            // Check if a result was found
            if (result != null)
            {

                // Convert the result to a string and store it as the game state JSON
                gameStateJson = result.ToString();
            }

            // Close the database connection
            db.Close();
        }

        // Return the retrieved game state JSON string
        return gameStateJson;
    }

    /// <summary>
    /// Deletes the game state data from the database with the specified name.
    /// </summary>
    /// <param name="name">The name associated with the game state data to delete.</param>
    public static void DeleteGameState(string name)
    {
        // Get the path to the local database file
        string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "database.db");

        // Open a connection to the SQLite database
        using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
        {
            db.Open();

            // Create a SQL command to delete the game state by name
            SqliteCommand deleteCommand = new SqliteCommand();
            deleteCommand.Connection = db;

            // Use parameterized queries to specify the name to delete
            deleteCommand.CommandText = "DELETE FROM GameState WHERE Name = @name;";
            deleteCommand.Parameters.AddWithValue("@name", name);

            // Execute the SQL command to delete the game state
            deleteCommand.ExecuteNonQuery();

            // Close the database connection
            db.Close();
        }
    }
}
