using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageEncounterStatus : PageAutoNavigation
{    
	public NokiaTextRenderer textPlayerEngines; 
	public NokiaTextRenderer textPlayerWeapons; 
	public NokiaTextRenderer textPlayerShields;
    public NokiaTextRenderer textEnemyEngines;
    public NokiaTextRenderer textEnemyWeapons;
    public NokiaTextRenderer textEnemyShields;

    public override void OnPush()
	{
		base.OnPush();

        var encounter = Game.Instance.campaign.encounter;
        var playerStatus = encounter.playerEncounter.Status;
        var enemyStatus = encounter.opponentEncounter.Status;
        bool hasScan = encounter.playerEncounter.AbilityScan.IsActive;

        textPlayerEngines.Text = playerStatus.engines.ToString();
        textPlayerWeapons.Text = playerStatus.weapons.ToString();
        textPlayerShields.Text = playerStatus.shields.ToString();

        textEnemyEngines.Text = hasScan ? enemyStatus.engines.ToString() : "?";
        textEnemyWeapons.Text = hasScan ? enemyStatus.weapons.ToString() : "?";
        textEnemyShields.Text = hasScan ? enemyStatus.shields.ToString() : "?";
    }
}
