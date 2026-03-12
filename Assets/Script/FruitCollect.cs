using UnityEngine;

public class FruitCollect : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Fruit Collected!");
            Destroy(gameObject);
        }
    }
}
