using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;                                  
    public Vector3 targetOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private PlayerInput playerInput;

    [Header("Khoảng cách / Zoom")]
    public float distance = 4f;
    public float minDistance = 1.2f;
    public float maxDistance = 8f;
    public float zoomSpeed = 3f;
    public float zoomSmooth = 10f;

    [Header("Góc xoay")]
    public float sensitivityX = 3f;
    public float sensitivityY = 2f;
    public float minPitch = -30f;
    public float maxPitch = 70f;
    public bool invertY = false;
    public bool ignoreWhenOverUI = true;    
    public bool hideCursorWhileDragging = true;

    [Header("Độ mượt")]
    public float followSmoothTime = 0.05f;

    [Header("Chống xuyên tường/sàn")]
    public LayerMask collisionMask = ~0;    
    public float collisionRadius = 0.2f;
    public float collisionPadding = 0.15f;
    public float collisionInSmoothSpeed = 25f;
    public float collisionOutSmoothSpeed = 8f;

    private float yaw;
    private float pitch;
    private float desiredDistance;
    private float currentDistance;
    private Vector3 pivot;
    private Vector3 pivotVelocity;
    private bool isDragging;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (playerInput == null && target != null)
        {
            playerInput = target.GetComponent<PlayerInput>();
        }

        if (target != null)
        {
            yaw = target.eulerAngles.y;
            pitch = 15f;  
            pivot = target.position + targetOffset;
        }
        else
        {
            Vector3 angles = this.transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x > 180f ? angles.x - 360f : angles.x;
        }

        desiredDistance = currentDistance = distance;
    }

    void Update()
    {
        bool rotateDown = playerInput != null ? playerInput.IsRotateCameraDown : Input.GetMouseButtonDown(0);
        bool rotateHeld = playerInput != null ? playerInput.IsRotateCameraHeld : Input.GetMouseButton(0);
        bool rotateUp = playerInput != null ? playerInput.IsRotateCameraUp : Input.GetMouseButtonUp(0);

        if (rotateDown)
        {
            bool overUI = ignoreWhenOverUI && EventSystem.current != null
                          && EventSystem.current.IsPointerOverGameObject();
            if (!overUI)
            {
                isDragging = true;
                if (hideCursorWhileDragging) Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (rotateUp || !rotateHeld)
        {
            if (isDragging)
            {
                isDragging = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 lookDelta = playerInput != null ? playerInput.LookInputValue : new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float zoomDelta = playerInput != null ? playerInput.ZoomInputValue : Input.GetAxis("Mouse ScrollWheel");

        if (isDragging)
        {
            yaw += lookDelta.x * sensitivityX;
            pitch += lookDelta.y * sensitivityY * (invertY ? 1f : -1f);
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        if (Mathf.Abs(zoomDelta) > 0.001f)
        {
            desiredDistance -= zoomDelta * zoomSpeed;
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        }

        Vector3 targetPivot = target.position + targetOffset;
        pivot = followSmoothTime > 0f
            ? Vector3.SmoothDamp(pivot, targetPivot, ref pivotVelocity, followSmoothTime)
            : targetPivot;

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rot * Vector3.back;

        float targetDistance = desiredDistance;
        if (Physics.SphereCast(pivot, collisionRadius, dir, out RaycastHit hit,
                               desiredDistance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            targetDistance = Mathf.Clamp(hit.distance - collisionPadding, minDistance, desiredDistance);
        }

        float smoothSpeed = (targetDistance < currentDistance) ? collisionInSmoothSpeed : collisionOutSmoothSpeed;
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * smoothSpeed);

        this.transform.SetPositionAndRotation(pivot + dir * currentDistance, rot);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public Vector3 PlanarForward => Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
    public Vector3 PlanarRight => Quaternion.Euler(0f, yaw, 0f) * Vector3.right;
}