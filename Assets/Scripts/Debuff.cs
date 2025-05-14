using UnityEngine;


public enum DebuffType
{
    FirewallRage,
    Slow,
    Corruption
}
[System.Serializable] 
public class Debuff
{
    public DebuffType type; // The type of the debuff (FirewallRage, Slow, Curruption)
    public Sprite icon; // The icon representing the debuff (assign in the inspector)
    public float duration = 10f; // Duration of the debuff in seconds
}
