using System.Collections;
using UnityEngine;

public abstract class EffectBoarding : Effect
{    
    protected IEnumerator MoveCrew(Vector3 from, Vector3 to)
    {
        var effects = Game.Instance.effects;
        yield return CoroutineComposer.MakeParallel(
            this,
            CoroutineComposer.MakeDelayed(0, effects.Create<EffectSpark>("Spark").Setup(from, to, 1).Run()),
            CoroutineComposer.MakeDelayed(0.25f, effects.Create<EffectSpark>("Spark").Setup(from, to, 1).Run()),
            CoroutineComposer.MakeDelayed(0.5f, effects.Create<EffectSpark>("Spark").Setup(from, to, 1).Run())
        );
    }
}