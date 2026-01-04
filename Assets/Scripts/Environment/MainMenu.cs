using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelSelectScene = "LevelSelect";
    public SceneFader sceneFader;

    // PLAY #########################################################
    public void Play()
    {
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("ButtonClick");
        }
        Time.timeScale = 1f; // Reset time scale before loading game scene
        sceneFader.FadeTo(levelSelectScene);
    }

    // QUIT #########################################################
    public void Quit()
    {
        Debug.Log("Exiting");
        Application.Quit();
    }
}
