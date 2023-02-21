using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public int maxPlayers = 2;
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject player2Prefab;
    public GameObject enemyPrefab;
    public GameObject shieldPrefab;

    [Header("Enemigos")]
    public int minimumAmount = 1;
    public int maximumAmount = 4;

    private Vector3 _playerStartPosition;
    private Vector3 _enemyStartPosition;
    private Vector3 _shieldStartPosition;

    private List<Player> listOfPlayers = new List<Player>();
    private List<Player> listOfInitialPlayers = new List<Player>();
    private List<Enemy> listOfEnemies = new List<Enemy>();

    private int _level;
    //private int _startLives;
    private int _initialNumPlayers;

    public int currentLevel { get; private set; }   //public leer, privado modificar
    public int record { get; private set; }
    public int currentLives;// { get; private set; }

    private bool[] playersExists;               //Player activos
    private List<GameObject> playersPrefabs;    //Lista de prefabs

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
        _initialNumPlayers = 0;

        //Pos temporales
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
        //listOfInitialPlayers.Add(playerCreated);
        Debug.Log($"Nº de Jugadores {listOfPlayers.Count}");
    }
    private void OnPlayerDestroyed(Player playerDestroyed, Vector3 startPosition)
    {
        //Borro el player destruido de la lista de Players
        listOfPlayers.Remove(playerDestroyed);

        if (listOfPlayers.Count == 0)
            OnPlayerRoundEnds();
    }

    private void OnEnemyCreated(Enemy enemyCreated, Vector3 startPosition)
    {
        //Añado el enemigo creado a la lista de Enemigos
        listOfEnemies.Add(enemyCreated);
        Debug.Log($"Nº de Enemigos {listOfEnemies.Count}");
    }
    private void OnEnemyDestroyed(Enemy enemyDestroyed, Vector3 startPosition)
    {
        //Borro el enemigo destruido de la lista de Enemigos
        listOfEnemies.Remove(enemyDestroyed);
        Debug.Log($"Nº de Enemigos {listOfEnemies.Count}");

        //REFACTORIZAR EVENTO   --- Crear una clase ENEMY independiente del control
        //Compruebo si no quedan enemigos para terminar la ronda    (lo mismo con los players)
        if (listOfEnemies.Count == 0)
            OnEnemyRoundEnds();
    }

    private void OnPause()
    {
        Debug.Log($"Pausa");
        _inGameView.PauseGame();
    }
    //----------

    void BeginGame()
    {
        Debug.Log($"Nivel -> {_level}");
        Debug.Log($"Nº de vidas -> {currentLives}");
        currentLevel = _level;
        if (currentLevel > record)
            record = currentLevel;


        //Escudos
        Instantiate(shieldPrefab, _shieldStartPosition, Quaternion.identity);

        //Jugador/es
        SpawnPlayers();

        //Enemigos
        SpawnEnemies();
    }

    void SpawnPlayers()
    {
        Debug.Log($"listOfPlayers -> {listOfPlayers.Count}");
        Debug.Log($"_initialNumPlayers -> {_initialNumPlayers}");
        //int initialPlayersNum = listOfInitialPlayers.Count;
        if (_initialNumPlayers == 0)
        {
            //Vector3 playerStartPosition = stageGenerator.GetPlayerStartPosition();
            Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
            _initialNumPlayers++;

            /*
            Instantiate(player2Prefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
            _initialNumPlayers++;*/
        }
        else
        {
            if (listOfPlayers.Count < _initialNumPlayers)
            {
                Debug.Log("Falta alguno");
                while(listOfPlayers.Count < _initialNumPlayers)
                {
                    if(listOfPlayers.Count == 0)
                    {
                        Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
                    }
                    else
                    {
                        if (listOfPlayers[0].color == Player.Colors.blue)
                            Instantiate(player2Prefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
                        else
                            Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
                    }
                }                
            }
            else if (listOfPlayers.Count == _initialNumPlayers)
            {
                //Si todos siguen vivos, recolocarlos en el inicio (o pos "aleatoria")
                for (int i = 0; i < _initialNumPlayers; i++)
                    listOfPlayers[i].transform.position = listOfPlayers[i].startPosition;
            }
            else if (listOfPlayers.Count > _initialNumPlayers)
                Debug.Log("-----HAY MÁS DE LO NORMAL-----");
        }

        Debug.Log($"listOfPlayers -> {listOfPlayers.Count}");
        Debug.Log($"_initialNumPlayers -> {_initialNumPlayers}");
    }

    void SpawnEnemies()
    {
        if (listOfEnemies.Count > 0)
        {
            foreach (Enemy enemy in listOfEnemies)
            {
                //Recoloca al Enemigo en su Pos inicial
                enemy.GetComponent<NavMeshAgent>().enabled = false;
                enemy.transform.position = enemy.startPosition;
                enemy.GetComponent<NavMeshAgent>().enabled = true;
            }
        }
        else
        {
            CreateEnemies();
        }
    }

    void CreateEnemies()
    {
        int numEnemies = Random.Range(minimumAmount, maximumAmount + 1);
        Debug.Log($"numEnemies -> { numEnemies}");
        for (int i = 0; i < numEnemies; i++)
        {
            //Vector3 nestStartPosition = stageGenerator.GetEnemyNestPosition(playerStartPosition);
            Instantiate(enemyPrefab, _enemyStartPosition, Quaternion.Euler(0, 270, 0));
        }
    }

    /*void CreatePlayers()
    {
        listOfInitialPlayers.Clear();
        //Vector3 playerStartPosition = stageGenerator.GetPlayerStartPosition();
        Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
        //Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
    }*/

    void OnPlayerRoundEnds()
    {
        Debug.Log($"Ronda {currentLevel} terminada!!!");

        if (currentLives == 0)
        {
            OnGameOver();
        }
        else
        {
            ReplayLevel();
        }
    }
    void OnEnemyRoundEnds()
    {
        Debug.Log($"Ronda {currentLevel} terminada!!!");

        NextLevel();
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

    void ReplayLevel()
    {
        currentLives--;
        deactivateAllShields();
        ClearSceneItems();
        BeginGame();
    }
    void NextLevel()
    {
        _level++;
        currentLives++;

        listOfEnemies.Clear();

        deactivateAllShields();
        ClearSceneItems();

        //Reinicio temporal PRUEBAS
        BeginGame();
    }

    void ClearSceneItems()
    {
        //Borrar las balas y las minas y los escudos
        DestroyGameObjectsWithTag("Bullet");
        DestroyGameObjectsWithTag("Shield");
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
}
