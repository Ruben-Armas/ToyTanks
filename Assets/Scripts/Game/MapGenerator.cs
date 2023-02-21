using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGenerator : MonoBehaviour
{
    public GameObject floor;
    public GameObject cubePrefab;
    public GameObject cube2Prefab;
    //public int numObstacles = 10;
    //public int gridSize = 8; // Tamaño de la cuadrícula, en número de cubos
    //public float gridSpacing = 1f; // Espaciado entre cubos
    
    public float cubeSize = 2.5f;
    public int width;
    public int height;
    public GameObject[,] cuadricula;

    private void Awake()
    {
        cuadricula = new GameObject[width, height];

    }
    void Start()
    {
        //GenerateMap();
        width = (int)(floor.transform.localScale.x / cubeSize);
        height = (int)(floor.transform.localScale.z / cubeSize);
        Debug.Log($"width -> {width}");
        Debug.Log($"height -> {height}");
        //cuadricula = new GameObject[x][y];

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

        // Instanciar los cubos
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
                /*float xPos = x * cubeSize + startPosition.x;
                float zPos = y * cubeSize + startPosition.z;
                Vector3 cubePosition = new Vector3(xPos, startPosition.y, zPos);
                Instantiate(cubePrefab, cubePosition, Quaternion.identity);
                */
                if(Random.Range(1,100) > 50)
                {
                    //Genera el cubo
                    GameObject cube = Instantiate(cubePrefab, transform);
                    cube.transform.position = new Vector3(
                        (x - width / 2) * cubeSize,
                        0f,
                        (y - height / 2) * cubeSize
                    );
                    cuadricula[x, y] = cube;
                }
                
            }
        }

        //Malla de navegación por código con el paquete experimental
        NavMeshSurface navmesh = floor.gameObject.GetComponent<NavMeshSurface>();
        navmesh.BuildNavMesh();

        /*
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
                // Asegura que los cubos estén unidos sin superponerse
                /*if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    Instantiate(cubePrefab, position, Quaternion.identity);
                }
                else if (x % 2 == 0 && y % 2 == 0)
                {
                    Instantiate(cubePrefab, position, Quaternion.identity);
                }*/
        /*}
    }*/
    }
}
