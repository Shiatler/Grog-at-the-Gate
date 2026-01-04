using UnityEngine;

[System.Serializable]
public class TurretBlueprint 
{
    public bool useUpgrades = false;

    public GameObject prefab;
    public int cost;

    public GameObject upgradedPrefab;
    public int upgradeCost;

    public int GetSellAmount()
    {
        return cost / 2;
    }

    public int GetUpgradedSellAmount()
    {
        return upgradeCost / 2 + cost / 2;
    }
}
