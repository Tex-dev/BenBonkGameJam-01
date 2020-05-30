using UnityEngine;
using UnityEngine.Tilemaps;

using System.IO;
using System;

internal enum TilesID : ushort
{
    UP = 1,
    RIGHT = 2,
    DOWN = 4,
    LEFT = 8
}

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private Tile[] m_highlightTile;

    [SerializeField]
    private Tilemap m_highlightMap;

    /// <summary>
    /// GameObject to display on pause.
    /// </summary>
    [SerializeField]
    private GameObject m_PauseMenu = null;

    public static int CurrentLevel => Instance.m_CurrentLevel;

    private int m_CurrentLevel = 1;

    /// <summary>
    /// Don't destroy this game object on load.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Called when the application focus is changed.
    /// </summary>
    /// <param name="focus">Status of the focus. ON = true, OFF = false.</param>
    private void OnApplicationFocus(bool focus)
    {
        m_PauseMenu.SetActive(!focus);

        if (focus)
            GenerateLevelFromFile(Application.dataPath + $"/Resources/level_{m_CurrentLevel}/level_{m_CurrentLevel}_blocks.ini");
    }

    public void GenerateLevelFromFile(string p_levelFilePath)
    {
        Vector3Int currentCell;
        StreamReader reader;
        string content;

        // Tableaux représentant le niveaux.
        int[,] levelTiles;          // Chaque case contient l'indice de la tile qu'elle contient.
        bool[,] level;              // Chaque case vaut ici, true si elle contient une tile, false sinon.

        // 1°) Extraction des informations du fichier de config
        //////////////////////////////////////////////////////////////

        reader = new StreamReader(p_levelFilePath);
        content = reader.ReadToEnd();
        reader.Close();

        // On supprime les potentiels saut de ligne en fin de fichier
        while (content.EndsWith("\n") || content.EndsWith("\r"))
            content = content.Remove(content.Length - 1);

        string[] lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].EndsWith("\r"))
                lines[i] = lines[i].Remove(lines[i].Length - 1);
        }

        // On déduis la taille du niveau des informations lues
        int height = lines.Length;
        int width = 0;
        foreach (string line in lines)
            width = Mathf.Max(width, line.Length);

        levelTiles = new int[width, height];
        level = new bool[width, height];

        // 2°) Chargement du niveau à paritr du fichier
        //////////////////////////////////////////////////////////////

        // Boucle permettant de savoir s'il y a ou pas une tiles sur la case
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (i < lines[j].Length)
                {
                    if (lines[j][i] == ' ')
                        level[i, height - 1 - j] = false;
                    else
                        level[i, height - 1 - j] = true;
                }
                else
                    level[i, height - 1 - j] = false;
            }
        }

        // Boucle afin d'affecter la bonne tile à chaque case (dépendant des voisins)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (level[i, j])
                {
                    int tileID = 0;

                    // UP
                    if (j < height - 1 && level[i, j + 1])
                        tileID += (int)TilesID.UP;

                    // DOWN
                    if (j > 0 && level[i, j - 1])
                        tileID += (int)TilesID.DOWN;

                    // RIGHT
                    if (i < width - 1 && level[i + 1, j])
                        tileID += (int)TilesID.RIGHT;

                    // LEFT
                    if (i > 0 && level[i - 1, j])
                        tileID += (int)TilesID.LEFT;

                    levelTiles[i, j] = tileID;
                }
            }
        }

        // 3°) Affichage des tiles à l'écran
        //////////////////////////////////////////////////////////////

        currentCell = new Vector3Int(0, 0, 0);

        // On efface l'écran (on enlève les anciennes tiles)
        for (currentCell.x = 0; currentCell.x < 50; currentCell.x++)
        {
            for (currentCell.y = 0; currentCell.y < 200; currentCell.y++)
                m_highlightMap.SetTile(currentCell, null);
        }

        // Boucle permettant l'affichage des tiles.
        for (currentCell.x = 0; currentCell.x < width; (currentCell.x)++)
        {
            for (currentCell.y = 0; currentCell.y < height; (currentCell.y)++)
            {
                if (level[currentCell.x, currentCell.y])
                    m_highlightMap.SetTile(currentCell, m_highlightTile[levelTiles[currentCell.x, currentCell.y]]);
                else
                    m_highlightMap.SetTile(currentCell, null);
            }
        }
    }
}