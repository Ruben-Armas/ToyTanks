using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGenerator : MonoBehaviour
{
    public GameObject floor;
    public GameObject cubePrefab;
    public GameObject cube2Prefab;
    /*public GameObject cube3Prefab;
    public GameObject cube4Prefab;
    public GameObject cube5Prefab;
    public GameObject cube6Prefab;*/
    private GameObject _nextCubePrefab;
    //public int numObstacles = 10;
    //public int gridSize = 8; // Tamaño de la cuadrícula, en número de cubos
    //public float gridSpacing = 1f; // Espaciado entre cubos
    
    public float cubeSize = 2.5f;
    public float wallSpacing = 2;
    public int width;
    public int height;
    public GameObject[,] cuadricula;

    private NavMeshSurface _navmesh;

    /*//Instanciar Player,etc.. en MapGenerator
    //private bool _isNavMeshReady = false;

    //Pos Temporales
    private Vector3 _playerStartPosition;
    private Vector3 _enemyStartPosition;
    private Vector3 _shieldStartPosition;*/

    private void Awake()
    {
        cuadricula = new GameObject[width, height];
        wallSpacing = wallSpacing * cubeSize;
        width = (int)(floor.transform.localScale.x / cubeSize);
        height = (int)(floor.transform.localScale.z / cubeSize);
        Debug.Log($"width -> {width}");
        Debug.Log($"height -> {height}");

    }
    void Start()
    {
        //GenerateMap();
        //cuadricula = new GameObject[x][y];w

    }

    public void GenerateMap()
    {
        /*// Generate obstacles around the perimeter of the map
        for (float x = -mapSize.x / 2; x < mapSize.x / 2; x += 2)
        {
            Instantiate(obstaclePrefab, new Vector3(x, 0.5f, mapSize.z / 2), Quaternion.identity);
            Instantiate(obstaclePrefab, new Vector3(x, 0.5f, -mapSize.z / 2), Quaternion.identity);
        }
        for (float z = -mapSize.z / 2; z < mapSize.z / 2; z += 2)
        {
            Instantiate(obstaclePrefab, new Vector3(mapSize.x / 2, 0.5f, z), Quaternion.identity);
            Instantiate(obstaclePrefab, new Vector3(-mapSize.x / 2, 0.5f, z), Quaternion.identity);
        }*/

        /*// Generate obstacles in the interior of the map
        for (int i = 0; i < numObstacles; i++)
        {
            float x = Random.Range(-mapSize.x / 2, mapSize.x / 2);
            float z = Random.Range(-mapSize.z / 2, mapSize.z / 2);
            Instantiate(obstaclePrefab, new Vector3(x, 0, z), Quaternion.identity);
        }*/

        StartCoroutine(DoClearMap());

        StartCoroutine(DoCreateCubes());

        StartCoroutine(DoCreateNavMesh());
    }

    public IEnumerator DoCreateNavMesh()
    {
        //Para que no se solape la malla nueva con la vieja
        if(_navmesh != null)
            yield return StartCoroutine(DoRemoveNavMesh());

        _navmesh = floor.gameObject.GetComponent<NavMeshSurface>();

        _navmesh.BuildNavMesh();

        Debug.Log($"Nav --> {_navmesh.navMeshData}");

        //Instanciar Player,etc.. en MapGenerator
        //_isNavMeshReady = true;
        //yield return 0;
    }
    public IEnumerator DoRemoveNavMesh()
    {
        Debug.Log($"Nav a eliminar--> {_navmesh.navMeshData}");
        _navmesh.RemoveData();
        Debug.Log($"Nav ELIMINADA --> {_navmesh.navMeshData}");
        yield return 0;
    }

    /*//Instanciar Player,etc.. en MapGenerator
    public IEnumerator InstantiatePlayerAndEnemies(GameObject playerPrefab, GameObject enemyPrefab)
    {
        while (!_isNavMeshReady)
        {
            yield return null;
        }
        _isNavMeshReady = false;

        //Pos temporales
        _playerStartPosition = new Vector3(-24, 0, 5);
        _enemyStartPosition = new Vector3(24, 0, 0);
        _shieldStartPosition = new Vector3(0, 0, 0);

        // Instancia el jugador y los enemigos
        Instantiate(playerPrefab, _playerStartPosition, Quaternion.Euler(0, 90, 0));
        Instantiate(enemyPrefab, _enemyStartPosition, Quaternion.Euler(0, 270, 0));

    }*/

    IEnumerator DoClearMap()
    {
        // Destruir los cubos existentes en la cuadrícula
        if (cuadricula != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject cube = cuadricula[x, y];
                    if (cube != null)
                    {
                        Destroy(cube);
                        cuadricula[x, y] = null;
                    }
                }
            }
        }
        yield return 0;
        //yield return new WaitForSeconds(2f);
    }

    
    IEnumerator DoCreateCubes()
    {
        //StartCoroutine(DoClearMap());
        int i = 0;
        List<Vector2> usedPositions = new List<Vector2>();

        //while (i < width * height)
        while (i < 30)
        {
            Vector2Int randomPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            // Check if the position is already used
            if (usedPositions.Contains(randomPos))
            {
                continue;
            }

            int wallSize = Random.Range(2, 7);
            int xDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            int yDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            Vector2 direction = new Vector2Int(xDirection, yDirection);

            // Check if the wall is going to fit
            bool wallFits = true;
            for (int j = 0; j < wallSize; j++)
            {
                Vector2 checkPos = randomPos + direction * j * (wallSpacing + 1);
                if (checkPos.x < 0 || checkPos.x >= width || checkPos.y < 0 || checkPos.y >= height ||
                    usedPositions.Contains(checkPos))
                {
                    wallFits = false;
                    break;
                }
            }
            if (!wallFits)
            {
                continue;
            }

            // Create the wall
            GameObject wall = new GameObject("Wall");
            for (int j = 0; j < wallSize; j++)
            {
                Vector2 pos = randomPos + direction * j * (wallSpacing + 1);
                GameObject cubePrefab;
                switch (wallSize)
                {
                    case 2:
                        cubePrefab = this.cubePrefab;
                        break;
                    case 3:
                        cubePrefab = this.cubePrefab;
                        break;
                    case 4:
                        cubePrefab = this.cubePrefab;
                        break;
                    case 5:
                        cubePrefab = this.cubePrefab;
                        break;
                    case 6:
                        cubePrefab = this.cubePrefab;
                        break;
                    default:
                        cubePrefab = this.cubePrefab;
                        break;
                }
                GameObject cube = Instantiate(cubePrefab, wall.transform);
                cube.transform.position = new Vector3(
                    (pos.x - width / 2) * cubeSize,
                    1.75f,
                    (pos.y - height / 2) * cubeSize
                );
                usedPositions.Add(pos);
                cuadricula[(int)(pos.x), (int)(pos.y)] = cube;
            }

            // Increment counter
            //i += wallSize;
            i++;
        }
        yield return 0;
        //yield return new WaitForSeconds(5f);
    }

    /*
    public IEnumerator GenerateMap()
    {
        // Eliminar la anterior NavMeshData
        if (_navmesh != null)
        {
            _navmesh.RemoveData();
        }

        // Generar los cubos
        List<Vector2Int> usedPositions = new List<Vector2Int>();
        List<GameObject> walls = new List<GameObject>();
        for (int i = 0; i < numWalls; i++)
        {
            int length = Random.Range(2, 7); // Longitud del muro (2 a 6 cubos)
            Vector2Int direction = directions[Random.Range(0, directions.Length)]; // Dirección del muro
            Vector2Int startPos = GetValidStartPosition(length, direction, usedPositions); // Posición inicial del muro
            usedPositions.AddRange(GetOccupiedPositions(length, direction, startPos)); // Añadir las posiciones ocupadas por el muro a la lista de posiciones usadas

            // Generar el muro
            for (int j = 0; j < length; j++)
            {
                Vector2Int pos = startPos + direction * j;
                GameObject wallCube = Instantiate(cubePrefab, transform);
                wallCube.transform.position = new Vector3(
                    (pos.x - width / 2) * cubeSize,
                    0f,
                    (pos.y - height / 2) * cubeSize
                );
                walls.Add(wallCube);
            }

            yield return null;
        }

        // Construir la NavMesh
        _navmesh = floor.gameObject.GetComponent<NavMeshSurface>();
        _navmesh.BuildNavMesh();
    }*/

    /*////
    private void CreateCubes()
    {
        // Generar nuevos cubos
        List<Vector2Int> takenPositions = new List<Vector2Int>();
        int i = 0;
        while (i < 100)
        {
            int wallLength = Random.Range(2, 7);
            int direction = Random.Range(0, 4);
            Vector2Int startPosition = new Vector2Int(Random.Range(2, width - wallLength - 2), Random.Range(2, height - wallLength - 2));

            if (IsPositionTaken(takenPositions, startPosition, wallLength, direction))
            {
                continue;
            }

            takenPositions.AddRange(GetPositions(startPosition, wallLength, direction));
            i += wallLength;

            for (int j = 0; j < wallLength; j++)
            {
                int x = startPosition.x + j * (int)Mathf.Cos(direction * 90f * Mathf.Deg2Rad);
                int y = startPosition.y + j * (int)Mathf.Sin(direction * 90f * Mathf.Deg2Rad);

                if (x < 2 || x >= width - 2 || y < 2 || y >= height - 2)
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    _nextCubePrefab = cubePrefab;
                }
                else
                {
                    _nextCubePrefab = cube2Prefab;
                }

                GameObject cube = Instantiate(_nextCubePrefab, transform);
                cube.transform.position = new Vector3((x - width / 2) * cubeSize, 0f, (y - height / 2) * cubeSize);
                cuadricula[x, y] = cube;
            }
            i += 2;
        }
    }*/

    /*  //Rellena todo
        // Instancia los cubos en una cuadrícula
        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cubeSize, 0, y * cubeSize) + gridOffset;

                if(i % 2 == 0)
                    Instantiate(cubePrefab, position, Quaternion.identity);
                else
                    Instantiate(cube2Prefab, position, Quaternion.identity);
                i++;
                /*
                // Asegura que los cubos estén unidos sin superponerse
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    Instantiate(cubePrefab, position, Quaternion.identity);
                }
                else if (x % 2 == 0 && y % 2 == 0)
                {
                    Instantiate(cubePrefab, position, Quaternion.identity);
                }*//*
            }
        }*/
    /*//Rellena aleatoriamente
        // Instanciar los cubos
        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Comprobar si estamos en un borde
                if (x < 2 || x >= width - 2 || y < 2 || y >= height - 2)
                {
                    // Estamos en un borde, no instanciar cubo
                    continue;
                }

                // Instanciar cubo
                if(Random.Range(1,100) > 80)
                {
                    if (i % 2 == 0 ? _nextCubePrefab = cubePrefab : _nextCubePrefab = cube2Prefab);
                    //Genera el cubo
                    GameObject cube = Instantiate(cubePrefab, transform);
                    cube.transform.position = new Vector3(
                        (x - width / 2) * cubeSize,
                        0f,
                        (y - height / 2) * cubeSize
                    );
                    cuadricula[x, y] = cube;
                }
                i++;
            }
        }
    */
}
