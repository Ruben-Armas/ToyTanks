using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerInput))]
public class InGameOverView : MonoBehaviour
{
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI record;
    public TextMeshProUGUI totalKills;
    public TextMeshProUGUI totalDeaths;
    public TextMeshProUGUI deathsP1;
    public TextMeshProUGUI deathsP2;
    public TextMeshProUGUI killsP1;
    public TextMeshProUGUI killsP2;


    private void Awake()
    {
        int getCurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        int getRecord = PlayerPrefs.GetInt("Record", 0);
        int getTotalKills = PlayerPrefs.GetInt("TotalKills", 0);
        int getTotalDeaths = PlayerPrefs.GetInt("TotalDeaths", 0);
        int getDeathsP1 = PlayerPrefs.GetInt("NumOfDeathsPlayer1", 0);
        int getDeathsP2 = PlayerPrefs.GetInt("NumOfDeathsPlayer2", -1);
        int getkillsP1 = PlayerPrefs.GetInt("NumOfkillsPlayer1", 0);
        int getkillsP2 = PlayerPrefs.GetInt("NumOfkillsPlayer2", -1);

        currentLevel.text = $"Level: <color=#e60000><b>{getCurrentLevel}</b>";
        record.text = $"Record: <color=#e60000><b>{getRecord}</b>";
        totalKills.text = $"<color=#e60000><b>{getTotalKills}</b>";
        totalDeaths.text = $"<color=#e60000><b>{getTotalDeaths}</b>";
        deathsP1.text = $"<color=#e60000><b>{getDeathsP1}</b>";
        killsP1.text = $"<color=#e60000><b>{getkillsP1}</b>";
        if (getDeathsP2 == -1 || getkillsP2 == -1)
        {
            deathsP2.text = $"<color=#e60000><b>Not</b>";
            killsP2.text = $"<color=#e60000><b>Playing</b>";
        }
        else
        {
            deathsP2.text = $"<color=#e60000><b>{getDeathsP2}</b>";
            killsP2.text = $"<color=#e60000><b>{getkillsP2}</b>";
        }
    }
}