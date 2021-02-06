using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigatableItemAutoPushPop : NavigatableItemAuto
{
    public string targetPage;

    private void Awake()
    {
        triggerPrevious = GameInput.Action.Up;
        triggerNext = GameInput.Action.Down;
        onConfirm = MakeEvent(PushPage);
        onCancel = MakeEvent(PopPage);
    }

    protected override void ConfigureNavigation(List<Navigation> navigation)
    {
        base.ConfigureNavigation(navigation);
        navigation.Add(new Navigation { trigger = GameInput.Action.Left, navAction = MakeEvent(PopPage) });
        navigation.Add(new Navigation { trigger = GameInput.Action.Right, navAction = MakeEvent(PushPage) });
    }

    private void PushPage()
    {
        if (targetPage == null || targetPage.Length == 0)
            return;
        //Game.Instance.audioManager.Play("success");
        Game.Instance.pageManager.PushPage(targetPage);
    }
    private void PopPage()
    {
        //Game.Instance.audioManager.Play("failure");
        Game.Instance.pageManager.PopPage();
    }

    private UnityEvent MakeEvent(UnityAction target)
    {
        UnityEvent e = new UnityEvent();
        e.AddListener(target);
        return e;
    }
}
