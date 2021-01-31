using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLaser : Effect
{
    private Vector3 origin;
    private Vector3 target;

    public EffectLaser Setup(Vector3 origin, Vector3 target)
    {
        this.origin = origin;
        this.target = target;
        return this;
    }
    
    private IEnumerator DelayedBolt(float delay, int offset)
    {
        yield return new WaitForSeconds(delay);
        yield return Game.Instance.effects.Create<EffectLaserBolt>("LaserBolt")
            .Setup(origin, target + new Vector3(0,offset,0))
            .Run();
    }

    protected override IEnumerator TickUntilDone()
    {
        yield return CoroutineComposer.MakeParallel(this,
            DelayedBolt(0, 1),
            DelayedBolt(.1f, -2),
            DelayedBolt(.2f, 2),
            DelayedBolt(.3f, -1),
            DelayedBolt(.4f, 0)
        );
    }
}
