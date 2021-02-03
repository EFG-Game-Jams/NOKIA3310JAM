using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpark : EffectTranslate
{
    public float sparkDuration;
    public PixelPerfectSprite sparkSprite;

    private PixelPerfectSprite spark;

    public EffectSpark Setup(Vector3 origin, Vector3 target, float duration = -1)
    {
        spark = Instantiate(sparkSprite, transform);

        if (duration < 0)
            duration = this.sparkDuration;

        base.Setup(spark.transform, origin, target, duration);

        return this;
    }
}
