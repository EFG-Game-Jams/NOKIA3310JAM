public class HostileAiBehaviour : AiBehaviour
{
    public float aggressiveness = 1f;

    public override void Act(VesselEncounter self)
    {
        // Call into AI component instead here
        if (!self.AbilityShields.TryTrigger())
            self.AbilityLaser.TryTrigger();
    }
}