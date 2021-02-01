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
            () => "Fire missle\nBypass shields\nSystem: " + Status.weapons + "%",
            () => modifiers.CanMissle,
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
            OnBoardingStop,
            OnBoardingTick
        );
        abilities["board"] = AbilityBoard;

        AbilityShields = new VesselAbilityDelegated(
            2, 1,
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
        abilities["shields"] = AbilityShields;

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
    private int GetSystemDependentRollEffect(float roll, int systemStatus, int effectMin, int effectMax)
    {
        // roll [0, 1] clamped to system status [0, 1] transposed to range [min, max]
        float rawEffect = Mathf.Min(roll, systemStatus / (float)VesselStatus.MaxSystemStatus);
        float scaledEffect = Mathf.Lerp(effectMin, effectMax, rawEffect);
        return Mathf.RoundToInt(scaledEffect);
    }
    private void FinishTurn()
    {
        if (abilityUsedThisTurn)
            throw new System.Exception("Multiple abilities used in a single turn");
        abilityUsedThisTurn = true;
        owner.OnVesselEndTurn(this);
    }

    // ability callbacks
    private void OnActivateLaser()
    {
        Vector3 laserEmit = visuals.laserEmit.position;
        Vector3 laserReceive = (opponent.AbilityShields.IsActive ? opponent.visuals.laserReceiveShield : opponent.visuals.laserReceiveHull).position;
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectLaser>("Laser").Setup(laserEmit, laserReceive).Run());

        int damage = GetSystemDependentRollEffect(Stats.RollAttack(), Status.weapons, balance.laserDamageMin, balance.laserDamageMax);
        Debug.LogFormat("{0} firing laser with {1} damage", name, damage);

        if (opponent.AbilityShields.IsActive)
        {
            // shields absorb everything they can, any leftover is applied to health
            int absorbed = Mathf.Min(damage, opponent.Status.shields);
            opponent.Status.shields -= absorbed;
            opponent.Status.health -= Mathf.Min(opponent.Status.health, damage - absorbed);
            opponent.AbilityShields.Deactivate();

            Debug.LogFormat("> shields absorbed {0}", absorbed);
            Debug.LogFormat("> hull took {0}", damage - absorbed);
        }
        else
        {
            // damage applied to health
            opponent.Status.health -= Mathf.Min(opponent.Status.health, damage);
            Debug.LogFormat("> hull took {0}", damage);
        }

        FinishTurn();
    }

    private void OnActivateTorpedo()
    {
        Vector3 torpedoEmit = visuals.torpedoEmit.position;
        Vector3 torpedoReceive = opponent.visuals.laserReceiveHull.position;
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectTorpedo>("Torpedo").Setup(torpedoEmit, torpedoReceive).Run());

        int damage = GetSystemDependentRollEffect(Stats.RollAttack(), Status.weapons, balance.torpedoDamageMin, balance.torpedoDamageMax);
        Debug.LogFormat("{0} firing torpedo with {1} damage", name, damage);

        // damage applied to health
        opponent.Status.health -= Mathf.Min(opponent.Status.health, damage);
        Debug.LogFormat("> hull took {0}", damage);

        FinishTurn();
    }

    private void OnActivateBoarding()
    {
        Vector3 boardEmit = visuals.boardingEmit.position;
        Vector3 boardReceive = opponent.visuals.boardingReceive.position;
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectBoardingStart>("BoardingStart").Setup(boardEmit, boardReceive).Run());

        FinishTurn();
    }

    private void OnBoardingStop()
    {
        Vector3 boardEmit = opponent.visuals.boardingReceive.position;
        Vector3 boardReceive = visuals.boardingEmit.position;
        owner.EnqueueAnimation(Game.Instance.effects.Create<EffectBoardingStart>("BoardingStart").Setup(boardEmit, boardReceive).Run());
    }

    private void OnBoardingTick()
    {
        int damage = GetSystemDependentRollEffect(Stats.RollAttack(), Status.weapons, balance.boardingDamageMin, balance.boardingDamageMax);
        Debug.LogFormat("{0} boarding party did {1} damage", name, damage);

        // damage applied to health
        opponent.Status.health -= Mathf.Min(opponent.Status.health, damage);
        Debug.LogFormat("> hull took {0}", damage);
    }

    private void OnActivateShields()
    {
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
        Debug.Assert(opponent.AbilityBoard.IsActive);
        opponent.AbilityBoard.Deactivate();

        FinishTurn();
    }

    private void OnSkipTurn()
    {
        FinishTurn();
    }
}
