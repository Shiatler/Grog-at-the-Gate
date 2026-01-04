using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    public GameObject rangeIndicator;
    private Node target;

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
        FindObjectOfType<AudioManager>().Play("TurretSelect");

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
