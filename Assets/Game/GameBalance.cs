﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameBalance", menuName = "Game/Balance")]
public class GameBalance : ScriptableObject
{
	[Header("Campaign")]
	public int encounterCount = 20;
	public int initialStatPoints = 5;

	[Header("Progression")]
	public int healthMin = 100;
	public int healthMax = 400;

	[Header("Stat rolls")]
	public StatRoll statRollAttack;
	public StatRoll statRollDefense;
	public StatRoll statRollLuck;
}
