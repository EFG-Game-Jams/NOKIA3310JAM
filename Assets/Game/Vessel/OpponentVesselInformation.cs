public class OpponentVesselInformation
{
    public OpponentVesselInformation(bool hasScanned, VesselEncounter opponent)
    {
        Hitpoints = opponent.Status.health;
        ShieldsUp = opponent.AbilityShields?.IsActive == true;
        IsBoarded = opponent.AbilityBoard?.IsActive == true;

        if (hasScanned)
        {
            ScanInformation = new OpponentVesselScanInformation(opponent);
        }
    }

    public int Hitpoints {get; private set;}
    public bool ShieldsUp {get; private set;}
    public bool IsBoarded {get; private set;}

    public OpponentVesselScanInformation ScanInformation { get; private set; }

    public class OpponentVesselScanInformation
    {
        public OpponentVesselScanInformation(VesselEncounter opponent)
        {
            Stats = opponent.Stats;
            Status = opponent.Status;
        }

        public VesselStats Stats { get; private set; }
        public VesselStatus Status { get; private set; }
    }
}