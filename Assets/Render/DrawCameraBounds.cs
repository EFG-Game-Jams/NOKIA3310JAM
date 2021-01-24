using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCameraBounds : MonoBehaviour
{
    new public Camera camera;

    private void OnDrawGizmos()
    {
        if (camera == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(camera.aspect * camera.orthographicSize * 2, camera.orthographicSize * 2, 0));
    }
}
