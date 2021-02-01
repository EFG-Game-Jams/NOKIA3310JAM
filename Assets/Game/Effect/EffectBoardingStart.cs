using System.Collections;
using UnityEngine;

public class EffectBoardingStart : Effect
{
    private Vector3 origin;
    private Vector3 target;

    public EffectBoardingStart Setup(Vector3 origin, Vector3 target)
    {
        this.origin = origin;
        this.target = target;
        return this;
    }

    private IEnumerator DelayedLaunch(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return Game.Instance.effects.Create<EffectBoardingTravel>("BoardingTravel")
            .Setup(origin, target)
            .Run();
    }

    protected override IEnumerator TickUntilDone()
    {
        yield return DelayedLaunch(0.2f);
    }
}