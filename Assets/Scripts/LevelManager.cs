using UnityEngine;
using UnityEngine.Tilemaps;

using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal enum TilesID : ushort
{
    UP = 1,
    RIGHT = 2,
    DOWN = 4,
    LEFT = 8,
}

public class LevelManager : Singleton<LevelManager>
{
    public enum TilesType : ushort
    {
        EMPTY,
        BLOCK,
        UNMOVEABLE_BLOCK,
        ENEMY,
        SPAWN,
        DESTINATION,
    }

    /// <summary>
    /// Quantity of levels present in the game.
    /// </summary>
    [SerializeField]
    private int m_LevelAmount = 1;

    [SerializeField]
    private Tile[] m_highlightTile;

    [SerializeField]
    private Tilemap m_highlightMap;

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

    /// <summary>
    /// Get the current level ID.
    /// </summary>
    public static int CurrentLevel => Instance.m_CurrentLevel;

    /// <summary>
    /// Current level ID.
    /// </summary>
    private int m_CurrentLevel = 0;

    /// <summary>
    /// Is the current loading the first load of the scene?
    /// </summary>
    private bool m_IsInitialLevelLoading = true;

    /// <summary>
    /// Prevent destroying this game object on load.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(this);

        m_LevelAmount = Directory.GetDirectories(Application.streamingAssetsPath + $"/Levels/").Length;

        for (int i = 0; i < m_LevelAmount; i++)
        {
            if (!File.Exists(Application.dataPath + $"/Resources/level_{i}/level_{i}_blocks.ini"))
            {
                Directory.CreateDirectory(Application.dataPath + $"/Resources/level_{i}/");
                File.Copy(Application.streamingAssetsPath + $"/Levels/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini", Application.dataPath + $"/Resources/level_{i}/level_{i}_blocks.ini");

                List<string> lines = File.ReadLines(Application.dataPath + $"/Resources/level_{i}/level_{i}_blocks.ini").ToList();
                List<string> editedLines = new List<string>();
                foreach (string line in lines)
                {
                    Regex regex = new Regex("[a-zA-Z0-9 -]");
                    editedLines.Add(regex.Replace(line, " "));
                }
                File.WriteAllLines(Application.dataPath + $"/Resources/level_{i}/level_{i}_blocks.ini", editedLines);
            }
        }
    }

    /// <summary>
    /// Reset file for the specified level.
    /// </summary>
    /// <param name="level">Level to reset the file of.</param>
    public void ResetLevelFile(int level)
    {
        if (!File.Exists(Application.dataPath + $"/Resources/level_{level}/level_{level}_blocks.ini"))
        {
            Directory.CreateDirectory(Application.dataPath + $"/Resources/level_{level}/");
            File.Copy(Application.streamingAssetsPath + $"/Levels/level_{level}/level_{level}_blocks.ini", Application.dataPath + $"/Resources/level_{level}/level_{level}_blocks.ini");
        }

        List<string> originalLines = File.ReadLines(Application.streamingAssetsPath + $"/Levels/level_{level}/level_{level}_blocks.ini").ToList();
        List<string> editedLines = new List<string>();
        foreach (string line in originalLines)
        {
            Regex regex = new Regex("[a-zA-Z0-9 -]");
            editedLines.Add(regex.Replace(line, " "));
        }
        File.WriteAllLines(Application.dataPath + $"/Resources/level_{level}/level_{level}_blocks.ini", editedLines);

        GenerateLevelFromFile(Application.streamingAssetsPath + $"/Levels/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini");
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
            if (Instance.m_IsInitialLevelLoading)
            {
                GenerateLevelFromFile(Application.streamingAssetsPath + $"/Levels/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini");
                Instance.m_IsInitialLevelLoading = false;
            }
            else
                GenerateLevelFromFile(Application.dataPath + $"/Resources/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini");

            FileManager.Instance.CloseFile();
        }
        else
            PlayerManager.Pause();
    }

    /// <summary>
    /// Generate the level from a specified path.
    /// </summary>
    /// <param name="levelFilePath">PAth of the level file.</param>
    public void GenerateLevelFromFile(string levelFilePath)
    {
        Vector3Int currentCell;
        StreamReader reader;
        string content;

        // Tableaux représentant le niveaux.
        int[,] levelTiles;          // Chaque case contient l'indice de la tile qu'elle contient.

        TilesType[,] map;           // Chaque case contient le type de bloc qu'elle contient.

        // 1°) Extraction des informations du fichier de config
        //////////////////////////////////////////////////////////////

        reader = new StreamReader(levelFilePath);
        content = reader.ReadToEnd();
        reader.Close();

        // On supprime les potentiels saut de ligne en fin de fichier.
        while (content.EndsWith("\n") || content.EndsWith("\r"))
            content = content.Remove(content.Length - 1);

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
                        m_highlightMap.SetTile(currentCell, m_highlightTile[levelTiles[currentCell.x, currentCell.y]]);
                        break;

                    case TilesType.ENEMY:
                        GameObject enemy = Instantiate(m_EnemyPrefab);
                        enemy.transform.position = new Vector2(currentCell.x + 0.5f, currentCell.y + 0.5f);
                        break;

                    case TilesType.SPAWN:
                        PlayerManager.Instance.SetInitialPlayerPosition(new Vector2(currentCell.x + 0.5f, currentCell.y + 0.5f));
                        break;

                    case TilesType.DESTINATION:
                        GameObject flag = Instantiate(m_FlagPrefab);
                        flag.transform.position = new Vector2(currentCell.x + 0.48f, currentCell.y + 0.48f);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}