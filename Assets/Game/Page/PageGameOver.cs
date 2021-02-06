public class PageGameOver : PageAutoNavigation
{
    public override void OnActivate()
    {
        base.OnActivate();
    }

    public override void OnInput(GameInput.Action action)
    {
        if (action == GameInput.Action.Confirm)
        {
            Game.Instance.pageManager.PushPage("MainMenu");
        }
    }

    public void Skip()
    {
        pageManager.SetPage("MainMenu");
    }
}
