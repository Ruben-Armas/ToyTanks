using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

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

    public GameObject cube1Prefab;
    public GameObject cube2Prefab;

    [Header("Enemigos")]
    public int minAmountEnemies = 1;
    public int maxAmountEnemies = 4;

    [Header("Escudos")]
    public int minAmountShields = 1;
    public int maxAmountShields = 2;

    private Vector3 _player1StartPosition;
    private Vector3 _player2StartPosition;
    private Vector3 _enemyStartPosition;
    private Vector3 _shield1StartPosition;
    private Vector3 _shield2StartPosition;
    private List<Vector3> _listEnemyStartPositions = new List<Vector3>();
    private int _randNumEnemies;
    private int _randNumShields;
    private int _width;
    private int _height;

    private List<Player> listOfPlayers = new List<Player>();
    private List<Enemy> listOfEnemies = new List<Enemy>();  //Lista que controlará la cantidad de enemigos vivos en la escena
    private List<EnemyData> listOfEnemiesDataTemp = new List<EnemyData>();  //Lista que guarda los datos de los supervivientes para reInstanciarlos

    private bool _flagReplay = false;   //Controla que listOfEnemies se modifique solo durante la partida
    
    private int _level;
    //private int _startLives;
    private int _initialNumPlayers;
    private int _inputPlayer1;
    private PlayerInput _playerInput;

    public int currentLevel { get; private set; }   //public leer, privado modificar
    public int record { get; private set; }
    public int currentLives;// { get; private set; }

    private GameObject _obstacles;
    private List<Vector3> freePositions;    // referencia a la lista de posiciones libres
    private bool startingLevel = true;

    public InGameView _inGameView;
    public MainMenu _mainMenu;
    public MenuManager _menuManager;

    private int _numOfEnemiesDestroyed;
    private int _numOfDeaths;

    void Start()
    {
        _numOfEnemiesDestroyed = 0;
        _numOfDeaths = 0;
        _level = 1;
        record = 0;
        _width = mapGenerator.width;
        _height = mapGenerator.height;
        GetInitialNumOfPlayers();
        GetInputPlayer1();

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

        if (_flagReplay == false)
            _numOfDeaths++;

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
            _numOfEnemiesDestroyed++;
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

        SpawnShields();
        SpawnPlayers();
        SpawnEnemies();

        startingLevel = false;

        /*
        //Instancia un enemigo aleatorio de una lista   
        foreach (var spawnPoint in enemySpawnPoints)
        {
            //public List<Transform> enemySpawnPoints;
            //Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], spawnPoint.position, spawnPoint.rotation);
        }*/
    }

    void SpawnShields()
    {
        _randNumShields = Random.Range(minAmountShields, maxAmountShields + 1);

        GetShieldPositions();

        if(_randNumShields == 1)
            Instantiate(shieldPrefab, _shield1StartPosition, Quaternion.identity);
        else
        {
            Instantiate(shieldPrefab, _shield1StartPosition, Quaternion.identity);
            Instantiate(shieldPrefab, _shield2StartPosition, Quaternion.identity);
        }            
    }
    void SpawnPlayers()
    {
        GetPlayerPositions();

        Vector3 offset = new Vector3(0, 0, 1);
        if (_initialNumPlayers == 1)
        {
            Player player = Instantiate(playerPrefab, _player1StartPosition + offset, Quaternion.Euler(0, 90, 0)).GetComponent<Player>();
            //GetSchemes(player);
            SetInputPlayer1(player);
        }
        else
        {
            Player player = Instantiate(playerPrefab, _player1StartPosition + offset, Quaternion.Euler(0, 90, 0)).GetComponent<Player>();
            //SetInputPlayer1(player);
            Instantiate(player2Prefab, _player2StartPosition + offset, Quaternion.Euler(0, 90, 0));
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
        _randNumEnemies = Random.Range(minAmountEnemies, maxAmountEnemies + 1);

        GetEnemyPositions();

        for (int i = 0; i < _randNumEnemies; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, _listEnemyStartPositions[i], Quaternion.Euler(0, 270, 0)).GetComponent<Enemy>();
            listOfEnemies.Add(newEnemy);
        }
    }

    private void GetPlayerPositions()
    {
        if (_level <= 2)
        {
            _player1StartPosition = new Vector3(-27, 0, 5);
            _player2StartPosition = new Vector3(-27, 0, -5);
        }
        else if (startingLevel)
        {
            //Players
            if (_initialNumPlayers == 1)
                _player1StartPosition = SetLeftRightPosition(true);
            else
            {
                _player1StartPosition = SetLeftRightPosition(true);
                _player2StartPosition = SetLeftRightPosition(true);
            }
        }
    }
    private void GetEnemyPositions()
    {
        if (_level <= 2)
            for (int i = 0; i < _randNumEnemies; i++)
                _listEnemyStartPositions.Add(new Vector3(24, 0, 0));
        else if (startingLevel)
        {
            //Enemies
            if (_randNumEnemies > 0)
            {
                _listEnemyStartPositions.Clear();
                for (int i = 0; i <_randNumEnemies; i++)
                {
                    _listEnemyStartPositions.Add(SetLeftRightPosition(false));
                }
            }
        }
    }
    private void GetShieldPositions()
    {
        if (_level <= 2)
        {
            _shield1StartPosition = new Vector3(0, 0, 0);
            _shield2StartPosition = new Vector3(0, 0, 0);
        }
        else if (startingLevel)
        {
            //Shield
            if (_randNumShields == 1)
                _shield1StartPosition = SetMiddlePosition();
            else
            {
                _shield1StartPosition = SetMiddlePosition();
                _shield2StartPosition = SetMiddlePosition();
            }
        }
    }
    private Vector3 SetMiddlePosition()
    {
        freePositions = mapGenerator.freePositions; //Obtengo la lista

        // Crear una lista con las posiciones libres de la mitad del mapa
        List<Vector3> middleFreePositions = new List<Vector3>();
        foreach (Vector3 pos in freePositions)
        {
            if (MarginZ(pos) && pos.x > -_width / 2 && pos.x < _width / 2)
            {
                middleFreePositions.Add(pos);
                //Instantiate(cube1Prefab, pos, Quaternion.identity);
            }
        }
        // nº aleatorio entre 0 y el número de posiciones libres de la parte izquierda/derecha del mapa
        int randomIndex = Random.Range(0, middleFreePositions.Count);
        // Obtengo la posición de la lista de posiciones
        Vector3 newPos = middleFreePositions[randomIndex];
        Vector3 newPosFixed = new Vector3(newPos.x, 0, newPos.z);
        //Debug.Log($"pos middle to spawn --> {newPosFixed}");

        // elimina la posición usada
        freePositions.RemoveAt(randomIndex);

        return newPosFixed;
    }
    private Vector3 SetLeftRightPosition(bool left)
    {
        freePositions = mapGenerator.freePositions; //Obtengo la lista

        // Crear una lista con las posiciones libres de la parte izquierda/derecha del mapa
        List<Vector3> leftRightFreePositions = new List<Vector3>();
        foreach (Vector3 pos in freePositions)
        {
            if (left)
            {
                if (pos.x < -_width /2 && MarginZ(pos))
                //if (pos.x < 0)
                {
                    leftRightFreePositions.Add(pos);
                    //Instantiate(cube1Prefab, pos, Quaternion.identity);
                }                    
            }
            else
            {
                if (pos.x > _width /2 && MarginZ(pos))
                //if (pos.x > 0)
                {
                    leftRightFreePositions.Add(pos);
                    //Instantiate(cube2Prefab, pos, Quaternion.identity);
                }                    
            }            
        }
        // nº aleatorio entre 0 y el número de posiciones libres de la parte izquierda/derecha del mapa
        int randomIndex = Random.Range(0, leftRightFreePositions.Count);
        // Obtengo la posición de la lista de posiciones
        Vector3 newPos = leftRightFreePositions[randomIndex];
        Vector3 newPosFixed = new Vector3(newPos.x, 0, newPos.z);
        Debug.Log($"pos to spawn --> {newPosFixed}");

        // elimina la posición usada
        freePositions.RemoveAt(randomIndex);

        return newPosFixed;
    }
    private bool MarginZ(Vector3 pos)
    {
        if (pos.z < _height && pos.z > -_height)
            return true;
        else
            return false;
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
        //Debug.Log($"Partida terminada en la ronda {currentLevel}");

        //Guardar el nº de enemigos eliminados
        PlayerPrefs.SetInt("NumOfEnemiesDestroyed", _numOfEnemiesDestroyed);
        //Guardar el nº de muertes
        PlayerPrefs.SetInt("NumOfDeaths", _numOfDeaths);
        //Guardar puntuación actual
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);

        //Guardar puntuación record
        if (PlayerPrefs.GetInt("Record", 0) < currentLevel)
            PlayerPrefs.SetInt("Record", currentLevel);

        _mainMenu.LoadScene("Scenes/UIGameOver");
        //_menuManager.OpenView("Scenes/UIMainMenu");
    }

    IEnumerator DoReplayLevel()
    {
        currentLives--;
        _flagReplay = true;
        DeactivateAllShields();
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

        startingLevel = true;
        _flagReplay = false;
        listOfEnemies.Clear();

        DeactivateAllShields();
        ClearSceneItems();

        yield return StartCoroutine(ClearPlayers());
        listOfEnemiesDataTemp.Clear();

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

    void DeactivateAllShields()
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

    private void GetInitialNumOfPlayers()
    {
        // Obtener el valor de PlayerPrefs
        int numPlayers = PlayerPrefs.GetInt("numPlayers", 1);
        // Asignar el valor a la variable numOfPlayers
        if (numPlayers < 0)
            _initialNumPlayers = 1;
        else if (numPlayers > 2)
            _initialNumPlayers = 2;
        else
            _initialNumPlayers = numPlayers;
        //Debug.Log($"------numPlayers-->{numPlayers}"------");
    }
    private void GetInputPlayer1()
    {
        // Obtener el valor de PlayerPrefs
        int inputP1 = PlayerPrefs.GetInt("inputP1", 1);
        // Asignar el valor a la variable numOfPlayers
        if (inputP1 < 0)
            _inputPlayer1 = 1;
        else if (inputP1 > 2)
            _inputPlayer1 = 2;
        else
            _inputPlayer1 = inputP1;
        Debug.Log($"------Input-->{inputP1}------");
    }
    private void SetInputPlayer1(Player player)
    {
        _playerInput = player.GetComponent<PlayerInput>();
        Debug.Log($"PlayerInput --> {_playerInput}");
        if (_playerInput != null)
        {
            if (_inputPlayer1 == 0)
            {
                _playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
                //_playerInput.defaultControlScheme = "Keyboard&Mouse";
                Debug.Log("Switch to Keyboard&Mouse");
            }
            else
            {
                _playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
                //_playerInput.defaultControlScheme = "Gamepad";
                Debug.Log("Switch to Gamepad");
            }
        }        
    }    
    /*private void GetSchemes(Player player)
    {
        _playerInput = player.GetComponent<PlayerInput>();
        // Obtener el nombre del esquema predeterminado
        string defaultScheme = _playerInput.defaultControlScheme;

        // Obtener la lista de esquemas disponibles
        InputControlScheme[] schemes = _playerInput.controlSchemes.ToArray();

        // Imprimir los nombres de los esquemas disponibles
        Debug.Log("Schemes available:");
        foreach (InputControlScheme scheme in schemes)
        {
            Debug.Log(scheme.name);
        }
    }*/

    //Procedural
    IEnumerator DoCreateObstacles()
    {
        ClearSceneItems();
        mapGenerator.GenerateMap();
        yield return 0;
        //yield return new WaitForSeconds(10f);
    }
}
