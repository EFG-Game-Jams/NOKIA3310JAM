using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTranslate : Effect
{
    private Transform target;
    private Vector3 from;
    private Vector3 to;
    private float duration;

    public EffectTranslate Setup(Transform target, Vector3 from, Vector3 to, float duration)
    {
        this.target = target;
        this.from = from;
        this.to = to;
        this.duration = duration;
        UpdateTransform(0);
        return this;
    }

    private void UpdateTransform(float mu)
    {
        target.position = Vector3.Lerp(from, to, mu);
    }

    protected override bool OnUpdate(float time)
    {
        float mu = time / duration;
        UpdateTransform(mu);
        return mu < 1f;
    }
}
