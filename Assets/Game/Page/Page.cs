using UnityEngine;

public class Page : MonoBehaviour
{
	[System.NonSerialized] public PageManager pageManager; // owned by page manager
	[System.NonSerialized] public bool isActive; // owned by page manager

    // useful for button actions
    public void TriggerPushPage(string page) => pageManager.PushPage(page);
    public void TriggerPopPage() => pageManager.PopPage();

    // page manager callbacks
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
