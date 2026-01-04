using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-101)]
public class GameManager : MonoBehaviour
{
    public static bool GameIsOver;

    [Header("UI")]
    public GameObject gameOverUI;
    public GameObject levelWonUI;

    public GameObject shopUI;
    public GameObject framesUI;
    public GameObject showUpgradesUI;
    public GameObject canvasUI;

    [Header("Buttons")]
    public Button shopButton;
    public Button upgradesButton;


    void Start()
    {
        GameIsOver = false;
        Time.timeScale = 1f;

        framesUI.SetActive(true);
        shopUI.SetActive(true);
        showUpgradesUI.SetActive(false);

        RefreshButtons();
        RefreshHoverStates();
    }

    void Update()
    {
        if (GameIsOver) return;

        if (PlayerStats.Lives <= 0)
        {
            EndGame();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
            ToggleShop();
    }

    void EndGame()
    {
        GameIsOver = true;

        gameOverUI.SetActive(true);
        framesUI.SetActive(false);
        shopUI.SetActive(false);
        showUpgradesUI.SetActive(false);
        canvasUI.SetActive(false);

        RefreshButtons();
        RefreshHoverStates();
    }

    public void LevelWon()
    {
        GameIsOver = true;
        levelWonUI.SetActive(true);

        RefreshButtons();
        RefreshHoverStates();
    }

    public void ToggleShop()
    {
        if (GameIsOver) return;

        bool shopModeOpen = framesUI.activeSelf;
        if (shopModeOpen)
        {
            framesUI.SetActive(false);
            shopUI.SetActive(false);
            showUpgradesUI.SetActive(false);
        }
        else
        {
            framesUI.SetActive(true);
            shopUI.SetActive(true);
            showUpgradesUI.SetActive(false);
        }

        RefreshButtons();
        RefreshHoverStates();
    }

    public void ToggleUpgrades()
    {
        if (GameIsOver) return;

        if (!framesUI.activeSelf) return;

        bool openingUpgrades = !showUpgradesUI.activeSelf;

        if (openingUpgrades)
        {
            showUpgradesUI.SetActive(true);
            shopUI.SetActive(false); framesUI.SetActive(true);
        }
        else
        {
            showUpgradesUI.SetActive(false);
            shopUI.SetActive(true); framesUI.SetActive(true);
        }

        RefreshButtons();
        RefreshHoverStates();
    }

    private void RefreshButtons()
    {
        if (upgradesButton != null)
            upgradesButton.interactable = framesUI.activeSelf && !GameIsOver;

        if (shopButton != null)
            shopButton.interactable = !showUpgradesUI.activeSelf && !GameIsOver;
    }

    private void RefreshHoverStates()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        ResetButtonAnimator(shopButton);
        ResetButtonAnimator(upgradesButton);
    }

    private void ResetButtonAnimator(Button btn)
    {
        if (btn == null) return;

        if (!btn.interactable) return;

        var anim = btn.GetComponent<Animator>();
        if (anim == null) return;

        anim.Rebind();
        anim.Update(0f);
    }
}
