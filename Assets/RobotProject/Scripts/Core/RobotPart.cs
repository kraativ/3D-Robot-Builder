using UnityEngine;

public class RobotPart : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public float GetPartHeight()
    {
        if (meshRenderer == null) return 0f;
        return meshRenderer.bounds.size.y;
    }

    public void SetColor(Color color)
    {
        if (meshRenderer == null) return;

        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_BaseColor", color);
        meshRenderer.SetPropertyBlock(propBlock);
    }
}