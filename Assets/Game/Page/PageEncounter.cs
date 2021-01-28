using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageEncounter : PageAutoNavigation
{
	public HealthBar healthBarPlayer;
	public HealthBar healthBarOpponent;
	public VesselVisuals playerVisuals;
	public Transform opponentVisualsRoot;

	private VesselVisuals opponentVisuals;
	private Encounter encounter;

	public override void OnPush()
	{
		base.OnPush();
		encounter = Game.Instance.campaign.encounter;
		opponentVisuals = Instantiate(encounter.descriptor.visuals, opponentVisualsRoot);
	}
	public override void OnPop()
	{
		base.OnPop();
		Destroy(opponentVisuals.gameObject);
	}
}
