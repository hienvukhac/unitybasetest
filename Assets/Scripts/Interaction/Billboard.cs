using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera targetCamera;

    private void Start()
    {
        FindTargetCamera();
    }

    private void FindTargetCamera()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = FindObjectOfType<Camera>();
            }
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            FindTargetCamera();
            if (targetCamera == null) return;
        }

        transform.rotation = targetCamera.transform.rotation;
    }
}
