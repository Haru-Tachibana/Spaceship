namespace ProjectSpaceship;

public class Starbase
{
    public int MaximumDefenceStrength { get; }
    public List<Starship> DockedShips { get; }
    public int MaximumHealth { get; }
    public int CurrentHealth { get; set; }
    public string Sector { get; }
    public Fleet Fleet { get; }

    public int CurrentDefenceStrength
    {
        get
        {
            double baseDefence = MaximumDefenceStrength * ((double)CurrentHealth / MaximumHealth);
            double dockedDefence = 0;

            if (DockedShips.Count > 0)
            {
                int totalDockedDefence = DockedShips.Sum(ship => ship.CurrentDefenceStrength);
                dockedDefence = (double)totalDockedDefence * DockedShips.Count / MaximumDefenceStrength;
            }

            return (int)Math.Floor(baseDefence + dockedDefence);
        }
    }

    public Starbase(Fleet fleet, string sector, int maximumHealth = 500, int maximumDefenceStrength = 20)
    {
        Fleet = fleet;
        Sector = sector;
        MaximumHealth = maximumHealth;
        CurrentHealth = maximumHealth;
        MaximumDefenceStrength = maximumDefenceStrength;
        DockedShips = new List<Starship>();
    }

    public void DockShip(Starship ship)
    {
        if (!DockedShips.Contains(ship))
        {
            DockedShips.Add(ship);
        }
    }

    public void UndockShip(Starship ship)
    {
        DockedShips.Remove(ship);
    }

    public bool IsDisabled()
    {
        return CurrentHealth <= 0;
    }
}

