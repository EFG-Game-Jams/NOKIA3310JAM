using UnityEngine;

public class Campaign : MonoBehaviour
{
    public const int playerHealth = 100;

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
    public EncounterDescriptor[] encounterSequence;
    [SerializeField] private int nextEncounter;
    private int EncounterCount => encounterSequence.Length;

    public int EncounterIndex => nextEncounter - 1;

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

        state = State.InitialStats;
        int remainingPoints = gameBalance.initialStatPoints;
        while (remainingPoints > 0)
        {
            int statCount = System.Enum.GetValues(typeof(VesselStats.Type)).Length;
            for (int i = 0; remainingPoints > 0 && i < statCount; ++i)
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

        playerStatus.InitialiseFull(gameBalance, playerStats, playerHealth);

        NextEncounter();
    }

    private void NextEncounter()
    {
        if (nextEncounter < EncounterCount)
        {
            state = State.PreEncounter;
            SelectNextEncounterDescriptor();
            ++nextEncounter;

            Game.Instance.pageManager.PushPage("PreEncounter");
            Game.Instance.pageManager.PushPage("Warp");
        }
        else
        {
            Win();
        }
    }
    private void SelectNextEncounterDescriptor()
    {
        nextEncounterDescriptor = encounterSequence[nextEncounter];// Resources.Load<EncounterDescriptor>("Encounters/TestHostile");
    }

    private void Win()
    {
        Game.Instance.pageManager.SetPage("Win");
        Destroy(gameObject);
    }

    private void BeginEncounter()
    {
        Debug.Assert(state == State.PreEncounter);
        state = State.Encounter;

        var encounterObject = new GameObject("Encounter");
        encounter = encounterObject.AddComponent<Encounter>();
        encounter.BeginEncounter(this, nextEncounterDescriptor);
    }

    public void OnPreEncounterCompleteIvestigate()
    {
        Debug.Assert(state == State.PreEncounter);

        BeginEncounter();
    }
    public void OnPreEncounterCompleteIgnore()
    {
        Debug.Assert(state == State.PreEncounter);

        if (playerStats.RollLuck() >= gameBalance.encounterIgnoreThreshold)
            NextEncounter();
        else
            BeginEncounter();
    }
    public void OnPreEncounterCompleteAvoid()
    {
        Debug.Assert(state == State.PreEncounter);

        Debug.Assert(playerStatus.fuel > 0);
        --playerStatus.fuel;

        NextEncounter();
    }
    public void OnEncounterComplete(bool playerFled = false, bool opponentFled = false)
    {
        if (encounter != null)
            Destroy(encounter.gameObject);

        if (playerStatus.health <= 0)
        {
            Game.Instance.pageManager.SetPage("GameOver");
            return;
        }
        else if (nextEncounter >= EncounterCount)
        {
            Win();
            return;
        }

        Debug.Assert(state == State.Encounter);
        state = State.PostEncounter;

        bool wasHostile = (nextEncounterDescriptor.encounterCategory == EncounterDescriptor.Category.Hostile);
        bool canSpentStatPoint = wasHostile && !playerFled;
        bool canSalvage = wasHostile && !playerFled && !opponentFled;
        bool canRepair = canSalvage && playerStatus.CanRepair;
        string postEncounterTitle = (playerFled ? "You escaped" : (opponentFled ? "Enemy escaped" : "Enemy defeated"));

        PagePostEncounter page = Game.Instance.pageManager.GetPage<PagePostEncounter>();
        page.Configure(postEncounterTitle, canSpentStatPoint ? 1 : 0, canSalvage ? 1 : 0, canSalvage ? 1 : 0, canRepair);
        Game.Instance.pageManager.PushPage(page);
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
