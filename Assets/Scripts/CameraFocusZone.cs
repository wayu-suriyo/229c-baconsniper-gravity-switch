using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CameraFocusZone : MonoBehaviour
{
    private BoxCollider zoneCollider;
    private CameraFollow camFollow;

    void Start()
    {
        zoneCollider = GetComponent<BoxCollider>();
        zoneCollider.isTrigger = true;

        if (Camera.main != null)
        {
            camFollow = Camera.main.GetComponent<CameraFollow>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && camFollow != null)
        {
            camFollow.currentFocusZone = zoneCollider;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && camFollow != null)
        {
            if (camFollow.currentFocusZone == zoneCollider)
            {
                camFollow.currentFocusZone = null;
            }
        }
    }
}