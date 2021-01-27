using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItem : MonoBehaviour
{
	public GameObject selectionIndicator;

	private void Awake()
	{
		SetSelected(false);
	}

	public void SetSelected(bool selected)
	{
		// to anyone reading this and wondering why the null check...
		// you can't use null-propagation on unity objects when dealing wwith a serialised reference
		// that might seem arbitrary, but there's a good reason for it!
		if (selectionIndicator != null)
			selectionIndicator.SetActive(selected);
	}
}
