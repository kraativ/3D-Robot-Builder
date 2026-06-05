using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float sensitivity = 0.5f;

    private CameraControls controls;
    private Vector2 lookDelta;
    private bool isRotating;

    private float x = 0.0f;
    private float y = 0.0f;

    void Awake()
    {
        controls = new CameraControls();
        
        controls.Player.Look.performed += ctx => lookDelta = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookDelta = Vector2.zero;
        
        controls.Player.RotatePress.started += _ => isRotating = true;
        controls.Player.RotatePress.canceled += _ => isRotating = false;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void LateUpdate()
    {
        if (target != null && isRotating)
        {
            x += lookDelta.x * sensitivity;
            y -= lookDelta.y * sensitivity;

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}