using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject ui;
    public SceneFader sceneFader;

    public string menuSceneName = "MainMenu";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Toggle();
        }
    }

    // TOGGLE #########################################################
    public void Toggle()
    {
        ui.SetActive(!ui.activeSelf);
        if (ui.activeSelf)
        {
            Time.timeScale = 0f;
        } else
        {
            Time.timeScale = 1f;
        }
    }

    // RESTART #########################################################
    public void Restart()
    {
        Time.timeScale = 1f; // Reset time scale before reloading scene
        Toggle();
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    // MENU #########################################################
    public void Menu()
    {
        Time.timeScale = 1f; // Reset time scale before loading menu
        Toggle();
        sceneFader.FadeTo(menuSceneName);
    }
}
