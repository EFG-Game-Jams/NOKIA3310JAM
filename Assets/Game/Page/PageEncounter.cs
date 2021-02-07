using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageEncounter : PageAutoNavigation
{
	public HealthBar healthBarPlayer;
	public HealthBar healthBarOpponent;
	public VesselVisuals playerVisuals;
	public Transform opponentVisualsRoot;

	[System.NonSerialized] public VesselVisuals opponentVisuals;
	private Encounter encounter;

	public bool IsInputEnabled
	{
		get => (currentNavItem != null);
		set
		{
			if (value == IsInputEnabled)
				return;
			if (value)
				SetNavItem(defaultNavItem);
			else
				SetNavItem(null);
		}
	}

	public override void OnPush()
	{
		base.OnPush();
		encounter = Game.Instance.campaign.encounter;
		opponentVisuals = Instantiate(encounter.descriptor.enemyVisual, opponentVisualsRoot);
	}
	public override void OnPop()
	{
		base.OnPop();
		Destroy(opponentVisuals.gameObject);
	}

	public void PushPageAttack() => Game.Instance.pageManager.PushPage("AbilitiesAttack");
	public void PushPageDefend() => Game.Instance.pageManager.PushPage("AbilitiesDefend");
	public void PushPageUtility() => Game.Instance.pageManager.PushPage("AbilitiesUtility");
	public void PushPageInfo() => Game.Instance.pageManager.PushPage("EncounterInfo");
}
