using System.Collections;
using UnityEngine;

public class EffectTorpedo : Effect
{
    private Vector3 origin;
    private Vector3 target;

    public EffectTorpedo Setup(Vector3 origin, Vector3 target)
    {
        this.origin = origin;
        this.target = target;
        return this;
    }

    private IEnumerator DelayedLaunch(float delay, float offset)
    {
        yield return new WaitForSeconds(delay);
        yield return Game.Instance.effects.Create<EffectTorpedoLaunch>("TorpedoLaunch")
            .Setup(origin, target + new Vector3(0,offset,0))
            .Run();
    }

    protected override IEnumerator TickUntilDone()
    {
        yield return DelayedLaunch(0.2f, Random.value * 2f);
    }
}