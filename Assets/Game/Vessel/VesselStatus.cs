using UnityEngine;

public class VesselStatus : MonoBehaviour
{
    public const int MaxSystemStatus = 100;
    public const int MaxFuel = 5;
    public const int MaxAmmo = 10;
    public const int CriticalRepairRestoresTo = 50;

    public int health { get; private set; }
    public int engines { get; private set; }
    public int weapons { get; private set; }
    public int shields { get; private set; }

    public bool CanRepair =>
        health < GetMaxHealth() ||
        engines < MaxSystemStatus ||
        weapons < MaxSystemStatus ||
        shields < MaxSystemStatus;

    public bool HasCriticalSystems =>
        engines <= 0 ||
        weapons <= 0 ||
        shields <= 0;

    public bool CanFlee => engines > 0;

    public float healthPercentage => health / MaxSystemStatus;
    public float enginesPercentage => engines / MaxSystemStatus;
    public float weaponsPercentage => weapons / MaxSystemStatus;
    public float shieldsPercentage => shields / MaxSystemStatus;

    public int fuel;
    public int ammo;

    private GameBalance balance;
    private VesselStats stats;

    public void InitialiseFull(GameBalance balance, VesselStats stats)
    {
        this.balance = balance;
        this.stats = stats;

        RepairFull();

        fuel = balance.initialFuel;
        ammo = balance.initialAmmo;
    }

    public float GetHealthPercentage()
    {
        return health / (float)GetMaxHealth();
    }
    public int GetMaxHealth()
    {
        return 100;
        //return Mathf.RoundToInt(Mathf.Lerp(balance.healthMin, balance.healthMax, stats.GetDurability()));
    }

    public void Repair()
    {
        if (engines <= 0)
        {
            engines = CriticalRepairRestoresTo;
            return;
        }
        else if (weapons <= 0)
        {
            weapons = CriticalRepairRestoresTo;
            return;
        }
        else if (shields <= 0)
        {
            shields = CriticalRepairRestoresTo;
            return;
        }
        else
        {
            var roll = stats.RollDefense();
            var maxHealth = GetMaxHealth();
            health = Mathf.Min(maxHealth, health + Mathf.RoundToInt(roll * maxHealth));

            var subsystemBoost = Mathf.RoundToInt(roll * MaxSystemStatus);
            engines = Mathf.Min(MaxSystemStatus, engines + subsystemBoost);
            weapons = Mathf.Min(MaxSystemStatus, weapons + subsystemBoost);
            shields = Mathf.Min(MaxSystemStatus, shields + subsystemBoost);
        }
    }

    public void RepairFull()
    {
        health = GetMaxHealth();
        engines = MaxSystemStatus;
        weapons = MaxSystemStatus;
        shields = MaxSystemStatus;
    }

    public void ApplyShieldDamage(int amount, bool allowHullDamage, bool allowSystemsDamage)
    {
        if (amount < 0)
        {
            return;
        }

        var excessDamage = Mathf.Max(0, amount - shields);
        shields = Mathf.Max(shields - amount, 0);
        Debug.LogFormat("> shields absorbed {0}", amount);

        if (allowHullDamage && excessDamage > 0)
        {
            ApplyHullDamage(excessDamage, allowSystemsDamage);
        }
    }

    public void ApplyHullDamage(int amount, bool allowSystemsDamage)
    {
        if (amount < 0)
        {
            return;
        }

        health = Mathf.Max(health - amount, 0);

        Debug.LogFormat("> hull took {0}", amount);

        if (amount > 0 && allowSystemsDamage)
        {
            ApplySystemsDamage(amount);
        }
    }

    public void ApplySystemsDamage(int amount)
    {
        var damage = ReduceDamageByLuckRoll(amount);
        engines = Mathf.Max(engines - damage, 0);
        Debug.LogFormat("> engines took {0} (Pre luck roll damage of {1})", damage, amount);

        damage = damage = ReduceDamageByLuckRoll(amount);
        weapons = Mathf.Max(weapons - damage, 0);
        Debug.LogFormat("> weapons took {0} (Pre luck roll damage of {1})", damage, amount);

        damage = damage = ReduceDamageByLuckRoll(amount);
        shields = Mathf.Max(shields - damage, 0);
        Debug.LogFormat("> shields took {0} (Pre luck roll damage of {1})", damage, amount);
    }

    private int ReduceDamageByLuckRoll(int amount)
    {
        var luckRoll = 1f - stats.RollLuck(); // Maximum of 100% damage reduction
        return Mathf.RoundToInt(amount * luckRoll);
    }
}
