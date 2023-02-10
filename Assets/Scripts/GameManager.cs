using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject shieldPrefab;

    private Vector3 playerStartPosition;
    private Vector3 enemyStartPosition;

    void Start()
    {
        playerStartPosition = new Vector3(-24, 0, 5);

        enemyStartPosition = new Vector3(24, 0, 0);

        BeginGame();
    }

    void BeginGame()
    {
        //Vector3 playerStartPosition = stageGenerator.GetPlayerStartPosition();


        Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);

        
        //Vector3 nestStartPosition = stageGenerator.GetEnemyNestPosition(playerStartPosition);
        Instantiate(enemyPrefab, enemyStartPosition, Quaternion.identity);
    }


}
