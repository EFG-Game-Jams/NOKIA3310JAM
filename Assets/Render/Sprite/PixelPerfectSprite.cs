using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class PixelPerfectSprite : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public Vector2 SnapPosition()
	{
		Vector3 halfSize = spriteRenderer.bounds.size * .5f;
		Vector3 origin = transform.position - halfSize;
		origin.x = Mathf.Round(origin.x);
		origin.y = Mathf.Round(origin.y);
		transform.position = origin + halfSize;
		return origin;
	}

	// who needs performance anyway?
	private void LateUpdate()
	{
		SnapPosition();
	}
}
