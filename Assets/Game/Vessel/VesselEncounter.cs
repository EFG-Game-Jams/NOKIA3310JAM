using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// represents an encounter from the perspective of one of the participants
public class VesselEncounter
{
    // internal
    private string name = "vessel";
    private Encounter owner;
    private GameBalance balance;
    public VesselModifiers modifiers { get; private set; }
    private VesselVisuals visuals;
    private VesselEncounter opponent;
    private bool abilityUsedThisTurn;
    private Dictionary<string, VesselAbility> abilities;

    // self
    /// components
    public VesselStats Stats { get; private set; }
    public VesselStatus Status { get; private set; }
    /// abilities
    //// attack
    public VesselAbility AbilityLaser { get; private set; }
    public VesselAbility AbilityTorpedo { get; private set; }
    public VesselAbility AbilityBoard { get; private set; }
    //// defense
    public VesselAbility AbilityShields { get; private set; }
    public VesselAbility AbilityEvade { get; private set; }
    public VesselAbility AbilityRepel { get; private set; }
    public VesselAbility AbilityFlee { get; private set; }
    //// utility
    public VesselAbility AbilityRepair { get; private set; }
    public VesselAbility AbilityScan { get; private set; }
    public VesselAbility AbilityRespec { get; private set; }
    public VesselAbility AbilitySkipTurn { get; private set; }

    // opponent
    public OpponentVesselInformation GetOpponentInformation() => new OpponentVesselInformation(AbilityScan?.IsActive == true, opponent);

    public VesselStats OpponentStats => (AbilityScan.IsActive ? opponent.Stats : null);
    public VesselStatus OpponentStatus => (AbilityScan.IsActive ? opponent.Status : null);

    // construct and configure
    public VesselEncounter(
        string name,
        Encounter owner,
        GameBalance balance,
        VesselVisuals visuals,
        VesselStats stats,
        VesselStatus status,
        VesselModifiers modifiers)
    {
        this.name = name;
        this.owner = owner;
        this.balance = balance;
        this.modifiers = modifiers;
        this.visuals = visuals;

        Stats = stats;
        Status = status;

        visuals.ResetVisibility();

        abilities = new Dictionary<string, VesselAbility>();
        InitialiseAbilities();
    }
    public static void SetOpponents(VesselEncounter a, VesselEncounter b)
    {
        a.opponent = b;
        b.opponent = a;
    }

    // find an ability by name, or null
    public VesselAbility GetAbility(string name)
    {
        abilities.TryGetValue(name, out VesselAbility ability);
        return ability;
    }

    // begin turn
    public void BeginTurn()
    {
        foreach (var pair in abilities)
            pair.Value.Tick();
        abilityUsedThisTurn = false;
    }

    // ability setup
    private void InitialiseAbilities()
    {
        AbilityLaser = new VesselAbilityDelegated(
            0, 0,
            () => "Fire lasers\nHits shields\nCannot evade\nSystem: " + Status.weapons + "%",
            () => (Status.weapons > 0 && modifiers.HasWeapons),
            OnActivateLaser,
            null,
            null
        );
        abilities["laser"] = AbilityLaser;

        AbilityTorpedo = new VesselAbilityDelegated(
            0, 0,
            () => "Fire torpedo\nBypass shields\nSystem: " + Status.weapons + "%\nAmmo: " + Status.ammo,
            () => (Status.weapons > 0 && modifiers.HasWeapons && modifiers.CanMissle && Status.ammo > 0),
            OnActivateTorpedo,
            null,
            null
        );
        abilities["torpedo"] = AbilityTorpedo;

        AbilityBoard = new VesselAbilityDelegated(
            int.MaxValue, 0,
            () => "Board enemy\nBypass shields\nDamage enemy\nover time",
            () => modifiers.CanBoard,
            OnActivateBoarding,
            OnDeactivateBoarding,
            OnTickBoarding
        );
        abilities["board"] = AbilityBoard;

        AbilityShields = new VesselAbilityDelegated(
            int.MaxValue, 1,
            () => "Raise shields\nActive " + AbilityShields.Duration + " turns\nAbsorbs 1 hit\nSystem: " + Status.shields + "%",
            () => (Status.shields > 0 && modifiers.HasShields),
            OnActivateShields,
            () => owner.EnqueueAnimation(AnimateShield(this, false)),
            null
        );
        abilities["shields"] = AbilityShields;

        AbilityRepel = new VesselAbilityDelegated(
            0, 0,
            () => "Repel boarders",
            () => (modifiers.CanRepel && opponent.AbilityBoard.IsActive),
            OnActivateRepel,
            null,
            null
        );
        abilities["repel"] = AbilityRepel;

        AbilitySkipTurn = new VesselAbilityDelegated(
            0, 0,
            () => "Skip turn\nAI only!",
            () => true,
            OnSkipTurn,
            null,
            null);
        abilities["skipturn"] = AbilitySkipTurn;
    }

    // helpers
    private int GetSystemDependentRollEffect(float roll, int systemStatus, int effectMin, int effectMax, bool amplifiedByLuck)
    {
        // roll [0, 1] clamped to system status [0, 1] transposed to range [min, max]
        float rawEffect = Mathf.Min(roll, systemStatus / (float)VesselStatus.MaxSystemStatus);
        float scaledEffect = Mathf.Lerp(effectMin, effectMax, rawEffect);

        if (amplifiedByLuck)
        {
            scaledEffect *= 1f + Stats.RollLuck();
        }

        return Mathf.RoundToInt(scaledEffect);
    }
    private void FinishTurn()
    {
        if (abilityUsedThisTurn)
            throw new System.Exception("Multiple abilities used in a single turn");
        abilityUsedThisTurn = true;

        if (AbilityShields.IsActive && Status.shields <= 0)
            AbilityShields.Deactivate();
        if (opponent.AbilityShields.IsActive && opponent.Status.shields <= 0)
            opponent.AbilityShields.Deactivate();

        Debug.Assert(AbilityEvade == null, "Deactivate evasion when engines are dead");
        /*if (AbilityEvade.IsActive && Status.engines <= 0)
            AbilityEvade.Deactivate();
        if (opponent.AbilityEvade.IsActive && opponent.Status.engines <= 0)
            opponent.AbilityEvade.Deactivate();*/

        owner.OnVesselEndTurn(this);
    }

    // ability callbacks
    private void OnActivateLaser()
    {
        Vector3 laserEmit = visuals.laserEmit.position;
        Vector3 laserReceive = (opponent.AbilityShields.IsActive ? opponent.visuals.weaponReceiveShield : opponent.visuals.weaponReceiveHull).position;
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectLaser>("Laser").Setup(laserEmit, laserReceive).Run());

        int damage = GetSystemDependentRollEffect(
            Stats.RollAttack(),
            Status.weapons,
            balance.laserDamageMin,
            balance.laserDamageMax,
            true);
        Debug.LogFormat("{0} firing laser with {1} damage", name, damage);

        if (opponent.AbilityShields.IsActive)
        {
            opponent.Status.ApplyShieldDamage(damage, true, true);
            if (opponent.Stats.RollDefense() < balance.shieldsStayActiveOnDamageRoll)
                opponent.AbilityShields.Deactivate();
        }
        else
        {
            opponent.Status.ApplyHullDamage(damage, true);
        }

        FinishTurn();
    }

    private void OnActivateTorpedo()
    {
        Vector3 torpedoEmit = visuals.torpedoEmit.position;
        Vector3 torpedoReceive = opponent.visuals.weaponReceiveHull.position;
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectTorpedo>("Torpedo").Setup(torpedoEmit, torpedoReceive).Run());

        int damage = GetSystemDependentRollEffect(
            Stats.RollAttack(),
            Status.weapons,
            balance.torpedoDamageMin,
            balance.torpedoDamageMax,
            true);

        Debug.LogFormat("{0} firing torpedo with {1} damage", name, damage);

        // damage applied to health
        opponent.Status.ApplyHullDamage(damage, true);

        // consume ammo
        --Status.ammo;

        FinishTurn();
    }

    private void OnActivateBoarding()
    {
        Debug.LogFormat("Sending boarding party");
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectBoardingStart>("BoardingStart").Setup(visuals, opponent.visuals).Run());

        FinishTurn();
    }
    private void OnTickBoarding()
    {
        Debug.LogFormat("Executing boarding party tick");
        int damage = GetSystemDependentRollEffect(
            Stats.RollAttack(),
            Status.weapons,
            balance.boardingDamageMin,
            balance.boardingDamageMax,
            true);

        // damage applied to health
        opponent.Status.ApplySystemsDamage(damage);
        Debug.LogFormat("> hull took {0}", damage);
    }
    private void OnDeactivateBoarding()
    {
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectBoardingEnd>("BoardingEnd").Setup(visuals, opponent.visuals).Run());
    }

    private void OnActivateShields()
    {
        Debug.LogFormat("Raising shields");
        owner.EnqueueAnimation(AnimateShield(this, true));
        FinishTurn();
    }

    // animations
    private static IEnumerator AnimateShield(VesselEncounter vessel, bool state)
    {
        float interval = .8f;
        for (int i = 0; i < 8; ++i)
        {
            yield return new WaitForSeconds(interval);
            vessel.visuals.ShieldVisible = state;
            Game.Instance.audioManager.Play("shield");
            yield return new WaitForSeconds(.05f);
            vessel.visuals.ShieldVisible = !state;
            interval *= .5f;
        }
        Game.Instance.audioManager.Play("shield");
        vessel.visuals.ShieldVisible = state;
    }

    private void OnActivateRepel()
    {
        Debug.LogFormat("Repelling boarders");
        Debug.Assert(opponent.AbilityBoard.IsActive);
        opponent.AbilityBoard.Deactivate();

        FinishTurn();
    }

    private void OnSkipTurn()
    {
        Debug.LogFormat("Skipping turn");
        FinishTurn();
    }
}
