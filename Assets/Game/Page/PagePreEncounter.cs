using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagePreEncounter : PageAutoNavigation
{
    public NokiaTextRenderer itemAvoid;

    public override void OnActivate()
    {
        base.OnActivate();
        itemAvoid.Text = "Avoid (1/" + Game.Instance.campaign.playerStatus.fuel + " fuel)";
    }

    public void OnInvestigate()
    {
        SoundSuccess();
        pageManager.PopPage();
        Game.Instance.campaign.OnPreEncounterCompleteIvestigate();
    }
    public void OnIgnore()
    {
        SoundSuccess();
        pageManager.PopPage();
        Game.Instance.campaign.OnPreEncounterCompleteIgnore();
    }
    public void OnAvoid()
    {
        var status = Game.Instance.campaign.playerStatus;
        if (status.fuel > 0)
        {
            SoundSuccess();
            pageManager.PopPage();
            Game.Instance.campaign.OnPreEncounterCompleteAvoid();
        }
        else
        {
            SoundFailure();
        }
    }

    private void SoundFailure() => Game.Instance.audioManager.Play("success");
    private void SoundSuccess() => Game.Instance.audioManager.Play("failure");
}
