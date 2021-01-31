using UnityEngine;

public abstract class AiBehaviour : MonoBehaviour
{
    [System.NonSerialized]
    public EncounterDescriptor encounterDescriptor;

    public abstract void Act(VesselEncounter self);
}