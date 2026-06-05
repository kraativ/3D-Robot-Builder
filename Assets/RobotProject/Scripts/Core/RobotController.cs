using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RobotController : MonoBehaviour
{
    [Header("Spawn Slots")]
    [SerializeField] private Transform legsSlot;
    [SerializeField] private Transform torsoSlot;
    [SerializeField] private Transform headSlot;

    [Header("Available Configurations")]
    public List<RobotPartData> availableHeads;
    public List<RobotPartData> availableTorsos;
    public List<RobotPartData> availableLegs;

    [Header("Physics Settings")]
    public float jumpForceSmall = 100f;
    public float jumpForceMedium = 250f;
    public float jumpForceBig = 400f;

    private RobotPart spawnedLegs;
    private RobotPart spawnedTorso;
    private RobotPart spawnedHead;
    private Rigidbody robotRigidbody;

    private RobotPartData currentHeadData;
    private RobotPartData currentTorsoData;
    private RobotPartData currentLegsData;

    private Color currentHeadColor = Color.green;
    private Color currentTorsoColor = Color.yellow;
    private Color currentLegsColor = Color.black;

    public static event Action<float, float> OnStatsChanged;

    private void Awake()
    {
        robotRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (availableHeads.Count > 0 && availableTorsos.Count > 0 && availableLegs.Count > 0)
        {
            BuildRobot(availableHeads[0], availableTorsos[0], availableLegs[0]);
        }
    }

    public void BuildRobot(RobotPartData headData, RobotPartData torsoData, RobotPartData legsData)
    {
        UpdateLegs(legsData);
        UpdateTorso(torsoData);
        UpdateHead(headData);
    }

    public void UpdateLegs(RobotPartData legsData)
    {
        if (legsData == null) return;
        currentLegsData = legsData;

        spawnedLegs = ReplacePart(spawnedLegs, legsData, legsSlot);
        if (spawnedLegs != null) spawnedLegs.SetColor(currentLegsColor);

        UpdateTorsoPosition();
        UpdateHeadPosition();

        UpdateTotalStats();
    }

    public void UpdateTorso(RobotPartData torsoData)
    {
        if (torsoData == null) return;
        currentTorsoData = torsoData;

        spawnedTorso = ReplacePart(spawnedTorso, torsoData, torsoSlot);
        if (spawnedTorso != null) spawnedTorso.SetColor(currentTorsoColor);

        UpdateTorsoPosition();
        UpdateHeadPosition();
        UpdateTotalStats();
    }

    public void UpdateHead(RobotPartData headData)
    {
        if (headData == null) return;
        currentHeadData = headData;

        spawnedHead = ReplacePart(spawnedHead, headData, headSlot);
        if (spawnedHead != null) spawnedHead.SetColor(currentHeadColor);

        UpdateHeadPosition();
        UpdateTotalStats();
    }

private void UpdateTorsoPosition()
{
    if (spawnedLegs == null) return;

    float legsHeight = spawnedLegs.GetPartHeight();
    float torsoHeight = spawnedTorso != null ? spawnedTorso.GetPartHeight() : 0f;

    float targetY = legsSlot.localPosition.y + (legsHeight / 2f) + (torsoHeight / 2f);

    torsoSlot.localPosition = new Vector3(0f, targetY, 0f);
}

private void UpdateHeadPosition()
{
    if (spawnedTorso == null) return;

    float torsoHeight = spawnedTorso.GetPartHeight();
    float headHeight = spawnedHead != null ? spawnedHead.GetPartHeight() : 0f;

    float targetY = torsoSlot.localPosition.y + (torsoHeight / 2f) + (headHeight / 2f);

    headSlot.localPosition = new Vector3(0f, targetY, 0f);
}

    private void UpdateTotalStats()
    {
        float totalWeight = 0f;
        float totalPower = 0f;

        if (currentHeadData != null) { totalWeight += currentHeadData.weight; totalPower += currentHeadData.power; }
        if (currentTorsoData != null) { totalWeight += currentTorsoData.weight; totalPower += currentTorsoData.power; }
        if (currentLegsData != null) { totalWeight += currentLegsData.weight; totalPower += currentLegsData.power; }

        robotRigidbody.mass = totalWeight > 0f ? totalWeight : 1f;

        OnStatsChanged?.Invoke(totalWeight, totalPower);
    }

    private RobotPart ReplacePart(RobotPart currentPart, RobotPartData data, Transform slot)
    {
        if (currentPart != null) Destroy(currentPart.gameObject);
        return Instantiate(data.prefab, slot.position, slot.rotation, slot);
    }

    public void Jump(float force)
    {
        if (robotRigidbody != null)
        {
            robotRigidbody.linearVelocity = new Vector3(robotRigidbody.linearVelocity.x, 0f, robotRigidbody.linearVelocity.z);
            robotRigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
        }
    }

    public void SetPartColor(PartType type, Color color)
    {
        switch (type)
        {
            case PartType.Head:
                currentHeadColor = color;
                if (spawnedHead) spawnedHead.SetColor(color);
                break;
            case PartType.Torso:
                currentTorsoColor = color;
                if (spawnedTorso) spawnedTorso.SetColor(color);
                break;
            case PartType.Legs:
                currentLegsColor = color;
                if (spawnedLegs) spawnedLegs.SetColor(color);
                break;
        }
    }
}