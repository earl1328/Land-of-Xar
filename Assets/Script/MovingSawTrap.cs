using UnityEngine;

public class MovingSawTrap : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform[] points;   // add as many points as you want
    public float speed = 3f;

    private int currentPoint = 0;

    void Update()
    {
        MoveSaw();
    }

    void MoveSaw()
    {
        if (points.Length == 0) return;

        Transform target = points[currentPoint];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint++;

            if (currentPoint >= points.Length)
            {
                currentPoint = 0; // loop back to first point
            }
        }
    }

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
