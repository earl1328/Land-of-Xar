using UnityEngine;

public class MovingSawTrap : MonoBehaviour
{
    [Header("Movement")]
    public Transform pointA;   // Start point
    public Transform pointB;   // End point
    public float speed = 3f;   // Movement speed

    private Transform target;

    void Start()
    {
        // Start moving toward pointB
        target = pointB;
    }

    void Update()
    {
        MoveSaw();
    }

    void MoveSaw()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Switch target when reaching the point
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = target == pointA ? pointB : pointA;
        }
    }

    // Player contact
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
    }
}
