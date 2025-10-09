using UnityEngine;

public enum UpgradeType
{
    FireRateUp,        // +20%
    BulletSpeedUp,     // +20%
    Spread3,           // set to 3 projectiles, 10° spread
    Repair1            // +1 HP (to max)
}

public class UpgradeOption
{
    public UpgradeType type;
    public string title;
    public string description;
    public int cost;
    public bool oneTime; // e.g., Spread3 should not appear again after bought

    public UpgradeOption(UpgradeType t, string title, string desc, int cost, bool oneTime = false)
    {
        this.type = t;
        this.title = title;
        this.description = desc;
        this.cost = cost;
        this.oneTime = oneTime;
    }
}