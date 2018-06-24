﻿[System.Serializable]
public struct IncrementalFloat
{
    public float baseAmount;
    public float amountIncreaseEachLevel;

    public float GetAmount(short level)
    {
        return baseAmount + (amountIncreaseEachLevel * (level - 1));
    }
}
