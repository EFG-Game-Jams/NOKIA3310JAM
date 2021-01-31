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

    [NonSerialized] public VesselEncounter playerEncounter; // public for UI
    private VesselEncounter opponentEncounter;

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
        playerEncounter = new VesselEncounter("player", this, owner.gameBalance, pageEncounter.playerVisuals, owner.playerStats, owner.playerStatus);
        opponentEncounter = new VesselEncounter("opponent", this, owner.gameBalance, pageEncounter.opponentVisuals, opponentStats, opponentStatus);
        VesselEncounter.SetOpponents(playerEncounter, opponentEncounter);

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
        BeginTurn(playerEncounter);

        // enable input
        pageEncounter.IsInputEnabled = true;
    }
    private void BeginOpponentTurn()
    {
        BeginTurn(opponentEncounter);

        if (!opponentEncounter.AbilityShields.TryTrigger())
            opponentEncounter.AbilityLaser.TryTrigger();
    }

    private void EndTurn()
    {
        Debug.Assert(turnState == TurnState.Ending);

        // todo: end conditions
        if (playerEncounter.Status.health <= 0)
            throw new NotImplementedException("Player death");
        else if (opponentEncounter.Status.health <= 0)
            throw new NotImplementedException("Opponent death");

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
        var stats = descriptor.baseStats.Clone();

        // we won't be changing the weights though
        var weights = descriptor.statsScalingWeight.Clone();
        float totalWeight = weights.GetSumOfWeights();

        // distribute scaling points
        float weightToPoints = owner.playerStats.GetRawTotal() * descriptor.statsScaling / totalWeight;
        weights.MultiplyBy(weightToPoints);
        stats.AddWeights(weights);

        // apply
        opponentStats.SetStats(stats);
    }

    private void InitialiseOpponentStatus()
    {
        // initialise full first
        opponentStatus.InitialiseFull(owner.gameBalance, opponentStats);

        // then apply modifiers
        // todo: encounter modifiers
    }
}
