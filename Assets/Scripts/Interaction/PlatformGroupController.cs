using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGroupController : MonoBehaviour
{
    public enum RetractPreset
    {
        CustomOffset,
        DownIntoPit,
        IntoWallRight_PosZ,
        IntoWallLeft_NegZ,
        IntoWallBack_NegX,
        IntoWallFront_PosX
    }

    [System.Serializable]
    public class PlatformStep
    {
        public string stepName = "Bậc platform";
        public Transform platformTransform;
        public RetractPreset preset = RetractPreset.IntoWallRight_PosZ;

        [Tooltip("Khoảng cách / Vector dịch chuyển khi thu gọn trong không gian World (X, Y, Z)")]
        public Vector3 retractOffset = new Vector3(0f, 0f, -6f);

        [HideInInspector] public Vector3 openWorldPosition;
        [HideInInspector] public Vector3 closedWorldPosition;
    }

    [Header("Danh Sách Các Bậc Platform")]
    [SerializeField] private List<PlatformStep> platformSteps = new List<PlatformStep>();

    [Header("Cài Đặt Chuyển Động")]
    [SerializeField] private float transitionSpeed = 4f;
    [SerializeField] private float delayBetweenSteps = 0.15f;

    private bool isExtended = false;
    public bool IsExtended => isExtended;

    private void Awake()
    {
        InitializePlatformPositions();
        RetractAllImmediate();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        InitializePlatformPositions();
    }
#endif

    public void InitializePlatformPositions()
    {
        foreach (var step in platformSteps)
        {
            if (step.platformTransform == null) continue;

            step.openWorldPosition = step.platformTransform.position;

            switch (step.preset)
            {
                case RetractPreset.DownIntoPit:
                    step.retractOffset = new Vector3(0f, -10f, 0f);
                    break;
                case RetractPreset.IntoWallRight_PosZ:
                    step.retractOffset = new Vector3(0f, 0f, -6f);
                    break;
                case RetractPreset.IntoWallLeft_NegZ:
                    step.retractOffset = new Vector3(0f, 0f, 6f);
                    break;
                case RetractPreset.IntoWallBack_NegX:
                    step.retractOffset = new Vector3(-6f, 0f, 0f);
                    break;
                case RetractPreset.IntoWallFront_PosX:
                    step.retractOffset = new Vector3(6f, 0f, 0f);
                    break;
            }

            step.closedWorldPosition = step.openWorldPosition + step.retractOffset;
        }
    }

    [ContextMenu("Thử Thu Cầu (Retract All)")]
    public void RetractAllImmediate()
    {
        isExtended = false;
        foreach (var step in platformSteps)
        {
            if (step.platformTransform == null) continue;
            step.platformTransform.position = step.closedWorldPosition;
        }
    }

    [ContextMenu("Thử Mở Cầu (Extend Platforms)")]
    public void ExtendPlatforms()
    {
        if (isExtended)
        {
            Debug.Log("[PlatformGroupController] Các bậc platform đã mở rồi!");
            return;
        }

        isExtended = true;
        Debug.Log($"<color=green>[PlatformGroupController] ĐANG MỞ RỘNG {platformSteps.Count} BẬC PLATFORM VỀ VỊ TRÍ GỐC!</color>");
        StartCoroutine(ExtendSequenceCoroutine());
    }

    private IEnumerator ExtendSequenceCoroutine()
    {
        foreach (var step in platformSteps)
        {
            if (step.platformTransform != null)
            {
                StartCoroutine(MoveStepSmooth(step.platformTransform, step.openWorldPosition));
                yield return new WaitForSeconds(delayBetweenSteps);
            }
        }
    }

    private IEnumerator MoveStepSmooth(Transform target, Vector3 targetWorldPos)
    {
        while (Vector3.Distance(target.position, targetWorldPos) > 0.005f)
        {
            target.position = Vector3.MoveTowards(
                target.position,
                targetWorldPos,
                transitionSpeed * Time.deltaTime
            );
            yield return null;
        }

        target.position = targetWorldPos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (platformSteps == null) return;

        foreach (var step in platformSteps)
        {
            if (step.platformTransform != null)
            {
                Vector3 currentPos = step.platformTransform.position;
                Vector3 targetPos = currentPos + step.retractOffset;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(currentPos, targetPos);
                Gizmos.DrawWireCube(targetPos, Vector3.one * 0.3f);
            }
        }
    }
#endif
}
