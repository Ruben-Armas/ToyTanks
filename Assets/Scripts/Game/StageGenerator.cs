using System.Collections.Generic;
//using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageGenerator : MonoBehaviour
{
    public Terrain terrain;

    private int _width;
    private int _height;
    public int numDiggers = 5;

    //Lista de celdas vacías/disponibles para meter al jugador
    private List<Vector2Int> walkableCells;

    private int _dig_offset = 2;
    //Matriz del mapa de alturas
    private float[,] _heightmap;

    void Awake()
    {
        _width = terrain.terrainData.heightmapResolution;
        _height = terrain.terrainData.heightmapResolution;
        //GenerateLevel();
    }

    public void GenerateLevel()
    {
        //Matriz del mapa de alturas
        _heightmap = new float[_width, _height];
        //Inicializamos el terreno a MUROS
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                //heightmap[x, y] = 1;  //Suelo o montaña
                //heightmap[x, y] = 0.25f * Random.value;    //Más gradual
                //Gradual con Perlin    Min=0.05
                float perlinHeight = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.95f;
                _heightmap[x, y] = 0.05f + perlinHeight * perlinHeight;    //Más gradual

            }
        }
        /*
        //Tuneladores
        List<Vector2Int> diggers = new List<Vector2Int>();
        for (int i = 0; i < numDiggers; i++)
        {
            //Inicializado los diggers en medio del mapa    (también puedo usar un array)
            diggers.Add(new Vector2Int(_width / 2, _height / 2));
        }

        //Semilla si es siempre la misma siempre generaremos el mismo mapa
        //Random.InitState(42);
        for (int iterations = 0; iterations < _width * _height / 2; iterations++)
        {
            for (int i = 0; i < diggers.Count; i++)
            {
                //Excarbamos 3 Posiciones (la actual, la anterior y la siguiente)                
                for (int x = -_dig_offset; x <= _dig_offset; x++)
                {
                    for (int y = -_dig_offset; y <= _dig_offset; y++)
                    {
                        Vector2Int digPosition = new Vector2Int(diggers[i].x + x, diggers[i].y + y);
                        _heightmap[digPosition.x, digPosition.y] = 0;
                    }
                }

                Vector2Int delta = new Vector2Int(0, 0);
                //Limitar que no se mueva en diagonal
                if (Random.value < 0.5)
                    delta.x = Random.value < 0.5f ? -1 : 1;
                else
                    delta.y = Random.value < 0.5f ? -1 : 1;

                Vector2Int next_position = diggers[i] + delta;
                //Comprobar que no nos hemos salido del mapa    (dejando una celda alrededor como muro)
                if (next_position.x > _dig_offset && next_position.x < _width - _dig_offset - 1 &&
                    next_position.y > _dig_offset && next_position.y < _height - _dig_offset - 1)
                    diggers[i] = next_position;
            }
        }

        //Datos del terreno     (float, float, mapa de alturas)
        //terrain.terrainData.SetHeights(0, 0, _heightmap);
        //terrain.terrainData.SyncHeightmap();

        //Malla de navegación por código con el paquete experimental
        NavMeshSurface navmesh = terrain.gameObject.GetComponent<NavMeshSurface>();
        navmesh.BuildNavMesh();

        FindWalkablePositions();*/
    }

    //Para dejar espacio entre los muros y dónde se genera el jugador, para que no caiga
    void FindWalkablePositions()
    {
        walkableCells = new List<Vector2Int>();
        for (int x = _dig_offset; x < _width - _dig_offset; x++)
        {
            for (int y = _dig_offset; y < _height - _dig_offset; y++)
            {
                float height = _heightmap[x, y];
                if (height == 0)
                {
                    //Cuenta el número de paredes alrededor de la Posición
                    int walls = 0;
                    for (int nx = -_dig_offset; nx <= _dig_offset; nx++)
                    {
                        for (int ny = -_dig_offset; ny <= _dig_offset; ny++)
                        {
                            float nheight = _heightmap[x + nx, y + ny];
                            if (nheight != 0)
                                walls++;
                        }
                    }

                    if (walls == 0)
                    {
                        //Añado las nuevas posiciones escarbadas a la lista de Posiciones
                        if (walkableCells.Contains(new Vector2Int(x, y)) == false)
                            walkableCells.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }
    /*
    //Para pintar un Debug de los Gizmos (para ver que las celdas coinciden con el mapa)
    ///* MUY COSTOSO
    public void OnDrawGizmos()
    {
        if (walkableCells == null)
            return;
        foreach (Vector2Int pos in walkableCells)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(CellToWorldPoint(pos), 1);
        }
    }//*/
    /*
    public Vector2Int GetRandomWalkableCell()
    {
        return walkableCells[Random.Range(0, walkableCells.Count)];
    }

    //Colocar al enemigo
    public Vector3 GetPlayerStartPosition()
    {
        Vector2Int randomWalkableCell = GetRandomWalkableCell();
        return CellToWorldPoint(randomWalkableCell);
    }

    public Vector3 GetEnemyNestPosition(Vector3 playerWorldPosition)
    {
        Vector2Int playerCellPosition = WorlPointToCell(playerWorldPosition);
        Vector2Int furthestCell = GetCellAwayFrom(playerCellPosition);
        return (CellToWorldPoint(furthestCell));
    }
    */
    //Calcula la equivalencia del tamaño del mundo al terreno con el código
    private Vector3 CellToWorldPoint(Vector2Int cellPosition)
    {
        float modX = terrain.terrainData.size.x / terrain.terrainData.heightmapResolution;
        float modY = terrain.terrainData.size.z / terrain.terrainData.heightmapResolution;
        Vector3 worldPoint = new Vector3(cellPosition.y * modX, 0, cellPosition.x * modY);
        //Debug.Log(worldPoint);
        return worldPoint + Vector3.up;
    }
    //Calcula la equivalencia del tamaño del terreno al mundo con el código
    private Vector2Int WorlPointToCell(Vector3 worldPosition)
    {
        float modX = terrain.terrainData.size.x / terrain.terrainData.heightmapResolution;
        float modY = terrain.terrainData.size.z / terrain.terrainData.heightmapResolution;
        int x = Mathf.FloorToInt(worldPosition.x / modX);
        int y = Mathf.FloorToInt(worldPosition.z / modY);
        Vector2Int cellPosition = new Vector2Int(y, x);
        return cellPosition;
    }
    /*
    private Vector2Int GetCellAwayFrom(Vector2Int cellPosition)
    {
        if (walkableCells == null || walkableCells.Count == 0) return cellPosition;

        //Copia de la lista original
        List<Vector2Int> sortedCells = new List<Vector2Int>(walkableCells);
        //Ordena    Comparando 2 vectores (vector2Int)
        sortedCells.Sort((posA, posB) =>
        {
            float distA = (cellPosition - posA).magnitude;
            float distB = (cellPosition - posB).magnitude;
            if (distA == distB) return 0;
            if (distA < distB) return 1;
            return -1;
            //return distA > distB ? -1 : 1;
        });
        return sortedCells[0];
    }*/
}
