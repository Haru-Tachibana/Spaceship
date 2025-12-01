namespace ProjectSpaceship;

public class GameController
{
    private Game game;
    private GameUI ui;

    public GameController(Game game, GameUI ui)
    {
        this.game = game;
        this.ui = ui;
    }

    private bool RunSimulation()
    {
        Console.WriteLine("--- Running scripted simulation ---");

        var p1 = game.Player1Fleet;
        var p2 = game.Player2Fleet;

        // Ensure initial state summary
        Console.WriteLine("Initial state:");
        ui.DisplayGameState();

        // 1) Move all ships in the Player 1 fleet to Sector 2.
        Console.WriteLine("Moving all Player 1 ships to Sector 2...");
        p1.MobiliseToSector("Sector 2");

        // 2) Dock two ships from the Player 2 fleet into the Player 2 starbase.
        Console.WriteLine("Docking two Player 2 ships into Player 2 starbase...");
        if (p2.Starbases.Count > 0)
        {
            var p2Base = p2.Starbases[0];
            int docked = 0;
            foreach (var s in p2.Starships.ToList())
            {
                if (docked >= 2) break;
                if (s.Starbase == null && s.Sector == p2Base.Sector)
                {
                    s.DockWithStarbase(p2Base);
                    Console.WriteLine($"  Docked Player 2 ship (index {p2.Starships.IndexOf(s)}) at starbase.");
                    docked++;
                }
            }
        }

        // 3) Select one ship from Player 1 and attack the remaining undocked Player 2 ship twice.
        Console.WriteLine("Player 1 ship attacks remaining undocked Player 2 ship twice...");
        var attacker = p1.Starships.FirstOrDefault(s => !s.IsDisabled());
        var targetUndocked = p2.Starships.FirstOrDefault(s => s.Starbase == null && !s.IsDisabled());
        if (attacker != null && targetUndocked != null)
        {
            for (int i = 0; i < 2; i++)
            {
                int oldHealth = targetUndocked.CurrentHealth;
                int oldCrew = targetUndocked.CurrentCrew;
                attacker.AttackTarget(targetUndocked);
                Console.WriteLine($"  Attack {i + 1}: Target health {oldHealth}→{targetUndocked.CurrentHealth}, crew {oldCrew}→{targetUndocked.CurrentCrew}");
                if (targetUndocked.IsDisabled())
                {
                    Console.WriteLine("  Target ship destroyed during attacks.");
                    break;
                }
            }
        }

        // 4) Dock the remaining undocked Player 2 ship and repair it.
        Console.WriteLine("Docking and repairing remaining undocked Player 2 ship (if any)...");
        if (p2.Starbases.Count > 0)
        {
            var p2Base = p2.Starbases[0];
            var remaining = p2.Starships.FirstOrDefault(s => s.Starbase == null && !s.IsDisabled());
            if (remaining != null)
            {
                if (remaining.Sector != p2Base.Sector)
                {
                    remaining.MoveToSector(p2Base.Sector);
                    Console.WriteLine($"  Moved remaining ship to sector {p2Base.Sector} for docking.");
                }
                remaining.DockWithStarbase(p2Base);
                Console.WriteLine($"  Docked remaining ship (index {p2.Starships.IndexOf(remaining)}) at starbase.");
                int beforeH = remaining.CurrentHealth;
                int beforeC = remaining.CurrentCrew;
                remaining.Repair();
                Console.WriteLine($"  Repaired: Health {beforeH}→{remaining.CurrentHealth}, Crew {beforeC}→{remaining.CurrentCrew}, ActionsToSkip: {remaining.ActionsToSkip}");
            }
        }

        // 5) Command all Player 1 ships to attack Player 2 starbase until destroyed.
        Console.WriteLine("Player 1 fleet attacks Player 2 starbase until it is destroyed...");
        if (p2.Starbases.Count > 0)
        {
            var targetBase = p2.Starbases[0];
            int safety = 0;
            while (!targetBase.IsDisabled() && safety < 200)
            {
                p1.AttackTarget(targetBase);
                Console.WriteLine($"  Starbase health: {targetBase.CurrentHealth}/{targetBase.MaximumHealth}");
                safety++;
                // If no attacking ships remain or all out of sector, break to avoid infinite loop
                if (!p1.Starships.Any(s => !s.IsDisabled() && s.Starbase == null && s.Sector == targetBase.Sector))
                {
                    // Try to mobilise remaining undocked ships to the target sector
                    p1.MobiliseToSector(targetBase.Sector);
                    if (!p1.Starships.Any(s => !s.IsDisabled() && s.Starbase == null && s.Sector == targetBase.Sector))
                    {
                        Console.WriteLine("  No available attacking ships remain in the sector. Stopping attack loop.");
                        break;
                    }
                }
            }

            if (targetBase.IsDisabled())
            {
                Console.WriteLine("Player 2 starbase destroyed by Player 1 fleet.");
            }
            else
            {
                Console.WriteLine("Attack loop ended without destroying the starbase.");
            }
        }

        // Show final state
        Console.WriteLine("Final state after simulation:");
        ui.DisplayGameState();

        Console.WriteLine("--- Simulation complete ---");
        return false;
    }

    public bool ProcessCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return false;
        }

        var parts = command.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return false;
        }

        var cmd = parts[0].ToLower();

        try
        {
            switch (cmd)
            {
                case "move":
                    return HandleMove(parts);
                case "dock":
                    return HandleDock(parts);
                case "undock":
                    return HandleUndock(parts);
                case "repair":
                    return HandleRepair(parts);
                case "attack":
                    return HandleAttack(parts);
                case "fleet-move":
                    return HandleFleetMove(parts);
                case "fleet-attack":
                    return HandleFleetAttack(parts);
                case "run-simulation":
                    return RunSimulation();
                case "status":
                    ui.DisplayStatus();
                    return false;
                case "help":
                    ui.DisplayHelp();
                    return false;
                case "end":
                    return true;
                case "quit":
                    Console.WriteLine("Are you sure you want to quit? (yes/no)");
                    var response = Console.ReadLine()?.ToLower();
                    if (response == "yes" || response == "y")
                    {
                        Environment.Exit(0);
                    }
                    return false;
                default:
                    Console.WriteLine($"Unknown command: {cmd}. Type 'help' for available commands.");
                    return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return false;
        }
    }

    private bool HandleMove(string[] parts)
    {
        if (parts.Length < 3)
        {
            Console.WriteLine("Usage: move <ship_index> <sector>");
            return false;
        }

        if (!int.TryParse(parts[1], out int shipIndex))
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        var fleet = game.GetCurrentPlayerFleet();
        if (shipIndex < 0 || shipIndex >= fleet.Starships.Count)
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        var ship = fleet.Starships[shipIndex];
        string sector = parts[2];

        if (ship.Starbase != null)
        {
            Console.WriteLine("Ship is docked. Undock first.");
            return false;
        }

        ship.MoveToSector(sector);
        Console.WriteLine($"Ship [{shipIndex}] moved to {sector}.");
        return false;
    }

    private bool HandleDock(string[] parts)
    {
        if (parts.Length < 3)
        {
            Console.WriteLine("Usage: dock <ship_index> <starbase_index>");
            return false;
        }

        if (!int.TryParse(parts[1], out int shipIndex) || !int.TryParse(parts[2], out int starbaseIndex))
        {
            Console.WriteLine("Invalid index.");
            return false;
        }

        var fleet = game.GetCurrentPlayerFleet();
        if (shipIndex < 0 || shipIndex >= fleet.Starships.Count)
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        if (starbaseIndex < 0 || starbaseIndex >= fleet.Starbases.Count)
        {
            Console.WriteLine("Invalid starbase index.");
            return false;
        }

        var ship = fleet.Starships[shipIndex];
        var starbase = fleet.Starbases[starbaseIndex];

        if (ship.Sector != starbase.Sector)
        {
            Console.WriteLine("Ship and starbase must be in the same sector.");
            return false;
        }

        ship.DockWithStarbase(starbase);
        Console.WriteLine($"Ship [{shipIndex}] docked at starbase [{starbaseIndex}].");
        return false;
    }

    private bool HandleUndock(string[] parts)
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: undock <ship_index>");
            return false;
        }

        if (!int.TryParse(parts[1], out int shipIndex))
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        var fleet = game.GetCurrentPlayerFleet();
        if (shipIndex < 0 || shipIndex >= fleet.Starships.Count)
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        var ship = fleet.Starships[shipIndex];
        ship.UndockFromStarbase();
        Console.WriteLine($"Ship [{shipIndex}] undocked.");
        return false;
    }

    private bool HandleRepair(string[] parts)
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: repair <ship_index>");
            return false;
        }

        if (!int.TryParse(parts[1], out int shipIndex))
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        var fleet = game.GetCurrentPlayerFleet();
        if (shipIndex < 0 || shipIndex >= fleet.Starships.Count)
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        var ship = fleet.Starships[shipIndex];
        if (ship.Starbase == null)
        {
            Console.WriteLine("Ship must be docked to repair.");
            return false;
        }

        int oldHealth = ship.CurrentHealth;
        int oldCrew = ship.CurrentCrew;
        ship.Repair();
        Console.WriteLine($"Ship [{shipIndex}] repaired. Health: {oldHealth}→{ship.CurrentHealth}, Crew: {oldCrew}→{ship.CurrentCrew}, Actions to skip: {ship.ActionsToSkip}");
        return false;
    }

    private bool HandleAttack(string[] parts)
    {
        if (parts.Length < 4)
        {
            Console.WriteLine("Usage: attack <ship_index> <target_type> <target_index>");
            Console.WriteLine("  target_type: ship or starbase");
            return false;
        }

        if (!int.TryParse(parts[1], out int shipIndex))
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        var fleet = game.GetCurrentPlayerFleet();
        var opponentFleet = game.GetOpponentFleet();

        if (shipIndex < 0 || shipIndex >= fleet.Starships.Count)
        {
            Console.WriteLine("Invalid ship index.");
            return false;
        }

        var ship = fleet.Starships[shipIndex];
        if (ship.Starbase != null)
        {
            Console.WriteLine("Docked ships cannot attack.");
            return false;
        }

        string targetType = parts[2].ToLower();
        if (!int.TryParse(parts[3], out int targetIndex))
        {
            Console.WriteLine("Invalid target index.");
            return false;
        }

        if (targetType == "ship")
        {
            if (targetIndex < 0 || targetIndex >= opponentFleet.Starships.Count)
            {
                Console.WriteLine("Invalid target ship index.");
                return false;
            }

            var target = opponentFleet.Starships[targetIndex];
            if (target.Starbase != null)
            {
                Console.WriteLine("Cannot attack docked ships.");
                return false;
            }

            if (ship.Sector != target.Sector)
            {
                Console.WriteLine("Target must be in the same sector.");
                return false;
            }

            int oldHealth = target.CurrentHealth;
            int oldCrew = target.CurrentCrew;
            ship.AttackTarget(target);
            Console.WriteLine($"Ship [{shipIndex}] attacked opponent ship [{targetIndex}]. Health: {oldHealth}→{target.CurrentHealth}, Crew: {oldCrew}→{target.CurrentCrew}");

            if (target.IsDisabled())
            {
                Console.WriteLine($"Opponent ship [{targetIndex}] destroyed!");
            }
        }
        else if (targetType == "starbase")
        {
            if (targetIndex < 0 || targetIndex >= opponentFleet.Starbases.Count)
            {
                Console.WriteLine("Invalid target starbase index.");
                return false;
            }

            var target = opponentFleet.Starbases[targetIndex];
            if (ship.Sector != target.Sector)
            {
                Console.WriteLine("Target must be in the same sector.");
                return false;
            }

            int oldHealth = target.CurrentHealth;
            ship.AttackTarget(target);
            Console.WriteLine($"Ship [{shipIndex}] attacked opponent starbase [{targetIndex}]. Health: {oldHealth}→{target.CurrentHealth}");

            if (target.IsDisabled())
            {
                Console.WriteLine($"Opponent starbase [{targetIndex}] destroyed!");
            }
        }
        else
        {
            Console.WriteLine("Invalid target type. Use 'ship' or 'starbase'.");
            return false;
        }

        return false;
    }

    private bool HandleFleetMove(string[] parts)
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: fleet-move <sector>");
            return false;
        }

        string sector = parts[1];
        var fleet = game.GetCurrentPlayerFleet();
        fleet.MobiliseToSector(sector);
        Console.WriteLine($"All undocked ships moved to {sector}.");
        return false;
    }

    private bool HandleFleetAttack(string[] parts)
    {
        if (parts.Length < 3)
        {
            Console.WriteLine("Usage: fleet-attack <target_type> <target_index>");
            Console.WriteLine("  target_type: ship or starbase");
            return false;
        }

        var fleet = game.GetCurrentPlayerFleet();
        var opponentFleet = game.GetOpponentFleet();

        string targetType = parts[1].ToLower();
        if (!int.TryParse(parts[2], out int targetIndex))
        {
            Console.WriteLine("Invalid target index.");
            return false;
        }

        if (targetType == "ship")
        {
            if (targetIndex < 0 || targetIndex >= opponentFleet.Starships.Count)
            {
                Console.WriteLine("Invalid target ship index.");
                return false;
            }

            var target = opponentFleet.Starships[targetIndex];
            if (target.Starbase != null)
            {
                Console.WriteLine("Cannot attack docked ships.");
                return false;
            }

            int oldHealth = target.CurrentHealth;
            fleet.AttackTarget(target);
            Console.WriteLine($"All ships attacked opponent ship [{targetIndex}]. Health: {oldHealth}→{target.CurrentHealth}");

            if (target.IsDisabled())
            {
                Console.WriteLine($"Opponent ship [{targetIndex}] destroyed!");
            }
        }
        else if (targetType == "starbase")
        {
            if (targetIndex < 0 || targetIndex >= opponentFleet.Starbases.Count)
            {
                Console.WriteLine("Invalid target starbase index.");
                return false;
            }

            var target = opponentFleet.Starbases[targetIndex];
            int oldHealth = target.CurrentHealth;
            fleet.AttackTarget(target);
            Console.WriteLine($"All ships attacked opponent starbase [{targetIndex}]. Health: {oldHealth}→{target.CurrentHealth}");

            if (target.IsDisabled())
            {
                Console.WriteLine($"Opponent starbase [{targetIndex}] destroyed!");
            }
        }
        else
        {
            Console.WriteLine("Invalid target type. Use 'ship' or 'starbase'.");
            return false;
        }

        return false;
    }
}

