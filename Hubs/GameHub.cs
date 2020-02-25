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

            if (game.Player1.ConnectionId == null)
            {
                game.Player1.ConnectionId = Context.ConnectionId;

                //await Clients.Client(game.Player2.ConnectionId).PlayerReady("player-one");
                game.PlayersConnected = GetPlayersConnected(game);
            }
            else
            {
                game.Player2.ConnectionId = Context.ConnectionId;

                game.PlayersConnected = GetPlayersConnected(game);
            }

            await Clients.Group(game.GameId)
                .PlayerReady(game.Player1.ConnectionId != null, game.Player2.ConnectionId != null);

            await base.OnConnectedAsync();


            if (game.PlayersConnected)
            {
                await Clients.Group(game.GameId).AwaitPlayersReady();
            }
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle Disconnections
            //Game game = (Game)_repository.Games.Where(g => g.Player1.ConnectionId == Context.ConnectionId || g.Player2.ConnectionId == Context.ConnectionId);

            //game.Player1.ConnectionId = null;
            //game.Player1.Ready = false;
            //game.Player1.Name = null;
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
            await Clients.Group(game.GameId).DrawMap(game.ChosenMap);
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

        private Player GetPlayer(Game game, string id)
        {
            if (game.Player1.ConnectionId == id)
            {
                return game.Player1;
            }

            return game.Player2;
        }

        private Player GetOpponent(Game game, string id)
        {
            if (game.Player1.ConnectionId != id)
            {
                return game.Player1;
            }

            return game.Player2;
        }
    }
}
