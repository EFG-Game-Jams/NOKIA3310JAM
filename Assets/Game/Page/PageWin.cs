public class PageWin : PageAutoNavigation
{
    public override void OnActivate()
    {
        base.OnActivate();
    }

    public override void OnInput(GameInput.Action action)
    {
        if (action == GameInput.Action.Confirm)
        {
            Game.Instance.pageManager.SetPage("MainMenu");
        }
    }

    public void Skip()
    {
        pageManager.SetPage("MainMenu");
    }
}
