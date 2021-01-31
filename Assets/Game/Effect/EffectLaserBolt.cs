using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLaserBolt : Effect
{
    public float boltDuration;
    [SerializeField] private PixelPerfectSprite boltSprite;

    public int sparkCount;
    public float sparkRange;

    private Vector3 boltOrigin;
    private Vector3 boltTarget;
    private Vector3 sparksOrigin;

    private PixelPerfectSprite bolt;

    public EffectLaserBolt Setup(Vector3 origin, Vector3 target)
    {
        float dx = target.x - origin.x;
        float rx = boltSprite.Width * .5f;
        Vector3 offset = Vector3.right * (Mathf.Sign(dx) * rx);

        boltOrigin = origin + offset;
        boltTarget = target - offset;
        sparksOrigin = target;

        bolt = Instantiate(boltSprite, transform);
        return this;
    }

    private void UpdateBolt(float mu)
    {
        bolt.transform.position = Vector3.Lerp(boltOrigin, boltTarget, mu);
    }

    protected override bool OnUpdate(float time)
    {
        float mu = time / boltDuration;
        UpdateBolt(mu);
        return mu < 1f;
    }

    protected override IEnumerator TickUntilDone()
    {
        yield return base.TickUntilDone();
        Destroy(bolt.gameObject);

        IEnumerator[] sparks = new IEnumerator[sparkCount];
        for (int i = 0; i < sparkCount; ++i)
        {
            Vector3 movement = Vector3.Scale(Random.onUnitSphere, new Vector3(1,1,0)).normalized * sparkRange;
            Vector3 origin = sparksOrigin;
            Vector3 target = sparksOrigin + movement;
            sparks[i] = Game.Instance.effects.Create<EffectSpark>("Spark").Setup(origin, target).Run();
        }

        yield return CoroutineComposer.MakeParallel(this, sparks);
    }
}
