using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DAS_Capture_The_Flag.Models.Game
{
    public class Game : PageModel
    {
        public string GameId { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public bool PlayersConnected { get; set; }
        public bool Ready { get; set; }
        public int PlayersTurn { get; set; }
        public string[,] ChosenMap { get; set; }

        public Game()
        {
            Player1 = new Player();
            Player2 = new Player();
            PlayersConnected = false;
            PlayersTurn = 0;
            ChosenMap = new string[5, 5]
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
            if (Player1 != null && Player1.ConnectionId == connectionId)
            {
                return true;
            }
            if (Player2 != null && Player2.ConnectionId == connectionId)
            {
                return true;
            }
            return false;
        }

        public void AddToSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }
    }
}
