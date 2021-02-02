using System;
using System.Collections;
using UnityEngine;

public static class CoroutineComposer
{
	public static IEnumerator MakeAction(Action action)
	{
		action();
		yield break;
	}

    public static IEnumerator MakeDelayed(float delay, IEnumerator coroutine)
    {
        yield return new WaitForSeconds(delay);
        yield return coroutine;
    }

	public static IEnumerator MakeSequence(params IEnumerator[] coroutines)
	{
		for (int i = 0; i < coroutines.Length; ++i)
			yield return coroutines[i];
	}

	public static IEnumerator MakeParallel(MonoBehaviour owner, params IEnumerator[] coroutines)
	{
		int active = coroutines.Length;
		Action onDone = () => --active;

		foreach (var coroutine in coroutines)
			owner.StartCoroutine(Wrap(coroutine, onDone));

		while (active > 0)
			yield return null;
	}

	private static IEnumerator Wrap(IEnumerator coroutine, Action onDone)
	{
		yield return coroutine;
		onDone();
	}
}

