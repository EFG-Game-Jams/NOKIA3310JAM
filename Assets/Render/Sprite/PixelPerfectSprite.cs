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
		Vector3 origin = transform.localPosition - halfSize;
		origin.x = Mathf.Round(origin.x);
		origin.y = Mathf.Round(origin.y);
		transform.localPosition = origin + halfSize;
		return origin;
	}

	public void SnapSize()
	{
		if (spriteRenderer.drawMode == SpriteDrawMode.Sliced)
		{
			Vector2 size = spriteRenderer.size;
			size.x = Mathf.Round(size.x);
			size.y = Mathf.Round(size.y);
			spriteRenderer.size = size;
		}
	}

	// who needs performance anyway?
	private void LateUpdate()
	{
		SnapSize();
		SnapPosition();
	}
}
