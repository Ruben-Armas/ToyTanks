using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGenerator : MonoBehaviour
{
    public GameObject floor;
    public List<GameObject> listCubePrefab;
    public GameObject cubePrefab;
    private GameObject _nextCubePrefab;
    private Vector3 _gridOffset;
    private Vector3 _position;

    public int maxCubes = 50;
    public float cubeSize = 2.5f;
    public float diagonalProbability = 0.5f;
    public int width;
    public int height;
    public GameObject[,] cuadricula;
    public List<GameObject> listCubes;
    public List<Vector3> freePositions { get; private set; }

    private NavMeshSurface _navmesh;

    /*//Instanciar Player,etc.. en MapGenerator
    //private bool _isNavMeshReady = false;

    //Pos Temporales
    private Vector3 _playerStartPosition;
    private Vector3 _enemyStartPosition;
    private Vector3 _shieldStartPosition;*/

    private void Awake()
    {
        freePositions = new List<Vector3>();
        width = (int)(floor.transform.localScale.x / cubeSize);
        height = (int)(floor.transform.localScale.z / cubeSize);
        cuadricula = new GameObject[width, height];
        Debug.Log($"width -> {width}");
        Debug.Log($"height -> {height}");
        _gridOffset = new Vector3(-width * cubeSize / 2 + 1.15f, 1.75f, -height * cubeSize / 2 + 1.25f);

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

        //Debug.Log($"Nav --> {_navmesh.navMeshData}");

        //Instanciar Player,etc.. en MapGenerator
        //_isNavMeshReady = true;
        //yield return 0;
    }
    public IEnumerator DoRemoveNavMesh()
    {
        //Debug.Log($"Nav a eliminar--> {_navmesh.navMeshData}");
        _navmesh.RemoveData();
        //Debug.Log($"Nav ELIMINADA --> {_navmesh.navMeshData}");
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
        /*if (cuadricula != null)
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
        }*/
        if(listCubes != null)
        {
            for (int i = 0; i < listCubes.Count; i++)
            {
                GameObject cube = listCubes[i];
                if (cube != null)
                {
                    Destroy(cube);
                    //listCubes[i] = null;
                }
            }
            listCubes.Clear();
        }
        yield return 0;
        //yield return new WaitForSeconds(2f);
    }

    IEnumerator DoCreateCubes()
    {
        int cubesGenerated = 0;
        while (cubesGenerated < maxCubes)
        {
            Vector3 pos = GetRandomPosition();
            Vector3 position = pos + _gridOffset;
            if (!HasAdjacentCube(pos) && Random.value < diagonalProbability && HasNoDiagonalCube(pos))
            {
                _nextCubePrefab = listCubePrefab[cubesGenerated % listCubePrefab.Count];
                GameObject cube = Instantiate(_nextCubePrefab, position, Quaternion.identity);
                listCubes.Add(cube);
                cubesGenerated++;
            }

            // Agregar la posición a la lista de posiciones libres si no hay un cubo generado en esa posición
            if (!HasCube(position))
            {
                if(!freePositions.Contains(position))
    {
                    freePositions.Add(position);
                }
            }
        }
        //Guardo las posiciones libres
        //saveFreePositions();
        yield return 0;
    }

    private bool HasAdjacentCube(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / cubeSize);
        int y = Mathf.RoundToInt(pos.z / cubeSize);
        if (cuadricula[x, y] != null) return true;
        if (x > 0 && cuadricula[x - 1, y] != null) return true;
        if (x < width - 1 && cuadricula[x + 1, y] != null) return true;
        if (y > 0 && cuadricula[x, y - 1] != null) return true;
        if (y < height - 1 && cuadricula[x, y + 1] != null) return true;
        return false;
    }

    private bool HasNoDiagonalCube(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / cubeSize);
        int y = Mathf.RoundToInt(pos.z / cubeSize);
        if (x > 1 && y > 1 && cuadricula[x - 2, y - 2] != null) return false;
        if (x > 1 && y < height - 2 && cuadricula[x - 2, y + 2] != null) return false;
        if (x < width - 2 && y > 1 && cuadricula[x + 2, y - 2] != null) return false;
        if (x < width - 2 && y < height - 2 && cuadricula[x + 2, y + 2] != null) return false;
        return true;
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(0, width) * cubeSize;
        float y = 0;
        float z = Random.Range(0, height) * cubeSize;
        return new Vector3(x, y, z);
    }

    private bool HasCube(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, cubeSize / 2);
        foreach (Collider c in colliders)
        {
            if (c.gameObject.CompareTag("Cube"))
            {
                return true;
            }
        }
        return false;
    }


    /*IEnumerator DoCreateCubes()
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
                Vector2 checkPos = randomPos + direction * j * (wallSpacing *2);
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
                Vector2 pos = randomPos + direction * j;
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
    */
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

    /*//Rellena todo
    IEnumerator DoCreateCubes()
    {
        // Instancia los cubos en una cuadrícula
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _position = new Vector3(Random.Range(0, width), 0, Random.Range(0, height)) * cubeSize + _gridOffset;

                if (CanSpawnCube(_position))
                {
                    Instantiate(cubePrefab, _position, Quaternion.identity);

                    if (Random.value < spawnChance)
                    {
                        // Check adjacent positions and try to spawn a cube
                        Vector3[] adjacentOffsets = new Vector3[]
                        {
                    Vector3.forward,
                    Vector3.right,
                    Vector3.back,
                    Vector3.left
                        };

                        foreach (Vector3 adjacentOffset in adjacentOffsets)
                        {
                            Vector3 adjacentPosition = _position + adjacentOffset * cubeSize;

                            if (CanSpawnCube(adjacentPosition) && Random.value < spawnChance)
                            {
                                Instantiate(cubePrefab, adjacentPosition, Quaternion.identity);
                            }
                        }
                    }
                }

                //Vector3 position = new Vector3(x * cubeSize, 0, y * cubeSize) + _gridOffset;
                //_nextCubePrefab = listCubePrefab[(x + y) % listCubePrefab.Count];
                //Instantiate(_nextCubePrefab, position, Quaternion.identity);
            }
        }
        yield return 0;
    }

    private bool CanSpawnCube(Vector3 position)
    {
        // Check if the position is valid and there's no other cube nearby
        Collider[] colliders = Physics.OverlapSphere(position, cubeSize / 2);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Cube"))
            {
                return false;
            }
        }

        return true;
    }*/
    /*
    IEnumerator DoCreateCubes()
    {
        // Obtener el ancho y la altura del suelo
        int width = (int)(floor.transform.localScale.x / cubeSize);
        int height = (int)(floor.transform.localScale.z / cubeSize);

        // Generar una posición aleatoria dentro del área del suelo
        Vector3 GetRandomPosition()
        {
            float x = Random.Range(0, width) * cubeSize;
            float z = Random.Range(0, height) * cubeSize;
            return new Vector3(x, 0, z);
        }

        // Verificar si hay un cubo adyacente en cruz
        bool HasAdjacentCube(Vector3 pos)
        {
            bool hasCube = false;
            Vector3[] adjacentOffsets = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
            foreach (Vector3 offset in adjacentOffsets)
            {
                Vector3 adjacentPos = pos + offset * cubeSize;
                if (_positions.Contains(adjacentPos))
                {
                    hasCube = true;
                    break;
                }
            }
            return hasCube;
        }

        // Verificar si no hay un cubo en diagonal en una distancia mínima
        bool HasNoDiagonalCube(Vector3 pos)
        {
            bool hasNoCube = true;
            int numDiagonalCubes = 0;
            Vector3[] diagonalOffsets = { Vector3.forward + Vector3.right, Vector3.forward + Vector3.left,
                                      Vector3.back + Vector3.right, Vector3.back + Vector3.left };
            foreach (Vector3 offset in diagonalOffsets)
            {
                Vector3 diagonalPos = pos + offset * cubeSize;
                if (_positions.Contains(diagonalPos))
                {
                    numDiagonalCubes++;
                }
            }
            if (numDiagonalCubes >= 2 || (numDiagonalCubes == 1 && Random.value < probability))
            {
                hasNoCube = false;
            }
            return hasNoCube;
        }

        // Instancia los cubos en posiciones aleatorias
        for (int i = 0; i < numCubes; i++)
        {
            Vector3 position = GetRandomPosition();

            // Verificar si la posición es válida
            bool isValidPosition = !HasAdjacentCube(position) && HasNoDiagonalCube(position);

            if (isValidPosition)
            {
                _nextCubePrefab = listCubePrefab[Random.Range(0, listCubePrefab.Count)];
                Instantiate(_nextCubePrefab, position, Quaternion.identity);
                _positions.Add(position);
            }
        }

        yield return 0;
    }*/

    /*//Rellena todo
    IEnumerator DoCreateCubes()
    {
        // Instancia los cubos en una cuadrícula
        Vector3 ajust = new Vector3(-width * cubeSize / 2 + 1.15f, 0, -height * cubeSize / 2 + 1.25f);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cubeSize, 0, y * cubeSize) + _gridOffset;
                _nextCubePrefab = listCubePrefab[(x + y) % listCubePrefab.Count];
                Instantiate(_nextCubePrefab, position, Quaternion.identity);
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
        }
        yield return 0;
    }*/

    /*// Rellena aleatoriamente
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
