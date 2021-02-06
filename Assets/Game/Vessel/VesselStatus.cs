using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselStatus : MonoBehaviour
{
	public const int MaxSystemStatus = 100;
    public const int MaxFuel = 5;
    public const int MaxAmmo = 10;

	public int health;
	public int engines;
	public int weapons;
	public int shields;

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
}
