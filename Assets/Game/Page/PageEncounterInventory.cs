using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageEncounterInventory : PageAutoNavigation
{    
	public NokiaTextRenderer textPlayerFuel;
	public NokiaTextRenderer textPlayerAmmo;
    public NokiaTextRenderer textEnemyFuel;
    public NokiaTextRenderer textEnemyAmmo;

    public override void OnPush()
	{
		base.OnPush();

        var encounter = Game.Instance.campaign.encounter;
        var playerStatus = encounter.playerEncounter.Status;
        var enemyStatus = encounter.opponentEncounter.Status;
        bool hasScan = encounter.playerEncounter.AbilityScan.IsActive;

        textPlayerFuel.Text = playerStatus.fuel.ToString();
        textPlayerAmmo.Text = playerStatus.ammo.ToString();

        textEnemyFuel.Text = hasScan ? enemyStatus.fuel.ToString() : "?";
        textEnemyAmmo.Text = hasScan ? enemyStatus.ammo.ToString() : "?";
    }
}
