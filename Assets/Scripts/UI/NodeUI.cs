using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    public GameObject rangeIndicator;
    private Node target;
    private Outline currentTurretOutline;

    public Button upgradeButton;
    public TextMeshProUGUI upgradeCost;
    public TextMeshProUGUI sellAmount;

    // SET TARGET #########################################################

    public void SetTarget(Node _target)
    {
        target = _target;
        transform.position = target.GetBuildPosition();

        if (!target.isUpgraded && target.turretBlueprint.useUpgrades)
        {
            upgradeCost.text = "$$" + target.turretBlueprint.upgradeCost;
            upgradeButton.interactable = true;
        } else
        {
            upgradeCost.text = "MAXED";
            upgradeButton.interactable = false;
        }

        if (!target.isUpgraded)
        {
            sellAmount.text = "$$" + target.turretBlueprint.GetSellAmount();        
        } else
        {
            sellAmount.text = "$$" + target.turretBlueprint.GetUpgradedSellAmount();
        }
        
        ui.SetActive(true);

        // Highlight the turret with outline
        if (target.turret != null)
        {
            try
            {
                Outline outline = target.turret.GetComponent<Outline>();
                if (outline == null)
                {
                    outline = target.turret.AddComponent<Outline>();
                    outline.OutlineColor = Color.green;
                    outline.OutlineWidth = 5f;
                }
                outline.enabled = true;
                currentTurretOutline = outline;
            }
            catch (System.Exception e)
            {
                // Mesh might not be readable, skip outline highlighting
                Debug.LogWarning("Could not add outline to turret: " + e.Message);
                currentTurretOutline = null;
            }
        }

        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("TurretSelect");
        }

        rangeIndicator.SetActive(true);
        rangeIndicator.transform.position = target.GetBuildPosition();
        Turret turretComponent = target.turret.GetComponent<Turret>();
        if (turretComponent != null)
        {
            float range = turretComponent.range;
            rangeIndicator.transform.localScale = new Vector3(range, 0.5f, range);
        }
    }

    // HIDE #########################################################
    
    public void Hide()
    {
        ui.SetActive(false);
        rangeIndicator.SetActive(false);
        
        // Remove highlight from turret
        if (currentTurretOutline != null)
        {
            currentTurretOutline.enabled = false;
            currentTurretOutline = null;
        }
    }
    
    // UPGRADE #########################################################

    public void Upgrade()
    {
        target.UpgradeTurret();
        BuildManager.instance.DeselectNode();
    }

    // SELL #########################################################
    public void Sell()
    {
        target.SellTurret();
        BuildManager.instance.DeselectNode();
    }
}
