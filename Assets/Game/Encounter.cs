using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    private enum Turn
    {
        Player,
        Opponent,
    }
    private enum TurnState
    {
        NotStarted,
        Deciding,
        Animating,
        Ending,
    }

    public Campaign owner;
    public EncounterDescriptor descriptor;

    private VesselStats opponentStats;
    private VesselStatus opponentStatus;
    private AiBehaviour opponentAiBehaviour;

    [NonSerialized] public VesselEncounter playerEncounter; // public for UI
    [NonSerialized] public VesselEncounter opponentEncounter; // public for UI

    private PageEncounter pageEncounter;

    private Turn turn;
    private TurnState turnState;
    private List<IEnumerator> pendingCoroutines = new List<IEnumerator>();

    public void BeginEncounter(Campaign owner, EncounterDescriptor descriptor)
    {
        this.owner = owner;
        this.descriptor = descriptor;

        // setup opponent
        opponentStats = gameObject.AddComponent<VesselStats>();
        InitialiseOpponentStats();
        opponentStatus = gameObject.AddComponent<VesselStatus>();
        InitialiseOpponentStatus();

        // activate page
        pageEncounter = (PageEncounter)Game.Instance.pageManager.PushPage("Encounter");
        // disable input
        pageEncounter.IsInputEnabled = false;

        // initialise health bars
        pageEncounter.healthBarPlayer.SetFill(owner.playerStatus.GetHealthPercentage());
        pageEncounter.healthBarOpponent.SetFill(opponentStatus.GetHealthPercentage());

        // setup vessel encounters
        playerEncounter = new VesselEncounter("player", this, owner.gameBalance, pageEncounter.playerVisuals, owner.playerStats, owner.playerStatus, descriptor.playerModifiers);
        opponentEncounter = new VesselEncounter("opponent", this, owner.gameBalance, pageEncounter.opponentVisuals, opponentStats, opponentStatus, descriptor.enemyModifiers);
        VesselEncounter.SetOpponents(playerEncounter, opponentEncounter);

        opponentAiBehaviour = Instantiate(descriptor.enemyAiBehaviour, transform);

        // start
        BeginPlayerTurn();
    }

    public void EnqueueAnimation(IEnumerator animation)
    {
        pendingCoroutines.Add(animation);
    }
    public void OnVesselEndTurn(VesselEncounter vessel)
    {
        Debug.Assert(turnState == TurnState.Deciding);

        // disable input
        pageEncounter.IsInputEnabled = false;

        // enqueue health bar animations
        EnqueueAnimation(CoroutineComposer.MakeParallel(
            this,
            pageEncounter.healthBarPlayer.AnimateFill(owner.playerStatus.GetHealthPercentage(), 1),
            pageEncounter.healthBarOpponent.AnimateFill(opponentStatus.GetHealthPercentage(), 1)
        ));

        // enqueue end of turn
        pendingCoroutines.Add(CoroutineComposer.MakeAction(OnFinishAnimating));

        // run coroutines
        turnState = TurnState.Animating;
        StartCoroutine(CoroutineComposer.MakeSequence(pendingCoroutines.ToArray()));
    }

    private void OnFinishAnimating()
    {
        Debug.Log("Encounter.OnFinishAnimating");
        Debug.Assert(turnState == TurnState.Animating);

        turnState = TurnState.Ending;
        EndTurn();
    }

    private void BeginTurn(VesselEncounter vessel)
    {
        Debug.Assert(turnState == TurnState.NotStarted);
        vessel.BeginTurn();
        turnState = TurnState.Deciding;
    }
    private void BeginPlayerTurn()
    {
        Debug.Log("Starting Player turn");
        BeginTurn(playerEncounter);

        // enable input
        pageEncounter.IsInputEnabled = true;
    }
    private void BeginOpponentTurn()
    {
        Debug.Log("Starting AI turn");
        BeginTurn(opponentEncounter);

        opponentAiBehaviour.Act(opponentEncounter);

        // Assert the turn is advancing for the AI
        Debug.Assert(
            turn == Turn.Player ||
            (turnState != TurnState.Deciding && turnState != TurnState.NotStarted));
    }

    private void EndTurn()
    {
        Debug.Assert(turnState == TurnState.Ending);

        // todo: end conditions
        if (playerEncounter.Status.health <= 0 ||
            opponentEncounter.Status.health <= 0 ||
            playerEncounter.AbilityFlee.IsActive ||
            opponentEncounter.AbilityFlee.IsActive)
        {
            EndEncounter();
            return;
        }

        // begin next turn
        turnState = TurnState.NotStarted;
        if (turn == Turn.Player)
        {
            turn = Turn.Opponent;
            BeginOpponentTurn();
        }
        else if (turn == Turn.Opponent)
        {
            turn = Turn.Player;
            BeginPlayerTurn();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    private void EndEncounter()
    {
        pageEncounter.pageManager.PopPage();
        owner.OnEncounterComplete(playerEncounter.AbilityFlee.IsActive, opponentEncounter.AbilityFlee.IsActive);
    }

    private void OnDestroy()
    {
        Destroy(opponentStats);
        Destroy(opponentStatus);

        playerEncounter = null;
        opponentEncounter = null;
    }

    private void InitialiseOpponentStats()
    {
        // don't modify asset's stats array
        var stats = descriptor.enemyStats.Clone();

        // apply
        opponentStats.SetStats(stats);
    }

    private void InitialiseOpponentStatus()
    {
        // initialise full first
        opponentStatus.InitialiseFull(owner.gameBalance, opponentStats, descriptor.enemyHealth);

        // then apply modifiers
        if (!descriptor.enemyModifiers.CanFlee)
            opponentStatus.fuel = 0;
        opponentStatus.ammo = (descriptor.enemyModifiers.CanMissle ? descriptor.enemyMissles : 0);
        // todo: other encounter modifiers
    }
}
