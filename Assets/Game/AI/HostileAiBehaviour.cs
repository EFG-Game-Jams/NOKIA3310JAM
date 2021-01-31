using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HostileAiBehaviour : AiBehaviour
{
    private OpponentVesselInformation context;

    public float aggressiveness = 1f;

    public override void Act(VesselEncounter self)
    {
        var context = self.GetOpponentInformation();

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

        VesselAbility chosenAction = null;
        bool pickAction = Random.value < 0.95f;
        if (pickAction && list.Count > 0)
        {
            list = list.OrderBy(e => e.score).ToList();

            var index = Mathf.FloorToInt((1f - (Mathf.Sqrt(Random.value))) * list.Count);
            if (index > list.Count)
            {
                index = list.Count - 1;
            }

            chosenAction = list[index].ability;
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

    private int GetRaiseShieldScore() => 40;
    private int GetScanScore() => 40;
    private int GetFireLaserScore() => 80;
    private int GetRepelScore() => 30;
    private int GetBoardScore() => 20;
    private int GetTorpedoScore() => 50;
}