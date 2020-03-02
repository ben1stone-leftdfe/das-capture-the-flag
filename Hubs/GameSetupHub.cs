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
        Task StartGame(string gameId, string playerId);
        Task PlayerReady(bool playerOne, bool playerTwo);
        Task AwaitPlayersReady();
        Task UpdatePlayerReady();
        Task OpponentReady();
    }

    public class GameSetupHub : Hub<IGameClient>
    {

        private IGameRepository _repository;

        public GameSetupHub(IGameRepository repository)
        {
            _repository = repository;
        }

        public override async Task OnConnectedAsync()
        {
            var game = _repository.Games.FirstOrDefault(g => !g.Setup.PlayersConnected);

            if (game == null)
            {
                game = new Game();
                game.Setup = new GameSetup();

                _repository.Games.Add(game);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Setup.GameId);

            if (game.Setup.Player1.ConnectionId == null)
            {
                game.Setup.Player1.ConnectionId = Context.ConnectionId;
                game.Setup.PlayersConnected = GetPlayersConnected(game.Setup);
            }
            else
            {
                game.Setup.Player2.ConnectionId = Context.ConnectionId;
                game.Setup.PlayersConnected = GetPlayersConnected(game.Setup);
            }

            await Clients.Group(game.Setup.GameId)
                .PlayerReady(game.Setup.Player1.ConnectionId != null, game.Setup.Player2.ConnectionId != null);

            await base.OnConnectedAsync();


            if (game.Setup.PlayersConnected)
            {
                await Clients.Group(game.Setup.GameId).AwaitPlayersReady();
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
            var game = _repository.Games.FirstOrDefault(g => g.Setup.HasPlayer(Context.ConnectionId));

            var player = GetPlayer(game.Setup, Context.ConnectionId);

            player.Ready = true;

            if (GetPlayersReady(game.Setup))
            {
                await Clients.Client(game.Setup.Player1.ConnectionId).StartGame(game.Setup.GameId.ToString(), game.Setup.Player1.ConnectionId.ToString());

                await Clients.Client(game.Setup.Player2.ConnectionId).StartGame(game.Setup.GameId.ToString(), game.Setup.Player2.ConnectionId.ToString());

            }
            else
            {
                var otherPlayer = GetOpponent(game.Setup, Context.ConnectionId);
                await Clients.Client(otherPlayer.ConnectionId).OpponentReady();
            }
        }

       
        private bool GetPlayersConnected(GameSetup game)
        {
            if (game.Player1.ConnectionId != null && game.Player2.ConnectionId != null)
            {
                return true;
            }

            return false;
        }

        private bool GetPlayersReady(GameSetup game)
        {
            if (game.Player1.Ready && game.Player2.Ready)
            {
                return true;
            }

            return false;
        }

        private Player GetPlayer(GameSetup game, string id)
        {
            if (game.Player1.ConnectionId == id)
            {
                return game.Player1;
            }

            return game.Player2;
        }

        private Player GetOpponent(GameSetup game, string id)
        {
            if (game.Player1.ConnectionId != id)
            {
                return game.Player1;
            }

            return game.Player2;
        }
    }
}
