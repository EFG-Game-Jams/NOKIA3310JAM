using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagePostEncounter : PageAutoNavigation
{
    public void OnContinue()
    {
        pageManager.PopPage();
        Game.Instance.campaign.OnPostEncounterComplete();
    }
}
