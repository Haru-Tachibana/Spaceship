namespace ProjectSpaceship;

public class Game
{
    public Fleet Player1Fleet { get; }
    public Fleet Player2Fleet { get; }
    public string CurrentPlayer { get; private set; }
    public int TurnNumber { get; private set; }

    public Game()
    {
        Player1Fleet = new Fleet("Player 1");
        Player2Fleet = new Fleet("Player 2");
        CurrentPlayer = "Player 1";
        TurnNumber = 1;

        InitializeGame();
    }

    private void InitializeGame()
    {
        var player1Starbase = new Starbase(Player1Fleet, "Sector 1");
        Player1Fleet.AddStarbase(player1Starbase);

        var player1Ship1 = new Starship(Player1Fleet, "Sector 1");
        var player1Ship2 = new Starship(Player1Fleet, "Sector 1");
        var player1Ship3 = new Starship(Player1Fleet, "Sector 1");
        Player1Fleet.AddStarship(player1Ship1);
        Player1Fleet.AddStarship(player1Ship2);
        Player1Fleet.AddStarship(player1Ship3);

        var player2Starbase = new Starbase(Player2Fleet, "Sector 2");
        Player2Fleet.AddStarbase(player2Starbase);

        var player2Ship1 = new Starship(Player2Fleet, "Sector 2");
        var player2Ship2 = new Starship(Player2Fleet, "Sector 2");
        var player2Ship3 = new Starship(Player2Fleet, "Sector 2");
        Player2Fleet.AddStarship(player2Ship1);
        Player2Fleet.AddStarship(player2Ship2);
        Player2Fleet.AddStarship(player2Ship3);
    }

    public Fleet GetCurrentPlayerFleet()
    {
        return CurrentPlayer == "Player 1" ? Player1Fleet : Player2Fleet;
    }

    public Fleet GetOpponentFleet()
    {
        return CurrentPlayer == "Player 1" ? Player2Fleet : Player1Fleet;
    }

    public void SwitchTurn()
    {
        CurrentPlayer = CurrentPlayer == "Player 1" ? "Player 2" : "Player 1";
        if (CurrentPlayer == "Player 1")
        {
            TurnNumber++;
        }
    }

    public bool IsGameOver()
    {
        return Player1Fleet.IsDefeated() || Player2Fleet.IsDefeated();
    }

    public string GetWinner()
    {
        if (Player1Fleet.IsDefeated())
        {
            return "Player 2";
        }
        if (Player2Fleet.IsDefeated())
        {
            return "Player 1";
        }
        return "";
    }

    public List<string> GetAvailableSectors()
    {
        var sectors = new HashSet<string>();
        foreach (var ship in Player1Fleet.Starships)
        {
            sectors.Add(ship.Sector);
        }
        foreach (var starbase in Player1Fleet.Starbases)
        {
            sectors.Add(starbase.Sector);
        }
        foreach (var ship in Player2Fleet.Starships)
        {
            sectors.Add(ship.Sector);
        }
        foreach (var starbase in Player2Fleet.Starbases)
        {
            sectors.Add(starbase.Sector);
        }
        return sectors.OrderBy(s => s).ToList();
    }
}

