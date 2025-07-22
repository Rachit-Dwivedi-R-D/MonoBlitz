using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public GameObject startMenuUI;     // Your Start Menu (buttons)
    public GameObject multiplayerUI;   // The existing Multiplayer UI Canvas

    private void Start()
    {
        CursorManager.instance?.SetDefaultCursor();
        AudioManager.instance?.PlayMusic(); // Only if music is not playing
    }

    public void OnMultiplayerClicked()
    {
        startMenuUI.SetActive(false);
        multiplayerUI.SetActive(true);
    }

    public void OnSinglePlayerClicked()
    {
        SceneManager.LoadScene("Single Player Game"); // Use exact name of the scene
    }

    public void OnQuitClicked()
    {
        AudioManager.instance?.PlaySoundEffect("Click");
        Debug.Log("Quit button pressed.");
        Application.Quit();
    }
}
