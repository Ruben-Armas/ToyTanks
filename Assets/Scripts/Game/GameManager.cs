using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public int maxPlayers = 2;
    public MapGenerator mapGenerator;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject player2Prefab;
    public GameObject enemyPrefab;
    public GameObject shieldPrefab;
    public GameObject obstaclesPrefab1;
    public GameObject obstaclesPrefab2;

    [Header("Enemigos")]
    public int minimumAmount = 1;
    public int maximumAmount = 4;

    //--TEMPORAL--
    private Vector3 _playerStartPosition;
    private Vector3 _enemyStartPosition;
    private Vector3 _shieldStartPosition;

    private List<Player> listOfPlayers = new List<Player>();
    private List<Enemy> listOfEnemies = new List<Enemy>();  //Lista que controlará la cantidad de enemigos vivos en la escena
    private List<EnemyData> listOfEnemiesDataTemp = new List<EnemyData>();  //Lista que guarda los datos de los supervivientes para reInstanciarlos

    private bool _flagReplay = false;   //Controla que listOfEnemies se modifique solo durante la partida
    
    private int _level;
    //private int _startLives;
    public int _initialNumPlayers;

    public int currentLevel { get; private set; }   //public leer, privado modificar
    public int record { get; private set; }
    public int currentLives;// { get; private set; }

    private GameObject _obstacles;

    private InGameView _inGameView;
    private MainMenu _mainMenu;
    private MenuManager _menuManager;


    private void Awake()
    {
        _mainMenu = new MainMenu();
        _menuManager = new MenuManager();
    }
    void Start()
    {
        _level = 1;
        record = 0;
        //Temporal
        //_initialNumPlayers = 0;

        //Pos temporales
        _playerStartPosition = new Vector3(-24, 0, 5);
        _enemyStartPosition = new Vector3(24, 0, 0);
        _shieldStartPosition = new Vector3(0, 0, 0);

        //Generar obstáculos
        //mapGenerator.GenerateMap();

        if (_level == 1)
        {
            _obstacles = Instantiate(obstaclesPrefab1, Vector3.zero, Quaternion.identity);
            StartCoroutine(mapGenerator.DoCreateNavMesh());
        }

        PlayGame();
    }

    //SUSCRIPCIÓN al EVENTO
    void OnEnable()
    {
        Player.onPlayerCreated += OnPlayerCreated;
        Player.onPlayerDestroyed += OnPlayerDestroyed;
        Enemy.onEnemyCreated += OnEnemyCreated;
        Enemy.onEnemyDestroyed += OnEnemyDestroyed;
        PlayerInputHandler.onPause += OnPause;
    }
    //DESUSCRIPCIÓN al EVENTO
    void OnDisable()
    {
        Player.onPlayerCreated -= OnPlayerCreated;
        Player.onPlayerDestroyed -= OnPlayerDestroyed;
        Enemy.onEnemyCreated -= OnEnemyCreated;
        Enemy.onEnemyDestroyed -= OnEnemyDestroyed;
        PlayerInputHandler.onPause -= OnPause;
    }
    //DELEGADOS
    private void OnPlayerCreated(Player playerCreated, Vector3 startPosition)
    {
        //Añado el player creado a la lista de Players
        listOfPlayers.Add(playerCreated);
        //Debug.Log($"Nº de Jugadores {listOfPlayers.Count}");
    }
    private void OnPlayerDestroyed(Player playerDestroyed, Vector3 startPosition)
    {
        //Borro el player destruido de la lista de Players
        listOfPlayers.Remove(playerDestroyed);

        //Compruebo si no quedan players para terminar la ronda
        if (listOfPlayers.Count == 0)
            OnPlayerRoundEnds();
    }

    private void OnEnemyCreated(Enemy enemyCreated, Vector3 startPosition)
    {
        //Informo de que se ha creado
        //Enemy se añade la lista de Enemigos en CreateEnemies()
        if (_flagReplay == false)
        {
            //Debug.Log("Creando enemy");
        }
    }
    private void OnEnemyDestroyed(Enemy enemyDestroyed, Vector3 startPosition)
    {
        //Borro el enemigo destruido de la lista de Enemigos
        if (_flagReplay == false)
        {
            //Debug.Log("Borrando enemy");
            listOfEnemies.Remove(enemyDestroyed);
        }
        //Compruebo si no quedan enemigos para terminar la ronda
        if (listOfEnemies.Count == 0)
            OnEnemyRoundEnds();
    }

    private void OnPause()
    {
        Debug.Log($"Pausa");
        _inGameView.PauseGame();
    }
    //----------

    void PlayGame()
    {
        //Debug.Log($"Nivel -> {_level}");
        //Debug.Log($"Nº de vidas -> {currentLives}");
        currentLevel = _level;
        if (currentLevel > record)
            record = currentLevel;

        //---------------------
        StartCoroutine(SpawnObjectsWhenReady());
        
        //Instanciar Player,etc.. en MapGenerator
        //StartCoroutine(mapGenerator.InstantiatePlayerAndEnemies(playerPrefab, enemyPrefab));

        //Escudos
        //Instantiate(shieldPrefab, _shieldStartPosition, Quaternion.identity);
    }

    private IEnumerator SpawnObjectsWhenReady()
    {
        // Espera hasta que la navMesh esté lista
        yield return StartCoroutine(mapGenerator.DoCreateNavMesh());

        SpawnPlayers();

        SpawnEnemies();

        /*
        //Instancia un enemigo aleatorio de una lista   
        foreach (var spawnPoint in enemySpawnPoints)
        {
            //public List<Transform> enemySpawnPoints;
            //Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], spawnPoint.position, spawnPoint.rotation);
        }*/
    }

    void SpawnPlayers()
    {
        if (_initialNumPlayers == 1)
        {
            Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
        }
        else
        {
            Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
            Instantiate(player2Prefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
        }
    }
    void SpawnEnemies()
    {
        if (listOfEnemiesDataTemp.Count > 0)
        {
            foreach (EnemyData enemyData in listOfEnemiesDataTemp)
            {
                //Recoloca al Enemigo en su Pos inicial
                //enemy.GetComponent<NavMeshAgent>().enabled = false;
                //enemy.transform.position = enemy.startPosition;
                //enemy.GetComponent<NavMeshAgent>().enabled = true;

                //Creo a los enemigos supervivientes (evita error al generar el navMesh)
                Enemy newEnemy = Instantiate(enemyPrefab, enemyData.initPos, Quaternion.Euler(0, 270, 0)).GetComponent<Enemy>();
                //Asigna el objetivo
                newEnemy.GetComponent<EnemyController>().traking = enemyData.traking;

                listOfEnemies.Add(newEnemy);
            }
            _flagReplay = false;
        }
        else
            CreateEnemies();
    }

    void CreateEnemies()
    {
        int numEnemies = Random.Range(minimumAmount, maximumAmount + 1);
        for (int i = 0; i < numEnemies; i++)
        {
            //Vector3 nestStartPosition = stageGenerator.GetEnemyNestPosition(playerStartPosition);
            Enemy newEnemy = Instantiate(enemyPrefab, _enemyStartPosition, Quaternion.Euler(0, 270, 0)).GetComponent<Enemy>();

            listOfEnemies.Add(newEnemy);
        }
    }

    void OnPlayerRoundEnds()
    {
        Debug.Log($"Ronda {currentLevel} terminada!!!");

        if (currentLives == 0)
        {
            OnGameOver();
        }
        else
        {
            StartCoroutine(DoReplayLevel());
        }
    }
    void OnEnemyRoundEnds()
    {
        Debug.Log($"Ronda {currentLevel} terminada!!!");

        StartCoroutine(DoNextLevel());
    }

    //Para cuando mueren los players y no tienen más vidas
    void OnGameOver()
    {
        Debug.Log($"Partida terminada en la ronda {currentLevel}");
        //Guardar puntuación
        if (PlayerPrefs.GetInt("Record", 0) < currentLevel)
            PlayerPrefs.SetInt("Record", currentLevel);

        _mainMenu.LoadScene("Scenes/UIGameOver");
        //_menuManager.OpenView("Scenes/UIMainMenu");
    }

    IEnumerator DoReplayLevel()
    {
        currentLives--;
        _flagReplay = true;
        deactivateAllShields();
        ClearSceneItems();
        yield return StartCoroutine(ClearEnemies());

        PlayGame();
    }
    IEnumerator DoNextLevel()
    {
        ClearObstaclesPrefab();
        _level++;
        if(_level % 2 == 0)
            currentLives++;

        _flagReplay = false;
        listOfEnemies.Clear();

        deactivateAllShields();
        ClearSceneItems();

        yield return StartCoroutine(ClearPlayers());

        //Generar obstáculos
        if (_level == 2)
        {
            _obstacles = Instantiate(obstaclesPrefab2, Vector3.zero, Quaternion.identity);
            StartCoroutine(mapGenerator.DoCreateNavMesh());
        }
        else
        {
            //Procedural
            StartCoroutine(DoCreateObstacles());
            StartCoroutine(mapGenerator.DoCreateNavMesh());
        }

        PlayGame();
    }

    private void ClearSceneItems()
    {
        //Borrar las balas y las minas y los escudos
        DestroyGameObjectsWithTag("Bullet");
        DestroyGameObjectsWithTag("Shield");

        //Limpiar procedural
        if (_level > 2)
        {
            DestroyGameObjectsWithTag("Obstacles");
            //Debug.Log("Destruyendo Obstáculos");
        }
    }
    private void ClearObstaclesPrefab()
    {
        if (_level == 1 || _level == 2)
        {
            Destroy(_obstacles);
            //Debug.Log("Destruyendo Obstáculos --Prefabs--");
        }
    }

    //Borro los players que quedaban para instanciarlos después (y no den error al generar el navMesh)
    IEnumerator ClearPlayers()
    {
        if (listOfPlayers.Count > 0)
        {
            //Debug.Log("------CLEAR ENEMIES------");
            foreach (Player player in listOfPlayers)
            {
                Destroy(player.gameObject);
            }
            listOfPlayers.Clear();
        }
        yield return new WaitForSeconds(1f);
    }

    //Borro los enemigos que quedaban para instanciarlos después (y no den error al generar el navMesh)
    IEnumerator ClearEnemies()
    {
        listOfEnemiesDataTemp.Clear();
        if (listOfEnemies.Count > 0)
        {
            //Debug.Log("------CLEAR ENEMIES------");
            foreach (Enemy enemy in listOfEnemies)
            {
                //Guardo los datos de Enemy para más adelante
                listOfEnemiesDataTemp.Add(new EnemyData(enemy));

                Destroy(enemy.gameObject);
            }
        }
        listOfEnemies.Clear();
        yield return new WaitForSeconds(1f);
    }

    void deactivateAllShields()
    {
        foreach (Player player in listOfPlayers)
        {
            Shield currentShield = player.GetComponent<Shield>();
            bool haveShield = currentShield.getShield();

            if (currentShield != null && haveShield)   //Si tiene escudo
                currentShield.deactivateShield();   //Lo desactivo
        }
    }

    private void DestroyGameObjectsWithTag(string tag)
    {
        //Buscamos el GameObject a eliminar
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < objects.Length; i++)    //Destruimos los GameObjects uno a uno
            Destroy(objects[i]);
    }


    //Procedural
    IEnumerator DoCreateObstacles()
    {
        ClearSceneItems();
        mapGenerator.GenerateMap();
        yield return 0;
        //yield return new WaitForSeconds(10f);
    }
}
