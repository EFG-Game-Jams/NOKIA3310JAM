using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VesselAbilityDelegated : VesselAbility
{
	public Func<bool> onCanTrigger;
	public Action onActivate;
	public Action onDeactivate;
	public Action onTick;

	public override bool CanTrigger => base.CanTrigger && (onCanTrigger?.Invoke() ?? true);
	protected override void OnActivate() => onActivate?.Invoke();
	protected override void OnDeactivate() => onDeactivate?.Invoke();
	protected override void OnTick() => onTick?.Invoke();

	public VesselAbilityDelegated(int duration, int cooldown, Func<bool> onCanTrigger, Action onActivate, Action onDeactivate, Action onTick)
		: base(duration, cooldown)
	{
		this.onCanTrigger = onCanTrigger;
		this.onActivate = onActivate;
		this.onDeactivate = onDeactivate;
		this.onTick = onTick;
	}
}
