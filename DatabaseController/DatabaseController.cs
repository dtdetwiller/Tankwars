// Authors(s): Daniel Detwiller and Warren Kidman
// Date: December 5th 2019
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TankWars
{
    /// <summary>
    /// Handles all communication with the database, saving statistics when the game completes, and retrieving statistics when an http request is made.
    /// </summary>
    public class DatabaseController
    {
        /// <summary>
        /// Used to hold the servers world
        /// </summary>
        private World world;

        /// <summary>
        /// ID of the game that was saved
        /// </summary>
        private object gameID;

        /// <summary>
        /// Contains information for connection such as password
        /// </summary>
        public const string connectionString = "server=atr.eng.utah.edu;database=cs3500_u1148322;uid=cs3500_u1148322;password=cs3500";

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatabaseController()
        {

        }

        /// <summary>
        /// Constructor that intitailized the world
        /// </summary>
        /// <param name="w"></param>
        public DatabaseController(World w)
        {
            world = w;
        }


        /// <summary>
        /// Saves the game including the duration of that game
        /// </summary>
        /// <param name="duration"></param>
        public void Save(long duration)
        {
            SaveToGames(duration);
            SaveToGamesPlayed();
        }

        /// <summary>
        /// Inserts saved information into MySql database
        /// </summary>
        private void SaveToGames(long duration)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "insert into Games(Duration) values(" + duration + ")";
                command.ExecuteNonQuery();

            }

        }

        /// <summary>
        /// Inserts saved info such as accuracy, score, name and game id
        /// </summary>
        private void SaveToGamesPlayed()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();

                string playerName = "";
                int score = 0;
                int accuracy = 0;

                foreach (Tank tank in world.GetTanks().Values)
                {
                    command.CommandText = "select LAST_INSERT_ID()";
                    gameID = command.ExecuteScalar();

                    playerName = tank.GetName();
                    score = tank.GetScore();
                    accuracy = tank.GetAccuracy();

                    command.CommandText = "insert into GamesPlayed(gID, Name, Score, Accuracy) values(" + gameID + ", \"" + playerName + "\", " + score + ", " + accuracy + ")";
                    command.ExecuteNonQuery();

                }
            }
        }


        /// <summary>
        /// Retrieves past games and puts them and the information into a dictionary that is returned.
        /// Retrieves games based on game ID
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, GameModel> RetrieveGames()
        {

            Dictionary<uint, GameModel> games = new Dictionary<uint, GameModel>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();

                command.CommandText = "select gID, Duration from Games";

                // add games and information to dictionary
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string gID = "" + reader["gID"];
                        string duration = "" + reader["Duration"];

                        games.Add(uint.Parse(gID), new GameModel(uint.Parse(gID), uint.Parse(duration)));
                    }
                }

                command.CommandText = "select gID, Name, Score, Accuracy from GamesPlayed";

                // parses through score and accuracy from past games
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string gID = "" + reader["gID"];
                        string score = "" + reader["Score"];
                        string name = "" + reader["Name"];
                        string accuracy = "" + reader["Accuracy"];

                        games[uint.Parse(gID)].AddPlayer(name, uint.Parse(score), uint.Parse(accuracy));

                    }
                }
            }

            return games;
        }

        /// <summary>
        /// Retrieves past games and puts them and the information into a dictionary that is returned.
        /// Retrieves games based on player name
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PlayerModel> RetrievePlayerGames()
        {

            Dictionary<string, PlayerModel> playerGames = new Dictionary<string, PlayerModel>();


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = connection.CreateCommand();

                command.CommandText = "select Games.gID, Games.Duration, GamesPlayed.gID, Name, Accuracy, Score from GamesPlayed join Games on Games.gID = GamesPlayed.gID";

                // add information to dictionary
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string gID = "" + reader["gID"];
                        string name = "" + reader["Name"];
                        string accuracy = "" + reader["Accuracy"];
                        string score = "" + reader["Score"];
                        string duration = "" + reader["Duration"];

                        if (!playerGames.ContainsKey(name))
                            playerGames.Add(name, new PlayerModel(uint.Parse(gID), uint.Parse(duration), name, uint.Parse(score), uint.Parse(accuracy)));
                    }
                }
            }

            return playerGames;
        }

    }
}
