using Microsoft.Data.Sqlite;
using System;
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

        // Open a connection to the SQLite database
        using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
        {
            db.Open();

            // Define the SQL command to create the 'GameState' table if it doesn't exist
            String tableCommand = "CREATE TABLE IF NOT EXISTS GameState (Id INTEGER PRIMARY KEY AUTOINCREMENT, GameData TEXT)";

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
    /// This function takes a serialized game state in JSON format and stores it
    /// as a new record in the database for later retrieval.
    /// </summary>
    /// <param name="gameStateJson">A JSON string representing the serialized game state to be stored.</param>
    public static void SetGameState(string gameStateJson)
    {
        // Get the path to the local database file
        string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "database.db");

        // Open a connection to the SQLite database
        using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
        {
            db.Open();

            // Create a SQL command for inserting the game state JSON into the database
            SqliteCommand insertCommand = new SqliteCommand();
            insertCommand.Connection = db;

            // Use a parameterized query to prevent SQL injection attacks
            insertCommand.CommandText = "INSERT INTO GameState VALUES (NULL, @gameState);";
            insertCommand.Parameters.AddWithValue("@gameState", gameStateJson);

            // Execute the SQL command to insert the game state JSON
            insertCommand.ExecuteNonQuery();

            // Close the database connection
            db.Close();
        }
    }


    /// <summary>
    /// Retrieves the serialized game state as a JSON string from the local database.
    /// This function fetches the latest saved game state from the database and returns
    /// it as a JSON string for deserialization.
    /// </summary>
    /// <returns>
    /// A JSON string containing the serialized game state if found; otherwise, null.
    /// </returns>
    public static string GetGameState()
    {
        // Initialize the game state JSON string to null
        string gameStateJson = null;

        // Get the path to the local database file
        string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "database.db");

        // Open a connection to the SQLite database
        using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
        {
            db.Open();

            // Create a SQL command to select the latest saved game state from the database
            SqliteCommand selectCommand = new SqliteCommand("SELECT GameState FROM GameState ORDER BY Id DESC LIMIT 1;", db);

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
}
