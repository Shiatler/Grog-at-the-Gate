using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-101)]
public class WaveSpawner : MonoBehaviour
{


    public Wave[] waves;
    public Transform spawnPoint;

    public float timeBetweenWaves = 5f;
    private float countdown = 2f;

    public Text waveCountdownText;
    public Text waveCountText;

    public GameManager gameManager;

    public static int enemiesAlive = 0;
    private int waveIndex = 0;
    private bool isSpawning = false;
    private bool needsCountdownReset = false;
    private bool allWavesSpawned = false;

    // START #########################################################
    void Start()
    {
        // Reset static state when scene loads
        enemiesAlive = 0;
        waveIndex = 0;
        isSpawning = false;
        needsCountdownReset = false;
        countdown = 2f;
        // Ensure time scale is reset (safety check)
        Time.timeScale = 1f;
        
        // Auto-find GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    // UPDATE #########################################################
    void Update()
    {
        // Check if level is complete (all waves spawned AND all enemies defeated)
        if (allWavesSpawned && enemiesAlive == 0)
        {
            Debug.Log($"Level Complete! All waves spawned and all enemies defeated.");
            if (gameManager != null)
            {
                gameManager.LevelWon();
            }
            else
            {
                Debug.LogWarning("GameManager is null! Cannot complete level.");
            }
            this.enabled = false;
            return;
        }

        if (enemiesAlive > 0)
        {
            needsCountdownReset = true; // Mark that we need to reset when enemies die
            return;
        }
        
        // Reset countdown once when all enemies are dead and not spawning
        if (!isSpawning && needsCountdownReset)
        {
            countdown = timeBetweenWaves;
            needsCountdownReset = false;
        }
        
        if (countdown <= 0f)
        {
            if (!isSpawning && waves != null && waveIndex < waves.Length) // Only start if not already spawning and waves remain
            {
                StartCoroutine(SpawnWave());
                AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
                if (am != null)
                {
                    am.Play("WarHorn");
                }
            }
            return;
        }

        if (PlayerStats.Rounds == 0)
        {
            waveCountText.text = "Waves Survived: 0 / " + waves.Length.ToString();
        } else
        {
            waveCountText.text = "Waves Survived: " + PlayerStats.Rounds.ToString() + " / " + waves.Length.ToString();
        }

        countdown -= Time.deltaTime;

        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

        waveCountdownText.text = string.Format("Wave in:{0:00}  ", countdown);
    }

    // SPAWN WAVE #########################################################
    IEnumerator SpawnWave()
    {
        isSpawning = true;
        PlayerStats.Rounds++;

        Wave wave = waves[waveIndex];

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemy);
            yield return new WaitForSeconds(1f / wave.rate);
        }

        waveIndex++;
        isSpawning = false;
        
        // Check if all waves have been spawned
        if (waveIndex >= waves.Length)
        {
            allWavesSpawned = true;
            Debug.Log($"All waves spawned! waveIndex={waveIndex}, waves.Length={waves.Length}, enemiesAlive={enemiesAlive}");
        }
        // Don't reset countdown here - let it reset when enemies are dead in Update()
    }

    // SPAWN ENEMY #########################################################

    void SpawnEnemy(GameObject enemy)
    {
        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;
    }


}
