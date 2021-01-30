using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselStatus : MonoBehaviour
{
	public const int MaxSystemStatus = 100;

	public int health;
	public int engines;
	public int weapons;
	public int shields;

	private GameBalance balance;
	private VesselStats stats;

	public void InitialiseFull(GameBalance balance, VesselStats stats)
	{
		this.balance = balance;
		this.stats = stats;

		health = GetMaxHealth();
		engines = MaxSystemStatus;
		weapons = MaxSystemStatus;
		shields = MaxSystemStatus;
	}

	public float GetHealthPercentage()
	{
		return health / (float)GetMaxHealth();
	}
	public int GetMaxHealth()
	{
		return Mathf.RoundToInt(Mathf.Lerp(balance.healthMin, balance.healthMax, stats.GetDurability()));
	}
}
