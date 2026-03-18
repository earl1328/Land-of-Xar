using UnityEngine;

public class FruitCollect : MonoBehaviour
{
    public int points = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        CheckPlayer(collision);
    }

    void CheckPlayer(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        ScoreManager.Instance.AddScore(points);
        Destroy(gameObject);
    }
}
