using System.Collections;
using UnityEngine;

public class EffectBoardingStart : EffectBoarding
{    
    private VesselVisuals origin;
    private VesselVisuals target;

    public EffectBoardingStart Setup(VesselVisuals origin, VesselVisuals target)
    {
        this.origin = origin;
        this.target = target;
        return this;
    }

    protected override IEnumerator TickUntilDone()
    {
        // show shuttle
        origin.ShuttleVisible = true;
        origin.shuttle.transform.position = origin.boardingEmit.position;

        // crew to shuttle
        yield return MoveCrew(origin.transform.position, origin.boardingEmit.position);

        // shuttle to destination
        yield return new WaitForSeconds(.25f);
        yield return Game.Instance.effects.Create<EffectTranslate>("Translate")
            .Setup(origin.shuttle.transform, origin.boardingEmit.position, target.boardingReceive.position, 1)
            .Run();

        // crew to destination
        yield return new WaitForSeconds(.25f);
        yield return MoveCrew(target.boardingReceive.position, target.transform.position);
    }
}