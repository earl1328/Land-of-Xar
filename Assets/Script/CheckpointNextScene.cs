using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CheckpointNextScene : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName;
    public float delayBeforeLoad = 1.5f;

    private bool isActivated = false;
    private Animator anim;

    private Vector3 startPosition; // 🔥 store original position

    void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position; // save position
    }

    void LateUpdate()
    {
        // 🔥 force object to stay in place
        if (isActivated)
        {
            transform.position = startPosition;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CheckPlayer(other);
    }

    void CheckPlayer(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        isActivated = true;

        PlayAnimation();
        StartCoroutine(LoadAfterDelay());
    }

    void PlayAnimation()
    {
        if (anim != null)
        {
            anim.SetTrigger("Activate");
        }
    }

    IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
