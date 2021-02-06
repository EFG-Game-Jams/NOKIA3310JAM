using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageLevelUp : PageAutoNavigation
{
	public NokiaTextRenderer textDurability;
	public NokiaTextRenderer textAttack; 
	public NokiaTextRenderer textDefense; 
	public NokiaTextRenderer textLuck;

    public System.Action onPointSpent;
	
	public override void OnActivate()
	{
		base.OnActivate();
		UpdateDisplay();
	}

	public override void OnInput(GameInput.Action action)
	{
		base.OnInput(action);

        if (action == GameInput.Action.Cancel)
            pageManager.PopPage();
	}

	private void UpdateDisplay()
	{
		Campaign campaign = Game.Instance.campaign;
		VesselStats stats = campaign.playerStats;
		textDurability.Text = stats.GetRaw(VesselStats.Type.Durability).ToString();
		textAttack.Text = stats.GetRaw(VesselStats.Type.Attack).ToString();
		textDefense.Text = stats.GetRaw(VesselStats.Type.Defense).ToString();
		textLuck.Text = stats.GetRaw(VesselStats.Type.Luck).ToString();
	}

	public void TryInc(VesselStats.Type stat)
	{
		Campaign campaign = Game.Instance.campaign;
		VesselStats stats = campaign.playerStats;

		if (stats.GetRaw(stat) >= VesselStats.MaxStatValue)
        {
            Game.Instance.audioManager.Play("failure");
            return;
		}

		stats.SetRaw(stat, stats.GetRaw(stat) + 1);
        Game.Instance.audioManager.Play("success");

        pageManager.PopPage();
        onPointSpent?.Invoke();
    }

    public void TryIncDurability() => TryInc(VesselStats.Type.Durability);
	public void TryIncAttack() => TryInc(VesselStats.Type.Attack);
	public void TryIncDefense() => TryInc(VesselStats.Type.Defense);
	public void TryIncLuck() => TryInc(VesselStats.Type.Luck);
}
