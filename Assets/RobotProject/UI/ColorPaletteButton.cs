using UnityEngine;
using UnityEngine.UI;

public class ColorPaletteButton : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private PartType sectionType;

    private Color buttonColor;
    private RobotController controller;

    private void Awake()
    {
        if (TryGetComponent<Image>(out var image))
        {
            buttonColor = image.color;
        }
    }

    public void Init(RobotController robotController)
    {
        controller = robotController;

        if (TryGetComponent<Button>(out var button))
        {
            button.onClick.AddListener(OnColorClicked);
        }
    }

    private void OnColorClicked()
    {
        if (controller != null)
        {
            controller.SetPartColor(sectionType, buttonColor);
        }
    }
}