using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HealthBar : MonoBehaviour
{
	public SpriteRenderer bar;
	public int barStart;
	public int barEnd;

	[Range(0, 1)]
	[SerializeField]
	private float fill;

	public void SetFill(float fill)
	{
		this.fill = fill;
		int width = Mathf.RoundToInt((barEnd - barStart) * fill);

		bar.transform.localScale = new Vector3(width, 1, 1);
		Vector3 pos = bar.transform.localPosition;
		pos.x = barStart + width * .5f;
		bar.transform.localPosition = pos;
	}

	public IEnumerator AnimateFill(float fill, float duration)
	{
		if (fill == this.fill)
			yield break;

		float initial = this.fill;
		float target = fill;

		float time = 0;
		while (time < duration)
		{
			yield return null;
			time += Time.deltaTime;
			SetFill(Mathf.Lerp(initial, target, time / duration));
		}
	}

	private void Update()
	{
		SetFill(fill); // update in editor
	}
}
