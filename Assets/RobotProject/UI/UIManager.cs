using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RobotController robotController;
    [SerializeField] private GameObject partButtonPrefab;

    [Header("Containers for Grid")]
    [SerializeField] private Transform headsContainer;
    [SerializeField] private Transform torsosContainer;
    [SerializeField] private Transform legsContainer;

    [Header("Containers for Color Palettes")]
    [SerializeField] private Transform headPaletteRow;
    [SerializeField] private Transform torsoPaletteRow;
    [SerializeField] private Transform legsPaletteRow;

    [Header("Stats TextMeshPro")]
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI powerText;

    [Header("3D Photo Studio Setup")]
    [SerializeField] private Camera iconCamera;
    [SerializeField] private Transform previewSpawnPoint;

    private void OnEnable()
    {
        RobotController.OnStatsChanged += UpdateStatsUI;
    }

    private void OnDisable()
    {
        RobotController.OnStatsChanged -= UpdateStatsUI;
    }

    private void Start()
    {
        StartCoroutine(GenerateMenuWithPreviews());

        InitializePalette(headPaletteRow);
        InitializePalette(torsoPaletteRow);
        InitializePalette(legsPaletteRow);
    }

    private void InitializePalette(Transform paletteRow)
    {
        if (paletteRow == null) return;

        foreach (Transform child in paletteRow)
        {
            if (child.TryGetComponent<ColorPaletteButton>(out var paletteButton))
            {
                paletteButton.Init(robotController);
            }
        }
    }

    private IEnumerator GenerateMenuWithPreviews()
    {
        iconCamera.gameObject.SetActive(true);

        yield return CreateMenuButtonsRoutine(robotController.availableHeads, headsContainer);
        yield return CreateMenuButtonsRoutine(robotController.availableTorsos, torsosContainer);
        yield return CreateMenuButtonsRoutine(robotController.availableLegs, legsContainer);

        iconCamera.gameObject.SetActive(false);
    }

    private IEnumerator CreateMenuButtonsRoutine(List<RobotPartData> parts, Transform container)
    {
        foreach (var part in parts)
        {
            if (part == null || part.prefab == null) continue;

            RobotPart tempModel = Instantiate(part.prefab, previewSpawnPoint.position, previewSpawnPoint.rotation);

            if (tempModel.TryGetComponent<Rigidbody>(out var rb)) Destroy(rb);

            yield return new WaitForEndOfFrame();

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = iconCamera.targetTexture;

            Texture2D texture2D = new Texture2D(iconCamera.targetTexture.width, iconCamera.targetTexture.height, TextureFormat.RGB24, false);
            texture2D.ReadPixels(new Rect(0, 0, iconCamera.targetTexture.width, iconCamera.targetTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;

            GameObject buttonGo = Instantiate(partButtonPrefab, container);
            PartButton buttonScript = buttonGo.GetComponent<PartButton>();

            if (buttonScript != null)
            {
                buttonScript.SetupButton(part, robotController, texture2D);
            }

            Destroy(tempModel);
        }
    }

    private void UpdateStatsUI(float weight, float power)
    {
        if (weightText != null) weightText.text = $"Weight: {weight} kg";
        if (powerText != null) powerText.text = $"Power: {power} kW";
    }

    public void OnSmallJumpPressed() => robotController.Jump(robotController.jumpForceSmall);
    public void OnMediumJumpPressed() => robotController.Jump(robotController.jumpForceMedium);
    public void OnBigJumpPressed() => robotController.Jump(robotController.jumpForceBig);
}