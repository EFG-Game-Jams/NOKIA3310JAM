using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagePreEncounter : PageAutoNavigation
{
    public void OnContinue()
    {
        pageManager.PopPage();
        Game.Instance.campaign.OnPreEncounterComplete();
    }
}
