using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
	private float activeTime;

	/// <summary>
	/// Get a generator which will remain active until the effect is destroyed
	/// </summary>
	public IEnumerator Run()
	{
		yield return TickUntilDone();
		Destroy(gameObject);
	}

	protected virtual IEnumerator TickUntilDone()
	{
		while (OnUpdate(activeTime))
		{
			yield return null;
			activeTime += Time.deltaTime;
		}
	}

	/// <summary>
	/// Update the effect
	/// </summary>
	/// <param name="time">time since effect creation</param>
	/// <returns>true to keep going, false to destroy the effect</returns>
	protected virtual bool OnUpdate(float time) => false;
}
