using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "Game/Encounter")]
public class Encounter : ScriptableObject
{
	public enum Category
	{
		Hostile,
		Friendly,
		Special,
	}

	public Category category;
	public VesselVisuals enemyVisuals;
}
