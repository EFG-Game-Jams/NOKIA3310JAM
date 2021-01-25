using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PixelPerfectTransform : MonoBehaviour
{
	public Vector2 SnapPosition()
	{
		Vector3 origin = transform.position;
		origin.x = Mathf.Round(origin.x);
		origin.y = Mathf.Round(origin.y);
		transform.position = origin;
		return origin;
	}

	private void Update()
	{
		SnapPosition();
	}
}
