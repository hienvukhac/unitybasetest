using System;
using System.Collections;
using UnityEngine;

public enum DoorState
{
    Closed,
    Opening,
    Open,
    Closing
}

public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Cài đặt góc mở")]
    [SerializeField] private Vector3 openRotationAngle = new Vector3(0f, 90f, 0f);

    [Tooltip("Tốc độ mở cửa")]
    [SerializeField] private float openSpeed = 3f;

    [Header("Cài đặt Tương Tác")]
    [SerializeField] private string promptText = "[E] Mở cửa";
    [SerializeField] private Collider obstacleCollider; 

    public event Action<DoorController> OnDoorOpened;

    private DoorState currentState = DoorState.Closed;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    public string PromptText => promptText;
    public bool CanInteract => currentState == DoorState.Closed;
    public Transform PromptTransform => transform;

    private void Start()
    {
        closedRotation = this.transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(openRotationAngle);

        if (obstacleCollider == null)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                if (!col.isTrigger)
                {
                    obstacleCollider = col;
                    break;
                }
            }
        }
    }


    public void Interact()
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (currentState != DoorState.Closed) return;

        StartCoroutine(AnimateDoorRotation(openRotation, DoorState.Open, () =>
        {
            if (obstacleCollider != null)
            {
                obstacleCollider.enabled = false; 
            }
            OnDoorOpened?.Invoke(this);
            Debug.Log("[Cửa] Đã mở hoàn tất: " + gameObject.name);
        }));
    }

    private IEnumerator AnimateDoorRotation(Quaternion targetRotation, DoorState targetState, Action onComplete)
    {
        currentState = DoorState.Opening;
        Quaternion startRotation = this.transform.localRotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            this.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        this.transform.localRotation = targetRotation;
        currentState = targetState;
        onComplete?.Invoke();
    }
}
