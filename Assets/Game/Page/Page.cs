using UnityEngine;

public class Page : MonoBehaviour
{
	[System.NonSerialized] public PageManager pageManager; // owned by page manager
	[System.NonSerialized] public bool isActive; // owned by page manager

	public virtual void OnPush() { }
	public virtual void OnPop() { }
	public virtual void OnActivate() { }
	public virtual void OnDeactivate() { }
	public virtual void OnInput(GameInput.Action action) { }

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(new Vector3(42, 24), new Vector3(84, 48));
	}
}
