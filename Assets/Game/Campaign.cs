using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaign : MonoBehaviour
{
	public GameBalance gameBalance;

	public void SetBalance(string name)
	{
		gameBalance = Resources.Load<GameBalance>(name);
		Debug.Assert(gameBalance != null);
	}
}
