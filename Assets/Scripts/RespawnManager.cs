using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    public LayerMask groundLayer;
    public float maxRayDistance = 30f;
    public float timeBeforeRespawn = 2f;

    [Header("Death Settings")]
    public float deathDelay = 1f;

    private PlayerControl playerControl;
    private Rigidbody rb;
    private float outOfBoundsTimer = 0f;
    private bool isDead = false; 

    void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDead) return; 

        Vector3 checkDirection = playerControl.isFlipped ? Vector3.up : Vector3.down;
        bool hitGround = Physics.Raycast(transform.position, checkDirection, maxRayDistance, groundLayer);

        if (!hitGround)
        {
            outOfBoundsTimer += Time.deltaTime;

            if (outOfBoundsTimer >= timeBeforeRespawn)
            {
                Respawn();
            }
        }
        else
        {
            outOfBoundsTimer = 0f;
        }
    }

    public void Respawn()
    {
        if (isDead) return; 
        isDead = true;

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        if (playerControl != null) playerControl.canControl = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        yield return new WaitForSeconds(deathDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}