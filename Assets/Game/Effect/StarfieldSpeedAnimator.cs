using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarfieldSpeedAnimator : MonoBehaviour
{
    public StarfieldGenerator target;

    public float initial = 0;
    public float final = -50;
    public float duration = 2f;

    private float time;

    private void OnEnable()
    {
        time = 0;
        Apply();
    }

    private void Update()
    {
        time += Time.deltaTime;
        Apply();
    }

    private void Apply()
    {
        if (target != null)
            target.speed = Mathf.Lerp(initial, final, time / duration);
    }
}
