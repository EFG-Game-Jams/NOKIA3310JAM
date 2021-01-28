using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
	public Campaign owner;
	public EncounterDescriptor descriptor;

	private VesselStats opponentStats;
	private VesselStatus opponentStatus;

	public void BeginEncounter(Campaign owner, EncounterDescriptor descriptor)
	{
		this.owner = owner;
		this.descriptor = descriptor;

		opponentStats = gameObject.AddComponent<VesselStats>();
		InitialiseOpponentStats();

		opponentStatus = gameObject.AddComponent<VesselStatus>();
		InitialiseOpponentStatus();

		Game.Instance.pageManager.PushPage("Encounter");
	}
	
	private void OnDestroy()
	{
		Destroy(opponentStats);
		Destroy(opponentStatus);
	}
	
	private void InitialiseOpponentStats()
	{
		// don't modify asset's stats array
		int[] stats = (int[])descriptor.statsBase.Clone();
		Debug.Assert(stats.Length == 4);

		// we won't be changing the weights though
		float[] weights = descriptor.statsScalingWeight;
		Debug.Assert(stats.Length == 4);
		float totalWeight = 0;
		foreach (var weight in weights)
			totalWeight += weight;

		// distribute scaling points
		float weightToPoints = owner.playerStats.GetRawTotal() * descriptor.statsScaling / totalWeight;
		for (int i = 0; i < stats.Length; ++i)
			stats[i] += Mathf.FloorToInt(weights[i] * weightToPoints);

		// apply
		opponentStats.SetStats(stats);
	}

	private void InitialiseOpponentStatus()
	{
		// initialise full first
		opponentStatus.InitialiseFull(owner.gameBalance, opponentStats);

		// then apply modifiers
		// todo: encounter modifiers
	}
}
