using UnityEngine;

[DefaultExecutionOrder(-99)]
public class HandleUI : MonoBehaviour
{
    public GameObject canvasUI;
    public GameObject canvasShopUI;
    public GameObject levelWonUI;

    void Update()
    {
        if (levelWonUI.activeSelf)
        {
            hideUI();
        }
    }

    public void showUI()
    {
        canvasUI.SetActive(true);
        canvasShopUI.SetActive(true);
    }

    public void hideUI()
    {
        canvasUI.SetActive(false);
        canvasShopUI.SetActive(false);
    }
}
