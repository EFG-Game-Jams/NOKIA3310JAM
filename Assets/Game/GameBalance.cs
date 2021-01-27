using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameBalance", menuName = "Game/Balance")]
public class GameBalance : ScriptableObject
{
	[Header("Stat rolls")]
	public StatRoll statRollAttack;
	public StatRoll statRollDefense;
	public StatRoll statRollLuck;
}
