using UnityEngine;

[System.Serializable]
public class VesselStatWeights
{
    [SerializeField]
    public float Durability = 1;

    [SerializeField]
    public float Attack = 1;

    [SerializeField]
    public float Defense = 1;

    [SerializeField]
    public float Luck = 1;

    public VesselStatWeights Clone()
    {
        return new VesselStatWeights
        {
            Durability = Durability,
            Attack = Attack,
            Defense = Defense,
            Luck = Luck
        };
    }

    public float GetSumOfWeights()
    {
        return Durability + Attack + Defense + Luck;
    }

    public void MultiplyBy(float value)
    {
        Durability *= value;
        Attack *= value;
        Defense *= value;
        Luck *= value;
    }
}