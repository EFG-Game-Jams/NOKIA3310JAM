using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HostileAiBehaviour : AiBehaviour
{
    private OpponentVesselInformation enemy;
    private VesselEncounter self;

    public float aggressiveness = 1f;

    public override void Act(VesselEncounter self)
    {
        enemy = self.GetOpponentInformation();
        this.self = self;

        var list = new List<(VesselAbility ability, int score)>();
        if (self.AbilityShields?.CanTrigger == true) {
            list.Add((self.AbilityShields, GetRaiseShieldScore()));
        }
        if (self.AbilityScan?.CanTrigger == true) {
            list.Add((self.AbilityScan, GetScanScore()));
        }
        if (self.AbilityLaser?.CanTrigger == true) {
            list.Add((self.AbilityLaser, GetFireLaserScore()));
        }
        if (self.AbilityRepel?.CanTrigger == true) {
            list.Add((self.AbilityRepel, GetRepelScore()));
        }
        if (self.AbilityBoard?.CanTrigger == true) {
            list.Add((self.AbilityBoard, GetBoardScore()));
        }
        if (self.AbilityTorpedo?.CanTrigger == true) {
            list.Add((self.AbilityTorpedo, GetTorpedoScore()));
        }
        if (self.AbilityRepair?.CanTrigger == true) {
            list.Add((self.AbilityRepair, GetRepairScore()));
        }
        if (self.AbilityFlee?.CanTrigger == true) {
            list.Add((self.AbilityFlee, GetFleeScore()));
        }
        if (self.AbilityEvade?.CanTrigger == true) {
            list.Add((self.AbilityEvade, GetEvadeScore()));
        }

        VesselAbility chosenAction = null;
        bool pickAction = Random.value < 0.98f;
        if (pickAction && list.Count > 0)
        {
            var choices = list.Where(e => e.score > 0).OrderBy(e => e.score).ToArray();
            var index = Mathf.FloorToInt((Mathf.Sqrt(1f - Random.value)) * choices.Length);
            if (index >= choices.Length)
            {
                index = choices.Length - 1;
            }

            chosenAction = choices[index].ability;
        }
        else
        {
            chosenAction = self.AbilitySkipTurn;
        }

        if (!chosenAction.TryTrigger())
        {
            throw new System.Exception("Failed to trigger ability which reported CanTrigger true");
        }
    }

    private int GetRaiseShieldScore() => (self.Status.healthPercentage < 1f && self.Status.shieldsPercentage > 0f) ? 80 : 20;
    private int GetScanScore() => Mathf.RoundToInt(40 * (1f - aggressiveness));
    private int GetFireLaserScore() => Mathf.RoundToInt(80 * aggressiveness);
    private int GetRepelScore() => Mathf.RoundToInt(40 * (1f - self.Status.healthPercentage));
    private int GetBoardScore() => Mathf.RoundToInt(70 * aggressiveness);
    private int GetTorpedoScore() => Mathf.RoundToInt(70 * aggressiveness) + (self.Status.healthPercentage < 0.2f ? 100 : 0);
    private int GetRepairScore() => Mathf.RoundToInt(70 * Mathf.Max(self.Status.healthPercentage, self.Status.HasCriticalSystems ? 1f : 0f));
    private int GetFleeScore() => Mathf.RoundToInt(self.Status.healthPercentage < 0.2f ? 10 * (1f - aggressiveness) : 0);
    private int GetEvadeScore() => enemy.ScanInformation?.Status.ammo > 0 ? 60 : 0;
}