using UnityEngine;

[CreateAssetMenu(fileName = "NewGameBalance", menuName = "Game/Balance")]
public class GameBalance : ScriptableObject
{
    [Header("Campaign")]
    public int encounterCount = 20;
    public int initialStatPoints = 5;
    public int initialFuel = 3;
    public int initialAmmo = 3;

    [Header("Systems")]
    public int healthMin = 100;
    public int healthMax = 400;
    public int laserDamageMin = 5;
    public int laserDamageMax = 50;

    public int torpedoDamageMin = 25;
    public int torpedoDamageMax = 100;

    public int boardingDamageMin = 0;
    public int boardingDamageMax = 5;

    [Header("Stat rolls")]
    public StatRoll statRollAttack;
    public StatRoll statRollDefense;
    public StatRoll statRollLuck;
}
