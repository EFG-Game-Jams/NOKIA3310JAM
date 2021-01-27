using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaign : MonoBehaviour
{
	public int nextEncounter = 0;

	[System.NonSerialized] public GameBalance gameBalance;

	public void Begin(string balanceName)
	{
		gameBalance = Resources.Load<GameBalance>(name);
		Debug.Assert(gameBalance != null);
	}
}
