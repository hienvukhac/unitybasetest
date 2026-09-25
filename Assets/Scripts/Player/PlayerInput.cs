using UnityEngine;

[DefaultExecutionOrder(-10)]
public class PlayerInput : MonoBehaviour
{
    [Header("Cài đặt phím")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private int cameraRotateMouseButton = 0;

    private Vector2 moveInputValue;
    private bool isJumpButtonPressed;

    private bool isInteractButtonPressed;

    private Vector2 lookInputValue;
    private float zoomInputValue;
    private bool isRotateCameraHeld;
    private bool isRotateCameraDown;
    private bool isRotateCameraUp;

    public Vector2 MoveInputValue => moveInputValue;
    public bool IsJumpButtonPressed => isJumpButtonPressed;
    public bool IsInteractButtonPressed => isInteractButtonPressed;

    public Vector2 LookInputValue => lookInputValue;
    public float ZoomInputValue => zoomInputValue;
    public bool IsRotateCameraHeld => isRotateCameraHeld;
    public bool IsRotateCameraDown => isRotateCameraDown;
    public bool IsRotateCameraUp => isRotateCameraUp;
    public int CameraRotateMouseButton => cameraRotateMouseButton;

    private void Update()
    {
        ReadMovementInput();
        ReadInteractionInput();
        ReadCameraInput();
    }

    private void ReadMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 rawInput = new Vector2(h, v);
        moveInputValue = Vector2.ClampMagnitude(rawInput, 1f);

        isJumpButtonPressed = Input.GetButtonDown("Jump");
    }

    private void ReadInteractionInput()
    {
        isInteractButtonPressed = Input.GetKeyDown(interactKey);
    }

    private void ReadCameraInput()
    {
        lookInputValue = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        zoomInputValue = Input.GetAxis("Mouse ScrollWheel");

        isRotateCameraDown = Input.GetMouseButtonDown(cameraRotateMouseButton);
        isRotateCameraHeld = Input.GetMouseButton(cameraRotateMouseButton);
        isRotateCameraUp = Input.GetMouseButtonUp(cameraRotateMouseButton);
    }
}
