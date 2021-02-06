using UnityEngine;

[CreateAssetMenu(fileName = "NewGameBalance", menuName = "Game/Balance")]
public class GameBalance : ScriptableObject
{
    [Header("Campaign")]
    public int initialStatPoints = 5;
    public int initialFuel = 2;
    public int initialAmmo = 1;
    public float encounterIgnoreThreshold = .5f;

    [Header("Systems")]
    public int healthMin = 100;
    public int healthMax = 400;
    public int laserDamageMin = 5;
    public int laserDamageMax = 50;
    public int torpedoDamageMin = 25;
    public int torpedoDamageMax = 100;
    public int boardingDamageMin = 0;
    public int boardingDamageMax = 5;
    public float shieldsStayActiveOnDamageRoll = .75f;

    [Header("Stat rolls")]
    public StatRoll statRollAttack;
    public StatRoll statRollDefense;
    public StatRoll statRollLuck;
}
