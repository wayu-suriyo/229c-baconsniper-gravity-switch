using UnityEngine;

public class AutoGravityPad : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControl playerCtrl = other.GetComponent<PlayerControl>();

            if (playerCtrl != null)
            {
                playerCtrl.ForceFlip();
            }
        }
    }
}