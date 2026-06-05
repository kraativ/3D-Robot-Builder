using UnityEngine;

public enum PartType { Head, Torso, Legs }

[CreateAssetMenu(fileName = "NewRobotPart", menuName = "Robot/Part Data")]
public class RobotPartData : ScriptableObject
{
    public PartType partType;
    public string partName;
    public GameObject prefab;

    [Header("Stats")]
    public float weight;
    public float power;
}