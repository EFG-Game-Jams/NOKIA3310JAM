using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class VesselAbility
{
	public int Duration { get; protected set; }
	public int Cooldown { get; protected set; }

	public int ActiveTurnsRemaining { get; protected set; }
	public int CooldownTurnsRemaining { get; protected set; }

	public bool IsActive => (ActiveTurnsRemaining > 0);
	public virtual bool CanTrigger => (!IsActive && (CooldownTurnsRemaining == 0));

	protected abstract void OnActivate();
	protected abstract void OnTick();
	protected abstract void OnDeactivate();

	protected VesselAbility(int duration, int cooldown)
	{
		Duration = duration;
		Cooldown = cooldown;

		ActiveTurnsRemaining = 0;
		CooldownTurnsRemaining = 0;
	}

	public bool TryTrigger()
	{
		if (!CanTrigger)
			return false;
		ActiveTurnsRemaining = Duration;
		OnActivate();
		return true;
	}
	public void Tick()
	{
		if (IsActive)
		{
			OnTick();
			--ActiveTurnsRemaining;
			if (ActiveTurnsRemaining == 0)
			{
				CooldownTurnsRemaining = Cooldown;
				OnDeactivate();
			}
		}
		else if (CooldownTurnsRemaining > 0)
		{
			--CooldownTurnsRemaining;
		}
	}
	
	public void Deactivate()
	{
		ActiveTurnsRemaining = 0;
		CooldownTurnsRemaining = Cooldown + 1; // cooldown is decremented at start of turn, so cooldown turns need to be offset by 1 if we're deactivating manually
		OnDeactivate();
	}
}
