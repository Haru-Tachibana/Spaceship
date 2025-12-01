using Xunit;

namespace ProjectSpaceship.Tests
{
    public class GameSimulationTests
    {
        [Fact]
        public void RunSimulationCommand_Should_Destroy_Player2_Starbase_And_Mark_Winner()
        {
            // Arrange
            var game = new ProjectSpaceship.Game();
            var ui = new ProjectSpaceship.GameUI(game);
            var controller = new ProjectSpaceship.GameController(game, ui);

            // Act: run the simulation via the public command interface
            controller.ProcessCommand("run-simulation");

            // Assert: Player 2 should be defeated and Player 1 should be winner
            Assert.True(game.Player2Fleet.IsDefeated(), "Player 2 fleet should be defeated (no starbases remain).");
            Assert.Equal("Player 1", game.GetWinner());
        }
    }
}
