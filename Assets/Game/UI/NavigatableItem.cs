using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigatableItem : MonoBehaviour
{
	[System.Serializable]
	public class Navigation
	{
		public GameInput.Action trigger;
		public NavigatableItem navTarget;
		public UnityEvent navAction;
	}

	public Navigation[] navigation;
	
	public NavigatableItem OnAction(GameInput.Action action)
	{
		foreach (var nav in navigation)
		{
			if (nav.trigger == action)
			{
				nav.navAction?.Invoke();
				return nav.navTarget;
			}
		}
		return this;
	}
}
