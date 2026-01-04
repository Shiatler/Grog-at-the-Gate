using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TurretInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject infoPanel;

    void Start()
    {
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (infoPanel != null) infoPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null) infoPanel.SetActive(false);
    }
}
