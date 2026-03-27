using UnityEngine;
using UnityEngine.SceneManagement; 

public class RespawnManager : MonoBehaviour
{
    public Transform spawnPoint;
    public LayerMask groundLayer;
    public float maxRayDistance = 30f;
    public float timeBeforeRespawn = 2f;

    private PlayerControl playerControl;
    private Rigidbody rb;
    private float outOfBoundsTimer = 0f;

    void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}