using UnityEngine;

public class TurretTrap : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 15f;
    public float fireRate = 2f;

    [Header("Detection Settings")]
    public float detectRange = 10f;
    public float viewAngle = 90f;
    public LayerMask obstacleLayer;

    [Header("Tracking Settings")]
    public float turnSpeed = 5f;
    private Transform target;
    private Vector3 defaultForward;

    [Header("Audio")]
    public AudioClip shootSound;
    [Range(0f, 1f)]
    public float volume = 0.5f;

    private bool canSeePlayer = false;
    private float nextFireTime = 0f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) target = playerObj.transform;

        defaultForward = transform.forward;
        defaultForward.z = 0f;
        defaultForward.Normalize();
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isGameActive) return;

        canSeePlayer = false;

        if (target != null)
        {
            Vector3 directionToPlayer = target.position - transform.position;
            directionToPlayer.z = 0f;
            float distance = directionToPlayer.magnitude;

            if (distance <= detectRange)
            {
                float angle = Vector3.Angle(defaultForward, directionToPlayer.normalized);

                if (angle <= viewAngle / 2f)
                {
                    RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToPlayer.normalized, distance, obstacleLayer, QueryTriggerInteraction.Ignore);
                    bool viewBlocked = false;

                    foreach (RaycastHit hit in hits)
                    {
                        if (!hit.collider.CompareTag("GravityPad"))
                        {
                            viewBlocked = true;
                            break;
                        }
                    }

                    if (!viewBlocked)
                    {
                        canSeePlayer = true;

                        if (directionToPlayer != Vector3.zero)
                        {
                            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
                        }

                        float aimAngle = Vector3.Angle(transform.forward, directionToPlayer.normalized);

                        if (aimAngle <= 5f && Time.time >= nextFireTime)
                        {
                            Shoot();
                            nextFireTime = Time.time + fireRate;
                        }
                    }
                }
            }
        }

        if (!canSeePlayer && defaultForward != Vector3.zero)
        {
            Quaternion resetRotation = Quaternion.LookRotation(defaultForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, resetRotation, Time.deltaTime * turnSpeed);
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);

            if (shootSound != null) AudioSource.PlayClipAtPoint(shootSound, transform.position, volume);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}