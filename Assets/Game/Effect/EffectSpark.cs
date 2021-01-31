using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpark : Effect
{
    public float duration;
    public PixelPerfectSprite sparkSprite;
    
    private Vector3 origin;
    private Vector3 target;

    private PixelPerfectSprite spark;

    public EffectSpark Setup(Vector3 origin, Vector3 target)
    {
        this.origin = origin;
        this.target = target;
        spark = Instantiate(sparkSprite, transform);
        return this;
    }

    private void UpdateSpark(float mu)
    {
        spark.transform.position = Vector3.Lerp(origin, target, mu);
    }

    protected override bool OnUpdate(float time)
    {
        float mu = time / duration;
        UpdateSpark(mu);
        return mu < 1f;
    }
}
