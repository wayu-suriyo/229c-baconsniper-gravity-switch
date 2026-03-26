using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveOffset = new Vector3(5f, 0f, 0f);
    public float speed = 0.5f;

    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + moveOffset;
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = Vector3.Lerp(startPos, endPos, t);
    }
}