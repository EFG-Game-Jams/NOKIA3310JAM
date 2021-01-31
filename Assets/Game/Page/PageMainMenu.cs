using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageMainMenu : PageAutoNavigation
{
	public void OnNewGame()
	{
        Game.Instance.audioManager.Play("success");
        Game.Instance.StartCampaign();
	}
	public void OnCredits()
	{
        Game.Instance.audioManager.Play("success");
        Debug.Log("Credits");
	}
	public void OnQuit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
