using UnityEngine;

public class VesselStats : MonoBehaviour
{
    public const int MaxStatValue = 10;

    public enum Type
    {
        Attack = 0,
        Defense = 1,
        Luck = 2
    }

    [SerializeField] private VesselStatValues statValues;
    private StatRoll[] statRolls;

    private void Start()
    {
        // create new stats if needed
        statValues = statValues ?? new VesselStatValues();

        // fetch rolls from game balance
        GameBalance balance = Game.Instance.campaign.gameBalance;
        statRolls = new StatRoll[3];
        statRolls[0] = balance.statRollAttack;
        statRolls[1] = balance.statRollDefense;
        statRolls[2] = balance.statRollLuck;
    }

    public int GetRawTotal()
    {
        return statValues.GetSumOfValues();
    }

    public void SetStats(VesselStatValues values)
    {
        values.Clamp(0, MaxStatValue);
        statValues = values;
    }

    public int GetRaw(Type type)
    {
        return statValues.GetByType(type);
    }

    public void SetRaw(Type type, int value)
    {
        statValues.SetByType(type, value);
    }

    private float Roll(Type type, float scale = 1f)
    {
        StatRoll roller = statRolls[(int)type];
        Debug.Assert(roller != null);
        return roller.Roll(GetRaw(type), MaxStatValue, scale);
    }

    public float RollAttack(float scale = 1f) => Roll(Type.Attack, scale);
    public float RollDefense(float scale = 1f) => Roll(Type.Defense, scale);
    public float RollLuck(float scale = 1f) => Roll(Type.Luck, scale);

    public int GetMaxTransferable(Type from, Type to)
    {
        int maxSend = GetRaw(from);
        int maxReceive = MaxStatValue - GetRaw(to);
        return Mathf.Min(maxSend, maxReceive);
    }
    public int Transfer(Type from, Type to, int maxAmount)
    {
        int amount = Mathf.Min(maxAmount, GetMaxTransferable(from, to));
        statValues.SetByType(from, statValues.GetByType(from) - amount);
        statValues.SetByType(to, statValues.GetByType(to) + amount);

        return amount;
    }
}
