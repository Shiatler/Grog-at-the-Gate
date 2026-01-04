using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WavesSurvived : MonoBehaviour
{
    public Text roundsText;

    // ON ENABLE #########################################################
    void OnEnable()
    {
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        roundsText.text = "0";
        int round = 0;

        yield return new WaitForSeconds(0.5f);

        while (round < PlayerStats.Rounds)
        {
            round++;
            roundsText.text = round.ToString();

            yield return new WaitForSeconds(0.05f);
        }
    }
}
