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

    public void TryIncAttack() => TryInc(VesselStats.Type.Attack);
    public void TryIncDefense() => TryInc(VesselStats.Type.Defense);
    public void TryIncLuck() => TryInc(VesselStats.Type.Luck);
}
