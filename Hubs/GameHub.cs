using DAS_Capture_The_Flag.Models.Game;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DAS_Capture_The_Flag.Hubs
{

    public interface IGameClient
    {
        Task StartGame();
        Task PlayerReady(bool playerOne, bool playerTwo);
        Task AwaitPlayersReady();
        Task UpdatePlayerReady();
        Task OpponentReady();
        Task DrawMap(string[,] chosenMap);
        Task OpponentLeftLobby();
   
    }
    public class GameHub : Hub<IGameClient>
    {

        private IGameRepository _repository;

        public GameHub(IGameRepository repository)
        {
            _repository = repository;
        }

        public override async Task OnConnectedAsync()
        {
            var game = _repository.Games.FirstOrDefault(g => !g.PlayersConnected);

            if (game == null)
            {
                game = new Game();
                game.GameId = Guid.NewGuid().ToString();

                _repository.Games.Add(game);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.GameId);

            foreach (var player in game.Players)
            {
                if (player.ConnectionId == null)
                {
                    player.ConnectionId = Context.ConnectionId;
                    game.PlayersConnected = GetPlayersConnected(game);
                    break;
                }
            }
         

            await Clients.Group(game.GameId).PlayerReady(game.Players[0].ConnectionId != null, game.Players[1].ConnectionId != null);

            await base.OnConnectedAsync();

            if (game.PlayersConnected)
            {
                await Clients.Group(game.GameId).AwaitPlayersReady();
            }

        }
      
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle Disconnections
            Game game = (Game)_repository.Games.FirstOrDefault(g => g.Players[0].ConnectionId == Context.ConnectionId || g.Players[1].ConnectionId == Context.ConnectionId);

            Player player = game.Players.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            Player opponent = game.Players.Where(x => x.ConnectionId != Context.ConnectionId).FirstOrDefault();

            player.ConnectionId = null;
            player.Ready = false;
            player.Name = null;
            game.PlayersConnected = false;
                    
            await Clients.Client(opponent.ConnectionId).OpponentLeftLobby();
            await Clients.Group(game.GameId).PlayerReady(game.Players[0].ConnectionId != null, game.Players[1].ConnectionId != null);

                
        }

        
        private Player GetPlayerFromId(Game game, string connectionId)
        {
            if (game.Players[0].ConnectionId == connectionId)
            {
                return game.Players[0];
            }
            else
            {
                return game.Players[1];
            }
        }

        public async Task UpdatePlayerReady()
        {
            var repo = _repository;
            var game = _repository.Games.FirstOrDefault(g => g.HasPlayer(Context.ConnectionId));

            var player = GetPlayer(game, Context.ConnectionId);

            player.Ready = true;

            if (GetPlayersReady(game))
            {
                await Clients.Group(game.GameId).StartGame();
                await DrawMap(game);
            }
            else
            {
                var otherPlayer = GetOpponent(game, Context.ConnectionId);
                await Clients.Client(otherPlayer.ConnectionId).OpponentReady();
            }
        }

        public async Task DrawMap(Game game)
        {
            await Clients.Group(game.GameId).DrawMap(game.Map);
        }

        public async Task TestMethod()
        {
            var user = Context.ConnectionId;
        }
        private bool GetPlayersConnected(Game game)
        {
            if (game.Players[0].ConnectionId != null && game.Players[1].ConnectionId != null)
            {
                return true;
            }

            return false;
        }

        private bool GetPlayersReady(Game game)
        {
            if (game.Players[0].Ready && game.Players[1].Ready)
            {
                return true;
            }

            return false;
        }
        private Player GetPlayer(Game game,string id)
        {
            if (game.Players[0].ConnectionId == id)
            {
                return game.Players[0];
            }

            return game.Players[1];
        }

        private Player GetOpponent(Game game, string id)
        {
            if (game.Players[0].ConnectionId == id)
            {
                return game.Players[1];
            }
            return game.Players[0];
        }
    }
}
