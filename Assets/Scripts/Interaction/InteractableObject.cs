using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Cài Đặt Tương Tác")]
    [SerializeField] private string promptText = "[E] Tương tác";
    [SerializeField] private bool singleUse = true;

    [Header("Hiệu Ứng Bấm Nút")]
    [SerializeField] private bool enableButtonPressEffect = true;

    [SerializeField] private float pressDepth = 0.05f;
    [SerializeField] private float pressSpeed = 15f;

    [Header("Sự Kiện Khi Tương Tác")]
    [SerializeField] private UnityEvent onInteract;

    private bool hasInteracted = false;
    private Vector3 originalLocalPos;
    private bool isPressingAnimation = false;

    public string PromptText => promptText;
    public bool CanInteract => !singleUse || !hasInteracted;
    public Transform PromptTransform => transform;

    private void Awake()
    {
        originalLocalPos = this.transform.localPosition;
    }

    private void OnMouseDown()
    {
        if (CanInteract)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (!CanInteract || isPressingAnimation) return;

        Debug.Log($"[InteractableObject] Đã kích hoạt: {gameObject.name}");

        if (enableButtonPressEffect)
        {
            StartCoroutine(ButtonPressFeedbackRoutine());
        }

        onInteract?.Invoke();

        if (singleUse)
        {
            hasInteracted = true;
        }
    }

    private IEnumerator ButtonPressFeedbackRoutine()
    {
        isPressingAnimation = true;

        Vector3 pressedLocalPos = originalLocalPos + new Vector3(0f, 0f, -pressDepth);

        while (Vector3.Distance(this.transform.localPosition, pressedLocalPos) > 0.002f)
        {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, pressedLocalPos, pressSpeed * Time.deltaTime);
            yield return null;
        }
        this.transform.localPosition = pressedLocalPos;

        yield return new WaitForSeconds(0.08f);

        while (Vector3.Distance(this.transform.localPosition, originalLocalPos) > 0.002f)
        {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, originalLocalPos, pressSpeed * Time.deltaTime);
            yield return null;
        }
        this.transform.localPosition = originalLocalPos;

        isPressingAnimation = false;
    }

    public void ResetInteraction()
    {
        hasInteracted = false;
        this.transform.localPosition = originalLocalPos;
    }
}
