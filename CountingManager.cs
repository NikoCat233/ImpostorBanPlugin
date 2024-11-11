using Impostor.Api.Games.Managers;

namespace ImpostorBanPlugin;

public class CountingManager
{
    private readonly IGameManager gameManager;

    public CountingManager(IGameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public int GetTotalGames()
    {
        return gameManager.Games.Where(x => x.PlayerCount > 0).Count();
    }

    public int GetTotalPlayers()
    {
        int players = 0;
        foreach (var game in gameManager.Games)
        {
            players += game.Players.Count();
        }
        return players;
    }
}
