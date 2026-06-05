using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PartButton : MonoBehaviour
{
    [SerializeField] private RawImage previewImage;
    
    private RobotPartData partData;
    private RobotController controller;

    public void SetupButton(RobotPartData data, RobotController robotController, Texture2D previewTexture)
    {
        partData = data;
        controller = robotController;
        
        if (previewImage != null)
        {
            previewImage.texture = previewTexture;
        }

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        switch (partData.partType)
        {
            case PartType.Head:
                controller.UpdateHead(partData);
                break;
            case PartType.Torso:
                controller.UpdateTorso(partData);
                break;
            case PartType.Legs:
                controller.UpdateLegs(partData);
                break;
        }
    }
}