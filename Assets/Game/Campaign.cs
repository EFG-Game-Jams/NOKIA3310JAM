using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaign : MonoBehaviour
{
	public enum State
	{
		None,
		Introduction,
		InitialStats,
		PreEncounter,
		Encounter,
		PostEncounter,
	}

	[System.NonSerialized] public GameBalance gameBalance;
	[System.NonSerialized] public VesselStats playerStats;
	[System.NonSerialized] public VesselStatus playerStatus;
	[System.NonSerialized] public Encounter encounter;

	private State state;
	private int nextEncounter;
	private int encounterCount;

	public void BeginCampaign(string balanceName)
	{
		gameBalance = Resources.Load<GameBalance>(balanceName);
		Debug.Assert(gameBalance != null);

		playerStats = gameObject.AddComponent<VesselStats>();
		playerStats.SetStats(new int[4]);

		playerStatus = gameObject.AddComponent<VesselStatus>();
		//playerStatus.InitialiseFull(gameBalance, playerStats);

		encounter = null;

		state = State.None;
		nextEncounter = 0;
		encounterCount = gameBalance.encounterCount;

		//*
		state = State.InitialStats;
		Game.Instance.pageManager.SetPage("InitialStats");
		/*/
		MakeTestEncounter();
		//*/
	}

	public void OnInitialStatsComplete()
	{
		Debug.Assert(state == State.InitialStats);

		playerStatus.InitialiseFull(gameBalance, playerStats);

		MakeTestEncounter();
	}

	private void MakeTestEncounter()
	{
		state = State.Encounter;
		var encounterObject = new GameObject("Encounter");
		encounter = encounterObject.AddComponent<Encounter>();
		encounter.BeginEncounter(this, Resources.Load<EncounterDescriptor>("Encounters/TestHostile"));
	}
}
