namespace ProjectSpaceship;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to PROJECT SPACESHIP!");
        Console.WriteLine("Press Enter to start the game...");
        Console.ReadLine();

        var game = new Game();
        var ui = new GameUI(game);
        var controller = new GameController(game, ui);

        while (!game.IsGameOver())
        {
            ui.DisplayGameState();
            ui.DisplayCommands();

            Console.Write($"{game.CurrentPlayer} > ");
            string? command = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(command))
            {
                continue;
            }

            bool turnEnded = controller.ProcessCommand(command);

            if (turnEnded)
            {
                game.SwitchTurn();
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        ui.DisplayGameOver();
    }
}
