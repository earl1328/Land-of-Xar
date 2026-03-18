using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextLevelTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName;
    public float delayBeforeLoad = 1.5f;

    private bool isActivated = false;

    private FlagAnimation flagAnim;

    void Start()
    {
        flagAnim = GetComponent<FlagAnimation>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CheckPlayer(other);
    }

    void CheckPlayer(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            Activate();
        }
    }

    void Activate()
    {
        isActivated = true;

        PlayAnimation();
        StartCoroutine(LoadNextScene());
    }

    void PlayAnimation()
    {
        if (flagAnim != null)
        {
            flagAnim.PlayFlag();
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(nextSceneName);
    }
}
