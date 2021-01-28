using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaign : MonoBehaviour
{
	public VesselStats playerStats;

	[System.NonSerialized] public GameBalance gameBalance;

	private int nextEncounter = 0;

	public void Begin(string balanceName)
	{
		gameBalance = Resources.Load<GameBalance>(balanceName);
		Debug.Assert(gameBalance != null);
	}
}
