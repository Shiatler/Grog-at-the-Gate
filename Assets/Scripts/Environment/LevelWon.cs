using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelWon : MonoBehaviour
{
    public string nextLevel = "Level02";
    public int levelToUnlock = 2;

    public SceneFader sceneFader;
    public string menuSceneName = "MainMenu";

    // CONTINUE #########################################################
    public void Continue()
    {
        PlayerPrefs.SetInt("levelReached", levelToUnlock);
        
        // Transition to next level after a short delay
        if (sceneFader != null)
        {
            sceneFader.FadeTo(nextLevel);
        }
        else
        {
            Debug.LogWarning("SceneFader is null! Cannot transition to next level.");
        }
    }

    // MENU #########################################################
    public void Menu()
    {
        sceneFader.FadeTo(menuSceneName);
    }
}
