using UnityEngine;

public class BGParallax : MonoBehaviour
{
    [Header("Camera & Speed Settings")]
    public Transform cameraTransform;

    [Range(0f, 1f)]
    public float parallaxEffectMultiplier = 0.8f;
    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, deltaMovement.y * parallaxEffectMultiplier, 0);
        lastCameraPosition = cameraTransform.position;
    }
}