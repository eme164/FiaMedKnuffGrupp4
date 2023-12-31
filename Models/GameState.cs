﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace FiaMedKnuffGrupp4.Models
{
    internal class GameState
    {
        public List<Team> Teams { get; set; }
        public GameBoard.ActiveTeam CurrentActiveTeam { get; set; }
        public string Name { get; set; }

        public GameState(List<Team> teams, GameBoard.ActiveTeam currentActiveTeam, string name)
        {
            Teams = teams;
            CurrentActiveTeam = currentActiveTeam;
            Name = name;
        }

        /// <summary>
        /// Serializes the current instance of the GameState class and its sub-objects into a JSON string.
        /// This function converts the current game state into a format that can be stored in the database.
        /// </summary>
        /// <returns>A JSON string representing the serialized game state.</returns>
        public string SerializeGameState()
        {
            // Use the Newtonsoft.Json library to serialize the current GameState instance to JSON
            string serializedobject = JsonConvert.SerializeObject(this);
            Debug.WriteLine("JSON TXT: " + serializedobject);
            return serializedobject;
        }

        /// <summary>
        /// Deserializes a JSON string containing game state data and updates the current instance of the GameState class.
        /// This function converts a JSON representation of the game state into its original object structure.
        /// </summary>
        /// <param name="gameStateJson">A JSON string containing serialized game state data.</param>
        public void DeserializeGameState(string gameStateJson)
        {
            // Deserialize the JSON string into a GameState object using Newtonsoft.Json library
            var gameState = JsonConvert.DeserializeObject<GameState>(gameStateJson);

            // Update the current instance with the deserialized game state
            Teams = gameState.Teams;
            CurrentActiveTeam = gameState.CurrentActiveTeam;
        }
    }
}
