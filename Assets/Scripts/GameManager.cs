using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int maxPLayers = 2;
    [Header("Prefabs")]
    public GameObject playerPrefab;
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
    private int _startLives;
    private int _initialNumPlayers;

    public int currentLevel { get; private set; }   //public leer, privado modificar
    public int maxLevel { get; private set; }
    public int currentLives;// { get; private set; }

    void Start()
    {
        _level = 1;
        _startLives = 0;
        maxLevel = 0;
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

    void BeginGame()
    {
        Debug.Log($"Nivel -> {_level}");
        Debug.Log($"Nº de vidas -> {currentLives}");
        currentLevel = _level;
        if (currentLevel > maxLevel)
            maxLevel = currentLevel;


        //Escudos
        Instantiate(shieldPrefab, _shieldStartPosition, Quaternion.identity);
                
        //Jugador/es
        Debug.Log($"listOfPlayers -> {listOfPlayers.Count}");
        Debug.Log($"_initialNumPlayers -> {_initialNumPlayers}");
        //int initialPlayersNum = listOfInitialPlayers.Count;
        if (_initialNumPlayers == 0)
        {
            //Vector3 playerStartPosition = stageGenerator.GetPlayerStartPosition();
            Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
            _initialNumPlayers++;
            Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
            _initialNumPlayers++;
                //Comprobar si se puede crear más player y no están el máximo
        }
        else
        {
            if (listOfPlayers.Count < _initialNumPlayers)
            {
                Debug.Log("Falta alguno");
                for (int i=0; i< _initialNumPlayers; i++)
                {
                    Debug.Log(i);
                    //Si la cantidad actual de players coincide con la inicial
                    if(i < listOfPlayers.Count)
                        listOfPlayers[i].transform.position = listOfPlayers[i].startPosition;
                    else
                    {
                        Debug.Log("Creando Player destruido");
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


        //Enemigos
        if (listOfEnemies.Count > 0)
        {
            foreach (Enemy enemy in listOfEnemies)
            {
                enemy.transform.position = enemy.startPosition;
                /*
                Rigidbody _rigidbody = enemy.GetComponent<Rigidbody>();
                _rigidbody.isKinematic = true;
                _rigidbody.detectCollisions = false;
                enemy.transform.position = enemy.startPosition;
                _rigidbody.isKinematic = false;
                _rigidbody.detectCollisions = true;
                */
            }
        }
        else
        {
            CreateEnemies();
        }
    }

    void OnPlayerRoundEnds()
    {
        Debug.Log($"Ronda {currentLevel} terminada!!!");

        //Si la lista de players = 0 -> Siguiente nivel
        if (currentLives == 0)
        {
            OnGameOver();
        }
        else
        {
            currentLives--;
            //desactivar escudos
            BeginGame();
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
    }

    void NextLevel()
    {
        _level++;
        currentLives++;
        //desactivar escudos
        //ClearScene();
        //Reinicio temporal PRUEBAS
        BeginGame();
    }

    void ClearScene()
    {
        listOfPlayers.Clear();
        listOfEnemies.Clear();

        //Borrar las balas y las minas
    }

    void CreateEnemies()
    {
        int numEnemies = Random.Range(minimumAmount, maximumAmount +1);
        for (int i = 0; i < numEnemies; i++)
        {
            //Vector3 nestStartPosition = stageGenerator.GetEnemyNestPosition(playerStartPosition);
            Instantiate(enemyPrefab, _enemyStartPosition, Quaternion.Euler(0, 270, 0));
        }
    }
    /*
    void CreatePlayers()
    {
        listOfInitialPlayers.Clear();
        //Vector3 playerStartPosition = stageGenerator.GetPlayerStartPosition();
        Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
        //Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
    }*/
}
