using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint archerTurret;
    public TurretBlueprint crossbowTurret;
    public TurretBlueprint golemTurret;
    public TurretBlueprint wizardTurret;
    public TurretBlueprint trollTurret;
    public TurretBlueprint pyromancerTurret;
    public TurretBlueprint firebowTurret;
    
    BuildManager buildManager;

    // START #########################################################

    void Start()
    {
        buildManager = BuildManager.instance;
    }

    // SELECT STANDARD TURRET #########################################################

    public void SelectArcherTurret()
    {
        Debug.Log("Archer Turret Selected");
        buildManager.SelectTurretToBuild(archerTurret);
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("ChooseArcher");
        }
    }

    // SELECT FAST TURRET #########################################################

    public void SelectCrossbowTurret()
    {
        Debug.Log("Crossbow Turret Selected");
        buildManager.SelectTurretToBuild(crossbowTurret);
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("ChooseCrossbow");
        }
    }

    // SELECT TROLL TURRET #########################################################
    public void SelectTrollTurret()
    {
        Debug.Log("Troll Turret Selected");
        buildManager.SelectTurretToBuild(trollTurret);
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("ChooseTroll");
        }
    }

    // SELECT GOLEM TURRET #########################################################

    public void SelectGolemTurret()
    {
        Debug.Log("Golem Turret Selected");
        buildManager.SelectTurretToBuild(golemTurret);
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("ChooseGolem");
        }
    }

    // SELECT WIZARD TURRET #########################################################

    public void SelectWizardTurret()
    {
        Debug.Log("Wizard Turret Selected");
        buildManager.SelectTurretToBuild(wizardTurret);
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("ChooseWizard");
        }
    }

    // SELECT PYROMANCER TURRET #########################################################
    public void SelectPyromancerTurret()
    {
        Debug.Log("Pyromancer Turret Selected");
        buildManager.SelectTurretToBuild(pyromancerTurret);
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("ChoosePyromancer");
        }
    }

    // SELECT FIREBOW TURRET #########################################################
    public void SelectFirebowTurret()
    {
        Debug.Log("Firebow Turret Selected");
        buildManager.SelectTurretToBuild(firebowTurret);
        AudioManager am = AudioManager.instance != null ? AudioManager.instance : FindObjectOfType<AudioManager>();
        if (am != null)
        {
            am.Play("ChooseFirebow");
        }
    }
}
