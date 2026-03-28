using UnityEngine;

public class BallAfterImageSpawner : MonoBehaviour
{
    public float spawnInterval = 0.04f;
    public float minMoveDistance = 0.12f;
    public float ghostLifeTime = 0.4f;
    public Material ghostMaterial;

    private float timer;
    private Vector3 lastSpawnPos;

    void Start()
    {
        lastSpawnPos = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float moved = Vector3.Distance(transform.position, lastSpawnPos);

        if (timer >= spawnInterval && moved >= minMoveDistance)
        {
            SpawnGhost();
            timer = 0f;
            lastSpawnPos = transform.position;
        }
    }

    void SpawnGhost()
    {
        GameObject ghost = Instantiate(gameObject, transform.position, transform.rotation);

        PlayerControl pc = ghost.GetComponent<PlayerControl>();
        if (pc != null) Destroy(pc);

        BallAfterImageSpawner spawner = ghost.GetComponent<BallAfterImageSpawner>();
        if (spawner != null) Destroy(spawner);

        Rigidbody rb = ghost.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        Collider[] cols = ghost.GetComponentsInChildren<Collider>(true);
        foreach (var c in cols)
        {
            if (c != null) Destroy(c);
        }

        Vector3 moveDir = (transform.position - lastSpawnPos).normalized;
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            ghost.transform.position -= moveDir * 0.05f;
        }

        if (ghostMaterial != null)
        {
            Renderer[] rends = ghost.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in rends)
            {
                if (r == null) continue;

                Material[] mats = r.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = new Material(ghostMaterial);
                }
                r.materials = mats;
            }
        }

        BallAfterImageFade fade = ghost.AddComponent<BallAfterImageFade>();
        fade.lifeTime = ghostLifeTime;
        fade.startAlpha = 0.05f;  
    }
}