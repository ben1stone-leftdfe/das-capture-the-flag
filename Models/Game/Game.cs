using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS_Capture_The_Flag.Models.Game
{
    public class Game
    {
        public string GameId { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public bool PlayersConnected { get; set; }
        public bool Ready { get; set; }
        public int PlayersTurn { get; set; }
        public string[,] Map { get; set; }

        public List<Player> Players { get; set; }

        public Game()
        {
            Players = new List<Player>() { new Player() { Number = (int)PlayerEnum.One }, new Player() { Number = (int)PlayerEnum.Two } };
            Player1 = new Player() { Number = 1 };
            Player2 = new Player() { Number = 2 };
            PlayersConnected = false;
            PlayersTurn = 0;
            Map = new string[5, 5]
              {
                { "grass", "wall", "wall", "wall", "wall"},
                { "wall", "wall", "grass", "grass", "grass"},
                { "grass", "grass", "grass", "grass", "grass"},
                { "grass", "grass", "grass", "grass", "grass"},
                { "grass", "grass", "grass", "grass", "grass"}
              };
        }
        public bool HasPlayer(string connectionId)
        {
            if (Players[0] != null && Players[0].ConnectionId == connectionId)
            {
                return true;
            }
            else if (Players[1] != null && Players[1].ConnectionId == connectionId)
            {
                return true;
            }
            return false;
        }

    }
}
