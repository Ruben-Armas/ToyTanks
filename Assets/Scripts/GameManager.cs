using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject shieldPrefab;

    private Vector3 _playerStartPosition;
    private Vector3 _enemyStartPosition;
    private Vector3 _shieldStartPosition;

    private List<Player> listOfPlayers = new List<Player>();
    private List<Enemy> listOfEnemies = new List<Enemy>();

    private int _level;

    public int currentLevel { get; private set; }   //public leer, privado modificar
    public int maxLevel { get; private set; }

    void Start()
    {
        _level = 1;
        maxLevel = 0;

        _playerStartPosition = new Vector3(-24, 0, 5);
        _enemyStartPosition = new Vector3(24, 0, 0);
        _shieldStartPosition = new Vector3(0, 0, 0);

        BeginGame();
    }

    //SUSCRIPCIÓN al EVENTO
    void OnEnable()
    {
        Player.onPlayerCreated += OnPlayerCreated;
        Player.onPlayerDestroyed += OnPlayerDestroyed;
        Enemy.onEnemyCreated += OnEnemyCreated;
        Enemy.onEnemyDestroyed += OnEnemyDestroyed;
    }
    //DESUSCRIPCIÓN al EVENTO
    void OnDisable()
    {
        Player.onPlayerCreated -= OnPlayerCreated;
        Player.onPlayerDestroyed -= OnPlayerDestroyed;
        Enemy.onEnemyCreated -= OnEnemyCreated;
        Enemy.onEnemyDestroyed -= OnEnemyDestroyed;
    }
    //DELEGADOS
    private void OnPlayerCreated(Player playerCreated, Vector3 position)
    {
        //Añado el player creado a la lista de Players
        listOfPlayers.Add(playerCreated);
        Debug.Log(listOfEnemies.Count);
    }
    private void OnPlayerDestroyed(Player playerDestroyed, Vector3 position)
    {
        //Borro el player destruido de la lista de Players
        listOfPlayers.Remove(playerDestroyed);

        if (listOfPlayers.Count == 0)
            /**/OnRoundEnds();
            //HACER aquí la COMPROBACIÓN de si quedan más VIDAS???
    }

    private void OnEnemyCreated(Enemy enemyCreated, Vector3 position)
    {
        //Añado el enemigo creado a la lista de Enemigos
        listOfEnemies.Add(enemyCreated);
        Debug.Log(listOfEnemies.Count);
    }
    private void OnEnemyDestroyed(Enemy enemyDestroyed, Vector3 position)
    {
        //Borro el enemigo destruido de la lista de Enemigos
        listOfEnemies.Remove(enemyDestroyed);

        //REFACTORIZAR EVENTO   --- Crear una clase ENEMY independiente del control
        //Compruebo si no quedan enemigos para terminar la ronda    (lo mismo con los players)
        if (listOfEnemies.Count == 0)
            OnRoundEnds();
    }



    void BeginGame()
    {
        Debug.Log($"Nivel {_level}");
        currentLevel = _level;
        if (currentLevel > maxLevel)
            maxLevel = currentLevel;

        //Crea al jugador, enemigos y escudos
        //Vector3 playerStartPosition = stageGenerator.GetPlayerStartPosition();
        Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));

        //Vector3 nestStartPosition = stageGenerator.GetEnemyNestPosition(playerStartPosition);
        Instantiate(enemyPrefab, _enemyStartPosition, Quaternion.Euler(0, 270, 0));

        Instantiate(shieldPrefab, _shieldStartPosition, Quaternion.identity);
    }

    void OnRoundEnds()
    {
        Debug.Log(listOfEnemies.Count);
        Debug.Log($"Ronda {currentLevel} terminada!!!");

        //Si la lista de players = 0 -> Siguiente nivel
        if (listOfPlayers.Count == 0)
        {
            //Reinicio temporal PRUEBAS
            BeginGame();
        }
        //Si tienen más vidas
        //  No limpiar listas -> spawn de los enemigos que quedaban
        //                      NO de todos en BeginGame()
        //      En BeginGame -> Comprobar si hay una listEnemies, sino, crearlos
        //  Spawn de compañero (si lo había)
        //  Repetir el nivel
        //Sino OnGameOver()

        //Si la lista de  enemigos = 0 -> Siguiente nivel
        if (listOfEnemies.Count == 0)
        {
            //Limpia la escena y las listas
            ClearScene();

            //Reinicio temporal PRUEBAS
            BeginGame();
        }        
    }

    //Para cuando mueren los players y no tienen más vidas
    void OnGameOver()
    {
        Debug.Log($"Partida terminada en la ronda {currentLevel}");
    }

    void ClearScene()
    {
        //listOfPlayers.Clear();
        listOfEnemies.Clear();

    }
}
