namespace ProjectSpaceship;

public class Fleet
{
    public string Player { get; }
    public List<Starship> Starships { get; }
    public List<Starbase> Starbases { get; }

    public Fleet(string player)
    {
        Player = player;
        Starships = new List<Starship>();
        Starbases = new List<Starbase>();
    }

    public void AddStarship(Starship starship)
    {
        if (!Starships.Contains(starship))
        {
            Starships.Add(starship);
        }
    }

    public void AddStarbase(Starbase starbase)
    {
        if (!Starbases.Contains(starbase))
        {
            Starbases.Add(starbase);
        }
    }

    public void RemoveStarship(Starship starship)
    {
        Starships.Remove(starship);
        if (starship.Starbase != null)
        {
            starship.Starbase.UndockShip(starship);
        }
    }

    public void RemoveStarbase(Starbase starbase)
    {
        Starbases.Remove(starbase);
        foreach (var ship in starbase.DockedShips.ToList())
        {
            ship.UndockFromStarbase();
            RemoveStarship(ship);
        }
    }

    public void MobiliseToSector(string sector)
    {
        foreach (var starship in Starships.ToList())
        {
            if (starship.Starbase == null && !starship.IsDisabled())
            {
                starship.MoveToSector(sector);
            }
        }
    }

    public void AttackTarget(Starship target)
    {
        foreach (var starship in Starships.ToList())
        {
            if (!starship.IsDisabled() && starship.Starbase == null && starship.Sector == target.Sector)
            {
                starship.AttackTarget(target);
            }
        }
    }

    public void AttackTarget(Starbase target)
    {
        foreach (var starship in Starships.ToList())
        {
            if (!starship.IsDisabled() && starship.Starbase == null && starship.Sector == target.Sector)
            {
                starship.AttackTarget(target);
            }
        }
    }

    public bool IsDefeated()
    {
        return Starbases.Count == 0;
    }
}

