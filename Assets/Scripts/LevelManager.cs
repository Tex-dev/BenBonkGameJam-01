using UnityEngine;
using UnityEngine.Tilemaps;

using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal enum TilesID : ushort
{
    UP = 1,
    RIGHT = 2,
    DOWN = 4,
    LEFT = 8,
}

/// <summary>
/// Manages level files.
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    /// <summary>
    /// Type of the tile.
    /// </summary>
    public enum TilesType : ushort
    {
        EMPTY,
        BLOCK,
        UNMOVEABLE_BLOCK,
        TRAPPED_BLOCK,
        ENEMY,
        SPAWN,
        DESTINATION,
    }

    /// <summary>
    /// Quantity of levels present in the game.
    /// </summary>
    [SerializeField]
    private int m_LevelAmount = 1;

    /// <summary>
    /// List of block tiles
    /// </summary>
    [SerializeField]
    private Tile[] m_blockTiles = null;

    /// <summary>
    /// List of block tiles
    /// </summary>
    [SerializeField]
    private Tile[] m_unmovableBlockTiles = null;

    /// <summary>
    /// The tilemap where write level
    /// </summary>
    [SerializeField]
    private Tilemap m_physicTilemap = null;

    /// <summary>
    /// The tilemap where write level
    /// </summary>
    [SerializeField]
    private Tilemap m_unphysicTilemap = null;

    /// <summary>
    /// GameObject to display on pause.
    /// </summary>
    [SerializeField]
    private GameObject m_PauseMenu = null;

    /// <summary>
    /// Prefab for the enemy.
    /// </summary>
    [SerializeField]
    private GameObject m_EnemyPrefab = null;

    /// <summary>
    /// Prefab for the destination flag.
    /// </summary>
    [SerializeField]
    private GameObject m_FlagPrefab = null;

    [SerializeField]
    private GameObject m_blackholePrefab = null;

    private GameObject m_beginningBlackhole;

    private Vector3 m_spawnPosition;

    /// <summary>
    /// All spawned items.
    /// </summary>
    private List<GameObject> m_SpawnedItems = new List<GameObject>();

    /// <summary>
    /// Get the current level ID.
    /// </summary>
    public static int CurrentLevel => Instance.m_CurrentLevel;

    /// <summary>
    /// Current level ID.
    /// </summary>
    [SerializeField]
    private int m_CurrentLevel = 0;

    /// <summary>
    /// Is the current loading the first load of the scene?
    /// </summary>
    private bool m_IsInitialLevelLoading = true;

    /// <summary>
    /// Number max of block can be in the level
    /// </summary>
    [SerializeField]
    private int m_maxBlockPossible;

    public int MaxBlockPossible { get => m_maxBlockPossible; private set => m_maxBlockPossible = value; }

    /// <summary>
    /// Number of block used in the level
    /// </summary>
    private int m_nbBlockUsed;

    public int NbBlockUsed { get => m_nbBlockUsed; private set => m_nbBlockUsed = value; }

    /// <summary>
    /// Prevent destroying this game object on load.
    /// </summary>
    private void Awake()
    {
        m_LevelAmount = Directory.GetDirectories(Application.streamingAssetsPath + $"/Levels/").Length;

        if (!File.Exists(Application.dataPath + $"/Resources/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini"))
        {
            Directory.CreateDirectory(Application.dataPath + $"/Resources/level_{m_CurrentLevel}/");
            File.Copy(Application.streamingAssetsPath + $"/Levels/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini", Application.dataPath + $"/Resources/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini");

            List<string> lines = File.ReadLines(Application.dataPath + $"/Resources/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini").ToList();
            List<string> editedLines = new List<string>();
            foreach (string line in lines)
            {
                Regex regex = new Regex("[a-zA-Z0-9 -]");
                editedLines.Add(regex.Replace(line, " "));
            }
            File.WriteAllLines(Application.dataPath + $"/Resources/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini", editedLines);
        }

        GenerateLevelFromFile(Application.streamingAssetsPath + $"/Levels/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini");

        if (m_CurrentLevel > 0)
        {
            m_beginningBlackhole = Instantiate(m_blackholePrefab, m_spawnPosition + new Vector3(0.5f, 0.5f), Quaternion.identity);
            m_beginningBlackhole.GetComponent<BlackHoleManager>().BeginLevel(true);
        }
    }

    /// <summary>
    /// Called by Unity on new level loaded.
    /// </summary>
    /// <param name="level">ID of the new level.</param>
    private void OnLevelWasLoaded(int level)
    {
        m_PauseMenu.SetActive(false);
    }

    /// <summary>
    /// Reset file for the specified level.
    /// </summary>
    /// <param name="level">Level to reset the file of.</param>
    public void LoadLevel(int level, bool reloadOriginalFile = true, bool forceRespawn = false)
    {
        if (!File.Exists(Application.dataPath + $"/Resources/level_{level}/level_{level}_blocks.ini"))
        {
            Directory.CreateDirectory(Application.dataPath + $"/Resources/level_{level}/");
            File.Copy(Application.streamingAssetsPath + $"/Levels/level_{level}/level_{level}_blocks.ini", Application.dataPath + $"/Resources/level_{level}/level_{level}_blocks.ini");
        }

        if (reloadOriginalFile)
        {
            // Rewrite original file in player file
            List<string> originalLines = File.ReadLines(Application.streamingAssetsPath + $"/Levels/level_{level}/level_{level}_blocks.ini").ToList();
            List<string> editedLines = new List<string>();
            foreach (string line in originalLines)
            {
                Regex regex = new Regex("[a-zA-Z0-9 -]");
                editedLines.Add(regex.Replace(line, " "));
            }
            File.WriteAllLines(Application.dataPath + $"/Resources/level_{level}/level_{level}_blocks.ini", editedLines);

            // Clear all previously spawned item.
            foreach (GameObject item in m_SpawnedItems)
            {
                Destroy(item);
            }
            m_SpawnedItems.Clear();
        }

        //*
        if (reloadOriginalFile)
            GenerateLevelFromFile(Application.streamingAssetsPath + $"/Levels/level_{level}/level_{level}_blocks.ini");
        else
            GenerateLevelFromFile(Application.streamingAssetsPath + $"/Levels/level_{level}/level_{level}_blocks.ini", Application.dataPath + $"/Resources/level_{level}/level_{level}_blocks.ini", true, forceRespawn);//*/

        //        GenerateLevelFromFile(Application.streamingAssetsPath + $"/Levels/level_{level}/level_{level}_blocks.ini");

        m_CurrentLevel = level;
    }

    /// <summary>
    /// Called when the application focus is changed.
    /// </summary>
    /// <param name="focus">Status of the focus. ON = true, OFF = false.</param>
    private void OnApplicationFocus(bool focus)
    {
        m_PauseMenu.SetActive(!focus);

        if (focus)
        {
            PlayerManager.Play();
            GenerateLevelFromFile(Application.streamingAssetsPath + $"/Levels/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini", Application.dataPath + $"/Resources/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini", true);

            FakeOS.Instance.CloseFile();
        }
        else
            PlayerManager.Pause();
    }

    /// <summary>
    /// Generate the level from a specified path.
    /// </summary>
    /// <param name="levelFilePath">PAth of the level file.</param>
    public void GenerateLevelFromFile(string levelFilePath, string levelFilePathPlayer = "", bool resetOnlyBlocks = false, bool forceRespawn = false)
    {
        int width = 0, height = 0;
        string[] fileLinesOther;
        string[] fileLinesBlock;

        if (levelFilePathPlayer == "")
            levelFilePathPlayer = levelFilePath;

        // Tableaux représentant le niveaux.
        int[,] levelTiles = null;          // Chaque case contient l'indice de la tile qu'elle contient.
        TilesType[,] map = null;           // Chaque case contient le type de bloc qu'elle contient.

        // 1°) Extraction des informations du fichier de config
        //////////////////////////////////////////////////////////////
        ReadInformationsFromFile(levelFilePath, ref width, ref height, out fileLinesOther);
        ReadInformationsFromFile(levelFilePathPlayer, ref width, ref height, out fileLinesBlock);

        // 2°) Chargement du niveau à partir du fichier
        //////////////////////////////////////////////////////////////
        LoadBlocksMap(fileLinesBlock, width, height, out map);
        LoadOtherMap(fileLinesOther, width, height, ref map);

        LoadTilesFromMap(width, height, map, out levelTiles);

        VerifyNbBlockUsed(width, height, map);

        // 3°) Affichage des tuiles à l'écran
        //////////////////////////////////////////////////////////////
        DrawLevel(width, height, map, levelTiles, resetOnlyBlocks, forceRespawn);
    }

    /*
    /// <summary>
    /// Generate the level from a specified path.
    /// </summary>
    /// <param name="levelFilePath">PAth of the level file.</param>
    public void GenerateLevelFromFile(string originalLevelFilePath, string playerLevelFilePath)
    {
        Vector3Int currentCell;
        StreamReader reader;
        string originalContent, playerContent;
        int nbPossiblebloc = m_highlightTile.Length / 16;

        // Tableaux représentant le niveaux.
        int[,] levelTiles;          // Chaque case contient l'indice de la tile qu'elle contient.

        TilesType[,] map;           // Chaque case contient le type de bloc qu'elle contient.

        // 1°) Extraction des informations du fichier de config
        //////////////////////////////////////////////////////////////

        reader = new StreamReader(originalLevelFilePath);
        originalContent = reader.ReadToEnd();
        reader.Close();

        reader = new StreamReader(playerLevelFilePath);
        playerContent = reader.ReadToEnd();
        reader.Close();

        // On supprime les potentiels saut de ligne en fin de fichier.
        while (originalContent.EndsWith("\n") || originalContent.EndsWith("\r"))
            originalContent = originalContent.Remove(originalContent.Length - 1);

        while (playerContent.EndsWith("\n") || playerContent.EndsWith("\r"))
            playerContent = playerContent.Remove(playerContent.Length - 1);

        string[] lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].EndsWith("\r"))
                lines[i] = lines[i].Remove(lines[i].Length - 1);
        }

        string[] lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].EndsWith("\r"))
                lines[i] = lines[i].Remove(lines[i].Length - 1);
        }

        // On déduit la taille du niveau en fonction des informations lues.
        int height = lines.Length;
        int width = 0;
        foreach (string line in lines)
            width = Mathf.Max(width, line.Length);

        levelTiles = new int[width, height];
        map = new TilesType[width, height];

        // 2°) Chargement du niveau à partir du fichier
        //////////////////////////////////////////////////////////////

        // Boucle permettant de savoir s'il y a ou pas une tile sur la case.
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (i < lines[j].Length)
                {
                    switch (lines[j][i])
                    {
                        case ' ':
                            map[i, height - 1 - j] = TilesType.EMPTY;
                            break;

                        case '*':
                            map[i, height - 1 - j] = TilesType.BLOCK;
                            break;

                        case 's':
                            map[i, height - 1 - j] = TilesType.SPAWN;
                            break;

                        case 'd':
                            map[i, height - 1 - j] = TilesType.DESTINATION;
                            break;

                        case 'e':
                            map[i, height - 1 - j] = TilesType.ENEMY;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    map[i, height - 1 - j] = TilesType.EMPTY;
                }
            }
        }

        // Boucle afin d'affecter la bonne tuile à chaque case (dépendant des voisins).
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j] == TilesType.BLOCK)
                {
                    int tileID = 0;

                    // UP
                    if (j < height - 1 && map[i, j + 1] == TilesType.BLOCK)
                        tileID += (int)TilesID.UP;

                    // DOWN
                    if (j > 0 && map[i, j - 1] == TilesType.BLOCK)
                        tileID += (int)TilesID.DOWN;

                    // RIGHT
                    if (i < width - 1 && map[i + 1, j] == TilesType.BLOCK)
                        tileID += (int)TilesID.RIGHT;

                    // LEFT
                    if (i > 0 && map[i - 1, j] == TilesType.BLOCK)
                        tileID += (int)TilesID.LEFT;

                    levelTiles[i, j] = tileID;
                }
            }
        }

        // 3°) Affichage des tuiles à l'écran
        //////////////////////////////////////////////////////////////

        currentCell = new Vector3Int(0, 0, 0);

        // On efface l'écran (on enlève les anciennes tuiles).
        for (currentCell.x = 0; currentCell.x < 50; currentCell.x++)
        {
            for (currentCell.y = 0; currentCell.y < 200; currentCell.y++)
                m_highlightMap.SetTile(currentCell, null);
        }

        // Boucle permettant l'affichage des tuiles.
        for (currentCell.x = 0; currentCell.x < width; (currentCell.x)++)
        {
            for (currentCell.y = 0; currentCell.y < height; (currentCell.y)++)
            {
                switch (map[currentCell.x, currentCell.y])
                {
                    case TilesType.EMPTY:
                        m_highlightMap.SetTile(currentCell, null);
                        break;

                    case TilesType.BLOCK:
                        {
                            int idTile;
                            int rand = Random.Range(0, nbPossiblebloc);
                            idTile = rand * 16 + levelTiles[currentCell.x, currentCell.y];

                            m_highlightMap.SetTile(currentCell, m_highlightTile[idTile]);
                            break;
                        }

                    case TilesType.ENEMY:
                        GameObject enemy = Instantiate(m_EnemyPrefab);
                        enemy.transform.position = new Vector2(currentCell.x + 0.5f, currentCell.y + 0.5f);

                        m_SpawnedItems.Add(enemy);
                        break;

                    case TilesType.SPAWN:
                        PlayerManager.Instance.transform.position = new Vector2(currentCell.x + 0.5f, currentCell.y + 0.5f);

                        break;

                    case TilesType.DESTINATION:
                        GameObject flag = Instantiate(m_FlagPrefab);
                        flag.transform.position = new Vector2(currentCell.x + 0.48f, currentCell.y + 0.48f);

                        m_SpawnedItems.Add(flag);
                        break;

                    default:
                        break;
                }
            }
        }
    }//*/

    /// <summary>
    /// This function extract informations from config file to have the width and the height of the level
    /// </summary>
    /// <param name="levelFilePath">Path of the file</param>
    /// <param name="width">ref parameter to get width of the level</param>
    /// <param name="height">ref parameter to get height of the level</param>
    private void ReadInformationsFromFile(string levelFilePath, ref int width, ref int height, out string[] lines)
    {
        StreamReader reader;
        string content;
        int h, w;

        // 1°) Extraction des informations du fichier de config
        //////////////////////////////////////////////////////////////

        reader = new StreamReader(levelFilePath);
        content = reader.ReadToEnd();
        reader.Close();

        // On supprime les potentiels saut de ligne en fin de fichier.
        while (content.EndsWith("\n") || content.EndsWith("\r"))
            content = content.Remove(content.Length - 1);

        lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].EndsWith("\r"))
                lines[i] = lines[i].Remove(lines[i].Length - 1);
        }

        // On déduit la taille du niveau en fonction des informations lues.
        h = lines.Length;
        w = 0;
        foreach (string line in lines)
            w = Mathf.Max(w, line.Length);

        width = Mathf.Max(width, w);
        height = Mathf.Max(height, h);
    }

    private void LoadBlocksMap(string[] lines, int width, int height, out TilesType[,] map)
    {
        map = new TilesType[width, height];

        // Boucle permettant de savoir s'il y a ou pas une tile sur la case.
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (i < lines[j].Length)
                {
                    switch (lines[j][i])
                    {
                        case ' ':
                            map[i, height - 1 - j] = TilesType.EMPTY;
                            break;

                        case '*':
                            map[i, height - 1 - j] = TilesType.BLOCK;
                            break;

                        default:
                            map[i, height - 1 - j] = TilesType.EMPTY;
                            break;
                    }
                }
                else
                {
                    map[i, height - 1 - j] = TilesType.EMPTY;
                }
            }
        }
    }

    private void LoadOtherMap(string[] lines, int width, int height, ref TilesType[,] map)
    {
        // Boucle permettant de savoir s'il y a ou pas une tile sur la case.
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (i < lines[j].Length)
                {
                    switch (lines[j][i])
                    {
                        case 's':
                            map[i, height - 1 - j] = TilesType.SPAWN;
                            map[i, height - 2 - j] = TilesType.UNMOVEABLE_BLOCK;

                            m_spawnPosition = new Vector3(i, height - 1 - j);

                            break;

                        case 'd':
                            map[i, height - 1 - j] = TilesType.DESTINATION;
                            map[i, height - 2 - j] = TilesType.UNMOVEABLE_BLOCK;
                            break;

                        case '+':
                            map[i, height - 1 - j] = TilesType.UNMOVEABLE_BLOCK;
                            break;

                        case 'e':
                            map[i, height - 1 - j] = TilesType.ENEMY;
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    map[i, height - 1 - j] = TilesType.EMPTY;
                }
            }
        }
    }

    private void LoadTilesFromMap(int width, int height, TilesType[,] map, out int[,] levelTiles)
    {
        levelTiles = new int[width, height];

        // Boucle afin d'affecter la bonne tuile à chaque case (dépendant des voisins).
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j] == TilesType.UNMOVEABLE_BLOCK || map[i, j] == TilesType.BLOCK)
                {
                    int tileID = 0;

                    // UP
                    if (j < height - 1 && map[i, j + 1] == map[i, j])
                        tileID += (int)TilesID.UP;

                    // DOWN
                    if (j > 0 && map[i, j - 1] == map[i, j])
                        tileID += (int)TilesID.DOWN;

                    // RIGHT
                    if (i < width - 1 && map[i + 1, j] == map[i, j])
                        tileID += (int)TilesID.RIGHT;

                    // LEFT
                    if (i > 0 && map[i - 1, j] == map[i, j])
                        tileID += (int)TilesID.LEFT;

                    levelTiles[i, j] = tileID;
                }
            }
        }
    }

    private void VerifyNbBlockUsed(int width, int height, TilesType[,] map)
    {
        int index = 0;
        int[] randomIDs;
        bool[] isTrapped;
        m_nbBlockUsed = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TilesType.BLOCK)
                    m_nbBlockUsed++;
            }
        }

        if (m_nbBlockUsed < m_maxBlockPossible)
            return;

        randomIDs = Utils.GenerateListRandomNUmber(0, m_nbBlockUsed, m_nbBlockUsed - m_maxBlockPossible);
        isTrapped = new bool[m_nbBlockUsed];

        for (int i = 0; i < m_nbBlockUsed; i++)
            isTrapped[i] = false;

        for (int i = 0; i < randomIDs.Length; i++)
            isTrapped[randomIDs[i]] = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TilesType.BLOCK)
                {
                    if (isTrapped[index])
                        map[x, y] = TilesType.TRAPPED_BLOCK;

                    index++;
                }
            }
        }
    }

    private void DrawLevel(int width, int height, TilesType[,] map, int[,] levelTiles, bool resetOnlyBlocks, bool forceRespawn)
    {
        int nbPossiblebloc = m_blockTiles.Length / 16;
        Vector3Int currentCell = new Vector3Int(0, 0, 0);

        // On efface l'écran (on enlève les anciennes tuiles).
        for (currentCell.x = 0; currentCell.x < 50; currentCell.x++)
        {
            for (currentCell.y = 0; currentCell.y < 200; currentCell.y++)
                m_physicTilemap.SetTile(currentCell, null);
        }

        // Boucle permettant l'affichage des tuiles.
        for (currentCell.x = 0; currentCell.x < width; (currentCell.x)++)
        {
            for (currentCell.y = 0; currentCell.y < height; (currentCell.y)++)
            {
                switch (map[currentCell.x, currentCell.y])
                {
                    case TilesType.EMPTY:
                        m_physicTilemap.SetTile(currentCell, null);
                        break;

                    case TilesType.BLOCK:
                        {
                            int idTile;
                            int rand = Random.Range(0, nbPossiblebloc);
                            idTile = rand * 16 + levelTiles[currentCell.x, currentCell.y];

                            m_physicTilemap.SetTile(currentCell, m_blockTiles[idTile]);
                            break;
                        }

                    case TilesType.UNMOVEABLE_BLOCK:
                        {
                            int idTile;
                            int rand = Random.Range(0, nbPossiblebloc);
                            idTile = rand * 16 + levelTiles[currentCell.x, currentCell.y];

                            m_physicTilemap.SetTile(currentCell, m_unmovableBlockTiles[idTile]);
                            break;
                        }

                    case TilesType.TRAPPED_BLOCK:
                        {
                            int idTile;
                            int rand = Random.Range(0, nbPossiblebloc);
                            idTile = rand * 16 + levelTiles[currentCell.x, currentCell.y];

                            m_unphysicTilemap.SetTile(currentCell, m_blockTiles[idTile]);
                            break;
                        }

                    case TilesType.ENEMY:
                        if (!resetOnlyBlocks)
                        {
                            GameObject enemy = Instantiate(m_EnemyPrefab);
                            enemy.transform.position = new Vector2(currentCell.x + 0.5f, currentCell.y + 0.5f);

                            m_SpawnedItems.Add(enemy);
                        }
                        break;

                    case TilesType.SPAWN:
                        if (forceRespawn || !resetOnlyBlocks)
                        {
                            PlayerManager.Instance.transform.position = new Vector2(currentCell.x + 0.5f, currentCell.y + 0.5f);
                            PlayerManager.Instance.Respawn();
                        }

                        break;

                    case TilesType.DESTINATION:
                        if (!resetOnlyBlocks)
                        {
                            GameObject flag = Instantiate(m_FlagPrefab);
                            flag.transform.position = new Vector2(currentCell.x + 0.24f, currentCell.y + 0.48f);

                            m_SpawnedItems.Add(flag);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public GameObject GetBeginningBlackhole()
    {
        return m_beginningBlackhole;
    }

    public void GoToNextLevel()
    {
        m_CurrentLevel++;
    }
}