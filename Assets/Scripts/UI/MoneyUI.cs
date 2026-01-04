using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{

    public Text moneyText;

    // UPDATE #########################################################

    // Update is called once per frame
    void Update()
    {
        moneyText.text = "  $$" + PlayerStats.Money.ToString();
    }
}
