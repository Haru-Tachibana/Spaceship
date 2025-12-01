# Spaceship

Minimal console spaceship game project. Includes a scripted simulation and unit tests.

Requirements
.NET SDK 8.0 or later

Build

From the project root:

```bash
dotnet build ProjectSpaceship.csproj -c Debug
```

Run (interactive)

```bash
dotnet run --project ./ProjectSpaceship.csproj
```

Run the scripted simulation (non-interactive example)

```bash
# Feed the commands to auto-run the example simulation and then quit
printf "\nrun-simulation\n\nquit\nyes\n" | dotnet run --project ./ProjectSpaceship.csproj
```

Tests

The repository includes an xUnit test project. From the repository root run:

```bash
dotnet test ProjectSpaceship.Tests/ProjectSpaceship.Tests.csproj
```

Repository notes
- The `run-simulation` command executes a scripted example that performs the sequence described in the requirements (moves, docks, attacks, repairs, fleet attack).
- Tests exercise the scripted simulation and assert the final expected outcome.
