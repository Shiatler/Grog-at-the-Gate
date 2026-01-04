using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public SceneFader fader;

    public Button [] levelButtons;

    void Start()
    {

        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
                levelButtons[i].interactable = false;
        }
    }

    public void Select (string levelName)
    {
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("SelectLevel");
        }
        fader.FadeTo(levelName);
    }
}
