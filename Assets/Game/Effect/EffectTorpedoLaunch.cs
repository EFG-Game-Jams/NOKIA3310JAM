using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTorpedoLaunch : Effect
{
    public float travelDuration;
    [SerializeField] private PixelPerfectSprite torpedoSprite;

    public int sparkCount;
    public float sparkRange;

    private Vector3 torpedoOrigin;
    private Vector3 torpedoTarget;

    private PixelPerfectSprite torpedo;

    public EffectTorpedoLaunch Setup(Vector3 origin, Vector3 target)
    {
        float dx = target.x - origin.x;
        float rx = torpedoSprite.Width * .5f;
        Vector3 offset = Vector3.right * (Mathf.Sign(dx) * rx);

        torpedoOrigin = origin + offset;
        torpedoTarget = target - offset;

        torpedo = Instantiate(torpedoSprite, transform);
        if (torpedoOrigin.x < torpedoTarget.x) torpedo.GetRenderer().flipX = true;

        Game.Instance.audioManager.Play("laser"); // TODO Pick a better sound
        return this;
    }

    protected override bool OnUpdate(float time)
    {
        float mu = time / travelDuration;
        UpdateTorpedo(mu);
        return mu < 1f;
    }

    private void UpdateTorpedo(float mu)
    {
        torpedo.transform.position = Vector3.Lerp(torpedoOrigin, torpedoTarget, mu);
    }

    protected override IEnumerator TickUntilDone()
    {
        yield return base.TickUntilDone();
        Destroy(torpedo.gameObject);

        IEnumerator[] sparks = new IEnumerator[sparkCount];
        for (int i = 0; i < sparkCount; ++i)
        {
            Vector3 movement = (Vector3)Random.insideUnitCircle * sparkRange;
            Vector3 origin = torpedoTarget;
            Vector3 target = torpedoTarget + movement;
            sparks[i] = Game.Instance.effects.Create<EffectSpark>("Spark").Setup(origin, target, .5f).Run();
        }

        Game.Instance.audioManager.Play("torpedo_impact");
        yield return CoroutineComposer.MakeParallel(this, sparks);
    }
}
