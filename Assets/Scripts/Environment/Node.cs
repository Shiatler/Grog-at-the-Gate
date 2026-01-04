using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{

    public Material hoverColor;
    public Material notEnoughMoneyColor;
    public Vector3 positionOffset;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBlueprint turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    private Renderer rend;
    private Material startMaterial;

    BuildManager buildManager;

    // START #########################################################

    void Start ()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            startMaterial = rend.material;
        }

        buildManager = BuildManager.instance;
    }

    public Vector3 GetBuildPosition ()
    {
        return transform.position + positionOffset;
    }

    // ON MOUSE DOWN #########################################################

    void OnMouseDown ()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
            
        if (turret != null)
        {
            buildManager.SelectNode(this);
            return;
        }

        if (!buildManager.CanBuild)
            return;

        BuildTurret(buildManager.GetTurretToBuild());
    }

    // BUILD TURRET #########################################################

    void BuildTurret (TurretBlueprint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerStats.Money -= blueprint.cost;

        GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBlueprint = blueprint;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Debug.Log("Turret built!");
    }

    // UPGRADE TURRET #########################################################
    public void UpgradeTurret()
    {
        if (PlayerStats.Money < turretBlueprint.upgradeCost)
        {
            Debug.Log("Not enough money to upgrade that!");
            return;
        }

        PlayerStats.Money -= turretBlueprint.upgradeCost;

        // Destroy the old turret
        Destroy(turret);

        // Build the new turret
        GameObject _turret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        isUpgraded = true;

        Debug.Log("Turret upgraded!");
    }

    // SELL TURRET #########################################################
    public void SellTurret()
    {
        PlayerStats.Money += turretBlueprint.GetSellAmount();

        // Spawn cool effect
        GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);
        
        Destroy(turret);
        turretBlueprint = null;
    }
    // ON MOUSE ENTER #########################################################

    void OnMouseEnter ()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (rend == null || buildManager == null)
            return;

        if (!buildManager.CanBuild)
            return;

        if (buildManager.HasMoney)
        {
            if (hoverColor != null)
                rend.material = hoverColor;
        } else
        {
            if (notEnoughMoneyColor != null)
                rend.material = notEnoughMoneyColor;
        }
    }

    // ON MOUSE EXIT #########################################################

    void OnMouseExit ()
    {
        if (rend == null || startMaterial == null)
            return;

        rend.material = startMaterial;
    }

}
