using UnityEngine;

public class HazardZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>() 
                             ?? other.GetComponentInParent<PlayerMovement>()
                             ?? other.GetComponentInChildren<PlayerMovement>();

        if (player != null && !player.IsDead)
        {
            Debug.Log("[HazardZone] Nhân vật chạm vào vùng nguy hiểm");
            player.Die();
        }
    }
}
