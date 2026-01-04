using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public Slider livesSlider;
    private PlayerStats playerStats;

    // START #########################################################

    void Start()
    {
        // Find PlayerStats instance to get startLives
        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            livesSlider.maxValue = playerStats.startLives;
            livesSlider.value = PlayerStats.Lives;
        }
    }

    // UPDATE #########################################################

    void Update()
    {
        // Update slider value to match PlayerStats.Lives
        livesSlider.value = PlayerStats.Lives;
    }

    public void SetMaxLives(int lives)
    {
        livesSlider.maxValue = lives;
        livesSlider.value = lives;
    }

    public void SetLives(int lives)
    {
        livesSlider.value = lives;
    }
}
