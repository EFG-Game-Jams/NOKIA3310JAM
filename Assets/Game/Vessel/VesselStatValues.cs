using UnityEngine;

[System.Serializable]
public class VesselStatValues
{
    [SerializeField]
    public int Durability;

    [SerializeField]
    public int Attack;

    [SerializeField]
    public int Defense;

    [SerializeField]
    public int Luck;

    public VesselStatValues Clone()
    {
        return new VesselStatValues
        {
            Durability = Durability,
            Attack = Attack,
            Defense = Defense,
            Luck = Luck
        };
    }

    public void Clamp(int min, int max)
    {
        Durability = Mathf.Clamp(Durability, min, max);
        Attack = Mathf.Clamp(Attack, min, max);
        Defense = Mathf.Clamp(Defense, min, max);
        Luck = Mathf.Clamp(Luck, min, max);
    }

    public int GetSumOfValues()
    {
        return Durability + Attack + Defense + Luck;
    }

    public int GetByType(VesselStats.Type type)
    {
        switch (type)
        {
            case VesselStats.Type.Attack:
                return Attack;
            case VesselStats.Type.Defense:
                return Defense;
            case VesselStats.Type.Durability:
                return Durability;
            case VesselStats.Type.Luck:
                return Luck;

            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }

    public void SetByType(VesselStats.Type type, int value)
    {
        switch (type)
        {
            case VesselStats.Type.Attack:
                Attack = value;
                break;
            case VesselStats.Type.Defense:
                Defense = value;
                break;
            case VesselStats.Type.Durability:
                Durability = value;
                break;
            case VesselStats.Type.Luck:
                Luck = value;
                break;

            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}
