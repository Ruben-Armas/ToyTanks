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
    public TextMeshProUGUI enemyDestroyed;
    public TextMeshProUGUI deaths;
    public TextMeshProUGUI deathsP1;
    public TextMeshProUGUI deathsP2;


    private void Awake()
    {
        int getCurrentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        int getRecord = PlayerPrefs.GetInt("Record", 0);
        int getEnemyDestroyed = PlayerPrefs.GetInt("NumOfEnemiesDestroyed", 0);
        int getDeaths = PlayerPrefs.GetInt("NumOfDeaths", 0);
        int getDeathsP1 = PlayerPrefs.GetInt("NumOfDeathsPlayer1", 0);

        int getDeathsP2 = PlayerPrefs.GetInt("NumOfDeathsPlayer2", -1);

        currentLevel.text = $"Current Level: <color=red><b>{getCurrentLevel}</b>";
        record.text = $"Record: <color=red><b>{getRecord}</b>";
        enemyDestroyed.text = $"Enemies destroyed: <color=red><b>{getEnemyDestroyed}</b>";
        deaths.text = $"Deaths: <color=red><b>{getDeaths}</b>";
        deathsP1.text = $"P1 Deaths: <color=red><b>{getDeathsP1}</b>";
        deathsP2.text = $"P2 Deaths: <color=red><b>{getDeathsP2}</b>";
    }

    void Update()
    {
    }
}