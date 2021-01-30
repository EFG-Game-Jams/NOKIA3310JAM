using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageAutoNavigation : Page
{
    public NavigatableItem defaultNavItem;

    protected NavigatableItem currentNavItem;

    public override void OnPush()
    {
        SetNavItem(defaultNavItem);
    }

    public override void OnInput(GameInput.Action action)
    {        
        if (currentNavItem != null)
            SetNavItem(currentNavItem.OnAction(action));
    }

    protected void SetNavItem(NavigatableItem item)
    {
        if (currentNavItem == item)
            return;

        if (currentNavItem != null)
            currentNavItem.GetComponent<SelectableItem>()?.SetSelected(false);

        currentNavItem = item;

        if (currentNavItem != null)
            currentNavItem.GetComponent<SelectableItem>()?.SetSelected(true);
    }
}
