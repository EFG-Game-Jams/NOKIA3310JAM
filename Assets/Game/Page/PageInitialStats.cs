using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageInitialStats : PageAutoNavigation
{
	public NokiaTextRenderer textRemaining;
	public NokiaTextRenderer textDurability;
	public NokiaTextRenderer textAttack; 
	public NokiaTextRenderer textDefense; 
	public NokiaTextRenderer textLuck;
	
	public override void OnActivate()
	{
		base.OnActivate();
		UpdateDisplay();
	}

	public override void OnInput(GameInput.Action action)
	{
		base.OnInput(action);

		if (action == GameInput.Action.Confirm)
		{
			if (GetRemaining() > 0)
			{
                Game.Instance.audioManager.Play("failure");
                return;
			}
			else
			{
                Game.Instance.audioManager.Play("success");
				Game.Instance.campaign.OnInitialStatsComplete();
				return;
			}
		}
	}

	private int GetRemaining()
	{
		Campaign campaign = Game.Instance.campaign;
		VesselStats stats = campaign.playerStats;
		return campaign.gameBalance.initialStatPoints - stats.GetRawTotal();		
	}

	private void UpdateDisplay()
	{
		textRemaining.Text = GetRemaining().ToString();

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

		if (GetRemaining() <= 0 || stats.GetRaw(stat) >= VesselStats.MaxStatValue)
        {
            Game.Instance.audioManager.Play("failure");
            return;
		}

		stats.SetRaw(stat, stats.GetRaw(stat) + 1);
		UpdateDisplay();
        Game.Instance.audioManager.Play("success");
    }
    public void TryDec(VesselStats.Type stat)
	{
		Campaign campaign = Game.Instance.campaign;
		VesselStats stats = campaign.playerStats;

		if (stats.GetRaw(stat) <= 0)
		{
            Game.Instance.audioManager.Play("failure");
            return;
		}

		stats.SetRaw(stat, stats.GetRaw(stat) - 1);
		UpdateDisplay();
        Game.Instance.audioManager.Play("success");
    }

    public void TryIncDurability() => TryInc(VesselStats.Type.Durability);
	public void TryDecDurability() => TryDec(VesselStats.Type.Durability);
	public void TryIncAttack() => TryInc(VesselStats.Type.Attack);
	public void TryDecAttack() => TryDec(VesselStats.Type.Attack);
	public void TryIncDefense() => TryInc(VesselStats.Type.Defense);
	public void TryDecDefense() => TryDec(VesselStats.Type.Defense);
	public void TryIncLuck() => TryInc(VesselStats.Type.Luck);
	public void TryDecLuck() => TryDec(VesselStats.Type.Luck);
}
