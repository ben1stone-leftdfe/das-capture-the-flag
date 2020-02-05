using DAS_Capture_The_Flag.Models.Game;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS_Capture_The_Flag.Hubs
{
    public interface IGameSetup
    {
        Task StartOrJoinGame(string connectionId);

    }
    public class GameSetup : IGameSetup
    {
        public GameHub Hub { get; set; }
        public IGameRepository _repository { get; set; }

        
   
        public GameSetup( IGameRepository repository)
        {
            
            _repository = repository;
        }

        //public GameSetup()
        //{
        //}

        public async Task StartOrJoinGame(string connectionId)
        {
            
        }
        private bool GetPlayersConnected(Game game)
        {
            if (game.Player1.ConnectionId != null && game.Player2.ConnectionId != null)
            {
                return true;
            }

            return false;
        }

        private bool GetPlayersReady(Game game)
        {
            if (game.Player1.Ready && game.Player2.Ready)
            {
                return true;
            }

            return false;
        }
    }
        
}
