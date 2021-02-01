using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageAbilityDescription : Page
{
    public NokiaTextRenderer[] lines;

    public void Display(VesselAbility ability)
    {
        string[] text = ability.Description.Split('\n');
        Debug.Assert(text.Length <= lines.Length);

        for (int i = 0; i < lines.Length; ++i)
            lines[i].Text = (i < text.Length ? text[i] : "");

        Game.Instance.pageManager.PushPage(this);
    }

    public override void OnInput(GameInput.Action action)
    {
        if (action == GameInput.Action.Left || action == GameInput.Action.Cancel || action == GameInput.Action.Confirm)
            Game.Instance.pageManager.PopPage();
    }
}
