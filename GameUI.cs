namespace ProjectSpaceship;

public class GameUI
{
    private Game game;

    public GameUI(Game game)
    {
        this.game = game;
    }

    public void DisplayGameState()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine($"           PROJECT SPACESHIP - Turn {game.TurnNumber}");
        Console.WriteLine($"           Current Player: {game.CurrentPlayer}");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine();

        DisplayFleet(game.Player1Fleet, "Player 1");
        Console.WriteLine();
        DisplayFleet(game.Player2Fleet, "Player 2");
        Console.WriteLine();
    }

    private void DisplayFleet(Fleet fleet, string playerName)
    {
        Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"{playerName} Fleet:");
        Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        if (fleet.Starbases.Count > 0)
        {
            Console.WriteLine("Starbases:");
            for (int i = 0; i < fleet.Starbases.Count; i++)
            {
                var starbase = fleet.Starbases[i];
                Console.WriteLine($"  [{i}] Starbase in {starbase.Sector}");
                Console.WriteLine($"      Health: {starbase.CurrentHealth}/{starbase.MaximumHealth}");
                Console.WriteLine($"      Defence: {starbase.CurrentDefenceStrength}");
                Console.WriteLine($"      Docked Ships: {starbase.DockedShips.Count}");
                if (starbase.DockedShips.Count > 0)
                {
                    foreach (var ship in starbase.DockedShips)
                    {
                        var shipIndex = fleet.Starships.IndexOf(ship);
                        Console.WriteLine($"        - Ship [{shipIndex}] (Health: {ship.CurrentHealth}/{ship.MaximumHealth}, Crew: {ship.CurrentCrew}/{ship.MaximumCrew})");
                    }
                }
            }
        }

        if (fleet.Starships.Count > 0)
        {
            Console.WriteLine("Starships:");
            for (int i = 0; i < fleet.Starships.Count; i++)
            {
                var ship = fleet.Starships[i];
                string dockedStatus = ship.Starbase != null ? $" [DOCKED at {ship.Starbase.Sector}]" : "";
                string skipStatus = ship.ActionsToSkip > 0 ? $" [REPAIRING - {ship.ActionsToSkip} actions to skip]" : "";
                Console.WriteLine($"  [{i}] Ship in {ship.Sector}{dockedStatus}{skipStatus}");
                Console.WriteLine($"      Health: {ship.CurrentHealth}/{ship.MaximumHealth}");
                Console.WriteLine($"      Crew: {ship.CurrentCrew}/{ship.MaximumCrew}");
                Console.WriteLine($"      Attack: {ship.CurrentAttackStrength} | Defence: {ship.CurrentDefenceStrength}");
            }
        }

        if (fleet.Starships.Count == 0 && fleet.Starbases.Count == 0)
        {
            Console.WriteLine("  [DEFEATED - No units remaining]");
        }
    }

    public void DisplayCommands()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("Available Commands:");
        Console.WriteLine("  move <ship_index> <sector>     - Move a ship to a sector");
        Console.WriteLine("  dock <ship_index> <starbase_index> - Dock a ship at a starbase");
        Console.WriteLine("  undock <ship_index>            - Undock a ship from starbase");
        Console.WriteLine("  repair <ship_index>            - Repair a docked ship");
        Console.WriteLine("  attack <ship_index> <target_type> <target_index> - Attack a target");
        Console.WriteLine("    target_type: ship (opponent ship) or starbase (opponent starbase)");
        Console.WriteLine("  fleet-move <sector>            - Move all undocked ships to sector");
        Console.WriteLine("  fleet-attack <target_type> <target_index> - All ships attack target");
        Console.WriteLine("  run-simulation                 - Run a scripted simulation example");
        Console.WriteLine("  status                         - Show detailed game status");
        Console.WriteLine("  end                            - End your turn");
        Console.WriteLine("  help                           - Show this help");
        Console.WriteLine("  quit                           - Quit the game");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
    }

    public void DisplayHelp()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("PROJECT SPACESHIP - GAME RULES");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("OBJECTIVE:");
        Console.WriteLine("  Destroy all opponent starbases to win!");
        Console.WriteLine();
        Console.WriteLine("SHIP ACTIONS:");
        Console.WriteLine("  - Ships can move between sectors");
        Console.WriteLine("  - Ships can attack enemy ships/starbases in the same sector");
        Console.WriteLine("  - Ships can dock at friendly starbases (same fleet, same sector)");
        Console.WriteLine("  - Docked ships cannot move or attack, but can repair");
        Console.WriteLine("  - Docked ships cannot be attacked");
        Console.WriteLine("  - Repairing restores health/crew but skips next actions");
        Console.WriteLine();
        Console.WriteLine("FLEET ACTIONS:");
        Console.WriteLine("  - Mobilise: Move all undocked ships to a sector");
        Console.WriteLine("  - Attack: All ships in same sector attack a target");
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public void DisplayStatus()
    {
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("DETAILED STATUS");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine($"Turn: {game.TurnNumber}");
        Console.WriteLine($"Current Player: {game.CurrentPlayer}");
        Console.WriteLine($"Available Sectors: {string.Join(", ", game.GetAvailableSectors())}");
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public void DisplayGameOver()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("                    GAME OVER!");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine($"Winner: {game.GetWinner()}!");
        Console.WriteLine();
        DisplayFleet(game.Player1Fleet, "Player 1");
        Console.WriteLine();
        DisplayFleet(game.Player2Fleet, "Player 2");
        Console.WriteLine();
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}

