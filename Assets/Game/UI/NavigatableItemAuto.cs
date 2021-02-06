using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigatableItemAuto : NavigatableItem
{
	public GameInput.Action triggerPrevious;
	public GameInput.Action triggerNext;

	public UnityEvent onConfirm;
	public UnityEvent onCancel;

    public void RebuildNavigation()
    {
        List<Navigation> nav = new List<Navigation>();
        ConfigureNavigation(nav);
        navigation = nav.ToArray();
    }

    private void Start()
	{
        RebuildNavigation();
	}
    
    protected virtual void ConfigureNavigation(List<Navigation> navigation)
    {
        navigation.Add(new Navigation { trigger = triggerPrevious, navTarget = FindPrevious() });
        navigation.Add(new Navigation { trigger = triggerNext, navTarget = FindNext() });
        navigation.Add(new Navigation { trigger = GameInput.Action.Confirm, navAction = onConfirm });
        navigation.Add(new Navigation { trigger = GameInput.Action.Cancel, navAction = onCancel });
    }

	private NavigatableItem FindPrevious()
	{
		return Find(-1);
	}
	private NavigatableItem FindNext()
	{
		return Find(1);
	}
	private NavigatableItem Find(int direction)
	{
		Transform parent = transform.parent;
		int siblingCount = parent.childCount;
		int rank = transform.GetSiblingIndex();
		for (int i = 1; i < siblingCount; ++i)
		{
			int siblingIndex = (i * direction + rank + siblingCount) % siblingCount;
			NavigatableItem target = parent.GetChild(siblingIndex).GetComponent<NavigatableItem>();
			if (target != null && target.gameObject.activeSelf)
				return target;
		}
		return null;
	}
}
