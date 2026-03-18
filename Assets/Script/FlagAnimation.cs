using UnityEngine;

public class FlagAnimation : MonoBehaviour
{
    private Animator anim;
    private Vector3 startPosition;

    void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void LateUpdate()
    {
        // Lock position so it doesn't move
        transform.position = startPosition;
    }

    public void PlayFlag()
    {
        if (anim != null)
        {
            anim.SetTrigger("Activate");
        }
    }

    // Detect player entering the checkpoint
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayFlag();
        }
    }
}
