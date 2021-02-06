using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScan : Effect
{
    public PixelPerfectSprite waveSprite;
        
    private Vector3 origin;
    private Vector3 target;

    public EffectScan Setup(Vector3 origin, Vector3 target)
    {
        this.origin = origin;
        this.target = target;
        return this;
    }
    
    private IEnumerator DelayedWave(float delay, int offset)
    {
        yield return new WaitForSeconds(delay);

        var translate = Game.Instance.effects.Create<EffectTranslate>("Translate");
        var wave = Instantiate(waveSprite, translate.transform);

        // reverse
        if (origin.x < target.x)
            wave.transform.localScale = new Vector3(-1, 1, 1);

        yield return translate.Setup(wave.transform, origin, target + new Vector3(0,offset,0), .35f).Run();
    }

    protected override IEnumerator TickUntilDone()
    {
        for (int i = 0; i < 3; ++i)
        {
            Game.Instance.audioManager.Play("scan");
            yield return CoroutineComposer.MakeParallel(this,
                DelayedWave(0, 8),
                DelayedWave(.05f, 0),
                DelayedWave(.1f, -8)
            );
        }
    }
}
