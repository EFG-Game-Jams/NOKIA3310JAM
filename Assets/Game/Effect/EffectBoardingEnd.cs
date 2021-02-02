using System.Collections;
using UnityEngine;

public class EffectBoardingEnd : EffectBoarding
{    
    private VesselVisuals origin;
    private VesselVisuals target;

    public EffectBoardingEnd Setup(VesselVisuals origin, VesselVisuals target)
    {
        this.origin = origin;
        this.target = target;
        return this;
    }

    protected override IEnumerator TickUntilDone()
    {
        // crew to shuttle
        yield return MoveCrew(target.transform.position, target.boardingReceive.position);

        // shuttle to home
        origin.shuttle.flipX = true;
        yield return new WaitForSeconds(.25f);
        yield return Game.Instance.effects.Create<EffectTranslate>("Translate")
            .Setup(origin.shuttle.transform, target.boardingReceive.position, origin.boardingEmit.position, 1)
            .Run();

        // crew to destination
        yield return new WaitForSeconds(.25f);
        yield return MoveCrew(origin.boardingEmit.position, origin.transform.position);

        // hide shuttle
        yield return new WaitForSeconds(.25f);
        origin.shuttle.flipX = false;
        origin.ShuttleVisible = false;
    }
}