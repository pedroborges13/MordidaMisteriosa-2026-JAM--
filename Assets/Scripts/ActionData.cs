using UnityEngine;

public enum ActionType { Water, Hand, Shake }
public enum BoxReaction { None, SmallShake, BigShake, Attack}

[CreateAssetMenu(fileName = "newaAction", menuName = "Action/ActionData")]
public class ActionData : ScriptableObject
{
    [SerializeField] private string actionName;
    [SerializeField] private int cost;
    [SerializeField] private int streessImpact;

    public string ActionName => actionName;
    public int Cost => cost;
    public int StreessImpact => streessImpact;  
}
