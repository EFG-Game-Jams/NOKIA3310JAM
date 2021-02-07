using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounterDescriptor", menuName = "Game/Encounter")]
public class EncounterDescriptor : ScriptableObject
{
    public enum Category
    {
        Hostile,
        Friendly,
        Special,
    }

    public Category encounterCategory;
    public VesselVisuals enemyVisual;

    public VesselStatValues enemyStats;

	public VesselModifiers enemyModifiers;

    public int enemyMissles = 1;
    public int enemyHealth = 100;

	public AiBehaviour enemyAiBehaviour;

	public VesselModifiers playerModifiers;
}
