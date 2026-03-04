using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * Clase para construir la grid lógica dados unos tilemaps que representen diferentes superficies (suelo, paredes, lava...) y que tengan diferentes comportaientos
 * 
 * La clave está en hacer una conversión y comprimir los tilemaps en 1
 * 
 * Se hará una traducción de tiles, según del tilemap que venga, esa tile será de un tipo o de otra
 * 
 * Se crea el SO TileType que contiene los atributos releveantes de una tile (isWalkable, coste de movimiento, daño...)
 * 
 * En el inspector se define el diccionario que traduce que tile de cada tilemap se convierte a una TileType dada (los tilemaps están formados por TileBase (objeto de unity), que se contempla como un sprite
 * sin propiedades. Nosotros lo que queremos hacer es otorgar lógica a las tiles, por lo que decimos "si esta tile viene del tilemap del suelo -> le asignamos esta tiletype que tiene estás propieda)
 * 
 * Se calcula cual es el tamaño más grande de tilemap para no dejar niguna tile fuera y se recorren las tiles para guardar la lógica en la grid lógica.
 * 
 */


/*
 * Clase que define los atributos relevantes que queremos implementar de las tiles. Habrá que crear SO por cada tile diferente que queramos en el proyecto
 */
[CreateAssetMenu(fileName = "TileType", menuName = "Scriptable Objects/TileType")]
public class TileType : ScriptableObject
{
    public int movementCost;
    public bool isWalkable;
}

public class DynamicTile
{
    public GameObject occupant;
}

/*
 * Clase auxiliar para mapear más fácilmente una tile cualquiera con una tyletype
 */
[System.Serializable]
public class TileMapping
{
    public TileBase tileBase;
    public TileType tileType;
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public Grid unityGrid;
    public Tilemap paredes, suelo;  //Tilemaps que nosotros dibujamos, separar según comportamiento esperado
    public List<TileMapping> tileMappings;  //Lista de mapeado, que tiles corresponden con cada tyletype

    public TileType defaultTileType;
    TileType[,] gridLogica;      //Grid lógica a la que le haremos las consultas de las acciones
    [HideInInspector]public DynamicTile[,] gridDinamica;
    Dictionary<TileBase, TileType> diccionarioTiles;    //Estructura que contiene las traducciones
    Vector2Int gridOffset;
    int width, height;

    public TileType[,] GetGridLogica() {  return gridLogica; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        //Convertir la lista de mapeado en el diccionario
        diccionarioTiles = tileMappings.ToDictionary(t => t.tileBase, t => t.tileType);
        ConstruirGridLogica();
        AStarManager.Instance.BuildGrid();
    }

    public void ConstruirGridLogica()
    {
        //Calcular el tamaño global combinado de todos los tilemaps
        BoundsInt boundGlobal = paredes.cellBounds;
        RecalcularBoundsGrid(ref boundGlobal, suelo.cellBounds);

        gridOffset = new Vector2Int(boundGlobal.xMin, boundGlobal.yMin);

        width = boundGlobal.size.x;
        height = boundGlobal.size.y;

        gridLogica = new TileType[width, height];
        gridDinamica = new DynamicTile[width, height];

        //Recorrer cada tile de la grid lógica
        for (int x = boundGlobal.xMin; x < boundGlobal.xMax; x++)
        {
            for (int y = boundGlobal.yMin; y < boundGlobal.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                TileType tile = defaultTileType;    //Pared es el tipo por defecto

                if (suelo.HasTile(cell))
                {
                    TileBase tileBase = suelo.GetTile(cell);
                    if (diccionarioTiles.TryGetValue(tileBase, out TileType mapped))
                        tile = mapped;
                }

                if (paredes.HasTile(cell))
                {
                    tile = defaultTileType; // o wallTileType explícito
                }

                //Obtener la tile correspondiente
                //TileBase tile = suelo.GetTile(cell);

                //Si no hay, asignamos una por defecto
                //if(tile == null)
                //{
                //    gridLogica[x - boundGlobal.xMin, y - boundGlobal.yMin] = defaultTileType;
                //    continue;
                //}

                //Si hay tile, obtenemos a que tiletype corresponde
                gridLogica[x - boundGlobal.xMin, y - boundGlobal.yMin] = tile;
                DynamicTile dtile = new DynamicTile();
                gridDinamica[x - boundGlobal.xMin, y - boundGlobal.yMin] = dtile;
            }
        }
    }

    public TileType GetTile(Vector2Int gridPos)
    {
        return gridLogica[
            gridPos.x,
            gridPos.y
        ];
        //return gridLogica[
        //    gridPos.x - gridOffset.x,
        //    gridPos.y - gridOffset.y
        //];
    }

    private void RecalcularBoundsGrid(ref BoundsInt total, BoundsInt other)
    {
        int xMin = Mathf.Min(total.xMin, other.xMin);
        int yMin = Mathf.Min(total.yMin, other.yMin);
        int xMax = Mathf.Max(total.xMax, other.xMax);
        int yMax = Mathf.Max(total.yMax, other.yMax);

        total = new BoundsInt(
            xMin,
            yMin,
            0,
            xMax - xMin,
            yMax - yMin,
            1
        );
    }
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        // Convertimos grid lógica → celda del tilemap
        Vector3Int cell = new Vector3Int(
            gridPos.x + gridOffset.x,
            gridPos.y + gridOffset.y,
            0
        );

        // Cell → mundo usando el Grid de Unity
        Vector3 world = unityGrid.CellToWorld(cell);

        // Centramos en la celda
        world += unityGrid.cellSize / 2f;

        return world;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cell = unityGrid.WorldToCell(worldPos);

        return new Vector2Int(
            cell.x - gridOffset.x,
            cell.y - gridOffset.y
        );
    }

    public bool TryMoverEntidadHasta(Entidad entityToMove, Vector2Int targetPos)
    {
        TileType targetCell = GetTile(targetPos);
        if (targetCell != null && targetCell.isWalkable && gridDinamica[targetPos.x, targetPos.y].occupant == null)
        {
            gridDinamica[entityToMove.currentPos.x, entityToMove.currentPos.y].occupant = null;
            entityToMove.currentPos = targetPos;
            gridDinamica[entityToMove.currentPos.x, entityToMove.currentPos.y].occupant = entityToMove.gameObject;
            entityToMove.transform.position = GridToWorld(entityToMove.currentPos);
            return true;
        }
        else
            return false;
    }

    public GameObject ObtenerOcupanteCelda(Vector2Int celda)
    {
        return gridDinamica[celda.x, celda.y].occupant;
    }

    void OnDrawGizmos()
    {
        if (gridLogica == null) return;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Gizmos.color = gridLogica[x, y].isWalkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(GridToWorld(new Vector2Int(x, y)), Vector3.one * 0.9f);
            }
    }

    public List<GameObject> ObtenerVecinos(Vector2Int celda)
    {
        var list = new List<GameObject>();

        if (gridDinamica[celda.x + 1, celda.y].occupant != null) list.Add(gridDinamica[celda.x + 1, celda.y].occupant);
        if (gridDinamica[celda.x - 1, celda.y].occupant != null) list.Add(gridDinamica[celda.x -1, celda.y].occupant);
        if (gridDinamica[celda.x, celda.y + 1].occupant != null) list.Add(gridDinamica[celda.x, celda.y + 1].occupant);
        if (gridDinamica[celda.x, celda.y -1].occupant != null) list.Add(gridDinamica[celda.x, celda.y - 1].occupant);
        return list;
    }
}
