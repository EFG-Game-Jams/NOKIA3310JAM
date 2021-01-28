﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageAutoNavigation : Page
{
    public NavigatableItem defaultNavItem;

    private NavigatableItem currentNavItem;

    public override void OnActivate()
    {
        SetNavItem(defaultNavItem);
    }

    public override void OnInput(GameInput.Action action)
    {
        if (currentNavItem != null)
            SetNavItem(currentNavItem.OnAction(action));
    }

    private void SetNavItem(NavigatableItem item)
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