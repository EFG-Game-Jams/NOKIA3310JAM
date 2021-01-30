using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTrigger : NavigatableItemAuto
{
    public NokiaTextRenderer display;

    public string abilityName;
    public string displayName;

    private void Awake()
    {
        onConfirm.AddListener(OnConfirm);
        onCancel.AddListener(OnCancel);
    }

    private void OnConfirm()
    {
        var encounter = Game.Instance.campaign.encounter.playerEncounter;
        VesselAbility ability = encounter.GetAbility(abilityName);

        if (ability.TryTrigger())
        {
            Debug.Log("todo: success sound");

            var pm = Game.Instance.pageManager;
            //while (!(pm.GetActivePage() is PageEncounter))
                pm.PopPage();
        }
        else
        {
            Debug.Log("todo: failure sound");
        }
    }
    private void OnCancel()
    {
        Game.Instance.pageManager.PopPage();
    }

    private void Update()
    {
        var encounter = Game.Instance.campaign.encounter.playerEncounter;
        VesselAbility ability = encounter.GetAbility(abilityName);

        bool flashState = (Time.time % 1f) <= .5f;

        if (ability == null)
            display.Text = (flashState ? "!!! null !!!" : displayName);
        else if (ability.IsActive)
            display.Text = (flashState ? "[active]" : displayName);
        else if (ability.CooldownTurnsRemaining > 0)
            display.Text = (flashState ? "[cooldown-" + ability.CooldownTurnsRemaining + "]" : displayName);
        else if (!ability.CanTrigger)
            display.Text = (flashState ? "[disabled]" : displayName);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (display != null)
        {
            display.initialText = displayName;
            UnityEditor.EditorUtility.SetDirty(display);
        }
    }
#endif
}
