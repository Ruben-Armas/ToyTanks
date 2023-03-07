using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public int startLives;

    [Range(0f, 10f)]
    public float timeToStartLevel = 3f;

    [Header("Referencias")]
    public MapGenerator mapGenerator;
    public InGameView inGameView;
    public MainMenu mainMenu;
    public MenuManager menuManager;

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
    public List<GameObject> listEnemyType = new List<GameObject>();
    public List<Vector3> listEnemyStartPositions = new List<Vector3>();

    [Header("Escudos")]
    public int minAmountShields = 1;
    public int maxAmountShields = 2;

    private int _numOfEnemiesDestroyed;
    private int _numOfEnemiesDestroyedByBlue;
    private int _numOfEnemiesDestroyedByGreen;
    private int _numOfDeaths;
    private int _numOfDeathsPlayer1;
    private int _numOfDeathsPlayer2;

    private Vector3 _player1StartPosition;
    private Vector3 _player2StartPosition;
    private Vector3 _shield1StartPosition;
    private Vector3 _shield2StartPosition;
    private List<Vector3> _listShieldStartPositions = new List<Vector3>();
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
    private int _initialNumPlayers;
    private int _inputPlayer1;
    private PlayerInput _playerInput;

    public int currentLevel { get; private set; }   //public leer, privado modificar
    public int record { get; private set; }
    public int currentLives { get; private set; }

    private GameObject _obstacles;
    private List<Vector3> freePositions;    // referencia a la lista de posiciones libres
    private bool startingLevel = true;

    private Player _player;
    private Player _player2;
    private GameObject _nextEnemyPrefab;
    private Vector3 _offset;

    void Start()
    {
        _numOfEnemiesDestroyed = 0;
        _numOfEnemiesDestroyedByBlue = 0;
        _numOfEnemiesDestroyedByGreen = 0;
        _numOfDeaths = 0;
        _numOfDeathsPlayer1 = 0;
        _numOfDeathsPlayer2 = 0;
        currentLives = startLives;
        _level = 1;
        record = 0;
        _width = mapGenerator.width;
        _height = mapGenerator.height;
        GetInitialNumOfPlayers();
        GetInputPlayer1();

        _offset = new Vector3(0, 0, 1.5f);

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
        {
            _numOfDeaths++;
            //Debug.Log(playerDestroyed.color);
            if(playerDestroyed.color.ToString() == "blue")
            {
                _numOfDeathsPlayer1++;
                //Debug.Log($"P1 Death --> {_numOfDeathsPlayer1}");
            }
            else if (playerDestroyed.color.ToString() == "green")
            {
                _numOfDeathsPlayer2++;
                //Debug.Log($"P2 Death --> {_numOfDeathsPlayer2}");
            }
        }

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
    private void OnEnemyDestroyed(Enemy enemyDestroyed, Vector3 startPosition, int destroyedById)
    {
        //Borro el enemigo destruido de la lista de Enemigos
        if (_flagReplay == false)
        {
            //Debug.Log("Borrando enemy");
            listOfEnemies.Remove(enemyDestroyed);

            _numOfEnemiesDestroyed++;
            if (destroyedById == 0)
                _numOfEnemiesDestroyedByBlue++;
            if (destroyedById == 1)
                _numOfEnemiesDestroyedByGreen++;

        }
        //Compruebo si no quedan enemigos para terminar la ronda
        if (listOfEnemies.Count == 0)
            OnEnemyRoundEnds();
    }

    private void OnPause()
    {
        Debug.Log($"Pausa");
        inGameView.PauseGame();
    }
    //----------

    void PlayGame()
    {
        currentLevel = _level;
        if (currentLevel > record)
            record = currentLevel;

        //---------------------
        //Debug.Log("----PLAY----");
        StartCoroutine(SpawnObjectsWhenReady());
        
        //Instanciar Player,etc.. en MapGenerator
        //StartCoroutine(mapGenerator.InstantiatePlayerAndEnemies(playerPrefab, enemyPrefab));
    }

    private IEnumerator SpawnObjectsWhenReady()
    {
        // Espera hasta que la navMesh esté lista
        yield return StartCoroutine(mapGenerator.DoCreateNavMesh());

        SpawnShields();
        SpawnPlayers();
        SpawnEnemies();

        startingLevel = false;
    }

    void SpawnShields()
    {
        if(startingLevel)
            _randNumShields = Random.Range(minAmountShields, maxAmountShields + 1);
        GetShieldPositions();

        if(_level <= 2)
        {
            if (_randNumShields == 1)
                Instantiate(shieldPrefab, _listShieldStartPositions[0], Quaternion.identity);
            else
            {
                Instantiate(shieldPrefab, _listShieldStartPositions[0], Quaternion.identity);
                Instantiate(shieldPrefab, _listShieldStartPositions[1], Quaternion.identity);
            }
        }
        else
        {
            for (int i = 0; i < _randNumShields; i++)
            {
                Instantiate(shieldPrefab, _listShieldStartPositions[i], Quaternion.identity);
            }
        }

                   
    }
    void SpawnPlayers()
    {
        GetPlayerPositions();

        if (_initialNumPlayers == 1)
        {
            _player = Instantiate(playerPrefab, _player1StartPosition + _offset, Quaternion.Euler(0, 90, 0)).GetComponent<Player>();
            //GetSchemes(player);
            SetInputPlayer1(_player);
        }
        else
        {
            _player = Instantiate(playerPrefab, _player1StartPosition + _offset, Quaternion.Euler(0, 90, 0)).GetComponent<Player>();
            SetInputPlayer1(_player);
            _player2 = Instantiate(player2Prefab, _player2StartPosition + _offset, Quaternion.Euler(0, 90, 0)).GetComponent<Player>();
            //SetInputPlayer2(_player2);

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
                Enemy newEnemy = Instantiate(enemyData.prefab, enemyData.initPos, Quaternion.Euler(0, 270, 0)).GetComponent<Enemy>();
                newEnemy.SetPrefab(enemyData.prefab);

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
            if (_level == 1)
                _nextEnemyPrefab = listEnemyType[Random.Range(0, 1)];
            else if (_level >= 2 && _level <= 3)
                _nextEnemyPrefab = listEnemyType[Random.Range(0, 2)];
            else
                _nextEnemyPrefab = listEnemyType[Random.Range(0, listEnemyType.Count)];

            Enemy newEnemy = Instantiate(_nextEnemyPrefab, _listEnemyStartPositions[i] - _offset, Quaternion.Euler(0, 270, 0)).GetComponent<Enemy>();
            newEnemy.SetPrefab(_nextEnemyPrefab);
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
                _listEnemyStartPositions.Add(listEnemyStartPositions[i]);
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
            _listShieldStartPositions.Add(new Vector3(0, 0, 0));
            _listShieldStartPositions.Add(new Vector3(-15, 0, 0));
        }
        else if (startingLevel)
        {
            //Shield
            for (int i = 0; i < _randNumShields; i++)
            {
                _listShieldStartPositions.Add(SetMiddlePosition());
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
        //Debug.Log($"pos to spawn --> {newPosFixed}");

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
        //Debug.Log($"Ronda {currentLevel} terminada!!!");
        if (currentLives == 0)
        {
            OnGameOver();
        }
        else
        {
            _flagReplay = true;
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

        //Guarda la puntuación, muertes, etc
        SaveStats();

        mainMenu.LoadScene("Scenes/UIGameOver");
        //menuManager.OpenView("Scenes/UIMainMenu");
    }

    IEnumerator DoReplayLevel()
    {
        currentLives--;
        _flagReplay = true;
        DeactivateAllShields();
        ClearSceneItems();
        yield return StartCoroutine(ClearEnemies());

        yield return StartCoroutine(WaitNextReplayLevel());


        PlayGame();
    }
    IEnumerator DoNextLevel()
    {
        _level++;
        //Nº Vidas
        if(_level % 2 == 0)
            if (_initialNumPlayers == 1 && currentLives < 5)
                currentLives++;
        else if (_initialNumPlayers == 2 && currentLives < 3)
            currentLives++;

        //Nº Enemigos
        if (_level > 4 && _level % 2 == 0 && maxAmountEnemies <= 10)
            maxAmountEnemies++;
        //Nº Escudos
        if (_level > 4 && _level % 2 == 0 && maxAmountShields <= 6)
            maxAmountShields++;

        startingLevel = true;
        _flagReplay = false;
        listOfEnemies.Clear();

        _listShieldStartPositions.Clear();
        DeactivateAllShields();
        ClearSceneItems();

        yield return StartCoroutine(WaitNextReplayLevel());
        //Empieza el siguiente nivel (después de esperar)
        ClearObstacles();

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
    }
    private void ClearObstacles()
    {
        if (_level == 1 || _level == 2)
        {
            Destroy(_obstacles);
            //Debug.Log("Destruyendo Obstáculos --Prefabs--");
        }
        //Limpiar procedural
        else if (_level > 2)
        {
            DestroyGameObjectsWithTag("Obstacles");
            //Debug.Log("Destruyendo Obstáculos");
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
        yield return 0;
        //yield return new WaitForSeconds(1f);
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

    IEnumerator WaitNextReplayLevel()
    {
        //Desactivo los controles
        DeactivatePayersInput();

        yield return new WaitForSeconds(timeToStartLevel);
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

    private void SaveStats()
    {
        //Guardar el nº de enemigos eliminados  Totales
        PlayerPrefs.SetInt("TotalKills", _numOfEnemiesDestroyed);
        //De Player 1
        PlayerPrefs.SetInt("NumOfkillsPlayer1", _numOfEnemiesDestroyedByBlue);
        //De Player 2
        if (_initialNumPlayers == 2)
        {
            //Guardar el nº de muertes
            PlayerPrefs.SetInt("NumOfkillsPlayer2", _numOfEnemiesDestroyedByGreen);
        }
        else
            PlayerPrefs.SetInt("NumOfkillsPlayer2", -1);

        //Guardar el nº de muertes  Totales
        PlayerPrefs.SetInt("TotalDeaths", _numOfDeaths);
        //De Player 1
        PlayerPrefs.SetInt("NumOfDeathsPlayer1", _numOfDeathsPlayer1);
        //De Player 2
        if (_initialNumPlayers == 2)
        {
            //Guardar el nº de muertes
            PlayerPrefs.SetInt("NumOfDeathsPlayer2", _numOfDeathsPlayer2);
        }
        else
            PlayerPrefs.SetInt("NumOfDeathsPlayer2", -1);

        //Guardar puntuación actual
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);

        //Guardar puntuación record
        if (PlayerPrefs.GetInt("Record", 0) < currentLevel)
            PlayerPrefs.SetInt("Record", currentLevel);
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
        //Debug.Log($"------Input-->{inputP1}------");
    }
    private void SetInputPlayer1(Player player)
    {
        _playerInput = player.GetComponent<PlayerInput>();
        //Debug.Log($"PlayerInput --> {_playerInput}");
        if (_playerInput != null)
        {
            if (_inputPlayer1 == 0)
            {
                _playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
                //_playerInput.defaultControlScheme = "Keyboard&Mouse";
                //Debug.Log("Switch to Keyboard&Mouse");
            }
            else
            {
                _playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
                //_playerInput.defaultControlScheme = "Gamepad";
                //Debug.Log("Switch to Gamepad");
            }
        }
    }
    private void SetInputPlayer2(Player player)
    {
        _playerInput = player.GetComponent<PlayerInput>();
        //Debug.Log($"PlayerInput --> {_playerInput}");
        if (_playerInput != null)
        {
            _playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
            //Debug.Log("Switch to Gamepad");
        }
    }
    private void DeactivatePayersInput()
    {
        if(_player != null)
        {
            _playerInput = _player.GetComponent<PlayerInput>();
            if (_playerInput != null)
                _playerInput.DeactivateInput();
        }
        
        if(_initialNumPlayers == 2)
        {
            if(_player2 != null)
            {
                _playerInput = _player2.GetComponent<PlayerInput>();
                if (_playerInput != null)
                    _playerInput.DeactivateInput();
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
