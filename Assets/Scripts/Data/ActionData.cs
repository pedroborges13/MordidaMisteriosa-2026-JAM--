using UnityEngine;

public enum ActionType { Water, Hand, Shake }
public enum BoxReaction { None, SmallShake, BigShake, Attack}

[CreateAssetMenu(fileName = "newaAction", menuName = "Action/ActionData")]
public class ActionData : ScriptableObject
{
    [SerializeField] private string actionName;
    [SerializeField] private ActionType actionType;
    [SerializeField] private int cost;

    public string ActionName => actionName;
    public ActionType Type => actionType;
    public int Cost => cost;
}
