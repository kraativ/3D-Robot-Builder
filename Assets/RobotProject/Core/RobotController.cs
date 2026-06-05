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
    public float jumpForceSmall = 5f;
    public float jumpForceMedium = 10f;
    public float jumpForceBig = 15f;

    private RobotPart spawnedLegs;
    private RobotPart spawnedTorso;
    private RobotPart spawnedHead;
    private Rigidbody robotRigidbody;

    private RobotPartData currentHeadData;
    private RobotPartData currentTorsoData;
    private RobotPartData currentLegsData;

    private Color currentHeadColor = Color.white;
    private Color currentTorsoColor = Color.white;
    private Color currentLegsColor = Color.white;

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

        UpdateHeadPosition();
        UpdateTotalStats();
    }

    public void UpdateHead(RobotPartData headData)
    {
        if (headData == null) return;
        currentHeadData = headData;

        spawnedHead = ReplacePart(spawnedHead, headData, headSlot);
        if (spawnedHead != null) spawnedHead.SetColor(currentHeadColor);

        UpdateTotalStats();
    }

    private void UpdateTorsoPosition()
    {
        float legsHeight = spawnedLegs != null ? spawnedLegs.GetPartHeight() : 0f;
        torsoSlot.localPosition = new Vector3(0f, legsSlot.localPosition.y + legsHeight, 0f);
    }

    private void UpdateHeadPosition()
    {
        float torsoHeight = spawnedTorso != null ? spawnedTorso.GetPartHeight() : 0f;
        headSlot.localPosition = new Vector3(0f, torsoSlot.localPosition.y + torsoHeight, 0f);
    }

    private void UpdateTotalStats()
    {
        float totalWeight = 0f;
        float totalPower = 0f;

        if (currentHeadData != null) { totalWeight += currentHeadData.weight; totalPower += currentHeadData.power; }
        if (currentTorsoData != null) { totalWeight += currentTorsoData.weight; totalPower += currentTorsoData.power; }
        if (currentLegsData != null) { totalWeight += currentLegsData.weight; totalPower += currentLegsData.power; }

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