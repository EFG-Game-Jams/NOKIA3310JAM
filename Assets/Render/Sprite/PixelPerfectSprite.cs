using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class PixelPerfectSprite : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	public int Width => Mathf.RoundToInt(GetRenderer().sprite.rect.width);
	public int Height => Mathf.RoundToInt(GetRenderer().sprite.rect.height);

	public SpriteRenderer GetRenderer()
	{
		if (spriteRenderer == null)
			spriteRenderer = GetComponent<SpriteRenderer>();
		return spriteRenderer;
	}

	public Vector2 SnapPosition()
	{
		Vector3 halfSize = GetRenderer().bounds.size * .5f;
		Vector3 origin = transform.localPosition - halfSize;
		origin.x = Mathf.Round(origin.x);
		origin.y = Mathf.Round(origin.y);
		transform.localPosition = origin + halfSize;
		return origin;
	}

	public void SnapSize()
	{
		if (GetRenderer().drawMode == SpriteDrawMode.Sliced)
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
