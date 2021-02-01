using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashRenderer : MonoBehaviour
{
    public Renderer target;
    const float onTime = .5f;
    const float offTime = .5f;

    private float time;
    
    private void Toggle()
    {
        target.enabled = !target.enabled;
        time = 0;
    }

    void Update()
    {
        time += Time.deltaTime;
        bool state = target.enabled;
        if (time >= (state ? onTime : offTime))
            Toggle();
    }
}
