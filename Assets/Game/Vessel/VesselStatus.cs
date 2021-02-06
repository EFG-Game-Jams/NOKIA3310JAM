﻿using UnityEngine;

public class VesselStatus : MonoBehaviour
{
    public const int MaxSystemStatus = 100;
    public const int MaxFuel = 5;
    public const int MaxAmmo = 10;

    public int health { get; private set; }
    public int engines { get; private set; }
    public int weapons { get; private set; }
    public int shields { get; private set; }

    public bool CanRepair => (health < GetMaxHealth() || (engines + weapons + shields) < 3 * MaxSystemStatus);

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
        return Mathf.RoundToInt(Mathf.Lerp(balance.healthMin, balance.healthMax, stats.GetDurability()));
    }

    public void RepairFull()
    {
        health = GetMaxHealth();
        engines = MaxSystemStatus;
        weapons = MaxSystemStatus;
        shields = MaxSystemStatus;
    }

    public void ApplyShieldDamage(int amount)
    {
        if (amount < 0)
        {
            return;
        }

        shields = Mathf.Max(shields - amount, 0);
    }

    public void ApplyHullDamage(int amount)
    {
        if (amount < 0)
        {
            return;
        }

        health = Mathf.Max(health - amount, 0);
    }
}
