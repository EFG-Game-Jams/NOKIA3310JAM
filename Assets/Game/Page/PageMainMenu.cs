using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageMainMenu : PageAutoNavigation
{
	public void OnNewGame()
	{
		Debug.Log("New Game");
	}
	public void OnCredits()
	{
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
