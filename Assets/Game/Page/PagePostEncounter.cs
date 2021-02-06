using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagePostEncounter : PageAutoNavigation
{
    public NavigatableItemAuto itemContinue;
    public NavigatableItemAuto itemStats;
    public NavigatableItemAuto itemFuel;
    public NavigatableItemAuto itemAmmo;
    public NavigatableItemAuto itemRepair;

    public NokiaTextRenderer textTitle;
    public NokiaTextRenderer textFuel;
    public NokiaTextRenderer textAmmo;

    private int statPoints;
    private int fuel;
    private int ammo;
    private bool repair;

    public override void OnPush()
    {
        // deactivate all items
        foreach (var nav in GetComponentsInChildren<NavigatableItemAuto>())
            nav.gameObject.SetActive(false);

        if (statPoints == 0 && fuel == 0 && ammo == 0 && !repair)
        {
            // no choices, continue only
            defaultNavItem = itemContinue;
            itemContinue.gameObject.SetActive(true);
        }
        else
        {
            // we always have a stat point
            Debug.Assert(statPoints > 0);
            defaultNavItem = itemStats;
            itemStats.gameObject.SetActive(true);

            // and maybe other choices
            itemFuel.gameObject.SetActive(fuel > 0);
            textFuel.Text = "Salvage " + fuel + " fuel";
            itemAmmo.gameObject.SetActive(ammo > 0);
            textAmmo.Text = "Salvage " + ammo + " ammo";
            itemRepair.gameObject.SetActive(repair);
        }

        // rebuild navigation
        foreach (var nav in GetComponentsInChildren<NavigatableItemAuto>())
            nav.RebuildNavigation();

        // activate auto navigation
        base.OnPush();
    }

    private void OnSelection(bool pop = true)
    {
        Game.Instance.audioManager.Play("success");
        if (pop)
            pageManager.PopPage();
    }

    public void OnContinue()
    {
        OnSelection();
        Game.Instance.campaign.OnPostEncounterComplete();
    }
    public void OnStats()
    {
        OnSelection(false);
        var page = pageManager.GetPage<PageLevelUp>();
        page.onPointSpent = () =>
        {
            Debug.Assert(pageManager.GetActivePage() == this);
            pageManager.PopPage();
            Game.Instance.campaign.OnPostEncounterComplete();
        };
        pageManager.PushPage(page);
    }
    public void OnFuel()
    {
        OnSelection();
        Game.Instance.campaign.playerStatus.fuel += fuel;
        Game.Instance.campaign.OnPostEncounterComplete();
    }
    public void OnAmmo()
    {
        OnSelection();
        Game.Instance.campaign.playerStatus.ammo += ammo;
        Game.Instance.campaign.OnPostEncounterComplete();
    }
    public void OnRepair()
    {
        OnSelection();
        Game.Instance.campaign.playerStatus.RepairFull();
        Game.Instance.campaign.OnPostEncounterComplete();
    }

    public void Configure(string title, int statPoints, int fuel, int ammo, bool repair)
    {
        textTitle.Text = title;
        this.statPoints = statPoints;
        this.fuel = fuel;
        this.ammo = ammo;
        this.repair = repair;
    }
}
