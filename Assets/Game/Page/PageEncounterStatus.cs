using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageEncounterStats : PageAutoNavigation
{    
	public NokiaTextRenderer textPlayerAttack; 
	public NokiaTextRenderer textPlayerDefense; 
	public NokiaTextRenderer textPlayerLuck;
    public NokiaTextRenderer textEnemyAttack;
    public NokiaTextRenderer textEnemyDefense;
    public NokiaTextRenderer textEnemyLuck;

    public override void OnPush()
	{
		base.OnPush();

        var encounter = Game.Instance.campaign.encounter;
        var playerStats = encounter.playerEncounter.Stats;
        var enemyStats = encounter.opponentEncounter.Stats;
        bool hasScan = encounter.playerEncounter.AbilityScan.IsActive;

        textPlayerAttack.Text = playerStats.GetRaw(VesselStats.Type.Attack).ToString();
        textPlayerDefense.Text = playerStats.GetRaw(VesselStats.Type.Defense).ToString();
        textPlayerLuck.Text = playerStats.GetRaw(VesselStats.Type.Luck).ToString();

        textEnemyAttack.Text = hasScan ? enemyStats.GetRaw(VesselStats.Type.Attack).ToString() : "?";
        textEnemyDefense.Text = hasScan ? enemyStats.GetRaw(VesselStats.Type.Defense).ToString() : "?";
        textEnemyLuck.Text = hasScan ? enemyStats.GetRaw(VesselStats.Type.Luck).ToString() : "?";
    }
}
