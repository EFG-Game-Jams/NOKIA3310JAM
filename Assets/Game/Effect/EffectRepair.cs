using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectRepair : EffectTranslate
{
    public int sparkCount = 0;
    public float spawnRadius = 10f;

    private Vector3 origin;
    private Rect spawnRect;
    private float sparkDuration;

    public EffectRepair Setup(Vector3 origin, Rect spawnRect, float duration = -1)
    {
        this.origin = origin;
        this.spawnRect = spawnRect;
        sparkDuration = duration;

        return this;
    }

    protected override IEnumerator TickUntilDone()
    {
        List<IEnumerator> sparks = new List<IEnumerator>();
        for (int i = 0; i < sparkCount; ++i)
        {
            sparks.Add(DelayedSpark(Random.value));
        }

        yield return CoroutineComposer.MakeParallel(this, sparks.ToArray());
    }

    private IEnumerator DelayedSpark(float delay)
    {
        Vector3 sparkOrigin = new Vector3
        {
            x = (Random.value - 0.5f) * spawnRect.width + origin.x,
            y = (Random.value - 0.5f) * spawnRect.height + origin.y,
            z = 0
        };
        Vector3 sparkTarget = sparkOrigin + new Vector3(0, 5f + Random.value * 3f, 0);

        yield return new WaitForSeconds(delay);
        yield return Game.Instance.effects.Create<EffectSpark>("Spark").Setup(sparkOrigin, sparkTarget).Run();
    }
}
