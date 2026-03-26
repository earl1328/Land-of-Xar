using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start the game at Level 1
    public void StartGame()
    {
        SceneManager.LoadScene("Level 1"); // make sure the name matches your scene
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); // works in build
    }
}
