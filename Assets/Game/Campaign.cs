using UnityEngine;

public class Campaign : MonoBehaviour
{
    public enum State
    {
        None,
        Introduction,
        InitialStats,
        PreEncounter,
        Encounter,
        PostEncounter,
    }

    [System.NonSerialized] public GameBalance gameBalance;
    [System.NonSerialized] public VesselStats playerStats;
    [System.NonSerialized] public VesselStatus playerStatus;
    [System.NonSerialized] public Encounter encounter;
    private EncounterDescriptor nextEncounterDescriptor;

    private State state;
    private int nextEncounter;
    private int encounterCount;

    public void BeginCampaign(string balanceName)
    {
        gameBalance = Resources.Load<GameBalance>(balanceName);
        Debug.Assert(gameBalance != null);

        playerStats = gameObject.AddComponent<VesselStats>();
        playerStats.SetStats(new VesselStatValues());

        playerStatus = gameObject.AddComponent<VesselStatus>();
        //playerStatus.InitialiseFull(gameBalance, playerStats);

        encounter = null;

        state = State.None;
        nextEncounter = 0;
        encounterCount = gameBalance.encounterCount;

        state = State.InitialStats;
        int remainingPoints = gameBalance.initialStatPoints;
        while (remainingPoints > 0)
        {
            for (int i = 0; remainingPoints > 0 && i < 4; ++i)
            {
                playerStats.SetRaw((VesselStats.Type)i, playerStats.GetRaw((VesselStats.Type)i) + 1);
                --remainingPoints;
            }
        }
        Game.Instance.pageManager.SetPage("InitialStats");
    }

    public void OnInitialStatsComplete()
    {
        Debug.Assert(state == State.InitialStats);

        playerStatus.InitialiseFull(gameBalance, playerStats);

        NextEncounter();
    }

    private void NextEncounter()
    {
        if (nextEncounter < encounterCount)
        {
            state = State.PreEncounter;
            SelectNextEncounterDescriptor();
            ++nextEncounter;

            Game.Instance.pageManager.PushPage("PreEncounter");
        }
        else
        {
            throw new System.Exception("You win, but that's not implemented!");
        }
    }
    private void SelectNextEncounterDescriptor()
    {
        nextEncounterDescriptor = Resources.Load<EncounterDescriptor>("Encounters/TestHostile");
    }

    private void BeginEncounter()
    {
        Debug.Assert(state == State.PreEncounter);
        state = State.Encounter;

        var encounterObject = new GameObject("Encounter");
        encounter = encounterObject.AddComponent<Encounter>();
        encounter.BeginEncounter(this, nextEncounterDescriptor);
    }

    public void OnPreEncounterComplete()
    {
        Debug.Assert(state == State.PreEncounter);
        BeginEncounter();
    }
    public void OnEncounterComplete()
    {
        if (encounter != null)
            Destroy(encounter.gameObject);

        if (playerStatus.health <= 0)
            throw new System.Exception("You lost, but that isn't implemented");

        Debug.Assert(state == State.Encounter);
        state = State.PostEncounter;
        Game.Instance.pageManager.PushPage("PostEncounter");
    }
    public void OnPostEncounterComplete()
    {
        Debug.Assert(state == State.PostEncounter);
        NextEncounter();
    }

    private void MakeTestEncounter()
    {
        state = State.Encounter;
        var encounterObject = new GameObject("Encounter");
        encounter = encounterObject.AddComponent<Encounter>();
        encounter.BeginEncounter(this, Resources.Load<EncounterDescriptor>("Encounters/TestHostile"));
    }
}
