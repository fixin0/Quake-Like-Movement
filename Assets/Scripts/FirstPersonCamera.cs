using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    Generalnput generalInput;
    public Transform player;
    

    [Range(0, 100)] public float mouseSensitivity = 50f;
    public float verticalRotationLimit = 80f;

    private float cameraVerticalRotation = 0f;
    private float playerYaw = 0f;
    private Vector2 inputRotation;

    void Awake()
    {
        generalInput = new Generalnput();
    }

    private void Start()
    {
        inputRotation = Vector2.zero;
        
    }

    private void OnEnable()
    {
        generalInput.Enable();
        generalInput.Player.Look.performed += ctx => inputRotation = ctx.ReadValue<Vector2>();
        generalInput.Player.Look.canceled += ctx => inputRotation = Vector2.zero;
    }

    private void OnDisable()
    {
        generalInput.Player.Look.performed -= ctx => inputRotation = ctx.ReadValue<Vector2>();
        generalInput.Player.Look.canceled -= ctx => inputRotation = Vector2.zero;
        generalInput.Disable();
    }

    void Update()
    {
        Vector2 moveRotation = inputRotation * mouseSensitivity * Time.deltaTime;
        cameraVerticalRotation -= moveRotation.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -verticalRotationLimit, verticalRotationLimit);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        // Rotate the parent object (player) around the Y-axis
        transform.parent.Rotate(Vector3.up * moveRotation.x);

    }
}