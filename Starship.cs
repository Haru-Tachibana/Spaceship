namespace ProjectSpaceship;

public class Starship
{
    public int MaximumAttackStrength { get; }
    public int MaximumDefenceStrength { get; }
    public int MaximumCrew { get; }
    public int MaximumHealth { get; }
    public int CurrentHealth { get; private set; }
    public int CurrentCrew { get; private set; }
    public string Sector { get; private set; }
    public Starbase? Starbase { get; private set; }
    public Fleet Fleet { get; }
    public int ActionsToSkip { get; private set; }

    public int CurrentAttackStrength
    {
        get
        {
            if (CurrentHealth <= 0) return 0;
            return (int)Math.Ceiling(MaximumAttackStrength * ((double)CurrentHealth / MaximumHealth));
        }
    }

    public int CurrentDefenceStrength
    {
        get
        {
            if (CurrentHealth <= 0) return 0;
            return (int)Math.Floor(MaximumDefenceStrength * ((double)(CurrentHealth + CurrentCrew) / (MaximumHealth + MaximumCrew)));
        }
    }

    public Starship(Fleet fleet, string sector, int maximumAttackStrength = 30, int maximumDefenceStrength = 10, int maximumCrew = 10, int maximumHealth = 100)
    {
        Fleet = fleet;
        Sector = sector;
        MaximumAttackStrength = maximumAttackStrength;
        MaximumDefenceStrength = maximumDefenceStrength;
        MaximumCrew = maximumCrew;
        MaximumHealth = maximumHealth;
        CurrentHealth = maximumHealth;
        CurrentCrew = maximumCrew;
        Starbase = null;
        ActionsToSkip = 0;
    }

    public void MoveToSector(string sector)
    {
        if (ActionsToSkip > 0)
        {
            ActionsToSkip--;
            return;
        }

        if (Starbase != null)
        {
            return;
        }

        Sector = sector;
    }

    public void DockWithStarbase(Starbase starbase)
    {
        if (ActionsToSkip > 0)
        {
            ActionsToSkip--;
            return;
        }

        if (Starbase != null)
        {
            return;
        }

        if (starbase.Fleet != Fleet)
        {
            return;
        }

        if (starbase.Sector != Sector)
        {
            return;
        }

        Starbase = starbase;
        starbase.DockShip(this);
    }

    public void UndockFromStarbase()
    {
        if (ActionsToSkip > 0)
        {
            ActionsToSkip--;
            return;
        }

        if (Starbase == null)
        {
            return;
        }

        Starbase.UndockShip(this);
        Starbase = null;
    }

    public void Repair()
    {
        if (ActionsToSkip > 0)
        {
            ActionsToSkip--;
            return;
        }

        if (Starbase == null)
        {
            return;
        }

        double healthPercentage = (double)CurrentHealth / MaximumHealth;
        CurrentHealth = MaximumHealth;
        CurrentCrew = MaximumCrew;

        if (healthPercentage < 0.25)
        {
            ActionsToSkip = 4;
        }
        else if (healthPercentage < 0.50)
        {
            ActionsToSkip = 3;
        }
        else if (healthPercentage < 0.75)
        {
            ActionsToSkip = 2;
        }
        else
        {
            ActionsToSkip = 1;
        }
    }

    public void AttackTarget(Starship target)
    {
        if (ActionsToSkip > 0)
        {
            ActionsToSkip--;
            return;
        }

        if (Starbase != null)
        {
            return;
        }

        if (target.Fleet == Fleet)
        {
            return;
        }

        if (target.Sector != Sector)
        {
            return;
        }

        if (target.Starbase != null)
        {
            return;
        }

        int damage = Math.Max(CurrentAttackStrength - target.CurrentDefenceStrength, 5);
        target.CurrentHealth = Math.Max(0, target.CurrentHealth - damage);

        int incapacitatedCrew = (int)Math.Ceiling((double)damage / target.MaximumHealth * target.CurrentCrew);
        target.CurrentCrew = Math.Max(1, target.CurrentCrew - incapacitatedCrew);

        if (target.CurrentHealth <= 0)
        {
            target.Fleet.RemoveStarship(target);
        }
    }

    public void AttackTarget(Starbase target)
    {
        if (ActionsToSkip > 0)
        {
            ActionsToSkip--;
            return;
        }

        if (Starbase != null)
        {
            return;
        }

        if (target.Fleet == Fleet)
        {
            return;
        }

        if (target.Sector != Sector)
        {
            return;
        }

        int damage = Math.Max(CurrentAttackStrength - target.CurrentDefenceStrength, 5);
        target.CurrentHealth = Math.Max(0, target.CurrentHealth - damage);

        if (target.CurrentHealth <= 0)
        {
            target.Fleet.RemoveStarbase(target);
        }
    }

    public bool IsDisabled()
    {
        return CurrentHealth <= 0;
    }
}

