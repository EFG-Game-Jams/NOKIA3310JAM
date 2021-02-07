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
            () => "Raise shields\nAbsorbs lasers\nSystem: " + Status.shields + "%",
            () => (Status.shields > 0 && modifiers.HasShields),
            OnActivateShields,
            () => owner.EnqueueAnimation(AnimateShield(this, false)),
            null
        );
        abilities["shields"] = AbilityShields;

        AbilityEvade = new VesselAbilityDelegated(
            int.MaxValue, 1,
            () => "Evade\nAttempt to\nevade a\ntorpedo\nSystem: " + Status.engines + "%",
            () => (Status.engines > 0 && modifiers.HasEngines),
            OnActivateEvade,
            OnDeactivateEvade,
            null
        );
        abilities["evade"] = AbilityEvade;

        AbilityRepel = new VesselAbilityDelegated(
            0, 0,
            () => "Repel boarders",
            () => (modifiers.CanRepel && opponent.AbilityBoard.IsActive),
            OnActivateRepel,
            null,
            null
        );
        abilities["repel"] = AbilityRepel;

        AbilityScan = new VesselAbilityDelegated(
            int.MaxValue, 0,
            () => "Scan enemy",
            () => modifiers.CanScan,
            OnActivateScan,
            null,
            null);
        abilities["scan"] = AbilityScan;

        AbilitySkipTurn = new VesselAbilityDelegated(
            0, 0,
            () => "Skip turn\nAI only!",
            () => true,
            OnSkipTurn,
            null,
            null);
        abilities["skipturn"] = AbilitySkipTurn;

        AbilityRepair = new VesselAbilityDelegated(
            1, 1,
            () => "Repair hull\nand systems",
            () => Status.CanRepair && modifiers.CanRepair,
            OnActivateRepair,
            null,
            null);
        abilities["repair"] = AbilityRepair;

        AbilityFlee = new VesselAbilityDelegated(
            1, 1,
            () => "Flee\nEscape the\nencounter",
            () => Status.CanFlee && modifiers.HasEngines,
            OnFlee,
            null,
            null);
        abilities["flee"] = AbilityFlee;
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

        if (AbilityEvade.IsActive && Status.engines <= 0)
            AbilityEvade.Deactivate();
        if (opponent.AbilityEvade.IsActive && opponent.Status.engines <= 0)
            opponent.AbilityEvade.Deactivate();

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
        var torpedoAnimation = Game.Instance.effects.Create<EffectTorpedo>("Torpedo").Setup(torpedoEmit, torpedoReceive).Run();

        bool evaded = false;
        if (opponent.AbilityEvade.IsActive)
        {
            float attackerRoll = Stats.RollAttack();
            float defenderRoll = opponent.Stats.RollDefense();
            float defenderBonus = opponent.Status.engines / (float)VesselStatus.MaxSystemStatus;
            evaded = (attackerRoll <= (defenderRoll + defenderBonus));
        }

        if (evaded)
        {
            owner.EnqueueAnimation(CoroutineComposer.MakeParallel(
                owner,
                torpedoAnimation,
                Game.Instance.effects.Create<EffectEvade>("Evade").Setup(opponent.visuals).Run()
            ));

            Debug.LogFormat("{0} firing evaded torpedo", name);
        }
        else
        {
            owner.EnqueueAnimation(torpedoAnimation);

            int damage = GetSystemDependentRollEffect(
                Stats.RollAttack(),
                Status.weapons,
                balance.torpedoDamageMin,
                balance.torpedoDamageMax,
                true);

            Debug.LogFormat("{0} firing torpedo with {1} damage", name, damage);

            // damage applied to health
            opponent.Status.ApplyHullDamage(damage, true);
        }

        // whether we evaded or not, evade ability deactivates
        if (opponent.AbilityEvade.IsActive)
            opponent.AbilityEvade.Deactivate();

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
        if (damage > 0)
        {
            opponent.Status.ApplySystemsDamage(damage);
            owner.EnqueueAnimation(AnimateBoardingTick());
        }
    }

    private IEnumerator AnimateBoardingTick()
    {
        visuals.ShuttleVisible = false;
        yield return new WaitForSeconds(.3f);
        Game.Instance.audioManager.Play("boardinghit");
        visuals.ShuttleVisible = true;
        yield return new WaitForSeconds(.3f);
        visuals.ShuttleVisible = false;
        yield return new WaitForSeconds(.3f);
        visuals.ShuttleVisible = true;
        yield return new WaitForSeconds(.5f);
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
    private void OnActivateScan()
    {
        Debug.LogFormat("Scanning");
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectScan>("Scan").Setup(visuals.transform.position, opponent.visuals.transform.position).Run());
        FinishTurn();
    }
    private void OnActivateRepair()
    {
        Debug.LogFormat("Repairing");
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectRepair>("Repair").Setup(visuals.transform.position, visuals.hull.sprite.rect).Run());
        Status.Repair();
        FinishTurn();
    }
    private void OnActivateEvade()
    {
        Debug.LogFormat("Evading");
        owner.EnqueueAnimation(CoroutineComposer.MakeDelayed(.25f, CoroutineComposer.MakeAction(() => visuals.TrailVisible = true)));
        FinishTurn();
    }
    private void OnDeactivateEvade()
    {
        owner.EnqueueAnimation(CoroutineComposer.MakeDelayed(.25f, CoroutineComposer.MakeAction(() => visuals.TrailVisible = false)));
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

    private void OnFlee()
    {
        float selfDefenseComponent = Stats.RollDefense();
        float opponentAttackComponent = opponent.Stats.RollAttack();
        bool success =
            !opponent.modifiers.HasEngines ||
            ((selfDefenseComponent + Status.enginesPercentage) - (opponent.Status.enginesPercentage + opponentAttackComponent)) > 0f;
        owner.EnqueueAnimation(AnimateFlee(success));

        FinishTurn();
    }

    private IEnumerator AnimateFlee(bool isSuccessful)
    {
        visuals.hull.flipX = !visuals.hull.flipX;

        Vector3 fleeOrigin = visuals.hull.transform.position;
        if (isSuccessful)
        {
            visuals.ShieldVisible = false;
            Vector3 fleeDestination = new Vector3
            {
                x = fleeOrigin.x > 41 ? 83 + visuals.hull.sprite.rect.width : -visuals.hull.sprite.rect.width,
                y = fleeOrigin.y + 5,
                z = 0
            };

            yield return Game.Instance.effects.Create<EffectTranslate>("Translate").Setup(
                visuals.hull.transform,
                fleeOrigin,
                fleeDestination,
                1.5f).Run();

            visuals.HullVisible = false;
            visuals.hull.transform.position = fleeOrigin;
        }
        else
        {
            Vector3 fleeDestination = new Vector3
            {
                x = fleeOrigin.x > 41 ? fleeOrigin.x + 5 : fleeOrigin.x - 5,
                y = fleeOrigin.y + 1,
                z = 0
            };

            yield return Game.Instance.effects.Create<EffectTranslate>("Translate").Setup(
                visuals.hull.transform,
                fleeOrigin,
                fleeDestination,
                .5f).Run();
            Game.Instance.audioManager.Play("failure");
            yield return new WaitForSeconds(.3f);
            yield return Game.Instance.effects.Create<EffectTranslate>("Translate").Setup(
                visuals.hull.transform,
                fleeDestination,
                fleeOrigin,
                .5f).Run();

            AbilityFlee.Deactivate();
        }

        visuals.hull.flipX = !visuals.hull.flipX;
    }
}
