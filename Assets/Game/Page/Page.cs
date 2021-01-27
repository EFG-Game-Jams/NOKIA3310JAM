using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
	public string pageName;

	[System.NonSerialized] public PageManager pageManager; // owned by page manager
	[System.NonSerialized] public bool isActive; // owned by page manager

	public virtual void OnActivate() { }
	public virtual void OnDeactivate() { }
	public virtual void OnInput(GameInput.Action action) { }
}
